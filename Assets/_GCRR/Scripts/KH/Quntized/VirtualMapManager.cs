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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class VirtualMapManager : MonoBehaviour
    {
        private static VirtualMapManager instance;
        public static VirtualMapManager Instance
        {
            get
            {
                if (instance == null)
                {
                    return null;
                }
                else
                {
                    return instance;
                }
            }
        }

        [SerializeField] private Transform trVirtualMap;
        public Transform TrVirtualMap => trVirtualMap;
        [SerializeField] private GameObject goVirtualMap;             // 가상지도 부모 오브젝트    
        public GameObject GoVirtualMap { get => goVirtualMap; set => goVirtualMap = value; }             // 가상지도 부모 오브젝트    
        [SerializeField] private GameObject goTargetData;             // 표적데이터 부모 오브젝트    
        public GameObject GoTargetData { get => goTargetData; set => goTargetData = value; }             // 표적데이터 부모 오브젝트    

        [SerializeField] private List<GameObject> goTers;         // 터레인 오브젝트 리스트
        public List<GameObject> goTerrains { get => goTers; set => goTers = value; }         // 터레인 오브젝트 리스트

        [SerializeField] private TerrainCreateClass terrainCreateClass;
        public TerrainCreateClass terrainCreateclass => terrainCreateClass;

        [SerializeField] private bool isAutoCreateVirtualMap = true;
        [SerializeField] private bool isCreateObject = true;
        [SerializeField] private bool isCreateTarget = true;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            StartCoroutine(CreateMap());            
        }

        public IEnumerator CreateMap()
        {
            GameManager.Instance.isLoadingDone = false;

            terrainCreateClass = GetComponent<TerrainCreateClass>();
            if (isAutoCreateVirtualMap)
                StartCoroutine(terrainCreateClass.Initialize(GameManager.Instance.zoomLevel, GameManager.Instance.Radius / 2, GameManager.Instance.CurrentTerrainCenter, GameManager.Instance.CurrentTextureCenter, ScenarioInfoClass.Instance.timeType));

            if (isCreateObject)
                StartCoroutine(CreateScenarioObject());

            if (isCreateTarget)
                StartCoroutine(CreateTarget(ScenarioInfoClass.Instance.timeType));

            GameManager.Instance.isLoadingDone = true;
            yield return null;
        }

        private IEnumerator CreateScenarioObject()
        {
            yield return new WaitForSeconds(0.5f);
            //yield return null;

            foreach (var go in ScenarioObjectManager.Instance.scenarioGO)
            {
                Destroy(go);
            }
            ScenarioObjectManager.Instance.scenarioGO.Clear();

            for (int i = 1; i < ScenarioInfoClass.Instance.scenarioData.objectCount + 1; i++)
            {
                ObjectData objectData = ScenarioInfoClass.Instance.scenarioData.objects[i - 1];

                ScenarioObject sO = ScenarioObjectManager.Instance.AssignObject(objectData);
                if (sO == null) continue;
                sO.SetObjectNum(i);

                yield return null;
            }
        }

        private IEnumerator CreateTarget(int timeType)
        {
            //yield return new WaitForSeconds(0.5f);
            yield return null;

            TargetCreateManager.Instance.Initialize(GameManager.Instance.zoomLevel, timeType);
        }        
    }
}