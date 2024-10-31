using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class SelectScenarioObjectBtn : MonoBehaviour
    {
        [SerializeField] private GameObject go_Sobj;
        [SerializeField] private TMP_Text obj_UintCode;
        [SerializeField] private TMP_Text obj_UnitName;
        [SerializeField] private TMP_Text obj_UintLat;
        [SerializeField] private TMP_Text obj_UintLon;
        [SerializeField] private TMP_Text obj_UintHeight;
        [SerializeField] private TMP_Text obj_UintDirection;
        [SerializeField] private TMP_Text obj_UintMission;

        public void SetBtn(GameObject sobj, ObjectData od)
        {
            go_Sobj = sobj;
            obj_UintCode.text = od.unitCode;
            obj_UnitName.text = od.unitName;
            obj_UintLat.text = od.unitLat.ToString("N4");
            obj_UintLon.text = od.unitLon.ToString("N4");
            obj_UintHeight.text = od.height.ToString();
            obj_UintDirection.text = od.movingDirection.ToString();
            obj_UintMission.text = od.mission;
        }

        public void OnClick()
        {
            ObjectSelectUIManager osUIM = UIManager.Instance.OSUIM;
            if (osUIM == null) return;

            osUIM.RemoveScrollViewOutline();

            GetComponent<Outline>().enabled = true;
            //Image image =  go_Sobj.Find("Image").;
            ScenarioObjectManager.Instance.SelectObject(go_Sobj);
            go_Sobj.GetComponent<Animation>().Play();
        }
    }
}