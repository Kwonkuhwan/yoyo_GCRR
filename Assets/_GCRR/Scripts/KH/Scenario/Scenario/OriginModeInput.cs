using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class OriginModeInput : MonoBehaviour
    {
        private ScenarioInfoView infoView;
        [SerializeField] private InputField input_Lat;
        [SerializeField] private InputField input_Lon;
        [SerializeField] private TMP_Dropdown dropdown_ZoomLevel;
        [SerializeField] private TMP_Dropdown dropdown_Range;

        private void Awake()
        {
            //sic = ScenarioInfoClass.Instance;
            infoView = SelectModeUIManager.Instance.InfoView;
        }

        private void Start()
        {
            ResetDropdown(dropdown_ZoomLevel, 11, 19);
            ResetDropdown(dropdown_Range, 1, 5);

            dropdown_ZoomLevel.value = 0;
            dropdown_Range.value = 0;

            GameManager.Instance.zoomLevel = int.Parse(dropdown_ZoomLevel.options[dropdown_ZoomLevel.value].text);
            GameManager.Instance.nRange = int.Parse(dropdown_Range.options[dropdown_Range.value].text);
        }

        private void ResetDropdown(TMP_Dropdown dropdown, int min, int max)
        {
            dropdown.options.Clear();

            for (int i = min; i <= max; i++)
            {
                if (i % 2 == 0) continue;
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = i.ToString();
                dropdown.options.Add(newData);
            }
        }

        public void InputField_EndEdit_Lat()
        {
            infoView.SetLat(input_Lat.text);
        }

        public void InputField_EndEdit_Lon()
        {
            infoView.SetLon(input_Lon.text);
        }

        public void Dropdown_ValueChanged_ZoomLevel()
        {
            infoView.SetZoomLevel(dropdown_ZoomLevel.options[dropdown_ZoomLevel.value].text);
        }

        public void Dropdown_ValueChanged_Range()
        {
            infoView.SetRange(dropdown_Range.options[dropdown_Range.value].text);
        }
    }
}