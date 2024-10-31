using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    [Serializable]
    public class MessageData
    {
        public string msg;
        public bool isError;

        public MessageData(string msg, bool isError)
        {
            this.msg = msg;
            this.isError = isError;
        }
    }

    public class VirtualMapUIManager : MonoBehaviour
    {
        private static VirtualMapUIManager instance;
        public static VirtualMapUIManager Instance => instance;

        #region Quntized
        [Header("Quntized")]
        [SerializeField] private GameObject panel_Object;
        [SerializeField] private GameObject panel_QuntizedMove;       // KKH : 가상지도 이동 Panel
        [SerializeField] private GameObject panel_QuntizedRotation;   // KKH : 가상지도 회전 Panel
        [SerializeField] private GameObject panel_QuntizedScale;      // KKH : 가상지도 스케일 Panel
        [SerializeField] private GameObject panel_QuntizedSetting;    // KKH : 가상지도 세팅 Panel
        [SerializeField] private GameObject panel_ObjectSelect;       // KKH : 오브젝트 선택 Panel
        [SerializeField] private GameObject panel_Message;            // KKH : 메시지 Panel

        [Space(10)]
        [Header("Button")]
        [SerializeField] private Button btn_QuntizedMove;
        [SerializeField] private Button btn_QuntizedRotation;
        [SerializeField] private Button btn_QuntizedScale;
        [SerializeField] private Button btn_QuntizedSetting;
        [SerializeField] private Button btn_ObjectSelect;

        //[Space(10)]
        //public Outline outline_MoveBtn;             // KKH : 가상지도 이동 outline
        //public Outline outline_RotationBtn;         // KKH : 가상지도 회전 outline
        //public Outline outline_ScaleBtn;            // KKH : 가상지도 스케일 outline
        //public Outline outline_SettingBtn;          // KKH : 가상지도 세팅 outline
        //public Outline outline_ObjectAddBtn;        // KKH : 오브젝트 추가 outline
        //public Outline outline_ObjectSelectBtn;     // KKH : 오브젝트 선택 outline

        [Space(10)]
        [SerializeField] private TMP_Text text_Message;               // KKH : 메시지 표시 텍스트
        #endregion

        [Space(10)]
        private float timeSpan;
        [SerializeField] private TMP_Text text_ProgressTime;
        [SerializeField] private TMP_Text text_ScenarioMode;

        [SerializeField] private Sprite sprite_MessageBg_Green;
        [SerializeField] private Sprite sprite_MessageBg_Red;

        private bool isMovePanel = false;
        private bool isScalePanel = false;
        private bool isRotatePanel = false;
        private bool isSelectPanel = false;

        public Action<string, bool> OnMessageEnqeue;

        public Queue<MessageData> messageDataQueue = new Queue<MessageData>();

        private Action<string, bool> ShowmessagePanelAction;
        public Action<string, bool> ShowMessagePanelAction => ShowmessagePanelAction;

        public TMP_Text TargetCheck_Text;

        private void Awake()
        {
            if(instance != this)
            {
                instance = this;
            }

            btn_QuntizedMove.onClick.AddListener(() => QuntizedMovePanelOnOff());
            btn_QuntizedRotation.onClick.AddListener(() => QuntizedRotaionPanelOnOff());
            btn_QuntizedScale.onClick.AddListener(() => QuntizedScalePanelOnOff());
            btn_QuntizedSetting.onClick.AddListener(() => QuntizedSettingPanelOnOff());
            btn_ObjectSelect.onClick.AddListener(() => ObjectSelectPanelOnOff());

            OnMessageEnqeue = PlayMessageQueue;
            ShowmessagePanelAction = PlayMessagePanel;

#if UNITY_EDITOR
            TargetCheck_Text.gameObject.SetActive(true);
#endif
        }

        private void Start()
        {
            if (GameManager.Instance.SelectMode == SelectScenrioMode.Location)
            {
                panel_Object.SetActive(false);
                text_ScenarioMode.text = $"위경도 모드";
            }
            else
            {
                panel_Object.SetActive(true);
                if (GameManager.Instance.SelectMode == SelectScenrioMode.Local)
                {
                    text_ScenarioMode.text = $"시나리오 모드 - 로컬";
                }
                else
                {
                    text_ScenarioMode.text = $"시나리오 모드 - 클라우드";
                }
            }

            panel_QuntizedMove.SetActive(false);
            panel_QuntizedRotation.SetActive(false);
            panel_QuntizedScale.SetActive(false);
            panel_QuntizedSetting.SetActive(false);
            panel_Message.SetActive(false);

            timeSpan = 0.0f;

            isMovePanel = panel_QuntizedMove.activeInHierarchy;
            isRotatePanel = panel_QuntizedRotation.activeInHierarchy;
            isScalePanel = panel_QuntizedScale.activeInHierarchy;
            isSelectPanel = panel_ObjectSelect.activeInHierarchy;
        }

        private void Update()
        {
            timeSpan += Time.deltaTime;
            text_ProgressTime.text = $"진행시간 {TimeSpan.FromSeconds(timeSpan).ToString(@"hh\:mm\:ss")}";

            if(messageDataQueue.Count > 0 )
            {
                MessageData messageData = messageDataQueue.Dequeue();
                PlayMessagePanel(messageData.msg,messageData.isError);
            }

#if UNITY_EDITOR
            TargetCheck_Text.text = $"{GameManager.Instance.fCreateTimeCheck}";
#endif
        }

        #region 가상지도 관련
        /// 작성 - KKH
        /// <summary>
        /// Hide시킨 Panel 다시 원복
        /// </summary>
        public void PanelShow()
        {
            panel_QuntizedMove.SetActive(isMovePanel);
            panel_QuntizedRotation.SetActive(isRotatePanel);
            panel_QuntizedScale.SetActive(isScalePanel);
            panel_ObjectSelect.SetActive(isSelectPanel);
        }

        /// 작성 - KKH
        /// <summary>
        /// 불필요한 Panel Hide
        /// </summary>
        public void PanelHide()
        {
            isMovePanel = panel_QuntizedMove.activeInHierarchy;
            isRotatePanel = panel_QuntizedRotation.activeInHierarchy;
            isScalePanel = panel_QuntizedScale.activeInHierarchy;
            isSelectPanel = panel_ObjectSelect.activeInHierarchy;

            panel_QuntizedMove.SetActive(false);
            panel_QuntizedRotation.SetActive(false);
            panel_QuntizedScale.SetActive(false);
            panel_ObjectSelect.SetActive(false);
        }

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 이동 Panel, Outline 켜기, 끄기
        /// </summary>
        public void QuntizedMovePanelOnOff()
        {
            if (panel_QuntizedSetting.activeInHierarchy) return;

            if (panel_QuntizedMove.activeInHierarchy)
            {
                //outline_MoveBtn.enabled = false;
                btn_QuntizedMove.gameObject.GetComponent<ButtonControl>().ButtonOff();
                panel_QuntizedMove.SetActive(false);
            }
            else
            {
                //outline_MoveBtn.enabled = true;
                btn_QuntizedMove.gameObject.GetComponent<ButtonControl>().ButtonOn();
                panel_QuntizedMove.SetActive(true);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 회전 Panel, Outline 켜기, 끄기
        /// </summary>
        public void QuntizedRotaionPanelOnOff()
        {
            if (panel_QuntizedSetting.activeInHierarchy) return;

            if (panel_QuntizedRotation.activeInHierarchy)
            {
                btn_QuntizedRotation.gameObject.GetComponent<ButtonControl>().ButtonOff();
                panel_QuntizedRotation.SetActive(false);
            }
            else
            {
                btn_QuntizedRotation.gameObject.GetComponent<ButtonControl>().ButtonOn();
                panel_QuntizedRotation.SetActive(true);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 스케일 Panel, Outline 켜기, 끄기
        /// </summary>
        public void QuntizedScalePanelOnOff()
        {
            if (panel_QuntizedSetting.activeInHierarchy) return;

            if (panel_QuntizedScale.activeInHierarchy)
            {
                btn_QuntizedScale.gameObject.GetComponent<ButtonControl>().ButtonOff();
                panel_QuntizedScale.SetActive(false);
            }
            else
            {
                btn_QuntizedScale.gameObject.GetComponent<ButtonControl>().ButtonOn();
                panel_QuntizedScale.SetActive(true);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 세팅 Panel, Outline 켜기, 끄기
        /// </summary>
        public void QuntizedSettingPanelOnOff()
        {
            if (panel_QuntizedSetting.activeInHierarchy)
            {
                PanelShow();

                btn_QuntizedSetting.gameObject.GetComponent<ButtonControl>().ButtonOff();
                panel_QuntizedSetting.SetActive(false);
            }
            else
            {
                PanelHide();

                btn_QuntizedSetting.gameObject.GetComponent<ButtonControl>().ButtonOn();
                panel_QuntizedSetting.SetActive(true);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 오브젝트 선택 Panel, Outline 켜기, 끄기
        /// </summary>
        public void ObjectSelectPanelOnOff()
        {
            if (panel_QuntizedSetting.activeInHierarchy) return;

            ObjectSelectUIManager osUIM = UIManager.Instance.OSUIM;
            if (osUIM == null) return;

            if (panel_ObjectSelect.activeInHierarchy)
            {
                btn_ObjectSelect.gameObject.GetComponent<ButtonControl>().ButtonOff();
                panel_ObjectSelect.SetActive(false);
            }
            else
            {
                btn_ObjectSelect.gameObject.GetComponent<ButtonControl>().ButtonOn();
                osUIM.RemoveScrollViewOutline();
                panel_ObjectSelect.SetActive(true);
            }
        }

        public void PlayMessageQueue(string msg, bool isError)
        {
            messageDataQueue.Enqueue(new MessageData(msg, isError));
        }

        public void PlayMessagePanel(string decs, bool error)
        {
            StartCoroutine(ShowMessagePanel(decs, error));
        }

        /// 작성 - KKH
        /// <summary>
        /// 설정창 버튼(로컬 저장, 클라우드 저장 등) 클릭시 메시지 Panel 보여주기
        /// </summary>
        /// <param name="decs"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public IEnumerator ShowMessagePanel(string decs, bool error)
        {
            panel_Message.SetActive(true);
            Image panelImage = panel_Message.gameObject.GetComponent<Image>();

            if (error) panelImage.sprite = sprite_MessageBg_Red;
            else panelImage.sprite = sprite_MessageBg_Green;

            text_Message.text = decs;

            yield return new WaitForSeconds(2.0f);

            panel_Message.SetActive(false);
        }
        #endregion
    }
}