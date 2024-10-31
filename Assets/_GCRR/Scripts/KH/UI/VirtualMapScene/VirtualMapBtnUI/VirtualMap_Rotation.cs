using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class VirtualMap_Rotation : MonoBehaviour
    {
        [SerializeField] private QuntizedControlClass quntizedControlClass;
        [SerializeField] private Scrollbar scrollbar_Rotation;        // KKH : 가상지도 회전 스크롤바

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 회전 UI 입력
        /// </summary>
        public void OnValueChanged()
        {
            if (scrollbar_Rotation == null)
            {
                UTILS.LogError("[Critical Error VirtualMap_Rotation] scrollbar_Rotation is null.");
                return;
            }

            if (!GameManager.Instance.isTargetCreated)
            {
                scrollbar_Rotation.value = 0;
                return;
            }

            if (!quntizedControlClass.isUpdateingScrollbar)
            {
                quntizedControlClass.Rotation_Quntized(scrollbar_Rotation.value);
            }
        }
    }
}