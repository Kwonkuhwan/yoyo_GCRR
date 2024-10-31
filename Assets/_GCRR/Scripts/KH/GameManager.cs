using GoShared;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Terrain.Tiles;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance => instance;

        [SerializeField] private DefaultData defData;
        public DefaultData defaultData => defData;

        [Space(10)]

        #region DataPath
        [SerializeField] private string terrainPath;
        [SerializeField] private string texturePath;
        [SerializeField] private string targetPath;

        public string TerrainPath
        {
            get { return terrainPath; }
            set { terrainPath = value; }
        }

        public string TexturePath
        {
            get { return texturePath; }
            set { texturePath = value; }
        }

        public string TargetPath
        {
            get { return targetPath; }
            set { targetPath = value; }
        }
        #endregion

        [Space(10)]

        #region Coordinates
        private Vector2 currentTerrainCenter;            // KKH : 현재 터레인 센터 좌표
        public Vector2 CurrentTerrainCenter // KKh : 현재 텍스쳐 센터 좌표
        {
            get => currentTerrainCenter;
            set => currentTerrainCenter = value;
        }

        private Vector2 currentTextureCenter;
        public Vector2 CurrentTextureCenter // KKh : 현재 텍스쳐 센터 좌표
        {
            get => currentTextureCenter;
            set => currentTextureCenter = value;
        }

        [SerializeField] private float latDgree;
        public float LatDgree => latDgree;
        [SerializeField] private float lonDgree;
        public float LongDgree => lonDgree;

        [SerializeField] private Coordinates startCoordinates;
        public Coordinates StartCoordinates
        {
            get => startCoordinates;
            set => startCoordinates = value;
        }

        [SerializeField] private Coordinates rangeCoordinates;
        public Coordinates RangeCoordinates => rangeCoordinates;

        [SerializeField] private int radius;
        public int Radius => radius;

        [SerializeField] private Vector2 tileSizeMul;
        public Vector2 TileSizeMul => tileSizeMul;
        #endregion

        [Space(10)]

        #region Quntized
        private const int tileSize = 32767;
        public float TileSize
        {
            get
            {
                if (ScenarioInfoClass.Instance != null)
                    return tileSize * FScale;
                else
                    return tileSize * 0.0005f;
            }
        }

        [SerializeField] private int zLevel = 15;                      // KKH : 줌레벨(테스트)
        public int zoomLevel
        {
            get => zLevel;
            set => zLevel = value;
        }

        [SerializeField] private int nRan = 0;
        public int nRange { get => nRan; set => nRan = value; }

        [SerializeField] private float fScale = 0.0005f;

        public float FScale
        {
            get
            {
                return fScale;
            }
            set
            {
                fScale = value;
            }
        }
        #endregion

        [Space(10)]

        #region Scenario
        [SerializeField] private bool isCld = false;                    // KKH : 클라우드 사용 유무
        public bool isCloud { get => isCld; set => isCld = value; }                    // KKH : 클라우드 사용 유무

        private SelectScenrioMode selectMode;           // KKH : 모드 선택
        public SelectScenrioMode SelectMode
        {
            get { return selectMode; }
            set
            {
                selectMode = value;
                if (selectMode == SelectScenrioMode.Local)
                {
                    isCloud = false;
                }
                else
                {
                    isCloud = true;
                }
            }
        }
        #endregion

        [Space(10)]

        [SerializeField] private bool isSver = false;

        public bool isServer { get => isSver; set => isSver = value; }

        [SerializeField] private bool isLoca = false;
        public bool isLocation { get => isLoca; set => isLoca = value; }

        [Space(10)]

        [SerializeField] private TcpSocketManager tcpSocketManager;
        public TcpSocketManager TcpSocketMana => tcpSocketManager;
        //[SerializeField] private NetworkManager_ networkManager;

        [Space(10)]

        [SerializeField] private bool isLoadDone = false;
        public bool isLoadingDone { get => isLoadDone; set => isLoadDone = value; }

        [SerializeField] private string nextScenName = string.Empty;
        public string nextSceneName { get => nextScenName; set => nextScenName = value; }

        [Space(10)]

        public bool isTargetCreated = false;
        public bool isCreateTimeCheck = false;
        public float fCreateTimeCheck = 0.0f; // 시작 시간 후 표적데이터 생성까지 걸리는 시간.
        public bool isUIEnable = false;
        public float fUIEnaableCheck = 0.0f;

        //private int nMaxTargetCount = 200;
        //private List<string> download_TargetPath = new List<string>();

        [Space(10)]
        public bool isMapDataDownLoad = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            defData = JsonManager.DefaultDataJsonLoad("DefaultData");

            SetDir();

            // 위도 1도당 거의 109 ~ 112 km
            latDgree = 1.0f / 111.0f;
            // 경도도 1도당 거의 88~90 km
            lonDgree = 1.0f / 88.74f;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (isCreateTimeCheck) fCreateTimeCheck += Time.deltaTime;
#endif

            if(isTargetCreated)
            {
                if (!isUIEnable)
                {
                    fUIEnaableCheck += Time.deltaTime;
                }

                if (fUIEnaableCheck > 2)
                {
                    isUIEnable = true;
                }
#if UNITY_EDITOR
                isCreateTimeCheck = false;
#endif
            }
            else
            {
                fUIEnaableCheck = 0.0f;
                isUIEnable = false;
            }
        }

        private void OnApplicationQuit()
        {
            if (TcpSocketManager.Instance != null)
            {
                TcpSocketManager.Instance.OnClose();
            }
        }

        #region 기본 설정
        /// 작성 - KKH
        /// <summary>
        /// 폴더 설정
        /// </summary>
        private void SetDir()
        {
#if UNITY_EDITOR
            TerrainPath = $"{Application.dataPath}{defaultData.terrainPath}";
            TexturePath = $"{Application.dataPath}{defaultData.texturePath}";
            TargetPath = $"{Application.dataPath}{defaultData.targetPath}";
#else
        TerrainPath = $"{Application.dataPath}\\..{defaultData.terrainPath}";
        TexturePath = $"{Application.dataPath}\\..{defaultData.texturePath}";
        TargetPath = $"{Application.dataPath}\\..{defaultData.targetPath}";
#endif
        }

        /// 작성 - KH
        /// <summary>
        /// 월드 중심 좌표 설정
        /// </summary>
        /// <param name="lat">위도</param>
        /// <param name="lon">경도</param>
        public void SetCoordinates(float lat, float lon)
        {
            Coordinates coordinates = new Coordinates(lat, lon);
            SetCoordinates(coordinates);
        }

        public void SetCoordinates(Coordinates coordinates)
        {
            SetZoomLevel();

            startCoordinates = coordinates;
            startCoordinates = startCoordinates.tileCenter(zoomLevel);
            Coordinates.setWorldOrigin(startCoordinates, FScale);

            CurrentTerrainCenter = UTILS.Calcul_TerrainIndex(StartCoordinates.latitude, StartCoordinates.longitude, zoomLevel);
            CurrentTextureCenter = UTILS.Calcul_TextureIndex(StartCoordinates.latitude, StartCoordinates.longitude, zoomLevel);

            rangeCoordinates = new Coordinates(StartCoordinates.latitude + latDgree * nRange, StartCoordinates.longitude);

            radius = 1;

            Coordinates coord = new Coordinates(ScenarioInfoClass.Instance.scenarioList.lat, ScenarioInfoClass.Instance.scenarioList.lon);

            Vector2 size = UTILS.GetTileSize(coord, zoomLevel);
            tileSizeMul = new Vector2((float)TileSize / size.x, (float)TileSize / size.y);
        }

        /// 작성 - KH
        /// <summary>
        /// 반경에 따른 줌레벨 조정
        /// </summary>
        public void SetZoomLevel()
        {
            if (nRange == 1) zoomLevel = 16;
            else if (nRange == 3) zoomLevel = 15;
            else if (nRange == 5) zoomLevel = 14;
        }
        #endregion

        public void EnableTcpSocket()
        {
            tcpSocketManager.Initialize();
        }

        public void DisableTcpSocket()
        {
            tcpSocketManager.OnClose();
        }

        #region 데이터 다운로드
        /// 작성 - KKH
        /// <summary>
        /// 시나리오 리스트 다운로드
        /// </summary>
        /// <param name="isCloud">클라우드 사용 유무</param>
        public void DownLoad_ScenarioLists()
        {
            if (isCloud)
            {
                string serviceUrl = $"{defaultData.serviceUri}/{defaultData.scenarioListsUri}";
                StartCoroutine(ServerConnectClass.Request_ScenarioList(serviceUrl));
            }
            else
            {

            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 데이터 다운로드
        /// </summary>
        /// <param name="isCloud">클라우드 사용 유무</param>
        public void DownLoad_ScenarioData()
        {
            if (isCloud)
            {
                string serviceUrl = $"{defaultData.serviceUri}/{defaultData.scenarioDataUri}/{ScenarioInfoClass.Instance.scenarioList.scenarioName}";
                StartCoroutine(ServerConnectClass.Request_ScenarioData(serviceUrl));
            }
            else
            {
                if (!isServer)
                {
                    Request_ScenarioData socketData = new Request_ScenarioData(ScenarioInfoClass.Instance.scenarioData.scenarioID);

                    if (TcpSocketManager.Instance.tcpClient != null)
                    {
                        TcpSocketManager.Instance.tcpClient.Send(socketData.Serialize());
                    }
                }
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시계열 정보 다운로드
        /// </summary>
        public void DownLoad_TimeSeriesData()
        {
            string serviceUrl = $"{defaultData.serviceUri}/{defaultData.timeSeriesUri}";
            StartCoroutine(ServerConnectClass.Request_TimeSeriesList(serviceUrl));
        }
        #endregion
    }
}