using BNG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public struct ObjectSettingData
    {
        public int forceSize;
        public string forceCode;
        public string unitName;
        public string unitCode;
        public float unitHeight;
        public int unitDirection;
        public string unitMission;
        public bool unitAlly;
        public bool unitShowMark;
    }

    public class ObjectSettingPanel : MonoBehaviour
    {
        [Header("SecnarioObject_Setting")]
        [SerializeField] private TMP_Dropdown dropdown_setForceSize;
        [SerializeField] private InputField input_setForceCode;
        [SerializeField] private InputField input_setUnitName;
        [SerializeField] private InputField input_setUnitCode;
        [SerializeField] private InputField input_setUnitHeight;
        [SerializeField] private InputField input_setDirection;
        [SerializeField] private InputField input_setMission;
        [SerializeField] private Toggle toggle_setAlly;
        [SerializeField] private Toggle toggle_setShowForceMark;

        [SerializeField] private UnityEngine.UI.Button btn_Remove;
        [SerializeField] private UnityEngine.UI.Button btn_Move;

        private void Awake()
        {
            SetForceSizeDropdown();
            ClearObjectSettingData();
            input_setUnitHeight.enabled = false;
        }

        /// 작성 - KKH
        /// <summary>
        /// 부대 규모 드롭다운 설정
        /// </summary>
        private void SetForceSizeDropdown()
        {
            dropdown_setForceSize.options.Clear();
            for (int i = 0; i <= Enum.GetValues(typeof(ForceSizeType)).Length - 1; i++)
            {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = Enum.GetName(typeof(ForceSizeType), i);
                dropdown_setForceSize.options.Add(newData);
            }

            dropdown_setForceSize.value = 0;
        }

        /// 작성 - KKH
        /// <summary>
        /// 부대 규모 드롭다운 설정
        /// </summary>
        public void SetForceSize_ValueChange()
        {
            if (dropdown_setForceSize == null) return;

            // [2023.06.19] [추가] KKH : 설정한 부대 규모가 고정익 또는 회전익일때
            if (dropdown_setForceSize.value == (int)ForceSizeType.FixWing || dropdown_setForceSize.value == (int)ForceSizeType.RoateWing)
            {
                input_setUnitHeight.enabled = true;
            }
            else
            {
                input_setUnitHeight.enabled = false;
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 수정 선택시 UI패널에 표시될 데이터 세팅
        /// </summary>
        public void SetUIObjectSettingData(ObjectData objectData, bool isRevision)
        {
            btn_Remove.gameObject.SetActive(isRevision);
            btn_Move.gameObject.SetActive(isRevision);

            SetUIObjectSettingData(objectData);
        }

        public void SetUIObjectSettingData(ObjectData objectData)
        {
            if (objectData == null) return;

            dropdown_setForceSize.value = objectData.forceSize;
            input_setForceCode.text = objectData.forceCode;
            input_setUnitName.text = objectData.unitName;
            input_setUnitCode.text = objectData.unitCode;
            toggle_setAlly.isOn = objectData.isAlly;
            input_setUnitHeight.text = objectData.height.ToString();
            input_setDirection.text = objectData.movingDirection.ToString();
            input_setMission.text = objectData.mission;
            toggle_setShowForceMark.isOn = objectData.showForceMark;
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 세팅 UI 초기화
        /// </summary>
        public void ClearObjectSettingData()
        {
            input_setForceCode.gameObject.GetComponent<VRTextInput>().inputtedString = string.Empty;
            input_setUnitName.gameObject.GetComponent<VRTextInput>().inputtedString = string.Empty;
            input_setUnitCode.gameObject.GetComponent<VRTextInput>().inputtedString = string.Empty;
            input_setUnitHeight.gameObject.GetComponent<VRTextInput>().inputtedString = string.Empty;
            input_setDirection.gameObject.GetComponent<VRTextInput>().inputtedString = string.Empty;
            input_setMission.gameObject.GetComponent<VRTextInput>().inputtedString = string.Empty;

            dropdown_setForceSize.value = 0;
            input_setForceCode.text = string.Empty;
            input_setUnitName.text = string.Empty;
            input_setUnitCode.text = string.Empty;
            toggle_setAlly.isOn = false;
            input_setUnitHeight.text = string.Empty;
            input_setDirection.text = string.Empty;
            input_setMission.text = string.Empty;
            toggle_setShowForceMark.isOn = false;
        }

        public ObjectSettingData GetObjectSetting()
        {
            ObjectSettingData objSetting = new ObjectSettingData();
            objSetting.forceSize = dropdown_setForceSize.value;
            objSetting.forceCode = input_setForceCode.text;
            objSetting.unitName = input_setUnitName.text;
            objSetting.unitCode = input_setUnitCode.text;
            objSetting.unitHeight = input_setUnitHeight.text == string.Empty ? 0.0f : float.Parse(input_setUnitHeight.text);
            objSetting.unitDirection = input_setDirection.text == string.Empty ? 0 : int.Parse(input_setDirection.text);
            objSetting.unitMission = input_setMission.text;
            objSetting.unitAlly = toggle_setAlly.isOn;
            objSetting.unitShowMark = toggle_setShowForceMark.isOn;

            return objSetting;
        }
    }
}