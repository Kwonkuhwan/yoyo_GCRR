using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class ScenarioObjectBtn : MonoBehaviour
    {
        [SerializeField] private Image obj_Image;
        [SerializeField] private TMP_Text obj_Name;
        [SerializeField] private ObjectID obj_ObjectID;

        public void SetBtn(Sprite sprite, string name, ObjectID objectID)
        {
            obj_Image.sprite = sprite;
            obj_Name.text = name;
            obj_ObjectID = objectID;
        }

        public void OnClick()
        {
            ScenarioObjectManager.Instance.SelectObject(obj_ObjectID);
            GetComponent<Outline>().enabled = true;
        }
    }
}