using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class Option_ControlCheck : MonoBehaviour
    {
        [SerializeField] private VirtualMapUIManager virtualMapUIManager;

        /// 작성 - KKH
        /// <summary>
        /// 제어권 확인 UI 입려
        /// </summary>
        public void OnClick()
        {
            string decs = string.Empty;
            bool error = false;
            if (DataSyncManager.Instance.Check_Control(ScenarioInfoClass.Instance.scenarioList.control))
            {
                decs = $"제어권(을)를 소유하고 있습니다.";
            }
            else
            {
                decs = $"제어권(을)를 소유하고 있지 않습니다.\n현재 제어권(을)를 {ScenarioInfoClass.Instance.scenarioList.control}이 소유 하고 있습니다.";
                error = true;
            }

            virtualMapUIManager.ShowMessagePanelAction(decs, error);

            //StartCoroutine(virtualMapUIManager.ShowMessagePanel(decs, error));
        }
    }
}