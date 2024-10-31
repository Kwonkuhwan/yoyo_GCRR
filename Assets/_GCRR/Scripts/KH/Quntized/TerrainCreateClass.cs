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

using Photon.Pun;
using System;
using System.Collections;
using System.IO;
using Terrain.Tiles;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Experimental.TerrainAPI.PaintContext;

namespace GCRR.VirtualMap
{
    public class TerrainCreateClass : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private GameObject terrainPanel;

        [SerializeField] private int zoomLevel = 0;

        /// <summary>
        /// 초기화
        /// </summary>
        public IEnumerator Initialize(int zoomLevel, int range, Vector2 currentTerrainCenter, Vector2 currentTextureCenter, int timeType)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(Initialize(zoomLevel, range, currentTerrainCenter, currentTextureCenter, true, timeType));
        }

        public IEnumerator Initialize(int zoomLevel, int range, Vector2 currentTerrainCenter, Vector2 currentTextureCenter, bool bisReversal, int timeType)
        {
            yield return null;

            // [2023.06.19] [추가] KKH : 객체 파괴 및 초기화
            // [2023.06.20] [추가] KKH : VirtualMapManager의 goVirtualMap이 null이면 동작
            if (VirtualMapManager.Instance.GoVirtualMap != null)
            {
                VirtualMapManager.Instance.GoVirtualMap = null;
            }

            if (VirtualMapManager.Instance.goTerrains.Count > 0)
            {
                VirtualMapManager.Instance.goTerrains.Clear();
            }

            if (VirtualMapManager.Instance.GoVirtualMap == null)
            {
                // [2023.06.20] [추가] KKH : [VirtualMap] 게임 오브젝트를 찾기
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Mapdata");
                if (objs != null && objs.Length > 0)
                {
                    foreach (var obj in objs)
                    {
                        Destroy(obj);
                    }
                }

                // [2023.06.20] [추가] KKH : [VirtualMap]이 없으면 동작
                if (VirtualMapManager.Instance.GoVirtualMap == null)
                {
                    // [2023.06.20] [추가] KKH : [VirtualMap] 게임 오브젝트 생성&
                    VirtualMapManager.Instance.GoVirtualMap = new GameObject("Mapdata");
                    // [2023.06.20] [추가] KKH : 레이어를 VirtualMap으로 변경
                    VirtualMapManager.Instance.GoVirtualMap.layer = LayerMask.NameToLayer("Mapdata");
                    VirtualMapManager.Instance.GoVirtualMap.tag = "Mapdata";
                    VirtualMapManager.Instance.GoVirtualMap.transform.parent = VirtualMapManager.Instance.TrVirtualMap;
                    // [2023.06.20] [추가] KKH : Transform 초기화
                    UTILS.Init_Transform(VirtualMapManager.Instance.GoVirtualMap.transform);
                }
            }

            // [2023.06.20] [추가] KKH : ScenarioInfoClass에 줌 레벨을 가져온다..
            this.zoomLevel = zoomLevel;

            for (int x = -GameManager.Instance.Radius; x <= GameManager.Instance.Radius; x++)
            {
                for (int y = -GameManager.Instance.Radius; y <= GameManager.Instance.Radius; y++)
                {
                    yield return null;

                    // [2023.06.20] [추가] KKH : 터레인 정보 생성
                    CreateTerrainInfo terrainInfo = new CreateTerrainInfo(this.zoomLevel, currentTerrainCenter, currentTextureCenter, x, y, bisReversal);
                    // [2023.06.20] [추가] KKH : 생성 시작
                    StartCoroutine(Create_VirtualMap(terrainInfo, timeType));
                }
            }
        }

        /// <summary>
        /// 가상지도 생성
        /// </summary>
        /// <returns></returns>
        /// <param name="terrainInfo">생성 터레인 정보</param>
        /// <param name="timeType">시계열 정보</param>
        public IEnumerator Create_VirtualMap(CreateTerrainInfo terrainInfo, int timeType)
        {
            yield return null;

            // 반경 안에 들어오면 실행 

            // [2023.06.20] [추가] KKH : 위도 기반 오브젝트 생성
            GameObject go_LatParent = Create_Lat_Terrain(terrainInfo);
            // [2023.06.20] [추가] KKH : 경도 기반 오브젝트 생성
            GameObject go_LonParent = Create_Lon_Terrain(go_LatParent.transform, terrainInfo, timeType);
            if (go_LonParent != null)
            {
                Add_TerrainObject(go_LonParent);
            }
        }

        /// <summary>
        /// 가상지도 위도 게임 오브젝트 생성
        /// </summary>
        /// <returns></returns>
        /// <param name="terrainInfo">생성 터레인 정보</param>
        public GameObject Create_Lat_Terrain(CreateTerrainInfo terrainInfo)
        {
            GameObject goLatParent = null;

            if (VirtualMapManager.Instance == null)
            {
                UTILS.LogError("[Critical Error TCC] VirtualMapManager.Instance is null");
                return null;
            }

            if (VirtualMapManager.Instance.GoVirtualMap == null)
            {
                UTILS.LogError("[Critical Error TCC] goVirtualMap of VirtualMapManager.Instance is null");
                return null;
            }
            if (VirtualMapManager.Instance.GoVirtualMap.transform.Find($"{terrainInfo.currentTextureNumber.x}") == null)
            {
                goLatParent = new GameObject($"{terrainInfo.currentTextureNumber.x}");
                goLatParent.transform.parent = VirtualMapManager.Instance.GoVirtualMap.transform;
                UTILS.Init_Transform(goLatParent.transform);
                goLatParent.layer = LayerMask.NameToLayer("VirtualMap");
            }
            else
            {
                goLatParent = VirtualMapManager.Instance.GoVirtualMap.transform.Find($"{terrainInfo.currentTextureNumber.x}").gameObject;
            }

            if (goLatParent == null)
            {
                UTILS.LogError($"[Critical Error TCC] Create Latitude Object Failed {terrainInfo.currentTextureNumber.x}");
                return null;
            }

            if (GameManager.Instance.CurrentTextureCenter.x - 1 > terrainInfo.currentTextureNumber.x || GameManager.Instance.CurrentTextureCenter.x + 1 < terrainInfo.currentTextureNumber.x)
            {
                goLatParent.SetActive(false);
            }

            return goLatParent;
        }

        /// <summary>
        /// 가상지도 경도 게임 오브젝트 생성(터레인)
        /// </summary>
        /// <returns></returns>
        /// <param name="trLatParent">위도 기반으로 생성된 오브젝트 transform</param>
        /// <param name="terrainInfo">생성 터레인 정보</param>
        public GameObject Create_Lon_Terrain(Transform trLatParent, CreateTerrainInfo terrainInfo, int timeType)
        {
            float tileSize = GameManager.Instance.TileSize;

            GameObject goLonParent = null;
            if (VirtualMapManager.Instance == null)
            {
                UTILS.LogError("[Critical Error TCC] VirtualMapManager.Instance is null");
                return null;
            }

            if (VirtualMapManager.Instance.GoVirtualMap == null)
            {
                UTILS.LogError("[Critical Error TCC] goVirtualMap of VirtualMapManager.Instance is null");
                return null;
            }

            if (trLatParent.Find($"{terrainInfo.currentTextureNumber.y}") == null)
            {
                goLonParent = new GameObject($"{terrainInfo.currentTextureNumber.y}");
                goLonParent.transform.parent = trLatParent;
                UTILS.Init_Transform(goLonParent.transform);

                GameObject goTerrain = Create_Terrain_Object(goLonParent.transform, terrainInfo, timeType);

                Vector3 tilePoint = new Vector3(tileSize * terrainInfo.x, 0, tileSize * terrainInfo.y);
                goLonParent.transform.localPosition = tilePoint;
                goLonParent.layer = LayerMask.NameToLayer("VirtualMap");

                GameObject goScenarioObjects = new GameObject("[ScenarioObjects]");
                UTILS.Init_Transform(goScenarioObjects.transform);
                goScenarioObjects.transform.parent = goLonParent.transform;

                GameObject goTargetDatas = new GameObject("[TargetDatas]");
                UTILS.Init_Transform(goTargetDatas.transform);
                goTargetDatas.transform.parent = goLonParent.transform;
            }
            else
            {
                goLonParent = trLatParent.Find($"{terrainInfo.currentTextureNumber.y}").gameObject;
            }

            if (goLonParent == null)
            {
                UTILS.LogError($"[Critical Error TCC] Create Longitude Object Failed {terrainInfo.currentTextureNumber.x}_{terrainInfo.currentTextureNumber.y}");
                return null;
            }

            if (GameManager.Instance.CurrentTextureCenter.y - 1 > terrainInfo.currentTextureNumber.y || GameManager.Instance.CurrentTextureCenter.y + 1 < terrainInfo.currentTextureNumber.y)
            {
                goLonParent.SetActive(false);
            }

            return goLonParent;
        }

        /// 작성 : KKH
        /// <summary>
        /// 터레인 오브젝트 생성
        /// </summary>
        /// <param name="trLonParent">경도 기반으로 생성된 오브젝트 transform</param>
        /// <param name="terrainInfo">생성 터레인 정보</param>
        /// <returns></returns>
        public GameObject Create_Terrain_Object(Transform trLonParent, CreateTerrainInfo terrainInfo, int timeType)
        {
            GameObject goTerrain = null;
            if (trLonParent.Find("Terrain") == null)
            {
                Material mat = new Material(material);

                goTerrain = new GameObject("Terrain");

                goTerrain.transform.parent = trLonParent.transform;
                UTILS.Init_Transform(goTerrain.transform);

                goTerrain.AddComponent<MeshFilter>();
                MeshCollider meshCollider = goTerrain.AddComponent<MeshCollider>();
                meshCollider.convex = true;
                meshCollider.isTrigger = true;
                MeshRenderer meshRenderer = goTerrain.AddComponent<MeshRenderer>();
                meshRenderer.material = mat;

                //Set_VirtualMapMesh(terrainInfo, goTerrain, timeType);
                StartCoroutine(Set_VirtualMapMesh(terrainInfo, goTerrain, timeType));
                StartCoroutine(Set_VirtualMapTexture(terrainInfo, goTerrain, timeType));
                goTerrain.layer = LayerMask.NameToLayer("VirtualMap");
                goTerrain.tag = "VirtualMap";

                goTerrain.AddComponent<PhotonView>();
            }
            else
            {
                goTerrain = trLonParent.Find("Terrain").gameObject;
            }

            if (goTerrain == null)
            {
                UTILS.LogError($"[Critical Error TCC] Create Terrain Object Failed {terrainInfo.currentTextureNumber.x}_{terrainInfo.currentTextureNumber.y}");
                return null;
            }

            return goTerrain;
        }

        /// 작성 : KKH
        /// <summary>
        /// 가상지도 터레인 메쉬 적용
        /// </summary>
        public IEnumerator Set_VirtualMapMesh(CreateTerrainInfo terrainInfo, GameObject goTerrain, int timeType)
        {
            int zoomLevel = GameManager.Instance.zoomLevel - 1;
            string dirPath = $"{GameManager.Instance.TerrainPath}\\{zoomLevel}\\{ScenarioInfoClass.Instance.tileseriesLists.terrainLayer[timeType].Identifier}\\{terrainInfo.currentTerrainNumber.x}\\";
            string fileName = $"{terrainInfo.currentTerrainNumber.y}.terrain";

            string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.terrainUri}/{ScenarioInfoClass.Instance.tileseriesLists.terrainLayer[timeType].Identifier}/{zoomLevel}/{terrainInfo.currentTerrainNumber.x}/{terrainInfo.currentTerrainNumber.y}.terrain";
            StartCoroutine(ServerConnectClass.Requset_TerrainData(serviceUrl, goTerrain, dirPath, fileName));

            yield return null;
        }


        public void SetMesh(TerrainTile tempTile, GameObject goTerrain)
        {
            if(tempTile == null)
            {
                Destroy(goTerrain.transform.parent.gameObject);
                return;
            }

            Mesh mesh = TerrainTileCreateor.CreateMesh(tempTile.Header, tempTile.VertexData, tempTile.IndexData16, GameManager.Instance.FScale, true);
            if (mesh == null)
            {
                //goTerrain.GetComponent<Mesh>().mesh
                UTILS.LogError($"[Critical Error TCC] VirualMapTerrain Mesh is null.");

                // 터레인 없을시 Plane으로 대체
                GameObject panel = Instantiate(terrainPanel, new Vector3(0, 0, 0), Quaternion.identity);
                panel.transform.parent = goTerrain.transform;
                float size = GameManager.Instance.FScale;
                panel.transform.localScale = new Vector3(size, size, size);
                goTerrain.transform.localPosition = new Vector3(-GameManager.Instance.TileSize / 2, 0, -GameManager.Instance.TileSize / 2);
            }
            else
            {
                goTerrain.transform.localPosition = new Vector3(-GameManager.Instance.TileSize / 2, tempTile.Header.MinimumHeight * 10.0f * GameManager.Instance.FScale, -GameManager.Instance.TileSize / 2);
                goTerrain.GetComponent<MeshFilter>().mesh = mesh;
                goTerrain.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 가상지도 텍스쳐 적용
        /// </summary>
        public IEnumerator Set_VirtualMapTexture(CreateTerrainInfo terrainInfo, GameObject goTerrain, int timeType)
        {
            int zoomLevel = GameManager.Instance.zoomLevel;
            string dirPath = $"{GameManager.Instance.TexturePath}\\{zoomLevel}\\{ScenarioInfoClass.Instance.tileseriesLists.imageLayer[timeType].Identifier}\\{terrainInfo.currentTextureNumber.x}\\";
            string fileName = $"{terrainInfo.currentTextureNumber.y}.png";

            string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.textureUri}/{ScenarioInfoClass.Instance.tileseriesLists.imageLayer[timeType].Identifier}/{zoomLevel}/{terrainInfo.currentTextureNumber.x}/{terrainInfo.currentTextureNumber.y}.png";

            StartCoroutine(ServerConnectClass.Request_TextureData(terrainInfo, serviceUrl, goTerrain, dirPath, fileName));

            yield return null;
        }

        public void BaseTextureDownload(CreateTerrainInfo terrainInfo, GameObject goTerrain)
        {
            int zoomLevel = GameManager.Instance.zoomLevel;
            string dirPath_ = $"{GameManager.Instance.TexturePath}\\{zoomLevel}\\BaseMap\\{terrainInfo.currentTextureNumber.x}\\";
            string fileName_ = $"{terrainInfo.currentTextureNumber.y}.png";
            string serviceUrl = $"{GameManager.Instance.defaultData.serviceUri}/{GameManager.Instance.defaultData.textureUri}/cpa-wmts-base/{zoomLevel}/{terrainInfo.currentTextureNumber.x}/{terrainInfo.currentTextureNumber.y}.png";
            StartCoroutine(ServerConnectClass.Request_TextureData(terrainInfo, serviceUrl, goTerrain, dirPath_, fileName_));
        }

        public void SetTexture(Texture texture, GameObject goTerrain)
        {
            if (texture != null)
            { 
                goTerrain.GetComponent<MeshRenderer>().material.mainTexture = texture;
            }
            else
            {
                Destroy(goTerrain.transform.parent.gameObject);

                UTILS.LogError($"[Critical Error TCC] VirualMapTexture Texture is null.");
                return;
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 리스트에 TerrainObject 추가
        /// <param name="goTerrain">추가할 터레인 게임 오브젝트</param>
        /// </summary>
        public void Add_TerrainObject(GameObject goTerrain)
        {
            foreach (GameObject obj in VirtualMapManager.Instance.goTerrains)
            {
                if (obj == goTerrain)
                {
                    return;
                }
            }

            VirtualMapManager.Instance.goTerrains.Add(goTerrain);
        }
    }
}