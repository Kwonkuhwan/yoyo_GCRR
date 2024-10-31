using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class UI_TimeSeriesPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdwon_TimeSeries;
        [SerializeField] private Button btn_OK;

        private void Awake()
        {
            ResetDropdown(dropdwon_TimeSeries);
            btn_OK.onClick.AddListener(() => OkButtonClick());
        }

        private void ResetDropdown(TMP_Dropdown dropdown)
        {
            dropdown.ClearOptions();

            if (ScenarioInfoClass.Instance.tileseriesLists == null) return;

            foreach (ImageLayer imageLayer in ScenarioInfoClass.Instance.tileseriesLists.imageLayer)
            {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = imageLayer.Title;
                dropdown.options.Add(newData);
            }
        }

        private void OkButtonClick()
        {
            ScenarioInfoClass.Instance.timeType = dropdwon_TimeSeries.value;

            // 적용 기능 추가
            StartCoroutine(VirtualMapManager.Instance.CreateMap());

            gameObject.SetActive(false);
        }
    }
}