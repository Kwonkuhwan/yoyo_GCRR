using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class TargetUIManager : MonoBehaviour
    {

        private static TargetUIManager instance;
        public static TargetUIManager Instance => instance;

        [SerializeField] private GameObject panel_TargetInfo;

        private void Awake()
        {
            if(instance == null || instance == this)
            {
                instance = this;
            }
        }

        public void Panel_On(TargetMetaData targetMetaData, float lat, float lon)
        {
            panel_TargetInfo.GetComponent<TargetInfoPanel>().Init(targetMetaData, lat, lon);
            panel_TargetInfo.SetActive(true);
        }

        public void Panel_Off()
        {
            panel_TargetInfo.GetComponent<TargetInfoPanel>().Clear();
            panel_TargetInfo.SetActive(false);
        }
    }
}
