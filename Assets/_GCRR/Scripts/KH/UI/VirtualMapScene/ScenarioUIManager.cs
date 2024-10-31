using UnityEngine;

namespace GCRR.VirtualMap
{
    public class ScenarioUIManager : MonoBehaviour
    {
        [Header("SecnarioObject_Info")]
        [SerializeField] private GameObject panel_ObjectInfo;
        [SerializeField] private ObjectInfoPanel objInfoPanel;
        public ObjectInfoPanel ObjInfoPanel => objInfoPanel;

        [Header("Object Move UI")]
        [SerializeField] private GameObject ui_MoveSelect;

        #region 시나리오 정보
        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 정보 Panel 켜기
        /// </summary>
        public void Info_ObjectPanelOn()
        {
            panel_ObjectInfo.SetActive(true);
        }

        public void Info_ObjectPanelOn(ObjectData objectData)
        {
            Info_ObjectPanelOn();
            objInfoPanel.SetObjectInfoData(objectData);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 정보 Panel 끄기
        /// </summary>
        public void Info_ObjectPanelOff()
        {
            panel_ObjectInfo.SetActive(false);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 정보 확인 버튼 클릭
        /// </summary>
        public void Info_ObjectOkBtn_Click()
        {
            ObjectSelectUIManager osUIM = UIManager.Instance.OSUIM;
            ScenarioObjectManager som = ScenarioObjectManager.Instance;
            if (osUIM == null) return;

            if (ScenarioObjectManager.Instance.isMoveOn)
            {
                GameObject sObj = som.select_Obj;
                if(!sObj.activeInHierarchy) sObj.SetActive(true);

                ScenarioObject sO = sObj.GetComponent<ScenarioObject>();
                Vector2 location = UTILS.GetObjLocation(ScenarioObjectManager.Instance.targetPos.transform.localPosition);

                sO.objectData.unitLat = location.x;
                sO.objectData.unitLon = location.y;

                // [2023.06.19] [추가] KKH : 객체 데이터 입력
                if (!sO.SetObjectData(sO.objectData))
                {
                    return;
                }

                // [2023.06.19] [추가] KKH : 시나리오 데이터 적용
                ScenarioObjectManager.Instance.ApplyObject(sObj);

                ScenarioObjectManager.Instance.ClearObject();

                // [2023.09.14] [추가] KKH : 이동 플래그 false 초기화
                ScenarioObjectManager.Instance.isMoveOn = false;
                // [2023.09.14] [추가] KKH : 오브젝트 정보 표시 Panel 데이터 수정
                panel_ObjectInfo.GetComponent<ObjectInfoPanel>().SetObjectInfoData(sO.objectData);
                // [2023.09.14] [추가] KKH : 이동 위치 표시 오브젝트 숨기기
                som.HideTargetPos();

                ui_MoveSelect.SetActive(false);
            }

            osUIM.RemoveScrollViewOutline();

            ScenarioObjectManager.Instance.ClearObject();
            objInfoPanel.ClearObjectInfoData();
            Info_ObjectPanelOff();
        }


        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 이동 버튼 클릭
        /// </summary>
        public void Info_ObjectMoveBtn_Click()
        {
            // [2023.06.19] [추가] KKH : 임시적으로 정보 UI 닫기
            Info_ObjectPanelOff();
            //// [2023.06.19] [추가] KKH : 임시적으로 설정 UI 닫기
            //Setting_ObjectPanelOff();
            // [2023.09.05] [추가] KKH : 이동모드 On
            ScenarioObjectManager.Instance.isMoveOn = true;

            ui_MoveSelect.SetActive(true);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 삭제 버튼 클릭
        /// </summary>
        public void Info_ObjectRemoveBtn_Click()
        {
            // [2023.06.19] [추가] KKH : 정보 UI 닫기
            Info_ObjectPanelOff();
            //// [2023.06.19] [추가] KKH : 설정 UI 닫기
            //Setting_ObjectPanelOff();
            // [2023.06.19] [추가] KKH : 객체 파괴 및 초기화
            ScenarioObjectManager.Instance.RemoveObject(true);
        }
        #endregion
    }
}