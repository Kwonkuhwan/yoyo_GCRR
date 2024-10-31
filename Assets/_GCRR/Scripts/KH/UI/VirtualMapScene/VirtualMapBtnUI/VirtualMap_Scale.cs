using Photon.Pun;

using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class VirtualMap_Scale : MonoBehaviour
    {
        [SerializeField] private QuntizedControlClass quntizedControlClass;
        [SerializeField] private Scrollbar scrollbar_Scale;        // KKH : 가상지도 스케일 스크롤바

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 확대/축소 UI 입력
        /// </summary>
        public void OnValueChanged()
        {
            if (scrollbar_Scale == null)
            {
                UTILS.LogError("[Critical Error VirtualMap_Scale] scrollbar_Scale is null.");
                return;
            }

            if (!GameManager.Instance.isTargetCreated)
            {
                scrollbar_Scale.value = 1;
                return;
            }

            if (!quntizedControlClass.isUpdateingScrollbar)
            {
                quntizedControlClass.Scale_Quntized(scrollbar_Scale.value);
            }
        }        
    }
}