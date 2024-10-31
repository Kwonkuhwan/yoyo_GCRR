/*************************************************************************************************************************
 * 
 * 최초 작성자           : 권구환
 * 작성 일자            : 2023.05.12
 * 작성 목록            : 변수 및 함수 선언
 * 
 * 수정 사항
 * 수정자 및 수정 일시  : kkh
 * 수정 내용           : 
 * 
 *************************************************************************************************************************/

using GoShared;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class QuntizedControlClass : MonoBehaviour/*, IPunObservable*/
    {
        private static QuntizedControlClass instance = null;
        public static QuntizedControlClass Instance => instance;

        [SerializeField] Scrollbar scrollbar_rotate;
        [SerializeField] Scrollbar scrollbar_scale;
        public bool isUISet = true;
        public QuntizedDirection quntizedDir = QuntizedDirection.None;

        private void Awake()
        {
            if (instance == null || instance != this)
            {
                instance = this;
            }
        }

        private Vector2 movePos = Vector2.zero;

        public bool isUpdateingScrollbar = false;

        /// <summary>
        /// 가상지도 이동
        /// </summary>
        [PunRPC]
        public void Move_Quntized(QuntizedDirection quntizedDirection, bool isUI = true)
        {
            if (quntizedDirection == QuntizedDirection.None) return;

            if (quntizedDirection == QuntizedDirection.Up || quntizedDirection == QuntizedDirection.Down)
            {
                if (quntizedDirection == QuntizedDirection.Up)
                {
                    //if (movePos.y >= maxRadius) return;
                    movePos.y += 3;

                    // [2023.05.12] kkh : 현재 터레인 중심점 변경
                    GameManager.Instance.CurrentTerrainCenter = new Vector2(GameManager.Instance.CurrentTerrainCenter.x, GameManager.Instance.CurrentTerrainCenter.y + 3);
                    // [2023.05.12] kkh : 현재 텍스쳐 중심점 변경
                    GameManager.Instance.CurrentTextureCenter = new Vector2(GameManager.Instance.CurrentTextureCenter.x, GameManager.Instance.CurrentTextureCenter.y - 3);
                }
                else
                {
                    //if (movePos.y <= -(maxRadius)) return;
                    movePos.y -= 3;

                    // [2023.05.12] kkh : 현재 터레인 중심점 변경
                    GameManager.Instance.CurrentTerrainCenter = new Vector2(GameManager.Instance.CurrentTerrainCenter.x, GameManager.Instance.CurrentTerrainCenter.y - 3);
                    // [2023.05.12] kkh : 현재 텍스쳐 중심점 변경
                    GameManager.Instance.CurrentTextureCenter = new Vector2(GameManager.Instance.CurrentTextureCenter.x, GameManager.Instance.CurrentTextureCenter.y + 3);
                }
            }
            else if (quntizedDirection == QuntizedDirection.Left || quntizedDirection == QuntizedDirection.Right)
            {
                if (quntizedDirection == QuntizedDirection.Left)
                {
                    //if (movePos.x <= -(maxRadius)) return;
                    movePos.x -= 3;

                    // [2023.05.12] kkh : 현재 터레인 중심점 변경
                    GameManager.Instance.CurrentTerrainCenter = new Vector2(GameManager.Instance.CurrentTerrainCenter.x - 3, GameManager.Instance.CurrentTerrainCenter.y);
                    // [2023.05.12] kkh : 현재 텍스쳐 중심점 변경
                    GameManager.Instance.CurrentTextureCenter = new Vector2(GameManager.Instance.CurrentTextureCenter.x - 3, GameManager.Instance.CurrentTextureCenter.y);
                }
                else
                {
                    //if (movePos.x >= maxRadius) return;
                    movePos.x += 3;

                    // [2023.05.12] kkh : 현재 터레인 중심점 변경
                    GameManager.Instance.CurrentTerrainCenter = new Vector2(GameManager.Instance.CurrentTerrainCenter.x + 3, GameManager.Instance.CurrentTerrainCenter.y);
                    // [2023.05.12] kkh : 현재 텍스쳐 중심점 변경
                    GameManager.Instance.CurrentTextureCenter = new Vector2(GameManager.Instance.CurrentTextureCenter.x + 3, GameManager.Instance.CurrentTextureCenter.y);
                }
            }

            GameManager.Instance.StartCoordinates = new Coordinates(GameManager.Instance.CurrentTextureCenter, GameManager.Instance.zoomLevel);
            //Empty_Create_Terrain(quntizedDirection, nRadius);
            StartCoroutine(VirtualMapManager.Instance.CreateMap());

            if (!GameManager.Instance.isCloud && isUI)
            {
                PhotonView pv = GetComponent<PhotonView>();
                pv.RPC("Move_Quntized", RpcTarget.Others, quntizedDirection, false);
                scrollbar_rotate.value = 0.0f;
                scrollbar_scale.value = 1.0f;
            }
        }

        /// <summary>
        /// 가상지도 회전
        /// </summary>
        /// <param name="value">가상지도 회전 값</param>
        [PunRPC]
        public void Rotation_Quntized(float value, bool isUI = true)
        {
            VirtualMapManager virtualMapManager = VirtualMapManager.Instance;

            virtualMapManager.TrVirtualMap.rotation = Quaternion.Euler(0, value * 360, 0);

            if (!isUI)
            {
                isUpdateingScrollbar = true;
                scrollbar_rotate.value = value;
                isUpdateingScrollbar = false;
            }
            else if (!GameManager.Instance.isCloud && isUI)
            {
                PhotonView pv = GetComponent<PhotonView>();
                pv.RPC("Rotation_Quntized", RpcTarget.Others, value, false);
            }
        }

        /// <summary>
        /// 가상지도 확대/축소
        /// </summary>
        /// <param name="value">가상지도 확대/축소 값</param>
        [PunRPC]
        public void Scale_Quntized(float value, bool isUI = true)
        {
            VirtualMapManager virtualMapManager = VirtualMapManager.Instance;

            Vector3 localScale = Vector3.zero;
            if (value < 0.2f)
            {
                value = 0.2f;
            }
            localScale = new Vector3(value, value, value);

            virtualMapManager.TrVirtualMap.localScale = localScale;
            if (!isUI)
            {
                isUpdateingScrollbar = true;
                scrollbar_scale.value = value;
                isUpdateingScrollbar = false;
            }
            else if (!GameManager.Instance.isCloud && isUI)
            {
                PhotonView pv = GetComponent<PhotonView>();
                pv.RPC("Scale_Quntized", RpcTarget.Others, value, false);
            }
        }

        /// <summary>
        /// UI OnOff
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="isUI"></param>
        [PunRPC]
        public void UIOnOff(bool isOn, bool isUI = true)
        {
            isUISet = isOn;

            foreach (GameObject obj in ScenarioObjectManager.Instance.scenarioGO)
            {
                ScenarioObject sO = obj.GetComponent<ScenarioObject>();
                if (!sO.objectData.showForceMark) continue;
                sO.SetMark(isOn);
            }

            if (!GameManager.Instance.isCloud && isUI)
            {
                PhotonView pv = GetComponent<PhotonView>();
                pv.RPC("UIOnOff", RpcTarget.Others, isOn, false);
            }
        }
    }
}