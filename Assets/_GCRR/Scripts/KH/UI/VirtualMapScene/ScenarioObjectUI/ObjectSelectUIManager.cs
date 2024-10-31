using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class ObjectSelectUIManager : MonoBehaviour
    {
        [Header("SecnarioObject_Select")]
        [SerializeField] private Transform view_ObjectSelectList;
        [SerializeField] private GameObject btn_SelectObj;

        private void Start()
        {
            RemoveScrollViewList();
            RemoveScrollViewOutline();
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 오브젝트 선택 리스트 초기화(스크롤뷰)
        /// </summary>
        public void LoadObjectSelectList()
        {
            ScenarioObjectManager som = ScenarioObjectManager.Instance;
            if (som == null) return;

            if (view_ObjectSelectList == null) return;
            RemoveScrollViewList();

            for (int i = 0; i < som.scenarioGO.Count; i++)
            {
                var obBtn = Instantiate(btn_SelectObj, view_ObjectSelectList);
                ObjectData od = som.scenarioGO[i].GetComponent<ScenarioObject>().objectData;
                obBtn.GetComponent<SelectScenarioObjectBtn>().SetBtn(som.scenarioGO[i].gameObject, od);
            }

            RemoveScrollViewOutline();
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 추가 오브젝트 목록 초기화(스크롤뷰)
        /// </summary>
        public void RemoveScrollViewList()
        {
            if (view_ObjectSelectList.childCount <= 0) return;

            // child 에는 부모와 자식이 함께 설정 된다.
            var child = view_ObjectSelectList.GetComponentsInChildren<SelectScenarioObjectBtn>();

            foreach (var iter in child)
            {
                Destroy(iter.gameObject);
            }
        }

        /// 작성 - KKH
        /// <summary>
        /// 시나리오 추가 오브젝트 아웃라인 전체 제거
        /// </summary>
        public void RemoveScrollViewOutline()
        {
            var child = view_ObjectSelectList.GetComponentsInChildren<SelectScenarioObjectBtn>();
            foreach (var iter in child)
            {
                iter.GetComponent<Outline>().enabled = false;
            }
        }
    }
}