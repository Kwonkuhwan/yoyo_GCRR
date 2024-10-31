using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GCRR.VirtualMap
{
    public class LoadScenario : MonoBehaviour
    {
        #region 시나리오 목록들
        /// 작성 : KKH
        /// <summary>
        /// 시나리오 목록들 요청
        /// </summary>
        /// <returns>true, false</returns>
        public IEnumerator Request_ScenarioLists(string stServiceUrl, string dirPath, string fileName)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Request_ScenarioLists(stServiceUrl, dirPath, fileName);
                }
                else
                {
                    UTILS.DownLoad_Data(uwr, dirPath, fileName);
                }
            }
        }
        #endregion

        #region 시나리오 데이터
        /// 작성 : KKH
        /// <summary>
        /// 시나리오 목록들 요청
        /// </summary>
        /// <returns>true, false</returns>
        public IEnumerator Request_ScenarioData(string stServiceUrl, string dirPath, string fileName)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Request_ScenarioData(stServiceUrl, dirPath, fileName);
                }
                else
                {
                    UTILS.DownLoad_Data(uwr, dirPath, fileName);
                }
            }
        }
        #endregion
    }
}