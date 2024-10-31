using Terrain.Tiles;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Collections;

namespace GCRR.VirtualMap
{
    public class LoadingManager : MonoBehaviour
    {
        #region UI
        [SerializeField] private GameObject canvas_Loding;
        [SerializeField] private GameObject image_Loding;
        [SerializeField] private TMP_Text text_NetState;
        #endregion

        #region instance
        private static LoadingManager instance;
        public static LoadingManager Instance => instance;
        #endregion

        #region Scenario
        private int nRadius;
        private int zoomLevel;
        private List<CreateTerrainInfo> createTerrainInfos;
        #endregion

        #region Loading Count
        [SerializeField] private float nMaxLoadingCnt = 10;
        [SerializeField] private float nLoadingCnt = 0;
        [SerializeField] private float fMaxTime = 30.0f;
        #endregion

        [SerializeField] private bool isLoadsceneCheck;

        [SerializeField] private bool isDataCheck = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            isLoadsceneCheck = false;

            nRadius = GameManager.Instance.Radius;
            zoomLevel = GameManager.Instance.zoomLevel;
            createTerrainInfos = new List<CreateTerrainInfo>();

            nLoadingCnt = 0;
        }

        void Start() 
        {
            GameManager.Instance.isLoadingDone = false;
            StartCoroutine(WaitLoadingTime());

            StartCoroutine(UTILS.LoadScene("03.VirtualMap"));
        }

        private void Update()
        {
            nLoadingCnt += Time.deltaTime;
            image_Loding.transform.Rotate(Vector3.forward * Time.deltaTime * 100);

            if (isLoadsceneCheck) return;        

            if (nLoadingCnt >= fMaxTime) isDataCheck = true;

            if (nLoadingCnt >= nMaxLoadingCnt)
            {
                if (isDataCheck)
                {
                    if (!isLoadsceneCheck)
                    {
                        isLoadsceneCheck = true;
                        GameManager.Instance.isLoadingDone = true;
                    }
                }
            }
        }


        /// <summary>
        /// 3D 게임 씬
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitLoadingTime()
        {
            NetworkStateUpdate($"가상 회의실 생성중...");
            yield return new WaitForSeconds(5.0f);

            NetworkStateUpdate($"가상 지도 생성중... ");
            yield return new WaitForSeconds(5.0f);
        }

        /// 작성 - KKH
        /// <summary>
        /// 클라우드 로그 업데이트
        /// </summary>
        /// <param name="state"></param>
        public void NetworkStateUpdate(string state)
        {
            if (text_NetState == null) return;

            text_NetState.text = state;
        }        
    }
}