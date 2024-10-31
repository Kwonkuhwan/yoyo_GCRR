using BNG;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GCRR.VirtualMap
{
    public class ScenarioObject : MonoBehaviour
    {
        [SerializeField] private Vector2 terrainTile;
        [SerializeField] private Vector2 textureTile;
        [SerializeField] private bool isMark = false;

        [SerializeField] private Canvas canvas_Mark;
        [SerializeField] private Image iconImage;
        [SerializeField] private LookAtTransform lookAtTransform;
        [SerializeField] private Sprite[] iconSprite;

        [SerializeField] private ObjectData objData = new ObjectData();
        public ObjectData objectData { get => objData; set => objData = value; }

        [SerializeField] private int objNum;           // 시나리오 객체 순번
        public int objectNum { get => objNum; set => objNum = value; }           // 시나리오 객체 순번


        [SerializeField] private ObjectID objID;
        public ObjectID objectID { get => objID; set => objID = value; }

        private void Awake()
        {
            if (lookAtTransform != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("LookAtCamera");
                lookAtTransform.LookAt = player.transform;
            }
        }

        public void SetObjectNum(int objectNum) { this.objectNum = objectNum; }

        public int GetObjectNum() { return objectNum; }

        /// <summary>
        /// 오브젝트 데이터 세팅
        /// </summary>
        /// <param name="_objectType">구분</param>
        /// <param name="lat">부대 위도</param>
        /// <param name="lon">부대 경도</param>
        /// <param name="objectSetting">세팅 데이터</param>
        public bool SetObjectData(float lat, float lon, ObjectSettingData objectSetting)
        {
            ObjectData objectData_ = new ObjectData();

            objectData_.objectID = ((int)objectID);
            objectData_.forceSize = (objectSetting.forceSize);                   // 부대 규모
            objectData_.forceCode = (objectSetting.forceCode);                   // 부대 약어
            objectData_.unitName = (objectSetting.unitName);                     // 부대 명
            objectData_.unitCode = (objectSetting.unitCode);                     // 부대 약어명
            objectData_.isAlly = (objectSetting.unitAlly);                       // 적아 구분
            objectData_.unitLat = (lat);                                        // 위도
            objectData_.unitLon = (lon);                                        // 경도
            objectData_.height = (objectSetting.unitHeight);                 // 고도
            objectData_.movingDirection = (objectSetting.unitDirection);         // 이동 방향
            objectData_.mission = (objectSetting.unitMission);               // 미션 정보
            objectData_.showForceMark = (objectSetting.unitShowMark);            // 부호 도시 여부

            return SetObjectData(objectData_);
        }

        public bool SetObjectData(ObjectData objectData)
        {
            try
            {
                this.objectData = objectData;
            }
            catch(Exception e)
            {
                UTILS.LogError($"{e}");
            }

            if (isMark)
                SetMark(this.objectData.showForceMark);

            return true;
        }

        public void SetMark(bool isOn)
        {
            if (canvas_Mark == null) return;
            canvas_Mark.enabled = isOn;

            if (iconSprite != null && iconSprite.Length > 0 && objectData.objectID == (int)ObjectID.InfantryUnit)
            {
                iconImage.sprite = iconSprite[objectData.forceSize-1];
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ObjectReset"))
            {
                transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            }
            else if (other.CompareTag("VirtualMap"))
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}