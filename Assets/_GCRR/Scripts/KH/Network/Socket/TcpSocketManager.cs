using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GCRR.VirtualMap
{
    public class ClientReceiveData
    {
        public TClient client { get; set; }
        public byte[] data { get; set; }

        public ClientReceiveData(TClient client, byte[] data)
        {
            this.client = client;
            this.data = data;
        }
    }

    public class TcpSocketManager : MonoBehaviour
    {
        private static TcpSocketManager instance;
        public static TcpSocketManager Instance => instance;

        [SerializeField] private string ip;
        [SerializeField] private int port;

        private IPEndPoint ipEndPoint;
        private TClient tcpclient;
        public TClient tcpClient => tcpclient;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneManager.GetActiveScene().name.Equals("03.VirtualMap") && (GameManager.Instance.isServer || GameManager.Instance.isCloud))
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (Instance != null)
            {
                instance = null;
            }

            instance = this;

            if (GameManager.Instance.SelectMode == SelectScenrioMode.Location)
            {
                return;
            }

            if (GameManager.Instance.isCloud)
            {
                ip = GameManager.Instance.defaultData.gameServerIP;
                port = GameManager.Instance.defaultData.gameServerPort;
            }
            else
            {
                if (GameManager.Instance.isServer)
                {
                    ip = GameManager.Instance.defaultData.myIP;
                }
                else
                {
                    ip = GameManager.Instance.defaultData.localConnectIP;
                }
                port = GameManager.Instance.defaultData.localConnectPort;
            }

            if (!GameManager.Instance.isCloud && GameManager.Instance.isServer)
            {
                TServer.Init(ip, port);
                TServer.ServerOpen();
            }
            else
            {
                tcpclient = new TClient();
                tcpclient.Connection(ip, port);
            }
        }

        public void OnClose()
        {
            TServer.Close();

            if (tcpclient != null)
            {
                tcpclient.DisConnection();
            }
        }
    }

    public static class TServer
    {
        public static bool isOpen { get; set; }

        private static TcpListener server;

        private static List<TClient> m_Clients = new List<TClient>();
        private static Thread thTcpClientCheck, thHandleClient, thSendData = null;

        public static Queue<ClientReceiveData> clientPostBox { get; set; }
        public static Postbox postboxSend = new Postbox();

        private static string ip;
        private static int port;

        public static void Init(string ip_, int port_)
        {
            isOpen = false;
            ip = ip_;
            port = port_;
        }

        private static void ThreadInit()
        {
            Task.Run(AcceptClient);

            thTcpClientCheck = new Thread(TcpClientCheck);
            thTcpClientCheck.IsBackground = true;

            thHandleClient = new Thread(() => HandleClient());
            thHandleClient.IsBackground = true;

            thSendData = new Thread(() => Send());
            thSendData.IsBackground = true;

            thTcpClientCheck.Start();
            thHandleClient.Start();
            thSendData.Start();
        }

        public static void ServerOpen()
        {
            isOpen = true;
            StartServer();
        }

        private static void StartServer()
        {
            try
            {
                // TcpListener 생성
                server = new TcpListener(IPAddress.Parse(GameManager.Instance.defaultData.myIP), port);

                // 서버 시작
                server.Start();

                ThreadInit();
            }
            catch (Exception e)
            {
                UTILS.LogError($"{e}");
            }
        }

        private static async Task AcceptClient()
        {
            while (isOpen)
            {
                Task.Delay(100).Wait();
                try
                {
                    var client = await server.AcceptSocketAsync();
                    //Socket client = server.AcceptSocket();
                    UTILS.Log($"클라이언트 연결 수락.");

                    TClient tcpClent = new TClient();
                    tcpClent.Connection(client);
                    lock (m_Clients)
                    {
                        m_Clients.Add(tcpClent);
                    }
                }
                catch (Exception e)
                {
                    UTILS.LogWarning($"{e}");
                }
            }
        }

        private static void HandleClient()
        {
            clientPostBox = new Queue<ClientReceiveData>();

            while (true)
            {
                Task.Delay(100).Wait();

                if (clientPostBox.Count > 0)
                {
                    ClientReceiveData clientReceiveData = clientPostBox.Dequeue();

                    try
                    {
                        BroadcastMessage(clientReceiveData);
                    }

                    catch { continue; }
                }
            }
        }

        private static void BroadcastMessage(ClientReceiveData clientReceiveData)
        {
            foreach (TClient client in m_Clients)
            {
                if (client == clientReceiveData.client) continue;
                client.Send(clientReceiveData.data);
            }
        }

        private static void Send()
        {
            while (isOpen)
            {
                byte[] data = postboxSend.GetData();
                if (data == null) continue;

                ClientReceiveData clientReceiveData = new ClientReceiveData(null, data);
                BroadcastMessage(clientReceiveData);
            }
        }

        private static void TcpClientCheck()
        {
            while (isOpen)
            {
                if (m_Clients.Count <= 0)
                {
                    Task.Delay(5000).Wait();
                    continue;
                }

                foreach (TClient client in m_Clients)
                {
                    if (client.isDisconn)
                    {
                        client.DisConnection();
                        m_Clients.Remove(client);

                        break;
                    }
                }

                Task.Delay(1000).Wait();
            }
        }

        public static void Close()
        {
            isOpen = false;

            if (thTcpClientCheck != null)
            {
                thTcpClientCheck.Abort();
                thTcpClientCheck = null;
            }

            if (server != null)
            {
                server.Stop();
            }

            lock (m_Clients)
            {
                foreach (TClient client in m_Clients)
                {
                    client.DisConnection();
                }

                m_Clients.Clear();
            }
        }
    }

    public class TClient
    {
        public Socket m_Client;

        private Postbox sendPostBox = new Postbox();
        private Postbox receivePostbox = new Postbox();

        private Thread thReceive, thSendQueue, thReceiveQueue = null;

        private IPEndPoint clientEndPoint;

        public bool isDisconn = false;

        /// <summary>
        /// 스레드 초기화
        /// </summary>
        private void ThreadInit()
        {
            thReceive = new Thread(Receive);
            thReceive.IsBackground = true;

            thSendQueue = new Thread(SendQueue);
            thSendQueue.IsBackground = true;

            thReceiveQueue = new Thread(ReceiveQueue);
            thReceiveQueue.IsBackground = true;

            thReceive.Start();
            thSendQueue.Start();
            thReceiveQueue.Start();
        }

        /// <summary>
        /// 클라이언트 접속
        /// </summary>
        /// <param name="client"></param>
        public async void Connection(string ip, int port)
        {
            clientEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await m_Client.ConnectAsync(clientEndPoint);
            }
            catch (Exception e)
            {
                if (isDisconn) return;

                UTILS.LogError($"[Critical Error TcpSocketManager] Not Tcp Server Connected - {e}");

                Task.Delay(1000).Wait();

                Connection(ip, port);
            }

            ThreadInit();
        }

        public void Connection(Socket client)
        {
            m_Client = client;

            clientEndPoint = (IPEndPoint)m_Client.RemoteEndPoint;

            //isAlive = true;

            ThreadInit();
        }

        /// <summary>
        /// 송신 Queue
        /// </summary>
        private void SendQueue()
        {
            while (true)
            {
                Task.Delay(10).Wait();

                if (isDisconn) break;
                if (sendPostBox == null) continue;

                if (m_Client != null && m_Client.Connected)
                {
                    try
                    {
                        byte[] data = sendPostBox.GetData();
                        if (data == null) continue;

                        m_Client.Send(data);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 수신 Queue
        /// </summary>
        private void ReceiveQueue()
        {
            while (true)
            {
                if (isDisconn) break;
                if (receivePostbox == null) continue;

                Task.Delay(10).Wait();

                byte[] data = receivePostbox.GetData();

                if (data == null) continue;

                if (!doProcess(data)) continue;
            }
        }

        /// <summary>
        /// 송신 Queue에 데이터를 넣는다.
        /// </summary>
        /// <param name="datas"></param>
        public void Send(byte[] data)
        {
            sendPostBox.PushData(data);
        }

        /// <summary>
        /// 수신 Queue에 데이터를 넣는다.
        /// </summary>
        private void Receive()
        {
            while (true)
            {
                Task.Delay(10).Wait();

                if (isDisconn) break;
                if (receivePostbox == null) continue;

                if (m_Client != null && m_Client.Available != 0)
                {
                    byte[] bReceive = new byte[m_Client.SendBufferSize];
                    int bReceiveRead = m_Client.Receive(bReceive);

                    if (bReceiveRead > 0)
                    {
                        receivePostbox.PushData(bReceive);
                    }
                }

            }
        }

        #region 패킷 처리
        /// <summary>
        /// 수신 받은 데이터 처리
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private bool doProcess(byte[] bytes)
        {
            PacketData packetData = PacketData.DeSerialization(bytes);
            ClientReceiveData clientReceiveData = new ClientReceiveData(this, bytes);

            if (packetData.packetID == PacketID.S_MyPlayerID)
            {
                return Processing_S_MyPlayerID(packetData);
            }
            else if (packetData.packetID == PacketID.S_ChangeObjInfo)
            {
                if (!GameManager.Instance.isCloud && TServer.clientPostBox != null)
                {
                    TServer.clientPostBox.Enqueue(clientReceiveData);
                }

                return Processing_S_ChangeObjInfo(packetData);
            }
            else if (packetData.packetID == PacketID.S_SaveScenarioResult)
            {
                return Processing_S_SaveScenarioResult(packetData);
            }
            else if (packetData.packetID == PacketID.S_BroadcastNewOwner)
            {
                return Processing_S_BroadcastNewOwner(packetData);
            }
            else if (packetData.packetID == PacketID.Response_ScenrioList)
            {
                Processing_Response_ScenarioLists(packetData);
            }
            else if (packetData.packetID == PacketID.Request_ScenarioList)
            {
                Send(Processing_Request_ScenarioLists(packetData));
            }
            else if (packetData.packetID == PacketID.Response_ScenrioData)
            {
                return Processing_Response_SecnarioData(packetData);
            }
            else if (packetData.packetID == PacketID.Request_ScenarioData)
            {
                Send(Processing_Request_ScenarioData(packetData));
            }
            return true;
        }

        public bool Processing_S_MyPlayerID(PacketData packetData)
        {
            S_MyPlayerID s_MyPlayerID = S_MyPlayerID.DeSerialization(packetData);
            if (s_MyPlayerID == null) return false;

            ScenarioInfoClass.Instance.playerID = s_MyPlayerID.playerID;
            VirtualMapUIManager.Instance.OnMessageEnqeue($"{ScenarioInfoClass.Instance.playerID} 입장", false);

            return true;
        }

        public bool Processing_S_ChangeObjInfo(PacketData packetData)
        {
            S_ChangeObjInfo s_ChangeObjInfo = S_ChangeObjInfo.DeSerialization(packetData);
            if (s_ChangeObjInfo == null) return false;

            ScenarioObjectManager.Instance.ChangeObjEnqueue(s_ChangeObjInfo);

            return true;
        }

        public bool Processing_S_SaveScenarioResult(PacketData packetData)
        {
            S_SaveScenarioResult s_SaveScenarioResult = S_SaveScenarioResult.DeSerialization(packetData);
            if (s_SaveScenarioResult == null) return false;

            if (VirtualMapUIManager.Instance != null)
            {
                if (s_SaveScenarioResult.saveResult)
                {
                    VirtualMapUIManager.Instance.OnMessageEnqeue($"클라우드에 정상적으로 저장되었습니다.", false);
                }
                else
                {
                    VirtualMapUIManager.Instance.OnMessageEnqeue($"클라우드 저장 권한이 없어 실패하였습니다.", true);
                }
            }
            return true;
        }

        public bool Processing_S_BroadcastNewOwner(PacketData packetData)
        {
            S_BroadcastNewOwner s_BroadcastNewOwner = S_BroadcastNewOwner.DeSerialization(packetData);
            if (s_BroadcastNewOwner == null) return false;

            return true;
        }

        public bool Processing_Response_ScenarioLists(PacketData packetData)
        {
            Response_ScenarioList response_ScenarioLists = Response_ScenarioList.DeSerialization(packetData);
            if (response_ScenarioLists == null) return false;

            bool isDup = false;
            foreach (ScenarioList sl in ScenarioInfoClass.Instance.scenarioLists)
            {
                if (sl.scenarioID == response_ScenarioLists.scenarioList.scenarioID)
                {
                    isDup = true;
                    break;
                }
            }
            if (!isDup)
            {
                ScenarioInfoClass.Instance.scenarioLists.Add(response_ScenarioLists.scenarioList);
            }

            return true;
        }

        public byte[] Processing_Request_ScenarioLists(PacketData packetData)
        {
            Request_ScenarioList request_ScenarioLists = Request_ScenarioList.DeSerialization(packetData);
            if (request_ScenarioLists == null) return null;

            ScenarioInfoClass.Instance.scenarioList.isPlaying = 1;
            ScenarioList scenarioList = ScenarioInfoClass.Instance.scenarioList;
            Response_ScenarioList response_ScenarioList = new Response_ScenarioList(scenarioList);

            return response_ScenarioList.Serialize();
        }

        public bool Processing_Response_SecnarioData(PacketData packetData)
        {
            Response_ScenarioData response_ScenarioData = Response_ScenarioData.DeSerialization(packetData);
            if (response_ScenarioData == null) return false;

            ScenarioInfoClass.Instance.scenarioData = response_ScenarioData.scenarioData;
            GameManager.Instance.nRange = response_ScenarioData.scenarioData.range;

            //GameManager.Instance.SetCoordinates((float)ScenarioInfoClass.Instance.scenarioData.latitude, (float)ScenarioInfoClass.Instance.scenarioData.longitude);
            return true;
        }

        public byte[] Processing_Request_ScenarioData(PacketData packetData)
        {
            Request_ScenarioData request_ScenarioData = Request_ScenarioData.DeSerialization(packetData);
            if (request_ScenarioData == null) return null;

            ScenarioData scenarioData = ScenarioInfoClass.Instance.scenarioData;
            scenarioData.latitude = (float)GameManager.Instance.StartCoordinates.latitude;
            scenarioData.longitude = (float)GameManager.Instance.StartCoordinates.longitude;
            Response_ScenarioData response_ScenarioData = new Response_ScenarioData(scenarioData);

            return response_ScenarioData.Serialize();
        }
        #endregion

        public void DisConnection()
        {
            isDisconn = true;

            if (thReceiveQueue != null)
            {
                thReceiveQueue.Abort();
                thReceiveQueue = null;
            }

            if (thSendQueue != null)
            {
                thSendQueue.Abort();
                thSendQueue = null;
            }

            if (m_Client != null)
            {
                m_Client.Close();
                m_Client.Dispose();
                m_Client = null;
            }
        }
    }
}