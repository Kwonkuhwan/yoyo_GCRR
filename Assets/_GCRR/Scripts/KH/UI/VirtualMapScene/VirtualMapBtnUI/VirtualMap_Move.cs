using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class VirtualMap_Move : MonoBehaviour
    {
        [SerializeField] private QuntizedControlClass quntizedControlClass;
        [SerializeField] private GameObject moveGameObj;
        [SerializeField] private GameObject roateGameObj;
        [SerializeField] private GameObject scaleGameObj;
        [SerializeField] private Button moveBtn;
        [SerializeField] private Button rotateBtn;
        [SerializeField] private Button scaleBtn;

        [SerializeField] private Sprite deactive_Img;

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 이동 UI 입력
        /// </summary>
        /// <param name="quntizedDirection">가상지도 이동 방향</param>
        public void OnClick(int quntizedDirection)
        {
            if (!GameManager.Instance.isTargetCreated) return;
            quntizedControlClass.Move_Quntized((QuntizedDirection)quntizedDirection);
            quntizedControlClass.quntizedDir = (QuntizedDirection)quntizedDirection;

            moveBtn.image.sprite = deactive_Img;
            rotateBtn.image.sprite = deactive_Img;
            scaleBtn.image.sprite = deactive_Img;
            moveGameObj.SetActive(false);
            roateGameObj.SetActive(false);
            scaleGameObj.SetActive(false);
        }
    }
}