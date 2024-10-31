using UnityEngine;

namespace GCRR.VirtualMap
{
    public class Option_UI_ONOFF : MonoBehaviour
    {
        [SerializeField] private bool isShowMark = true;

        /// 작성 - KKH
        /// <summary>
        /// 텍스트 및 표시 UI 켜기, 끄기
        /// </summary>
        public void Onclick()
        {
            isShowMark = !isShowMark;

            QuntizedControlClass.Instance.UIOnOff(isShowMark);
            QuntizedControlClass.Instance.isUISet = isShowMark;
        }        
    }
}