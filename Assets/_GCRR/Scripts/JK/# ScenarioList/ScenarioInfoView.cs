using TMPro;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class ScenarioInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtForderName;
        [SerializeField] private TextMeshProUGUI txtScenarioName;
        [SerializeField] private TextMeshProUGUI txtScenarioLat;
        [SerializeField] private TextMeshProUGUI txtScenarioLon;
        [SerializeField] private TextMeshProUGUI txtScenarioZoomLevel;
        [SerializeField] private TextMeshProUGUI txtScenarioRange;
        [SerializeField] private TextMeshProUGUI txtScenarioDescription;
        [SerializeField] private TextMeshProUGUI txtScenarioCreateDate;
        [SerializeField] private TextMeshProUGUI txtScenarioRevisionDate;
        [SerializeField] private TextMeshProUGUI txtScenarioStatus;

        private ScenarioList scenario;

        public void ShowInfo(ScenarioList scenario_)
        {
            scenario = scenario_;

            txtForderName.text = scenario.folderName;
            txtScenarioName.text = scenario.scenarioName;
            txtScenarioLat.text = $"위도 : {scenario.lat}";
            txtScenarioLon.text = $"경도 : {scenario.lon}";
            // txtScenarioZoomLevel.text = scenario.zoomlevel;
            txtScenarioRange.text = scenario.range.ToString();
            txtScenarioDescription.text = scenario.description;
            txtScenarioCreateDate.text = scenario.creationDate;
            txtScenarioRevisionDate.text = scenario.revisionDate;
            //txtScenarioStatus.text = scenario.control == "" ? "대기중" : "진행중";
        }

        public void SetLat(string lat)
        {
            ScenarioInfoClass.Instance.scenarioList.lat = double.Parse(lat);
        }

        public void SetLon(string lon)
        {
            ScenarioInfoClass.Instance.scenarioList.lon = double.Parse(lon);
        }

        public void SetRange(string range)
        {
            ScenarioInfoClass.Instance.scenarioList.range = int.Parse(range);
            GameManager.Instance.nRange = int.Parse(range);
        }

        public void SetZoomLevel(string zoomLevel)
        {
            GameManager.Instance.zoomLevel = int.Parse(zoomLevel);
        }

        public void Clear()
        {
            scenario = null;

            txtForderName.text = string.Empty;
            txtScenarioName.text = string.Empty;
            txtScenarioLat.text = $"위도 : {string.Empty}";
            txtScenarioLon.text = $"경도 : {string.Empty}";
            // txtScenarioZoomLevel.text = scenario.zoomlevel;
            txtScenarioRange.text = string.Empty;
            txtScenarioDescription.text = string.Empty;
            txtScenarioCreateDate.text = string.Empty;
            txtScenarioRevisionDate.text = string.Empty;
            //txtScenarioStatus.text = scenario.control == "" ? "대기중" : "진행중";
        }
    }
}