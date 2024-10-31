using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class TargetInfo : MonoBehaviour
    {
        private string fileName;
        public TargetMetaData metaData;

        private double westDrgee;
        private double southDrgee;
        private double eastDrgee;
        private double northDrgee;
        private double minHeight;
        private double maxHeight;

        public double WestDrgee { get => westDrgee; set => westDrgee = value; }
        public double SouthDrgee { get => southDrgee; set => southDrgee = value; }
        public double EastDrgee { get => eastDrgee; set => eastDrgee = value; }
        public double NorthDrgee { get => northDrgee; set => northDrgee = value; }
        public double MinHeight { get => minHeight; set => minHeight = value; }
        public double MaxHeight { get => maxHeight; set => maxHeight = value; }

        [Space(10)]

        [SerializeField] private double Lat;
        public double lat { get => Lat; set => Lat = value; }

        [SerializeField] private double Lon;
        public double lon { get => Lon; set => Lon = value; }

        [SerializeField] private float Height;
        public float height { get => Height; set => Height = value; }

        [SerializeField] Vector2 terrainTile;
        [SerializeField] Vector2 textureTile;

        private void Start()
        {
            Vector2 terrain_Tile = UTILS.Calcul_TerrainIndex(lat, lon, GameManager.Instance.zoomLevel);
            if (terrainTile != terrain_Tile)
            {
                terrainTile = terrain_Tile;
            }

            Vector2 texture_Tile = UTILS.Calcul_TextureIndex(lat, lon, GameManager.Instance.zoomLevel);
            if (textureTile != texture_Tile)
            {
                textureTile = texture_Tile;
            }

            try
            {
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
                    Destroy(gameObject);
                }
                if (parent != null && parent != transform.parent)
                {
                    transform.parent = parent;
                    gameObject.SetActive(true);
                }
            }
            catch
            {
                Destroy(gameObject);
                return;
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 표적 메타데이터 초기화
        /// </summary>
        /// <param name="fileName_"></param>
        public void MetaDataInit(string fileName_)
        {            
            fileName = fileName_;

            // [추가] KKH : 메타 데이터 요청 URL 설정
            string stServiceUrl = $"{GameManager.Instance.defaultData.serviceUri}/metadata/searchTargetList?buildingname={fileName}";

            // [추가] KKH : 메타 데이터 요청
            StartCoroutine(ServerConnectClass.Request_PostTargetMetaData(stServiceUrl, (data) =>
            {
                // [추가] KKH : 메타 데이터가 Null 또는 Empty이면 리턴
                if (string.IsNullOrEmpty(data)) return;

                // [추가] KKH : 데이터 전처리
                JArray jArray = JArray.Parse(data);
                if (jArray == null) return;
                if (jArray.First == null) return;

                TargetMetaData meta_Data = jArray.First.ToObject<TargetMetaData>();

                if(meta_Data == null) return;

                // [추가] KKH : 데이터 입력
                metaData = meta_Data;
            }));
        }

        /// 작성 : KKH
        /// <summary>
        /// 표적 정보 판넬 켜기
        /// </summary>
        public void ShowPanel()
        {
            if (TargetUIManager.Instance == null) return;
            if (ScenarioObjectManager.Instance.isMoveOn) return;

            // [추가] KKH : 메타 데이터를 통한 판넬 On
            TargetUIManager.Instance.Panel_On(metaData, (float)lat, (float)lon);
        }        
    }
}