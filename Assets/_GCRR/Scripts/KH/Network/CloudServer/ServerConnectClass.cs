/*************************************************************************************************************************
 * 
 * 최초 작성자           : 권구환
 * 작성 일자            : 2023.05.12
 * 작성 목록            : 변수 및 함수 선언
 * 
 * 수정 사항
 * 수정자 및 수정 일시  : KKH
 * 수정 내용           : 
 * 
 *************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using Terrain.Tiles;
using System.Linq;

namespace GCRR.VirtualMap
{
    public static class ServerConnectClass
    {
        #region 시나리오 목록
        /// 작성 : KKH
        /// <summary>
        /// 시나리오 목록 요청, 다운로드
        /// </summary>
        /// <returns>true, false</returns>
        public static IEnumerator Request_ScenarioList(string stServiceUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Request_ScenarioList(stServiceUrl);
                }
                else
                {
                    ScenarioInfoClass.Instance.scenarioLists = JsonManager.ScenarioListsJsonLoad(uwr.downloadHandler.data);
                }
            }
        }
        #endregion

        #region 시나리오 데이터
        /// 작성 : KKH
        /// <summary>
        /// 시나리오 데이터 요청, 다운로드
        /// </summary>
        /// <returns>true, false</returns>
        public static IEnumerator Request_ScenarioData(string stServiceUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Request_ScenarioData(stServiceUrl);
                }
                else
                {
                    ScenarioInfoClass.Instance.scenarioData = JsonManager.ScenarioDataJsonLoad(uwr.downloadHandler.data);

                    //DataSyncManager.Instance.Save_ScenarioData();
                }
            }
        }
        #endregion

        #region 시계열 데이터
        /// 작성 : KKH
        /// <summary>
        /// 시나리오 목록 요청, 다운로드
        /// </summary>
        /// <returns>true, false</returns>
        public static IEnumerator Request_TimeSeriesList(string stServiceUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Request_TimeSeriesList(stServiceUrl);
                }
                else
                {
                    // 시계열 정보 리스트 입력
                    Timeseries timeseries = JsonManager.TimeSeriesDataLoad(uwr.downloadHandler.data).Timeseries;

                    ScenarioInfoClass.Instance.tileseriesLists.imageLayer = Enumerable.Reverse(timeseries.imageLayer).ToList();
                    ScenarioInfoClass.Instance.tileseriesLists.terrainLayer = Enumerable.Reverse(timeseries.terrainLayer).ToList();
                    foreach (Tile3DLayer t3l in Enumerable.Reverse(timeseries.tile3dLayer).ToList())
                    {
                        if (t3l.Format != "glb") continue;

                        if (ScenarioInfoClass.Instance.tileseriesLists.tile3dLayer == null) ScenarioInfoClass.Instance.tileseriesLists.tile3dLayer = new List<Tile3DLayer>();
                        ScenarioInfoClass.Instance.tileseriesLists.tile3dLayer.Add(t3l);
                    }
                }
            }
        }
        #endregion

        #region 표적정보 데이터
        /// 작성 : KKH
        /// <summary>
        /// 표적정보 Json 데이터 요청, 다운로드
        /// </summary>
        /// <returns>true, false</returns>
        public static IEnumerator Request_TargetDataJson(string stServiceUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    TargetCreateManager.Instance.targetDones.Add(false);
                    //Request_TargetDataJson(stServiceUrl);
                }
                else
                {
                    //ScenarioInfoClass.Instance.targetData = JsonManager.TargetDataJsonLoad(uwr.downloadHandler.data);
                    TargetCreateManager.Instance.CreateTargetCorutine(JsonManager.TargetDataJsonLoad(uwr.downloadHandler.data));
                    //ScenarioInfoClass.Instance.targetData.Add(JsonManager.TargetDataJsonLoad(uwr.downloadHandler.data));
                    //DataSyncManager.Instance.Save_ScenarioData();
                }
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 표적 데이터(빌딩) 요청, 다운로드
        /// </summary>
        /// <returns>true, false</returns>
        public static IEnumerator Request_TargetData(string stServiceUrl, TileSet tileSet, string dirPath, string fileName)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Request_TargetData(stServiceUrl, tileSet, dirPath, fileName);
                }
                else
                {
                    //TargetCreateManager.Instance.SetCreate(uwr.downloadHandler.data, tileSet);
                    if (GameManager.Instance.isMapDataDownLoad && !File.Exists(dirPath + fileName))
                    {
                        UTILS.DownLoad_Data(uwr, dirPath, fileName);
                    }
                }
            }
        }

        /// <summary>
        /// 표적 정보 메타데이터 다운로드
        /// </summary>
        public static IEnumerator Request_PostTargetMetaData(string stServiceUrl, Action<string> callback)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, ""))
            {
                uwr.SetRequestHeader("Content-Type", "application/json");
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    UTILS.LogWarning(uwr.error);
                    UTILS.LogWarning("Request_TargetMetaData Error");
                }
                else
                {
                    callback(uwr.downloadHandler.text);
                }
            }
        }
        #endregion

        #region 터레인 데이터
        /// 작성 : KKH
        /// <summary>
        /// 가상지도 터레인 데이터 요청
        /// </summary>
        /// <returns>true, false</returns>
        public static IEnumerator Requset_TerrainData(string stServiceUrl, GameObject obj, string dirPath, string fileName)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    //Requset_TerrainData(stServiceUrl, obj, dirPath, fileName);
                    //GameObject.Destroy(obj.transform.parent.gameObject);
                    VirtualMapManager.Instance.terrainCreateclass.SetMesh(null, obj);
                }
                else
                {
                    VirtualMapManager.Instance.terrainCreateclass.SetMesh(TerrainTileParser.Parse(uwr.downloadHandler.data), obj);
                    if (GameManager.Instance.isMapDataDownLoad && !File.Exists(dirPath + fileName))
                    {
                        UTILS.DownLoad_Data(uwr, dirPath, fileName);
                    }
                    yield return null;
                }
            }
        }
        #endregion

        #region 텍스쳐 데이터
        /// 작성 : KKH
        /// <summary>
        /// 가상지도 텍스쳐 요청
        /// </summary>
        /// <returns>true, false</returns>

        public static IEnumerator Request_TextureData(CreateTerrainInfo terrainInfo, string stServiceUrl, GameObject obj, string dirPath, string fileName)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    VirtualMapManager.Instance.terrainCreateclass.BaseTextureDownload(terrainInfo, obj);
                }
                else
                {
                    Texture2D temp_Texture2D = new Texture2D(256, 256);
                    temp_Texture2D.LoadImage(uwr.downloadHandler.data);

                    VirtualMapManager.Instance.terrainCreateclass.SetTexture(temp_Texture2D, obj);
                    if (GameManager.Instance.isMapDataDownLoad && !File.Exists(dirPath + fileName))
                    {
                        UTILS.DownLoad_Data(uwr, dirPath, fileName);
                    }
                    yield return null;
                }
            }
        }
        #endregion
    }
}