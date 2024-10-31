using System;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GCRR.VirtualMap
{
    public class ScenarioObjectManager : MonoBehaviour
    {
        private static ScenarioObjectManager instance;
        public static ScenarioObjectManager Instance => instance;

        private ScenarioUIManager sUIM;
        private ObjectSelectUIManager osUIM;
        private TcpSocketManager tcpSocketManager;

        [SerializeField] private GameObject sel_Obj;
        public GameObject select_Obj {get => sel_Obj; set => sel_Obj = value; }

        [SerializeField] private List<GameObject> scenaGO = new List<GameObject>();
        public List<GameObject> scenarioGO { get => scenaGO; set => scenaGO = value; }

        public delegate void SyncScenarioObjectDelegate(S_ChangeObjInfo s_ChangeObjInfo);
        //private SyncScenarioObjectDelegate syncScenarioObjectDelegate;

        [SerializeField] private float fScale = 0.0005f;

        private ObjectID select_ObjectID;
        public ObjectID Select_ObjectID
        {
            get => select_ObjectID;
            set
            {
                select_ObjectID = value;
            }
        }

        private ObjectSelectMode objSelectMode;
        public ObjectSelectMode objcetSelectMode { get => objSelectMode; set => objSelectMode = value; }
        private bool IsMoveOn = false;
        public bool isMoveOn { get => IsMoveOn; set => IsMoveOn = value; }

        [SerializeField] private GameObject TargetPos;
        public GameObject targetPos => TargetPos;

        // 3D 객체 프리펩
        [SerializeField] private List<GameObject> allyScenarioGO = new List<GameObject>();
        [SerializeField] private List<GameObject> enemyScenarioGO = new List<GameObject>();

        private Queue<S_ChangeObjInfo> s_ChangeObjsQueue = new Queue<S_ChangeObjInfo>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            SetFScale();
        }

        private void Start()
        {
            if (sUIM == null) sUIM = UIManager.Instance.SUIM;
            if (osUIM == null) osUIM = UIManager.Instance.OSUIM;
            if (tcpSocketManager == null) tcpSocketManager = GameObject.Find("GameManager").GetComponent<TcpSocketManager>();

            targetPos.SetActive(false);

           // syncScenarioObjectDelegate = SyncScenarioObject;
        }

        private void Update()
        {
            Processing_QueueScenarioObjChange();
        }

        /// 작성 : KKH
        /// <summary>
        /// 객체 오브젝트 FScale 설정
        /// </summary>
        private void SetFScale()
        {
            int zoomLevel = GameManager.Instance.zoomLevel;
            if (zoomLevel == 16) fScale = 0.02f;
            else if (zoomLevel == 15) fScale = 0.015f;
            else if (zoomLevel == 14) fScale = 0.01f;
        }

        /// 작성 : KKH
        /// <summary>
        /// 시나리오 오브젝트 정보 초기화
        /// </summary>
        public void ClearObject()
        {
            if (select_Obj != null) select_Obj = null;
            objcetSelectMode = ObjectSelectMode.None;
            Select_ObjectID = ObjectID.None;
        }

        /// 작성 : KKH
        /// <summary>
        /// 선택된 시나리오 오브젝트 반환
        /// </summary>
        /// <returns></returns>
        public GameObject GetScenarioObject(bool isAlly, ObjectID objectID)
        {
            GameObject obj = null;
            if (isAlly)
                obj = allyScenarioGO[(int)objectID - 1];
            else
                obj = enemyScenarioGO[(int)objectID - 1];

            return obj;
        }

        /// 작성 : KKH
        /// <summary>
        /// 가상지도에서 오브젝트 선택
        /// </summary>
        /// <returns>선택한 GameObject</returns>
        public void SelectObject(GameObject sObj)
        {
            if (sObj == null) return;
            select_Obj = sObj;

            sUIM.Info_ObjectPanelOn(sObj.GetComponent<ScenarioObject>().objectData);
            objcetSelectMode = ObjectSelectMode.VirtualMap;
        }

        /// 작성 : KKH
        /// <summary>
        /// 오브젝트 추가 UI에서 오브젝트 선택
        /// </summary>
        /// <returns>선택한 GameObject</returns>
        public void SelectObject(ObjectID scenarioObjectID)
        {
            objcetSelectMode = ObjectSelectMode.UI;
            Select_ObjectID = scenarioObjectID;
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 오브젝트 적용
        /// </summary>
        public void ApplyObject(GameObject obj)
        {
            if (obj == null) return;

            ScenarioObject sO = obj.GetComponent<ScenarioObject>();
            if (sO == null) return;

            ScenarioObjectInteractionType interactionType = ScenarioObjectInteractionType.Add;

            if (objcetSelectMode == ObjectSelectMode.None)
            {
                return;
            }

            // [2023.05.30] [추가] KKH : 시나리오 객체를 UI에서 선택 했을때는 새로 생성
            if (objcetSelectMode == ObjectSelectMode.UI)
            {
                //AddObject(obj);
                //interactionType = ScenarioObjectInteractionType.Add;
            }
            // [2023.05.30] [추가] KKH : 시나리오 객체를 가상지도에서 선택했을때는 수정
            else if (objcetSelectMode == ObjectSelectMode.VirtualMap)
            {
                // [2023.05.30] [추가] KKH : 수정된 데이터를 UI에 적용
                sUIM.ObjInfoPanel.SetObjectInfoData(sO.objectData);
                RevisionObject(obj);

                ScenarioInfoClass.Instance.scenarioData.objects[sO.objectNum - 1] = sO.objectData;

                interactionType = ScenarioObjectInteractionType.Revision;
            }

            if (targetPos.activeInHierarchy)
            {
                float height = sO.objectData.height == 0 ? targetPos.transform.localPosition.y : targetPos.transform.localPosition.y + sO.objectData.height * GameManager.Instance.FScale;
                Vector3 pos = new Vector3(targetPos.transform.localPosition.x, height, targetPos.transform.localPosition.z);
                SetObject(obj, pos, sO.objectData.movingDirection);
            }

            SyncSendObjectData(sO, interactionType);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 위치 및 회전값 설정
        /// </summary>
        /// <param name="obj">시나리오 객체 게임 오브젝트 (3D 객체)</param>
        /// <param name="pos">적용할 위치값</param>
        /// <param name="movingDirection">적용할 회전 값</param>

        public void SetObject(GameObject obj, Vector3 pos, double lat, double lon, float movingDirection)
        {
            Vector2 textureIndex = UTILS.Calcul_TextureIndex(lat, lon, GameManager.Instance.zoomLevel);

            SetObject(obj, pos, movingDirection, textureIndex);
        }

        public void SetObject(GameObject obj, Vector3 pos, float movingDirection)
        {
            Vector2 coordinates = UTILS.GetObjLocation(pos);
            Vector2 textureIndex = UTILS.Calcul_TextureIndex(coordinates.x, coordinates.y, GameManager.Instance.zoomLevel);

            SetObject(obj, pos, movingDirection, textureIndex);
        }

        private void SetObject(GameObject obj, Vector3 pos, float movingDirection, Vector2 textureIndex)
        {
            pos = new Vector3(pos.x, pos.y + 0.05f, pos.z);
            obj.transform.localPosition = pos;
            obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, movingDirection, obj.transform.eulerAngles.z);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }

            Transform parent = null;
            try
            {
                parent = GameObject.Find(textureIndex.x.ToString()).transform.Find(textureIndex.y.ToString()).Find("[ScenarioObjects]");
            }
            catch
            {
                parent = GameObject.Find("NotParentScenarioObject").transform;
                obj.SetActive(false);
            }
            obj.transform.parent = parent;
        }

        /// 작성 : KKH
        /// <summary>
        /// 시나리오 오브젝트 추가
        /// </summary>
        /// <param name="obj">추가할 게임 오브젝트(3D 객체)</param>
        /// <returns>성공 : true, 실패 : false</returns>
        //public void AddObject(GameObject obj)
        //{
        //    // [2023.05.30] [추가] KKH : ScenarioInfoClass에 있는 scenarioData가 null이면 false를 반환한다.
        //    if (ScenarioInfoClass.Instance == null) return;

        //    ScenarioObject sO = obj.GetComponent<ScenarioObject>();
        //    AddObject(sO);
        //}

        //public void AddObject(ScenarioObject sO)
        //{
        //    if (ScenarioInfoClass.Instance == null) return;
        //    AddObject(sO.objectData);

        //    UpdateObjectList(sO.gameObject);
        //}

        //private void AddObject(ObjectData objectData)
        //{
        //    ScenarioInfoClass.Instance.AddObjectData(objectData);
        //}

        /// <summary>
        /// 시나리오 오브젝트 수정
        /// </summary>
        /// <param name="objectData">시나리오 오브젝트 데이터</param>
        /// <returns></returns>

        public void RevisionObject(GameObject obj)
        {
            if (ScenarioInfoClass.Instance == null) return;
            ScenarioObject sO = obj.GetComponent<ScenarioObject>();

            if (sO == null) return;
            RevisionObject(sO);
        }

        public void RevisionObject(ScenarioObject sO)
        {
            if (ScenarioInfoClass.Instance == null) return;
            ScenarioInfoClass.Instance.RevisionObjectData(sO);
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 오브젝트 제거
        /// </summary>
        public void RemoveObject(bool isSend = false)
        {
            // [2023.06.19] [추가] KKH : 객체 리스트에서 객체 삭제
            ScenarioInfoClass.Instance.RemoveObjectData(select_Obj);
            osUIM.LoadObjectSelectList();

            if (isSend) SyncSendObjectData(select_Obj.GetComponent<ScenarioObject>(), ScenarioObjectInteractionType.Remove);

            // [2023.06.19] [추가] KKH : 객체 파괴 및 초기화
            DestoryObject();
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 오브젝트 취소
        /// </summary>
        public void DestoryObject()
        {
            // [2023.06.19] [추가] KKH : 오브젝트 파괴
            Destroy(select_Obj);
            // [2023.06.19] [추가] KKH : 시나리오 객체 초기화
            ClearObject();
        }

        /// 작성 : KKH
        /// <summary>
        /// 시나리오 객체 생성
        /// </summary>
        /// <param name="objectData">생성할 객체 데이터</param>
        /// <returns>생성된 시나리오 게임 객체</returns>
        public GameObject CreateObject(bool isAlly, ObjectID objectID)
        {
            GameObject goScenarioObjParent = Instantiate(GetScenarioObject(isAlly, objectID)); //new GameObject("ScenarioObjs");
            if (goScenarioObjParent == null) return null;

            goScenarioObjParent.GetComponent<ScenarioObject>().objectID = objectID;

            // [2023.05.30] [추가] KKH : 생성한 오브젝트 Transform을 초기화한다.
            UTILS.Init_Position(goScenarioObjParent.transform);

            goScenarioObjParent.transform.localScale = new Vector3(fScale, fScale, fScale);

            // [2023.05.30] [추가] KKH : 레이어를 ScenarioObject로 변경해준다.
            goScenarioObjParent.layer = LayerMask.NameToLayer("ScenarioObject");

            return goScenarioObjParent;
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 객체 배치
        /// </summary>
        /// <param name="objectData"></param>
        /// <returns></returns>
        public ScenarioObject AssignObject(ObjectData objectData)
        {
            // [2023.05.30] [추가] KKH : 위 경도 기반으로 Vector3 값 계산
            Vector3 pos = UTILS.GetObjPos(GameManager.Instance.StartCoordinates, objectData.unitLat, objectData.unitLon, (objectData.objectID == (int)ObjectID.FixWing || objectData.objectID == (int)ObjectID.RoateWing) ? true : false);

            try
            {
                select_ObjectID = (ObjectID)objectData.objectID;
                GameObject obj = CreateObject(objectData.isAlly, select_ObjectID);
                if (obj == null) return null;

                //pos = new Vector3(pos.x, objectData.GetUnitHeight() * GameManager.Instance.FScale, pos.z);
                SetObject(obj, pos, objectData.unitLat, objectData.unitLon, objectData.movingDirection);

                // [2023.05.30] [추가] KKH : 오브젝트데이터 컴포넌트를 추가하고 오브젝트데이터를 넣어준다.
                ScenarioObject sO = obj.GetComponent<ScenarioObject>();
                sO.SetObjectData(objectData);
                UpdateObjectList(obj);
                ClearObject();

                return sO;
            }
            catch (Exception e)
            {
                UTILS.LogError($"[Critical Error ScenarioObjectManager] {e} AssignObject Fail");
                return null;
            }
        }

        public void UpdateObjectList(GameObject obj)
        {
            scenarioGO.Add(obj);
            osUIM.LoadObjectSelectList();
        }

        /// 작성 - KKH
        /// <summary>
        /// 클릭 위치를 표시할 오브젝트 배치
        /// </summary>
        /// <param name="pos">커서 위치</param>
        public void ShowTargetPos(Vector3 pos_)
        {
            // [2023.09.14] [추가] KKH : 생성할 객체 위치 표시 게임 오브젝트 활성화
            if (!targetPos.activeInHierarchy) targetPos.SetActive(true);
            // [2023.09.14] [추가] KKH : 생성할 객체 위치 표시 게임 오브젝트 위치 변경
            Vector3 pos = new Vector3(pos_.x, pos_.y + 0.005f, pos_.z);
            targetPos.transform.position = pos;
        }

        /// 작성 - KKH
        /// <summary>
        /// 클릭 위치 표시 오브젝트 숨기기
        /// </summary>
        public void HideTargetPos()
        {
            targetPos.SetActive(false);
        }

        /// 작성 - KKH
        /// <summary>
        /// 가상지도 클릭
        /// </summary>
        public void OnTerrainClick()
        {
            // [2023.06.19] [추가] KKH : 선택한 모드(가상지도, UI)가 없다면 리턴
            if (objcetSelectMode == ObjectSelectMode.None) return;

            // [2023.06.19] [추가] KKH : 현재 커서 위치정보 가져오기
            Transform cursor = GameObject.Find("UICursor(Clone)").transform;
            if (cursor == null) return;

            // [2023.06.19] [추가] KKH : UI에서 선택하였다면
            if (objcetSelectMode == ObjectSelectMode.UI)
            {
                ShowTargetPos(cursor.position);
                // [2023.06.19] [추가] KKH : 설정 UI 켜기
                //sUIM.Setting_ObjectPanelOn(false);
            }
            else if (objcetSelectMode == ObjectSelectMode.VirtualMap)
            {
                // [2023.06.19] [추가] KKH : 객체의 데이터를 추출
                ObjectData objectData = select_Obj.GetComponent<ScenarioObject>().objectData;
                // [2023.06.19] [추가] KKH : 객체 정보 UI 켜기
                sUIM.Info_ObjectPanelOn(objectData);
                // [2023.06.19] [추가] KKH : 객체 설정 UI 켜기
                //sUIM.Setting_ObjectPanelOn(objectData, true);
            }

            if (objcetSelectMode == ObjectSelectMode.VirtualMap && isMoveOn)
            {
                ShowTargetPos(cursor.position);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 오브젝트 변경 사항 보내기
        /// </summary>
        /// <param name="sO"></param>
        /// <param name="interctionType"></param>
        public void SyncSendObjectData(ScenarioObject sO, ScenarioObjectInteractionType interctionType)
        {
            if (tcpSocketManager == null) return;
            if (GameManager.Instance.isCloud)
            {
                C_ChangeObjInfo revisionObjectData = new C_ChangeObjInfo(ScenarioInfoClass.Instance.playerID, interctionType, sO.GetObjectNum(), sO.objectData);

                if (tcpSocketManager.tcpClient != null)
                {
                    tcpSocketManager.tcpClient.Send(revisionObjectData.Serialize());
                }
            }
            else
            {
                if (!PhotonNetwork.IsConnected || PhotonNetwork.PlayerList.Count() < 1) return;

                S_ChangeObjInfo revisionObjectData = new S_ChangeObjInfo(ScenarioInfoClass.Instance.playerID, interctionType, sO.GetObjectNum(), sO.objectData);

                if (tcpSocketManager.tcpClient != null)
                {
                    tcpSocketManager.tcpClient.Send(revisionObjectData.Serialize());
                }
                else
                {
                    TServer.postboxSend.PushData(revisionObjectData.Serialize());
                }
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 소켓 통신으로 수신 받은 데이터(ObjectChange) 처리
        /// </summary>
        /// <param name="s_ChangeObjInfo"></param>
        public void SyncScenarioObject(S_ChangeObjInfo s_ChangeObjInfo)
        {
            try
            {
                if (s_ChangeObjInfo.interactionType == ScenarioObjectInteractionType.Add)
                {
                    AssignObject(s_ChangeObjInfo.objectData);
                }
                else if (s_ChangeObjInfo.interactionType == ScenarioObjectInteractionType.Revision)
                {
                    GameObject obj = null;
                    foreach (GameObject go in scenarioGO)
                    {
                        if (go.GetComponent<ScenarioObject>().objectNum == s_ChangeObjInfo.objSequence)
                        {
                            obj = go;
                            break;
                        }
                    }

                    if (obj == null)
                    {
                        UTILS.LogWarning($"[Warning Error] Object is null");
                        return;
                    }

                    if (!obj.activeInHierarchy) obj.SetActive(true);

                    obj.GetComponent<ScenarioObject>().objectData = s_ChangeObjInfo.objectData;

                    Vector3 pos = UTILS.GetObjPos(GameManager.Instance.StartCoordinates, s_ChangeObjInfo.objectData.unitLat, s_ChangeObjInfo.objectData.unitLon, (s_ChangeObjInfo.objectData.objectID == (int)ObjectID.FixWing || s_ChangeObjInfo.objectData.objectID == (int)ObjectID.RoateWing) ? true : false);

                    pos = new Vector3(pos.x, pos.y + 0.05f, pos.z);
                    obj.transform.localPosition = pos;
                }
                else if (s_ChangeObjInfo.interactionType == ScenarioObjectInteractionType.Remove)
                {
                    GameObject obj = null;
                    foreach (GameObject go in scenarioGO)
                    {
                        if (go.GetComponent<ScenarioObject>().objectNum == s_ChangeObjInfo.objSequence)
                        {
                            obj = go;
                            break;
                        }
                    }

                    if (obj == null)
                    {
                        UTILS.LogWarning($"[Warning Error] Object is null");
                        return;
                    }

                    //select_Obj = scenarioGO[s_ChangeObjInfo.objSequence];
                    select_Obj = obj;
                    RemoveObject(false);
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"[Critical Error ScenarioObjectManager] {e} SyncScenarioObject Fail");
                return;
            }
        }

        public void Processing_QueueScenarioObjChange()
        {
            if (s_ChangeObjsQueue.Count <= 0)
            {
                return;
            }

            S_ChangeObjInfo co = s_ChangeObjsQueue.Dequeue();

            SyncScenarioObject(co);
        }

        public void ChangeObjEnqueue(S_ChangeObjInfo changeObj)
        {
            s_ChangeObjsQueue.Enqueue(changeObj);
        }
    }
}