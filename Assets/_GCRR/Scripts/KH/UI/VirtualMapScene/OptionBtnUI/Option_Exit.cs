using Photon.Pun;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class Option_Exit : MonoBehaviour
    {
        /// 작성 - KKH
        /// <summary>
        /// 시나리오 종료
        /// </summary>
        public void OnClick()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }

            if (GameManager.Instance.isCloud)
            {
                C_LeaveGame c_LeaveGame = new C_LeaveGame(ScenarioInfoClass.Instance.playerID);
                if (TcpSocketManager.Instance.tcpClient != null)
                {
                    TcpSocketManager.Instance.tcpClient.Send(c_LeaveGame.Serialize());
                }
            }

            var objs = GameObject.FindGameObjectsWithTag("DestroyLoad");
            foreach (var obj in objs)
            {
                Destroy(obj);
            }

            GameManager.Instance.DisableTcpSocket();
            //GameManager.Instance.DisableNetWorkManager();

            StartCoroutine(UTILS.LoadScene("01.StartScene"));
        }
    }
}