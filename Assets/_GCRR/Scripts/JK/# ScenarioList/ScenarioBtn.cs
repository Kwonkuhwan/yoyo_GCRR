using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class ScenarioBtn : MonoBehaviour
    {
        private SelectModeUIManager smUIM;
        [SerializeField] private TextMeshProUGUI txtScenarioName;
        [SerializeField] private TextMeshProUGUI txtScenarioLocate;
        [SerializeField] private TextMeshProUGUI txtScenarioStatus;

        private ScenarioList scenario;
        private ScenarioInfoView infoView;

        public void SetBtn(SelectModeUIManager smuim_, ScenarioList scenario_, ScenarioInfoView infoView_)
        {
            smUIM = smuim_;

            scenario = scenario_;
            infoView = infoView_;

            txtScenarioName.text = scenario.scenarioName;
            var loca_text = $"위도 : {scenario.lat} \t 경도 : {scenario.lon} \n {scenario.description}";
            txtScenarioLocate.text = loca_text;
            txtScenarioStatus.text = scenario.isPlaying == 0 ? "대기중" : "진행중";
        }

        public void Onclick()
        {
            if (smUIM == null) return;
            smUIM.ScenarioListRemoveOutline();

            infoView.ShowInfo(scenario);
            ScenarioInfoClass.Instance.scenarioList = scenario;

            if (scenario.scenarioID != null)
            {
                StartCoroutine(ScenarioInfoClass.Instance.LoadScenarioData(scenario.scenarioID));
            }

            GetComponent<Outline>().enabled = true;
        }
    }
}