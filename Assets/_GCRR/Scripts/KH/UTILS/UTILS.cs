using GoShared;
using System.Text;
using System;
using Terrain.Tiles;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

namespace GCRR.VirtualMap
{
    public static class UTILS
    {
        public static void Log(string msg)
        {
#if UNITY_EDITOR
            Debug.Log($"{msg}");
#endif
        }

        public static void LogWarning(string msg)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{msg}");
#endif
        }

        public static void LogError(string msg)
        {
#if UNITY_EDITOR
            Debug.LogError($"{msg}");
#endif
        }

        public static IEnumerator LoadScene(string sceneName)
        {
            //if (sceneName == "02.Loding") GameManager.Instance.isLoadingDone = true;
            GameManager.Instance.isLoadingDone = true;
            AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
            asyncOper.allowSceneActivation = false;
            while (!asyncOper.isDone)
            {
                yield return null;
                if (GameManager.Instance.isLoadingDone)
                {
                    asyncOper.allowSceneActivation = true;
                    GameManager.Instance.isLoadingDone = false;
                }
            }
        }

        /// <summary>
        /// Transform 초기화
        /// </summary>
        /// <param name="tr">초기화할 Transform</param>
        public static void Init_Transform(Transform tr)
        {
            tr.position = Vector3.zero;
            tr.rotation = Quaternion.Euler(Vector3.zero);

            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.Euler(Vector3.zero);

            tr.localScale = Vector3.one;
        }

        public static void Init_Position(Transform tr)
        {
            tr.position = Vector3.zero;
            tr.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 가상지도 터레인 인덱스 계산
        /// </summary>
        /// <param name="lat">위도</param>
        /// <param name="lon">경도</param>
        /// <returns></returns>
        public static Vector2 Calcul_TerrainIndex(double lat, double lon, int zoomLevel)
        {
            return TerrainTileIndexer.GetTerrainIndex(lat, lon, zoomLevel - 1);
        }

        /// <summary>
        /// 가상지도 텍스쳐 인덱스 계산
        /// </summary>
        /// <param name="lat">위도</param>
        /// <param name="lon">경도</param>
        /// <returns></returns>
        public static Vector2 Calcul_TextureIndex(double lat, double lon, int zoomLevel)
        {
            Coordinates coordinates = new Coordinates(lat, lon);
            Vector2 vec = coordinates.tileCoordinates(zoomLevel);
            return vec;
        }

        /// 작성 : KKH
        /// <summary>
        /// 위,경도 평균
        /// </summary>
        /// <param name="d1">위경도 계산 할 region 값(남, 서)</param>
        /// <param name="d2">위경도 계산 할 region 값(북, 동)</param>
        /// <returns></returns>
        public static double Cal_LocationArg(double d1, double d2)
        {
            return ((Cal_Location(d1)) + (Cal_Location(d2))) / 2;
        }

        /// 작성 : KKH
        /// <summary>
        /// 위, 경도 계산
        /// </summary>
        /// <param name="d">위경도 계산 할 region 값</param>
        /// <returns></returns>
        public static double Cal_Location(double d)
        {
            return (float)((d * 180.0f) / Mathf.PI);
        }

        /// <summary>
        /// 오브젝트 위경도 -> Vector3 값 계산
        /// </summary>
        /// <param name="latitude">위도</param>
        /// <param name="longitude">경도</param>
        /// <returns></returns>
        public static Vector3 GetObjPos(Coordinates startCoordnates, float latitude, float longitude, bool isWing)
        {
            Coordinates targetCoord = new Coordinates(latitude, longitude);                 // 시나리오에서 가져온 3D 객체의 위도, 경도값를 넣어서 Coordniates로 생성
            var tileCenter = startCoordnates.tileCenter(GameManager.Instance.zoomLevel);
            var targetVector = targetCoord.convertCoordinateToVector();                     // 3D 객체의 위경도 기반 좌표를 Coordinates 클래스의 convertCoordinateToVector() 함수 사용하여 ECEF 좌료계로 변환
            var centerVector = tileCenter.convertCoordinateToVector();                      // TileCenter의 위경도 기반 좌표를 Coordinates 클래스의 convertCoordinateToVector() 함수 사용하여 ECEF 좌료계로 변환
            var pos = targetVector - centerVector;                                          // tile center 좌표도 convertCoordinateToVector() 함수 사용하여 ECEF 좌료계로 변환 후 targetVector에서 빼줌

            pos = new Vector3(pos.x * GameManager.Instance.TileSizeMul.x, pos.y, pos.z * GameManager.Instance.TileSizeMul.y);
            if (isWing) pos.y = GetHeight();
            return pos;
        }

        public static Vector3 GetObjPos(Coordinates startCoordnates, double latitude, double longitude, bool isWing)
        {
            return GetObjPos(startCoordnates, (float)latitude, (float)longitude, isWing);
        }

        private static float GetHeight()
        {
            switch (GameManager.Instance.zoomLevel)
            {
                case 14:
                    return 1.0f;
                default:
                    return 0.5f;
            }
        }

        /// <summary>
        /// 오브젝트 Vector3값 -> 위경도 변환
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 GetObjLocation(Vector3 position)
        {
            position = new Vector3((position.x / GameManager.Instance.FScale) / GameManager.Instance.TileSizeMul.x, position.y, (position.z / GameManager.Instance.FScale) / GameManager.Instance.TileSizeMul.y);
            Coordinates targetCoord = Coordinates.convertVectorToCoordinates(position); // 시나리오에서 가져온 3D 객체의 위도, 경도값를 넣어서 Coordniates로 생성

            return new Vector2((float)targetCoord.latitude, (float)targetCoord.longitude);
        }

        public static Vector2 GetTileSize(Vector2 coord, int zoomLevel)
        {
            Coordinates coordinates = new Coordinates(coord, zoomLevel);
            return GetTileSize(coordinates, zoomLevel);
        }

        public static Vector2 GetTileSize(float lat, float lon, int zoomLevel)
        {
            Coordinates coordinates = new Coordinates(lat, lon);
            return GetTileSize(coordinates, zoomLevel);
        }

        public static Vector2 GetTileSize(double lat, double lon, int zoomLevel)
        {
            Coordinates coordinates = new Coordinates(lat, lon);
            return GetTileSize(coordinates, zoomLevel);
        }

        public static Vector2 GetTileSize(Coordinates coordinates, int zoomLevel)
        {
            Vector3[] vs = coordinates.tileCenter(zoomLevel).tileVertices(zoomLevel).ToArray();
            List<Vector3> vertices = vs.ToList();
            float tileHeight = Vector3.Distance(vertices[0], vertices[1]);
            float tileWidth = Vector3.Distance(vertices[1], vertices[2]);
            return new Vector2(tileWidth, tileHeight);
        }

        #region Byte 파싱
        public static byte[] StringToBytes(string str)
        {
            byte[] strBytes = Encoding.Unicode.GetBytes(str);
            byte[] lengthBytes = BitConverter.GetBytes((ushort)strBytes.Length);

            byte[] result = new byte[strBytes.Length + lengthBytes.Length];
            lengthBytes.CopyTo(result, 0);
            strBytes.CopyTo(result, lengthBytes.Length);

            return result;
        }

        public static byte[] AddBytes(byte[] bytes1, byte[] bytes2)
        {
            byte[] result = new byte[bytes1.Length + bytes2.Length];
            bytes1.CopyTo(result, 0);
            bytes2.CopyTo(result, bytes1.Length);

            return result;
        }

        public static int BytesToInt(byte[] bytes, ref int parsCnt)
        {
            byte[] result = new byte[sizeof(int)];
            Array.Copy(bytes, parsCnt, result, 0, sizeof(int));
            parsCnt += sizeof(int);
            return BitConverter.ToInt32(result, 0);
        }

        public static float BytesToFloat(byte[] bytes, ref int parsCnt)
        {
            byte[] result = new byte[sizeof(float)];
            Array.Copy(bytes, parsCnt, result, 0, sizeof(float));
            parsCnt += sizeof(float);
            return BitConverter.ToSingle(result, 0);
        }

        public static double BytesToDouble(byte[] bytes, ref int parsCnt)
        {
            byte[] result = new byte[sizeof(double)];
            Array.Copy(bytes, parsCnt, result, 0, sizeof(double));
            parsCnt += sizeof(double);
            return BitConverter.ToDouble(result, 0);
        }

        public static ushort BytesToUShort(byte[] bytes, ref int parsCnt)
        {
            byte[] result = new byte[sizeof(ushort)];
            Array.Copy(bytes, parsCnt, result, 0, sizeof(ushort));
            parsCnt += sizeof(ushort);
            return (ushort)BitConverter.ToInt16(result, 0);
        }

        public static bool BytesToBoolean(byte[] bytes, ref int parsCnt)
        {
            byte[] result = new byte[sizeof(bool)];
            Array.Copy(bytes, parsCnt, result, 0, sizeof(bool));
            parsCnt += sizeof(bool);
            return BitConverter.ToBoolean(result, 0);
        }

        public static string BytesToString(byte[] bytes, int byteSize, ref int parsCnt)
        {
            byte[] result = new byte[byteSize];
            Array.Copy(bytes, parsCnt, result, 0, byteSize);
            parsCnt += byteSize;
            return Encoding.Unicode.GetString(result);
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region 데이터 다운로드
        /// 작성 : KKH
        /// <summary>
        /// 시나리오 목록들 수신
        /// </summary>
        /// <returns>true, false</returns>
        public static bool DownLoad_Data(UnityWebRequest uwr, string dirPath, string fileName)
        {
            string fullFilePath = Path.Combine(dirPath, fileName);

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                System.IO.Stream tempStream = new System.IO.MemoryStream(uwr.downloadHandler.data);

                DownLoad_Data(uwr.downloadHandler.data, fullFilePath);

                tempStream.Dispose();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool DownLoad_Data(byte[] textureBytes, string dirPath, string fileName)
        {
            string fullFilePath = Path.Combine(dirPath, fileName);

            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            return DownLoad_Data(textureBytes, fullFilePath);
        }

        public static bool DownLoad_Data(byte[] textureBytes, string fullFilePath)
        {
            try
            {
                System.IO.Stream tempStream = new System.IO.MemoryStream(textureBytes);

                if (!File.Exists(fullFilePath))
                {
                    using (FileStream outputFileStream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        new System.IO.MemoryStream(textureBytes).CopyTo(outputFileStream);
                    }
                }
                tempStream.Dispose();
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion

        public static Stream GetStream(string dirPath, string fileName)
        {
            try
            {
                string file_Path = $"{dirPath}\\{fileName}";

                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                FileInfo fileInfo = new FileInfo(file_Path);
                if (!fileInfo.Exists)
                {
                    LogWarning($"{file_Path} : Data is Empty!!!");
                    return null;
                }

                System.IO.Stream tempStream = null;
                try
                {
                    tempStream = new System.IO.FileStream(file_Path, FileMode.Open);
                }
                catch (Exception e)
                {
                    LogError($"{e}");
                    Task.Delay(1000).Wait();

                    return GetStream(dirPath, fileName);
                }

                return tempStream;
            }
            catch (Exception e)
            {
                LogError($"{e}");
                return null;
            }
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            MemoryStream mStream = new MemoryStream();
            stream.CopyTo(mStream);
            return mStream.ToArray();
        }
    }
}