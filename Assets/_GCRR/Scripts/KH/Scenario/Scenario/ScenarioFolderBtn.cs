using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class ScenarioFolderBtn : MonoBehaviour
    {
        [SerializeField] private SelectModeUIManager smUIM;
        [SerializeField] private TextMeshProUGUI txtScenarioFolderName;

        public void SetBtn(SelectModeUIManager smuim, string scenarioFolderName)
        {
            smUIM = smuim;
            txtScenarioFolderName.text = scenarioFolderName;
        }

        public void Onclick()
        {
            if (smUIM == null) return;

            smUIM.ScenarioFolderRemoveOutline();

            smUIM.SelectFolderName = txtScenarioFolderName.text;
            StartCoroutine(smUIM.LoadSceanrioListScrollView(txtScenarioFolderName.text));

            GetComponent<Outline>().enabled = true;
        }
    }
}