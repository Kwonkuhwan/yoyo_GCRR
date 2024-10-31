using TMPro;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class ObjectInfoPanel : MonoBehaviour
    {
        [Header("SecnarioObject_Info")]
        [SerializeField] private TMP_Text text_setForceSize;
        [SerializeField] private TMP_Text text_setUnitCode;
        [SerializeField] private TMP_Text text_UnitLat;
        [SerializeField] private TMP_Text text_UnitLon;
        [SerializeField] private TMP_Text text_setUnitHeight;
        [SerializeField] private TMP_Text text_setDirection;
        [SerializeField] private TMP_Text text_setMission;

        private void Awake()
        {
            ClearObjectInfoData();
        }

        /// 작성 - KH
        /// <summary>
        /// 시나리오 객체 선택시 정보 UI패널에 표시될 데이터 세팅
        /// </summary>
        /// <param name="_sObj">선택된 시나리오 객체</param>
        public void SetObjectInfoData(ObjectData objectData)
        {
            if (objectData == null) return;

            text_setForceSize.text = ((ForceSizeType)objectData.forceSize).ToString();
            text_setUnitCode.text = objectData.unitCode;
            text_UnitLat.text = objectData.unitLat.ToString();
            text_UnitLon.text = objectData.unitLon.ToString();
            text_setUnitHeight.text = objectData.height.ToString();
            text_setDirection.text = objectData.movingDirection.ToString();
            text_setMission.text = objectData.mission;
        }

        /// 작성 - KH
        /// <summary>
        /// 시나리오 객체 정보 UI 초기화
        /// </summary>
        public void ClearObjectInfoData()
        {
            text_setForceSize.text = string.Empty;
            text_setUnitCode.text = string.Empty;
            text_UnitLat.text = string.Empty;
            text_UnitLon.text = string.Empty;
            text_setUnitHeight.text = string.Empty;
            text_setDirection.text = string.Empty;
            text_setMission.text = string.Empty;
        }
    }
}