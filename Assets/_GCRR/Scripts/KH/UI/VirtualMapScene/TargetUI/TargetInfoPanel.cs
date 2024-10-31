
using System;
using TMPro;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class TargetInfoPanel : MonoBehaviour
    {
        [SerializeField] TargetMetaData targetMetaData;
        [SerializeField] float lat;
        [SerializeField] float lon;

        [SerializeField] TMP_Text text_Name;
        [SerializeField] TMP_Text text_Address;
        [SerializeField] TMP_Text text_Lat;
        [SerializeField] TMP_Text text_Lon;
        [SerializeField] TMP_Text text_Create;
        [SerializeField] TMP_Text text_Picture;

        public void Init(TargetMetaData targetMetaData, float lat, float lon)
        {
            this.targetMetaData = targetMetaData;
            this.lat = lat;
            this.lon = lon;

            SetUI();
        }

        internal void Clear()
        {
            text_Name.text = string.Empty;
            text_Address.text = string.Empty;
            text_Lat.text = string.Empty;
            text_Lon.text = string.Empty;
            text_Create.text = string.Empty;
            text_Picture.text = string.Empty;
        }

        private void SetUI()
        {
            Clear();
            if (targetMetaData == null) return;

            text_Name.text = targetMetaData.name;
            text_Address.text = targetMetaData.address;
            text_Lat.text = lat.ToString();
            text_Lon.text = lon.ToString();
            text_Create.text = targetMetaData.createdate;
            text_Picture.text = targetMetaData.picturedate;
        }
    }
}