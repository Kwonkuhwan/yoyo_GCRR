using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class SelectModeUIManager : MonoBehaviour
    {
        private static SelectModeUIManager instance;
        public static SelectModeUIManager Instance => instance;

        #region ModeSelect
        [Header("ModeSelect")]
        #region Panel
        [SerializeField] private GameObject panel_SelectMode;                 // KKH : 모드 선택 Panel
        [SerializeField] private GameObject panel_ContentCommand;             // KKH : 지리공간 정보 전시/시나리오 기반 작전상황 전시 선택 Panel

        [SerializeField] private GameObject panel_SelectLocalMode;            // KKH : 로컬 서버, 클라이언트 선택

        // 지역 검색 모드
        [SerializeField] private GameObject panel_L_LocationMode;             // JK : 지리공간 정보 전시 모드 Panel

        // 시나리오 폴더
        [SerializeField] private GameObject panel_L_ScenarioFolder;

        // 시나리오 목록
        [SerializeField] private GameObject panel_L_ScenarioMode;             // JK : 시나리오 기반 작전상황 전시 모드 Panel
        [SerializeField] private GameObject panel_R_ScenarioMode;
        #endregion

        [SerializeField] private TMP_Text text_ModeName;
        [SerializeField] private GameObject btn_Refresh;

        [SerializeField] private GameObject btn_Scenario;             // KKH : 스크롤뷰에 들어갈 버튼 아이템
        [SerializeField] private Transform view_ScenarioList;         // JK : 시나리오 리스트 뷰 Transform
        [SerializeField] private ScenarioInfoView infoView;           // JK : 시나리오 정보 뷰
        public ScenarioInfoView InfoView => infoView;
        [SerializeField] private GameObject btn_ScenarioFolder;
        [SerializeField] private Transform view_ScenarioFolder;

        [SerializeField] private string selectFolderName;
        public string SelectFolderName { get => selectFolderName; set => selectFolderName = value; }
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        #region 모드 선택 관련
        /// 작성 - KKH
        /// <summary>
        /// 모드(로컬, 클라우드, 위경도) 선택
        /// </summary>
        /// <param name="selectScenrioMode"></param>
        public void Input_SelectModeBtn(int selectScenrioMode)
        {
            GameManager.Instance.SelectMode = (SelectScenrioMode)selectScenrioMode;
            // [2023.06] [추가] KKH : 선택 모드 Panel 비활성화
            panel_SelectMode.SetActive(false);

            if ((SelectScenrioMode)selectScenrioMode != SelectScenrioMode.Local)
            {
                // [2023.06] [추가] KKH : 지리공간 정보 전시/시나리오 기반 작전상황 전시 선택 Panel 활성화
                panel_ContentCommand.SetActive(true);
            }

            if ((SelectScenrioMode)selectScenrioMode == SelectScenrioMode.Location)
            {
                LoadModeLocation();
            }
            else if ((SelectScenrioMode)selectScenrioMode == SelectScenrioMode.Local)
            {
                LoadModeLocal();
            }
            else if ((SelectScenrioMode)selectScenrioMode == SelectScenrioMode.Cloud)
            {
                LoadModeCloud();
            }
        }

        private void LoadModeLocation()
        {
            text_ModeName.text = $"VR 시나리오 기반 작전 상황 전시\n위경도 기반 모드";

            LocationPanelOn();
            GameManager.Instance.isLocation = true;
            ScenarioInfoClass.Instance.scenarioList.Clear();
        }

        private void LoadModeLocal()
        {
            text_ModeName.text = $"VR 시나리오 기반 작전 상황 전시\n로컬 모드";

            GameManager.Instance.isLocation = false;
            panel_SelectLocalMode.SetActive(true);
        }

        private void LoadModeCloud()
        {
            text_ModeName.text = $"VR 시나리오 기반 작전 상황 전시 - 클라우드 모드";
            GameManager.Instance.isLocation = false;
            SetLoadScenario();
        }

        public void LoadSelectLocalMode(bool isServer)
        {
            if (isServer)
            {
                text_ModeName.text = $"VR 시나리오 기반 작전 상황 전시\n로컬 모드 : 서버";
            }
            else
            {
                text_ModeName.text = $"VR 시나리오 기반 작전 상황 전시\n로컬 모드 : 클라이언트";
            }

            GameManager.Instance.isServer = isServer;
            if (!isServer)
            {
                GameManager.Instance.EnableTcpSocket();
            }

            panel_SelectLocalMode.SetActive(false);

            SetLoadScenario();
        }

        private void SetLoadScenario()
        {
            ScenarioPanelOn();
            panel_ContentCommand.SetActive(true);

            StartCoroutine(LoadScenarioFolderScrollView());
        }

        private void LocationPanelOn()
        {
            panel_L_ScenarioFolder.SetActive(false);
            panel_L_ScenarioMode.SetActive(false);
            panel_R_ScenarioMode.SetActive(false);
            panel_L_LocationMode.SetActive(true);
            btn_Refresh.SetActive(false);
        }

        private void ScenarioPanelOn()
        {
            panel_L_ScenarioFolder.SetActive(true);
            panel_L_ScenarioMode.SetActive(true);
            panel_R_ScenarioMode.SetActive(true);
            panel_L_LocationMode.SetActive(false);
            btn_Refresh.SetActive(true);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시작 버튼 클릭
        /// </summary>
        public void Input_StartBtn()
        {
            if (!GameManager.Instance.isLocation)
            {
                //GameManager.Instance.EnableNetWorkManager();
                if (string.IsNullOrEmpty(ScenarioInfoClass.Instance.scenarioData.scenarioName)) return;

            }
            else
            {
                GameManager.Instance.SetCoordinates((float)ScenarioInfoClass.Instance.scenarioList.lat, (float)ScenarioInfoClass.Instance.scenarioList.lon);
            }
            StartCoroutine(ScenarioInfoClass.Instance.LoadTimeSeriesData());
            //StartCoroutine(UTILS.LoadScene("02.Loding"));
            StartCoroutine(UTILS.LoadScene("03.VirtualMap"));
        }

        /// 작성 - KKH
        /// <summary>
        /// 새로 고침 버튼 클릭
        /// </summary>
        public void Input_RefreshBtn()
        {
            StartCoroutine(LoadScenarioFolderScrollView());
        }

        /// 작성 - KKH
        /// <summary>
        /// 돌아가기 클릭
        /// </summary>
        public void Input_BackBtn()
        {
            GameManager.Instance.DisableTcpSocket();
            ScenarioInfoClass.Instance.ClearScenario();
            RemoveScenarioFolder();
            RemoveScenarioList();
            infoView.Clear();

            // [2023.06] [추가] KKH : 선택 모드 Panel 비활성화
            panel_SelectMode.SetActive(true);
            // [2023.06] [추가] KKH : 지리공간 정보 전시/시나리오 기반 작전상황 전시 선택 Panel 활성화
            panel_ContentCommand.SetActive(false);
            panel_SelectLocalMode.SetActive(false);

            text_ModeName.text = $"VR 시나리오 기반 작전 상황 전시 - 모드 선택";
        }
        #endregion

        #region 시나리오 폴더
        /// 작성 - KKH
        /// <summary>
        /// 시나리오 폴더 스크롤뷰 초기화
        /// </summary>
        public void RemoveScenarioFolder()
        {
            if (view_ScenarioFolder.childCount <= 0) return;

            // child 에는 부모와 자식이 함께 설정 된다.
            var childs = view_ScenarioFolder.GetComponentsInChildren<ScenarioFolderBtn>();

            foreach (var iter in childs)
            {
                Destroy(iter.gameObject);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 폴더 스크롤뷰 불러오기
        /// </summary>
        public IEnumerator LoadScenarioFolderScrollView()
        {
            RemoveScenarioFolder();
            RemoveScenarioList();
            infoView.Clear();

            if (GameManager.Instance.SelectMode == SelectScenrioMode.Local && !GameManager.Instance.isServer)
            {
                yield return new WaitForSeconds(0.1f);
            }

            ScenarioInfoClass.Instance.LoadScenarioList();

            yield return new WaitForSeconds(0.1f);

            if (ScenarioInfoClass.Instance.scenarioLists != null)
            {
                List<string> folderNames = new List<string>();
                foreach (var scenario in ScenarioInfoClass.Instance.scenarioLists)
                {
                    folderNames.Add(scenario.folderName.Trim());
                }

                folderNames = folderNames.Distinct().ToList();

                foreach (var folderName in folderNames)
                {
                    var scView = Instantiate(btn_ScenarioFolder, view_ScenarioFolder);
                    scView.GetComponent<ScenarioFolderBtn>().SetBtn(this, folderName);
                }

                ScenarioFolderRemoveOutline();
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 폴더 버튼 아웃라인 지우기
        /// </summary>
        public void ScenarioFolderRemoveOutline()
        {
            var childs = view_ScenarioFolder.GetComponentsInChildren<ScenarioFolderBtn>();

            foreach (var iter in childs)
            {
                iter.GetComponent<Outline>().enabled = false;
            }
        }
        #endregion

        #region 시나리오 목록
        /// 작성 - JK
        /// 수정 - KKH : Transform을 받아서 초기화 할 수 있도록 수정 (2023.05.23)
        /// <summary>
        /// 시나리오 목록 스크롤뷰 초기화
        /// </summary>
        public void RemoveScenarioList()
        {
            if (view_ScenarioList.childCount <= 0) return;

            // child 에는 부모와 자식이 함께 설정 된다.
            var childs = view_ScenarioList.GetComponentsInChildren<ScenarioBtn>();

            foreach (var iter in childs)
            {
                Destroy(iter.gameObject);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 목록 스크롤뷰 불러오기
        /// </summary>
        public void LoadSceanrioListScrollView()
        {
            StartCoroutine(LoadSceanrioListScrollView(string.Empty));
        }

        public IEnumerator LoadSceanrioListScrollView(string folderName)
        {
            RemoveScenarioList();
            //ScenarioInfoClass.Instance.LoadScenarioList();

            yield return new WaitForSeconds(0.1f);

            if (ScenarioInfoClass.Instance.scenarioLists != null)
            {

                foreach (var scenario in ScenarioInfoClass.Instance.scenarioLists)
                {
                    if (folderName != string.Empty)
                    {
                        if (scenario.folderName != folderName) continue;
                    }

                    ScenarioList newScenarioList = (ScenarioList)scenario.Clone();

                    var scView = Instantiate(btn_Scenario, view_ScenarioList);
                    scView.GetComponent<ScenarioBtn>().SetBtn(this, newScenarioList, infoView);
                }

                ScenarioListRemoveOutline();
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 목록 버튼 아웃라인 지우기
        /// </summary>
        public void ScenarioListRemoveOutline()
        {
            var childs = view_ScenarioList.GetComponentsInChildren<ScenarioBtn>();

            foreach (var iter in childs)
            {
                iter.GetComponent<Outline>().enabled = false;
            }
        }
        #endregion
    }
}