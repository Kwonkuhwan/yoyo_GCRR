using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class Option_LocalSave : MonoBehaviour
    {
        [SerializeField] private VirtualMapUIManager virtualMapUIManager;

        [SerializeField] private GameObject LcalSavePanel;

        /// 작성 - KKH
        /// <summary>
        /// 로컬 저장 UI 입력
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
                if (LcalSavePanel == null) return;

                LcalSavePanel.SetActive(true);
            }
        }
    }
}