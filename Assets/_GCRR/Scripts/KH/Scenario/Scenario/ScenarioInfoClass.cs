using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GCRR.VirtualMap
{
    public class ScenarioInfoClass : MonoBehaviour
    {
        private static ScenarioInfoClass instance;
        public static ScenarioInfoClass Instance { get => instance; }

        private int pID = 0;
        public int playerID { get => pID; set => pID = value; }

        [SerializeField] private List<ScenarioList> scenaLists = new List<ScenarioList>();        // KKH : 시나리오 리스트
        public List<ScenarioList> scenarioLists { get => scenaLists; set => scenaLists = value; }        // KKH : 시나리오 리스트

        [SerializeField] private ScenarioList scenaList = new ScenarioList();               // KKH : 시나리오 목록
        public ScenarioList scenarioList { get => scenaList; set => scenaList = value; }               // KKH : 시나리오 목록

        [SerializeField] private ScenarioData scenaData = new ScenarioData();               // KKH : 시나리오 데이터
        public ScenarioData scenarioData { get => scenaData; set => scenaData = value; }               // KKH : 시나리오 데이터

        [SerializeField] private List<TargetData> tarData = new List<TargetData>();
        public List<TargetData> targetData { get => tarData; set => tarData = value; }

        //[SerializeField] private List<TargetMetaData> tarMetaDatas;
        //public List<TargetMetaData> targetMetaDatas { get => tarMetaDatas; set => tarMetaDatas = value; }

        //public TcpSocketManager tcpSocketManager;

        [SerializeField] private Timeseries tileLists = new Timeseries();
        public Timeseries tileseriesLists { get => tileLists; set => tileLists = value; }

        [SerializeField] private int tType = 0;
        public int timeType { get => tType; set => tType = value; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 정보 초기화
        /// </summary>
        public void ClearScenario()
        {
            scenarioLists.Clear();
            scenarioList.Clear();
            scenarioData.Clear();
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 목록 불러오기
        /// </summary>
        public void LoadScenarioList()
        {
            if (GameManager.Instance.isCloud)
            {
                GameManager.Instance.DownLoad_ScenarioLists();
            }
            else
            {
                if (!GameManager.Instance.isServer)
                {
                    Request_ScenarioList request_ScenarioLists = new Request_ScenarioList();
                    if (TcpSocketManager.Instance.tcpClient != null)
                    {
                        TcpSocketManager.Instance.tcpClient.Send(request_ScenarioLists.Serialization());
                    }
                }
                else
                {
                    scenarioLists = JsonManager.ScenarioListsJsonLoad("ScenarioList");
                }
            }

            if (scenarioLists == null)
            {
                UTILS.LogError("[Critical Error] ScenarioList is null.");
                return;
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 데이터 불러오기
        /// </summary>
        /// <param name="scenarioDataFileName">시나리오 데이터 파일명</param>
        public IEnumerator LoadScenarioData(string scenarioDataFileName)
        {
            if (GameManager.Instance.isCloud)
            {
                GameManager.Instance.DownLoad_ScenarioData();
            }
            else
            {
                if (!GameManager.Instance.isServer)
                {
                    Request_ScenarioData request_ScenarioData = new Request_ScenarioData(scenarioList.scenarioName);
                    if (TcpSocketManager.Instance.tcpClient != null)
                    {
                        TcpSocketManager.Instance.tcpClient.Send(request_ScenarioData.Serialize());
                    }
                }
                else
                {
                    scenarioData = JsonManager.ScenarioDataJsonLoad(scenarioDataFileName);
                }
            }

            yield return new WaitForSeconds(0.1f);

            if (scenarioData != null)
            {
                GameManager.Instance.nRange = scenarioData.range;
                GameManager.Instance.SetCoordinates(scenarioData.latitude, scenarioData.longitude);
                //GameManager.Instance.SetCoordinates(scenarioData.GetLatitude(), scenarioData.GetLongitude());
            }
            else
            {
                UTILS.LogWarning("[Critical Error] ScenarioData is null.");
            }
        }

        public IEnumerator LoadTimeSeriesData()
        {
            GameManager.Instance.DownLoad_TimeSeriesData();

            yield return new WaitForSeconds(0.1f);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 Data 추가
        /// </summary>
        /// <param name="objectData"></param>
        public void AddObjectData(ObjectData objectData)
        {
            // [2023.05.30] [추가] KKH : ScenarioData의 오브젝트 수를 1 증가
            scenarioData.SetObjectCount(scenarioData.objectCount + 1);

            //ScenarioObject sO = obj.GetComponent<ScenarioObject>();
            // [2023.05.30] [추가] KKH : ScenarioInboClass에 ScenarioData의 Objects에 ObjectData 추가
            scenarioData.objects.Add(objectData);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 Data 제거
        /// </summary>
        /// <param name="objectData"></param>
        public void RemoveObjectData(GameObject obj)
        {
            // [2023.05.30] [추가] KKH : 시나리오 오브젝트 수를 1 감소
            scenarioData.SetObjectCount(scenarioData.objectCount - 1);
            // [2023.05.30] [추가] KKH : ScenarioInboClass에 ScenarioData의 Objects에서 삭제
            ScenarioObject sO = obj.GetComponent<ScenarioObject>();
            scenarioData.objects.Remove(sO.objectData);
            ScenarioObjectManager.Instance.scenarioGO.Remove(obj);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 Data 수정
        /// </summary>
        /// <param name="objectData"></param>
        /// <returns></returns>
        public bool RevisionObjectData(ScenarioObject sO)
        {
            try
            {
                scenarioData.objects[sO.GetObjectNum() - 1] = sO.objectData;
                return true;
            }
            catch (Exception e)
            {
                UTILS.LogError($"[Critical Error ScoketManager] {e}");
                return false;
            }
        }
    }
}