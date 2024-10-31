using GoShared;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Tiles
{
    public static class TerrainTileIndexer
    {
        /// <summary>
        /// RTV 제공 TMS 변환
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Vector2 LlaToTms(double lat, double lon, int zoom)
        {
            int numTiles = (int)Math.Pow(2, zoom);

            // X 인덱스 계산
            int x = (int)((lon + 180.0) / 360.0 * numTiles);

            // Y 인덱스 계산
            double latRad = lat * (Math.PI / 180.0);

            int y = (int)((1.0 - (Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI)) / 2.0 * numTiles);
            y = numTiles - y - 1; // Google TMS는 OSGeo TMS와 Y축 인덱스가 반대

            return new Vector2(x, y);
        }

        /// <summary>
        /// 터레인 인덱스 계산
        /// </summary>
        // 2023.02.09 - KKH : 참고 파일폴더 YoYoProject\YoYo_Mill\지리공간정보 클라우드기반 실세계 현실화 기술\from.D2\220809\타일데이터_자료
        public static Vector2 GetTerrainIndex(double lat_, double lon_, int zoom_)
        {
            Vector2 terrainIndex = new Vector2();

            #region from - D2
            lon_ = (180 + lon_) / 360;
            lat_ = (90 + lat_) / 180;

            var num = Math.Pow(2, zoom_);

            terrainIndex.x = (int)(lon_ * num * 2);
            terrainIndex.y = (int)(lat_ * num);
            #endregion

            return terrainIndex;
        }

        public static Vector2 GetIndexToLocation(double tile_x, double tile_y, int zoom_)
        {
            var num = Math.Pow(2, zoom_);
            float x = (float)(tile_x / num / 2);
            float y = (float)(tile_y / num);

            float lon = (360 * x) - 180;
            float lat = (180 * y) - 90;

            return new Vector2(lat,lon);
        }

        public static Vector2 GetTextureIndex(double lat_, double lon_, int zoom_)
        {
            Vector2 texutreIndex = new Vector2();

            texutreIndex = GetGeocenterMeter(lat_, lon_);
            texutreIndex = GetTMSTextureIndex(texutreIndex, zoom_);

            return new Vector2((int)texutreIndex.x, (int)texutreIndex.y);
        }

        #region 220809 FromD2
        private static double Googlemaps_Tileoffset_X = -20037508.342789244;
        private static double Googlemaps_Tileoffset_Y = 20037508.342789244;
        static double Googlemaps_ZeroLevelTileSize = 40075016.68557848;
        static double Googlemaps_TileSize_Half = 20037508.342789244;

        /// <summary>
        /// TMSTextureIndex 계산
        /// </summary>
        /// <param name="_geoCenterX"></param>
        /// <param name="_geoCenterY"></param>
        /// <param name="_ZoomLevel">줌 레벨</param>
        /// <returns></returns>
        public static Vector2 GetTMSTextureIndex(Vector2 geoCenter_/*_geoCenterX, double _geoCenterY*/, int zoomLevel_)
        {
            #region Origin Example
            //tileSizeMeter = GOOGLEMAPS_ZEROLEVELTILESIZE / pow(2.0, level);
            //indexX = (int)((geoCenter.x - GOOGLEMAPS_TILEOFFSET_X) / tileSizeMeter);
            //indexY = (int)((GOOGLEMAPS_TILEOFFSET_Y - geoCenter.y) / tileSizeMeter);
            #endregion

            Vector2 i = new Vector2();

            var tile_SizeMeter = Googlemaps_ZeroLevelTileSize / Math.Pow(2, zoomLevel_);
            i.x = (float)((geoCenter_.x - (Googlemaps_Tileoffset_X)) / tile_SizeMeter);
            i.y = (float)((Googlemaps_Tileoffset_Y - geoCenter_.y) / tile_SizeMeter);
            i.x = (int)i.x;
            i.y = (int)i.y;
            return i;
        }

        // DH : FromD2용 LatLong -> Meter
        // latlon to EPSG 900913
        // https://gist.github.com/springmeyer/871897
        /// <summary>
        /// Degrees -> Meters 변환
        /// </summary>
        public static Vector2 GetGeocenterMeter(double latitude_, double longitude_)
        {
            Vector2 i = new Vector2();
            i.x = (float)(longitude_ * Googlemaps_TileSize_Half / 180);
            i.y = (float)(Math.Log(Math.Tan((90 + latitude_) * Math.PI / 360)) / (Math.PI / 180));
            i.y = (float)(i.y * Googlemaps_TileSize_Half / 180);
            return i;
        }
        #endregion
    }

    public struct CreateTerrainInfo
    {
        public bool bUseAriangTMS;
        public int zoomLevel;
        public Vector2 index_TerrainNumber;
        public Vector2 index_TextureNumber;
        public int x;
        public int y;

        public Vector2 currentTerrainNumber;
        public Vector2 currentTextureNumber;

        private Vector2 GetTileSize(Vector2 coord, int zoomLevel)
        {
            Coordinates coordinates = new Coordinates(coord, zoomLevel);
            Vector3[] vs = coordinates.tileCenter(zoomLevel).tileVertices(zoomLevel).ToArray();
            List<Vector3> vertices = vs.ToList();
            float tileHeight = Vector3.Distance(vertices[0], vertices[1]);
            float tileWidth = Vector3.Distance(vertices[1], vertices[2]);

            return new Vector2(tileWidth, tileHeight);
        }


        public CreateTerrainInfo(int zoomLevel_, Vector2 indexTerrainCenter_, Vector2 indexTexureCenter_, int x_, int y_, bool bUseAriangTMS_)
        {
            this.bUseAriangTMS = bUseAriangTMS_;
            this.zoomLevel = zoomLevel_;
            this.index_TerrainNumber = indexTerrainCenter_;
            this.index_TextureNumber = indexTexureCenter_;
            this.x = x_;
            this.y = y_;
            this.currentTerrainNumber = new Vector2(indexTerrainCenter_.x + x_, indexTerrainCenter_.y + y_);

            if (bUseAriangTMS)
            {
                this.currentTextureNumber.x = index_TextureNumber.x + x;
                this.currentTextureNumber.y = index_TextureNumber.y + -y;
            }
            else
            {
                this.currentTextureNumber.x = index_TextureNumber.y + -y;
                this.currentTextureNumber.y = index_TextureNumber.x + x;
            }
        }

        public CreateTerrainInfo(int zoomLevel_, Vector2 indexTerrainCenter_, Vector2 indexTexureCenter_, int x_, int y_)
        {
            this.bUseAriangTMS = true;
            this.zoomLevel = zoomLevel_;
            this.index_TerrainNumber = indexTerrainCenter_;
            this.index_TextureNumber = indexTexureCenter_;
            this.x = x_;
            this.y = y_;
            this.currentTerrainNumber = new Vector2(indexTerrainCenter_.x + x_, indexTerrainCenter_.y + y_);

            this.currentTextureNumber.x = index_TextureNumber.x + x;
            this.currentTextureNumber.y = index_TextureNumber.y + -y;
        }

        public CreateTerrainInfo(int zoomLevel_, Vector2 indexTerrainCenter_, Vector2 indexTexureCenter_, bool bUseAriangTMS_)
        {
            this.bUseAriangTMS = bUseAriangTMS_;
            this.zoomLevel = zoomLevel_;
            this.index_TerrainNumber = indexTerrainCenter_;
            this.index_TextureNumber = indexTexureCenter_;
            this.x = 0;
            this.y = 0;
            this.currentTerrainNumber = indexTerrainCenter_;

            if (bUseAriangTMS)
            {
                this.currentTextureNumber = indexTerrainCenter_;
            }
            else
            {
                this.currentTextureNumber.x = index_TextureNumber.y;
                this.currentTextureNumber.y = index_TextureNumber.x;
            }
        }
    }
}