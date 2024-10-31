using System.Collections;
using UnityEngine;
using Siccity.GLTFUtility;
using System.Collections.Generic;
using Terrain.Tiles;
using GoShared;
using UnityEngine.Networking;
using System.IO;
using System.Data;

namespace GCRR.VirtualMap
{
    public class TargetCreateManager : MonoBehaviour
    {
        private static TargetCreateManager instance;
        public static TargetCreateManager Instance => instance;

        [SerializeField] private int zoomLevel;
        [SerializeField] private float fScale = 0.0009f;
        [SerializeField] private TargetData targetData;
        [SerializeField] private int timeType;

        [SerializeField] List<TileSet> createTileSets = new List<TileSet>();
        [SerializeField] List<GameObject> createdTileSets = new List<GameObject>();

        Coroutine targetDownLoad;
        public List<Coroutine> CreateTargetCorutines = new List<Coroutine>();

        [Space(10)]
        public List<Vector2> targetIndexs = new List<Vector2>();
        public List<Coroutine> coroutines = new List<Coroutine>();

        //public Dictionary<string, bool> targetDones = new Dictionary<string, bool>();
        public List<bool> targetDones = new List<bool>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Update()
        {
            //foreach(var isCreate in targetDones)
            //{
            //    if (!isCreate)
            //    {
            //        GameManager.Instance.isCreateTimeCheck = true;

            //        return;
            //    }
            //}

            if (targetDones.Count > 0)
            {
                if (coroutines.Count == targetDones.Count)
                {
                    GameManager.Instance.isTargetCreated = true;
                }
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 설정 초기화
        /// </summary>
        /// <param name="zoomLevel_">줌 레벨</param>
        /// <param name="targetData_">표적 정보</param>
        /// <param name="timeType_">시계열 타입</param>
        public void Initialize(int zoomLevel_, /*TargetData targetData_,*/ int timeType_)
        {
            zoomLevel = zoomLevel_;
            timeType = timeType_;
            SetFScale();

            DisableCoroutine();

            DownLoad_TargetJsonFile();

#if UNITY_EDITOR
            GameManager.Instance.isCreateTimeCheck = true;
            GameManager.Instance.fCreateTimeCheck = 0.0f;
#endif

            GameManager.Instance.isTargetCreated = false;
        }

        public void DisableCoroutine()
        {
            if (targetDownLoad != null)
            {
                StopCoroutine(targetDownLoad);
                targetDownLoad = null;
            }

            if (CreateTargetCorutines.Count > 0)
            {
                foreach (var cor in CreateTargetCorutines)
                {
                    StopCoroutine(cor);
                }
                CreateTargetCorutines.Clear();
            }

            foreach (var item in coroutines)
            {
                StopCoroutine(item);
            }

            // [추가] KKH : 생성된 표적 클리어
            createTileSets.Clear();
            createdTileSets.Clear();
            coroutines.Clear();
            targetIndexs.Clear();
            targetDones.Clear();

#if UNITY_EDITOR
            GameManager.Instance.fCreateTimeCheck = 0.0f;
#endif
        }

        /// 작성 : KKH
        /// <summary>
        /// 표적정보 오브젝트 FScale(스케일) 설정
        /// </summary>
        private void SetFScale()
        {
            if (zoomLevel == 19) fScale = 0.009f;
            else if (zoomLevel == 18) fScale = 0.007f;
            else if (zoomLevel == 17) fScale = 0.005f;
            else if (zoomLevel == 16)
            {
                //GameManager.Instance.defaultData.targetMaxCnt = GameManager.Instance.defaultData.targetMaxCnt / 4;
                fScale = 0.003f;
            }
            else if (zoomLevel == 15)
            {
                //GameManager.Instance.defaultData.targetMaxCnt = GameManager.Instance.defaultData.targetMaxCnt / 2;
                fScale = 0.0015f;
            }
            else if (zoomLevel == 14)
            {
                fScale = 0.0007f;
            }
        }

        /// 작성 : KH
        /// <summary>
        /// 표적정보 Json 파일 로드
        /// </summary>
        public void DownLoad_TargetJsonFile()
        {
            ScenarioInfoClass.Instance.targetData.Clear();
            Coordinates coord = new Coordinates(GameManager.Instance.CurrentTextureCenter, zoomLevel);//GameManager.Instance.StartCoordinates.tileCenter(14);
            //Coordinates coord = new Coordinates(test, zoomLevel);//GameManager.Instance.StartCoordinates.tileCenter(14);
            coord = coord.tileCenter(14);
            Vector2 targetIndex = TerrainTileIndexer.LlaToTms((float)coord.latitude, (float)coord.longitude, 14);

            for (int i = -GameManager.Instance.Radius; i <= GameManager.Instance.Radius; i++)
            {
                for (int j = -GameManager.Instance.Radius; j <= GameManager.Instance.Radius; j++)
                {
                    Vector2 targetIndex_ = targetIndex + new Vector2(i, j);
                    if (targetIndexs.Contains(targetIndex_))
                    {
                        continue;
                    }

                    targetIndexs.Add(targetIndex_);
                }
            }

            foreach (Vector2 targetIndex_ in targetIndexs)
            {
                string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.targetUri}/{ScenarioInfoClass.Instance.tileseriesLists.tile3dLayer[timeType].Identifier}/{14}/{targetIndex_.x}/{targetIndex_.y}/tileset.json";
                //string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.targetUri}/cpa-tile3d-latest/{14}/{targetIndex_.x}/{targetIndex_.y}/tileset.json";
                Coroutine datajsonCoroutine = StartCoroutine(ServerConnectClass.Request_TargetDataJson(serviceUrl));
                coroutines.Add(datajsonCoroutine);
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 표적 다운로드
        /// </summary>
        /// <returns></returns>
        public void CreateTargetCorutine(TargetData targetData)
        {
            //CreateTargetCorutines.Add(StartCoroutine(TargetDownLoad(targetData)));

            if (targetData != null)
            {
                // [추가] KKH : 시계열 정보 가져오기
                Tile3DLayer t3l = ScenarioInfoClass.Instance.tileseriesLists.tile3dLayer[timeType];

                // [추가] KKH : 다운로드 경로 지정
                string dirPath = $"{GameManager.Instance.TargetPath}\\{t3l.Identifier}";

                StartCoroutine(CreateTarget(targetData, t3l, dirPath));
            }
        }

        public IEnumerator TargetDownLoad(TargetData targetData)
        {
            yield return null;
            if(targetData != null) 
            {
                // [추가] KKH : 시계열 정보 가져오기
                Tile3DLayer t3l = ScenarioInfoClass.Instance.tileseriesLists.tile3dLayer[timeType];

                // [추가] KKH : 다운로드 경로 지정
                string dirPath = $"{GameManager.Instance.TargetPath}\\{t3l.Identifier}";

                StartCoroutine(CreateTarget(targetData, t3l, dirPath));
            }
        }

        IEnumerator CreateTarget(TargetData targetData, Tile3DLayer t3l, string dirPath)
        {
            foreach (TileSet tileSet in targetData.root.children)
            {
                //// [추가] KKH : 위 경도 계산
                double lat = UTILS.Cal_LocationArg(tileSet.boundingVolume.region[1], tileSet.boundingVolume.region[3]);
                double lon = UTILS.Cal_LocationArg(tileSet.boundingVolume.region[0], tileSet.boundingVolume.region[2]);

                // [추가] KKH : 파일 이름 가져오기
                string fileName = $"{tileSet.content.uri}";

                Vector2 coord2 = TerrainTileIndexer.LlaToTms(lat, lon, 14);
                string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.targetUri}/{t3l.Identifier}/14/{coord2.x}/{coord2.y}/LOD3/{fileName}";
                //string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.targetUri}/cpa-tile3d-latest/14/{coord2.x}/{coord2.y}/LOD3/{fileName}";

                using (UnityWebRequest uwr = UnityWebRequest.Get(serviceUrl))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                    {
                        //Request_TargetData(stServiceUrl, tileSet, dirPath, fileName);
                        UTILS.LogError($"TargetData DownLoad Error");
                    }
                    else
                    {
                        SetCreate(uwr.downloadHandler.data, tileSet);

                        if (GameManager.Instance.isMapDataDownLoad && !File.Exists(dirPath + fileName))
                        {
                            UTILS.DownLoad_Data(uwr, dirPath, fileName);
                        }                       
                    }
                }

                //StartCoroutine(ServerConnectClass.Request_TargetData(serviceUrl, tileSet, dirPath, fileName));

                //// [추가] KKH : 표적 생성 정보에 추가
                //createTileSets.Add(tileSet);
                //targetDones.Add(fileName, false);
                //// [추가] KKH : 생성 개수 오버시 종료
                //if (createTileSets.Count >= GameManager.Instance.defaultData.targetMaxCnt) break;

                // [추가] KKH : 표적 생성 정보에 추가
                //createTileSets.Add(tileSet);
                // [추가] KKH : 생성 개수 오버시 종료
                if (createTileSets.Count >= GameManager.Instance.defaultData.targetMaxCnt) break;

                yield return null;
            }

            targetDones.Add(true);

        }

        /// 작성 : KKH
        /// <summary>
        /// 표적 정보 오브젝트 생성
        /// </summary>

        public void SetCreate(byte[] datas, TileSet tileSet)
        {
            StartCoroutine(CreateTargetData(datas, tileSet));
        }

        private IEnumerator CreateTargetData(byte[] datas, TileSet tileSet)
        {
            //yield return new WaitForFixedUpdate();
            yield return null;

            double lat = UTILS.Cal_LocationArg(tileSet.boundingVolume.region[1], tileSet.boundingVolume.region[3]);
            double lon = UTILS.Cal_LocationArg(tileSet.boundingVolume.region[0], tileSet.boundingVolume.region[2]);

            Vector2 textureTile = UTILS.Calcul_TextureIndex(lat, lon, GameManager.Instance.zoomLevel);

            Transform parent;
            try
            {
                parent = VirtualMapManager.Instance.GoVirtualMap.transform.Find(textureTile.x.ToString()).Find(textureTile.y.ToString()).Find("[TargetDatas]");
            }
            catch
            {
                parent = null;
            }

            if (parent == null || !parent.gameObject.activeInHierarchy)
            {
                UTILS.LogError($"TargetData parent is null Error");
            }
            else
            {
                //targetDones.Add(tileSet.content.uri, false);
                GameObject target = Importer.LoadFromBytes(datas);
                SetTargetObject(target, tileSet);
                createTileSets.Add(tileSet);
            }
        }

        private void SetTargetObject(GameObject target, TileSet tileSet)
        {
            // [추가] KKH : 생성 오브젝트 레이어 변경
            target.layer = LayerMask.NameToLayer("TargetObject");

            // [추가] KKH : 생성 오브젝트 위치 초기화
            UTILS.Init_Transform(target.transform);

            // [2023.10.04] [추가] KKH : 최소높이와 최대 높이의 평균값으로 높이 설정(수정할수도 있음)
            float height = (float)((tileSet.boundingVolume.region[4] + tileSet.boundingVolume.region[5]) / 2);

            // [2023.10.04] [추가] KKH : 표적 데이터의 위치 구하기
            double lat = UTILS.Cal_LocationArg(tileSet.boundingVolume.region[1], tileSet.boundingVolume.region[3]);
            double lon = UTILS.Cal_LocationArg(tileSet.boundingVolume.region[0], tileSet.boundingVolume.region[2]);
            //Coordinates coord = new Coordinates(GameManager.Instance.CurrentTextureCenter, 15);
            //coord = coord.tileCenter(14);
            Vector3 pos = UTILS.GetObjPos(GameManager.Instance.StartCoordinates, lat, lon, false);
            ///=Vector3 pos = UTILS.GetObjPos(coord, lat, lon, false);

            // [추가] KKH : 표적 데이터 스케일 변경
            target.transform.localScale = new Vector3(fScale, fScale, fScale);

            // [추가] KKH : 표적 데이터 위치 변경
            pos = new Vector3(pos.x, height * GameManager.Instance.FScale, pos.z);
            target.transform.position = pos;

            // [추가] KKH : 표적 데이터가 90도 회전되어 있음으로 회전
            //target.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            target.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            // [추가] KKH : 확장자를 제외한 이름 추출
            target.name = tileSet.content.uri.Split('.')[0];

            MeshCollider meshCollider = target.AddComponent<MeshCollider>();
            meshCollider.convex = true;

            // [2023.10.04] [추가] KKH : 표적 정보 데이터 넣기
            TargetInfo targetInfo = target.AddComponent<TargetInfo>();
            targetInfo.WestDrgee = tileSet.boundingVolume.region[0];
            targetInfo.SouthDrgee = tileSet.boundingVolume.region[1];
            targetInfo.EastDrgee = tileSet.boundingVolume.region[2];
            targetInfo.NorthDrgee = tileSet.boundingVolume.region[3];
            targetInfo.MinHeight = tileSet.boundingVolume.region[4];
            targetInfo.MaxHeight = tileSet.boundingVolume.region[5];

            targetInfo.lat = lat;
            targetInfo.lon = lon;
            targetInfo.height = height;

            // [추가] KKH : 표적 메타 데이터 초기화 
            targetInfo.MetaDataInit(target.name);

            createdTileSets.Add(target);
        }
    }
}