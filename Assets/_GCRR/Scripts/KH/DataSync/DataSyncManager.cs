using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class DataSyncManager : MonoBehaviour
    {
        private static DataSyncManager instance;
        public static DataSyncManager Instance => instance;

        private void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 제어권 확인
        /// </summary>
        /// <returns></returns>
        public bool Check_Control(string controlIP)
        {
            // [2023.04] [추가] KKH : 시나리오 리스트에 있는 제어권을 통해 확인을 해주자 자신이 아니면 false 자신이면 true
            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            string localIP = string.Empty;

            foreach (IPAddress address in iphost.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = address.ToString();
                }
            }

            if (localIP == controlIP.Replace(" ", "")) return true;

            return false;
        }

        /// 작성 - KKH
        /// <summary>
        /// 로컬 데이터 저장
        /// </summary>
        /// <returns></returns>
        public bool Save_LocalData()
        {
            if (GameManager.Instance.isCloud) return false;

            ScenarioInfoClass.Instance.scenarioList.revisionDate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";

            // [2023.04] [추가] KKH : 현재 저장되어 있는 ScenarioList를 수정된 날짜와 제어권이 바꼇을수도 있으니 저장
            if (!Save_ScenarioList()) return false;
            // [2023.04] [추가] KKH : 현재 저장되어 있는 ScenarioData를 저장하자
            if (!Save_ScenarioData()) return false;
            return true;
        }

        public bool Save_NewLocalData()
        {
            string scenarioName_ = $"scen_VR_{DateTime.Now.ToString("yyyyMMdd_HH:mm:ss")}";
            string scenarioID_ = $"local_sce_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_VR";
            string folderName_ = $"{DateTime.Now.ToString("yyyy-MM")}-A-VR";
            string creationDate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            string revisionDate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";

            ScenarioList newscenarioList = ScenarioInfoClass.Instance.scenarioList;

            newscenarioList.scenarioName = scenarioName_;
            newscenarioList.scenarioID = scenarioID_;
            newscenarioList.folderName = folderName_;
            newscenarioList.description = $"{folderName_}의 {scenarioID_}입니다.";
            newscenarioList.creationDate = creationDate;
            newscenarioList.revisionDate = revisionDate;

            if (GameManager.Instance.isCloud)
            {
                List<ScenarioList> scenarioLists = JsonManager.ScenarioListsJsonLoad("ScenarioList");
                scenarioLists.Add(ScenarioInfoClass.Instance.scenarioList);
                // [2023.04] [추가] KKH : 현재 저장되어 있는 ScenarioList를 수정된 날짜와 제어권이 바꼇을수도 있으니 저장
                if (!Save_ScenarioList(scenarioLists)) return false;

                scenarioLists.Clear();
            }
            else
            {
                ScenarioInfoClass.Instance.scenarioLists.Add(ScenarioInfoClass.Instance.scenarioList);
                ScenarioInfoClass.Instance.scenarioList = newscenarioList;
                // [2023.04] [추가] KKH : 현재 저장되어 있는 ScenarioList를 수정된 날짜와 제어권이 바꼇을수도 있으니 저장
                if (!Save_ScenarioList()) return false;
            }

            ScenarioData newscenarioData = ScenarioInfoClass.Instance.scenarioData;

            newscenarioData.scenarioName = scenarioName_;
            newscenarioData.scenarioID = scenarioID_;
            newscenarioData.folderName = folderName_;
            newscenarioData.writer = "VR";
            newscenarioData.writeDay = creationDate;
            newscenarioData.revisionDay = revisionDate;

            if (!Save_ScenarioData(newscenarioData)) return false;
            return true;
        }

        public bool Save_ScenarioList()
        {
            ScenarioInfoClass.Instance.scenarioList.isPlaying = 0;
            return JsonManager.ScenarioListJsonWirte("ScenarioList");
        }

        public bool Save_ScenarioList(List<ScenarioList> scenarioLists)
        {
            ScenarioInfoClass.Instance.scenarioList.isPlaying = 0;
            return JsonManager.ScenarioListJsonWirte("ScenarioList", scenarioLists);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 목록 Json파일로 저장
        /// </summary>
        /// <param name="scenarioData"></param>
        /// <returns></returns>
        public bool Save_ScenarioData(ScenarioData scenarioData)
        {
            if (ScenarioInfoClass.Instance.scenarioData == null) return false;

            ScenarioInfoClass.Instance.scenarioData = scenarioData;
            return Save_ScenarioData();
        }

        public bool Save_ScenarioData()
        {            
            return JsonManager.ScenarioDataJsonWirte(ScenarioInfoClass.Instance.scenarioData.scenarioID);
        }
    }
}