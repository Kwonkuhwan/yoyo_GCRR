using UnityEngine;

namespace GCRR.VirtualMap
{
    public class Option_CloudUpdate : MonoBehaviour
    {
        [SerializeField] private VirtualMapUIManager virtualMapUIManager;

        /// 작성 - KKH
        /// <summary>
        /// 변경사항 업데이트 UI 입력 (클라우드에 전송)
        /// </summary>
        public void OnClick()
        {
            if (GameManager.Instance.SelectMode == SelectScenrioMode.Location)
            {
                string decs = $"위경도 모드에서는 실행할 수 없습니다.";
                virtualMapUIManager.ShowMessagePanelAction(decs, true);

                //StartCoroutine(virtualMapUIManager.ShowMessagePanel(decs, true));
            }
            else
            {
                try
                {
                    if (GameManager.Instance.isCloud)
                    {
                        ScenarioInfoClass sic = ScenarioInfoClass.Instance;
                        C_SaveScenario c_SaveScenario = new C_SaveScenario(
                            sic.playerID, sic.scenarioData.scenarioID,
                            sic.scenarioData.scenarioName, sic.scenarioData.folderName,
                            sic.scenarioData.latitude, sic.scenarioData.longitude,
                            sic.scenarioData.writer, sic.scenarioData.mapType,
                            sic.scenarioData.scale, sic.scenarioData.range,
                            sic.scenarioData.description, sic.scenarioData.writeDay);

                        if (TcpSocketManager.Instance.tcpClient != null)
                        {
                            TcpSocketManager.Instance.tcpClient.Send(c_SaveScenario.Serialize());
                        }
                    }
                }
                catch
                {
                    virtualMapUIManager.ShowMessagePanelAction($"클라우드 저장에 문제가 있습니다.", true);
                }
                //StartCoroutine(virtualMapUIManager.ShowMessagePanel(decs, error));
            }
        }
    }
}