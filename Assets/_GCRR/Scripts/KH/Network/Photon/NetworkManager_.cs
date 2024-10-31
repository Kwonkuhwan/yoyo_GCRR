using System;
using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace GCRR.VirtualMap
{
    public class NetworkManager_ : MonoBehaviourPunCallbacks
    {
        private static NetworkManager_ instance;
        public static NetworkManager_ Instance => instance;

        private RoomOptions roomOptions;

        [SerializeField] private GameObject go_RemotePrefab;
        [SerializeField] private GameObject spawnedPlayerPrefab;

        //public bool isJoinRoom = false;

        private Coroutine initConnectToServerCoroutine;

        [SerializeField] private string myIP;
        [SerializeField] private string connectIP;

        private void Awake()
        {
            if (GameManager.Instance.SelectMode != SelectScenrioMode.Local)
            {
                return;
            }

            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }

            myIP = GameManager.Instance.defaultData.myIP;
            connectIP = GameManager.Instance.defaultData.localConnectIP;

            // [2023.07.17] [추가] KKH : 포톤 설정
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 60;
            // [2023.07.17] [추가] KKH : PhotonServerV4를 사용하기 위해 프로토콜 타입을 V18 이상에서 V16으로 변경
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.SerializationProtocolType = SerializationProtocol.GpBinaryV16;

            PhotonNetwork.AutomaticallySyncScene = true;
            // [2023.07.24] [추가] KKH : 포톤 서버 연결 IP 재설정
            PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Udp;
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;

            InitConnectToServer();
        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 서버 연결
        /// </summary>
        /// <param name="ip"></param>
        public void InitConnectToServer()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = GameManager.Instance.isServer == true ? myIP : connectIP;

            // [2023.07.24] [추가] KKH : 포톤 서버 연결 코루틴이 null 아닐때
            if (initConnectToServerCoroutine != null)
            {
                // [2023.07.24] [추가] KKH : 포톤 서버 연결 코루틴 중단
                StopCoroutine(initConnectToServerCoroutine);
                // [2023.07.24] [추가] KKH : 포톤 서버 연결 코루틴 null로 초기화
                initConnectToServerCoroutine = null;
            }

            // [2023.07.24] [추가] KKH : 포톤 서버 연결 코루틴 재시작
            initConnectToServerCoroutine = StartCoroutine(ConnectToServer());
        }

        /// 작성 - KKH
        /// <summary>
        /// Photon 서버 연결 코루틴
        /// </summary>
        public IEnumerator ConnectToServer()
        {
            try
            {
                // [2023.07.24] [추가] KKH : 포톤 서버가 연결 되어있을때 동작
                if (PhotonNetwork.IsConnected || PhotonNetwork.IsConnectedAndReady)
                {
                    // [2023.07.24] [추가] KKH : 포톤 서버 연결 해제
                    PhotonNetwork.Disconnect();
                }
            }
            catch
            {
                InitConnectToServer();
            }

            yield return new WaitForSeconds(3.0f);

            try
            {
                // [2023.07.24] [추가] KKH : 포톤 서버 연결 시도
                PhotonNetwork.ConnectUsingSettings();
            }
            catch
            {
                InitConnectToServer();
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            JoinRoom();
        }

        /// 작성 - KKH
        /// <summary>
        /// 포톤 방 접속
        /// </summary>
        public void JoinRoom()
        {
            //StartCoroutine(JoinRoom(ScenarioInfoClass.Instance.scenarioData.scenarioID));
            StartCoroutine(JoinRoom(ScenarioInfoClass.Instance.scenarioList.scenarioID));
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 룸 입장
        /// </summary>
        /// <param name="roomName">방 이름(시나리오 ID)</param>
        private IEnumerator JoinRoom(string roomName)
        {
            //yield return new WaitForSeconds(1.0f);
            yield return null;

            if (GameManager.Instance.isServer)
            {
                try
                {
                    roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = 8;
                    roomOptions.IsVisible = true;
                    roomOptions.IsOpen = true;

                    // [2023.07.24] [추가] KKH : 방 생성
                    if (PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default))
                    {
                        
                    }
                }
                catch (Exception e)
                {
                    UTILS.LogWarning($"[Critical Error NetworkManager] {e}");
                }
            }
            //else if (!GameManager.Instance.isServer && GameManager.Instance.isCloud)
            //{
            //    if (PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default))
            //    {
            //        UTILS.Log($"{PhotonNetwork.PhotonServerSettings.AppSettings.Server} {roomName} 생성 성공 : 방 생성 완료");
            //    }
            //    else
            //    {
            //        UTILS.LogWarning("포톤 방 생성 조인 에러");
            //    }
            //}
            else
            {
                try
                {
                    // [2023.07.24] [추가] KKH : 방 접속 시도
                    if (!PhotonNetwork.JoinRoom(roomName))
                    {
                        UTILS.LogWarning($"{PhotonNetwork.PhotonServerSettings.AppSettings.Server} {roomName} 연결 실패 : 연결 재시도..");
                        // [2023.07.24] [추가] KKH : 방이 없을때 로컬 IP로 변경 후 방 생성 시도
                        InitConnectToServer();
                    }
                }
                catch
                {
                    UTILS.LogWarning($"{PhotonNetwork.PhotonServerSettings.AppSettings.Server} {roomName} 연결 실패 : 연결 재시도..");
                    InitConnectToServer();
                }
            }
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
        }

        /// 작성 - KKH
        /// <summary>
        /// 플레이어 생성
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public GameObject SpawnedPlayer(Vector3 pos, Quaternion quaternion)
        {
            if (spawnedPlayerPrefab != null) return null;
            spawnedPlayerPrefab = PhotonNetwork.Instantiate(go_RemotePrefab.name, pos, quaternion);

            BNG.NetworkPlayer np = spawnedPlayerPrefab.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
            }

            return spawnedPlayerPrefab;
        }

        /// 작성 - KKH
        /// <summary>
        /// 플레이어 제거
        /// </summary>
        public void DestroyPlayer()
        {
            if (spawnedPlayerPrefab != null) PhotonNetwork.Destroy(spawnedPlayerPrefab);
        }

        public override void OnJoinedRoom()
        {
            StartCoroutine(ConferenceRoomManager.Instance.Delay_Set_User());
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            UTILS.LogWarning($"방 입장 실패 : {PhotonNetwork.PhotonServerSettings.AppSettings.Server} {message}");
            InitConnectToServer();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        /// 작성 - KKH
        /// <summary>
        /// 방 나가기
        /// </summary>
        public void LeftRoom()
        {
            if (!PhotonNetwork.IsConnected) return;

            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            DestroyPlayer();
            PhotonNetwork.LeaveLobby();
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
        }
    }
}