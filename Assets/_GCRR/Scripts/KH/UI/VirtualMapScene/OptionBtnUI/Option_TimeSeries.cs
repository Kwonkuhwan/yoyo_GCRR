using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option_TimeSeries : MonoBehaviour
{
    [SerializeField] private GameObject ui_TimeseriesPanel;
    /// 작성 - KKH
    /// <summary>
    /// 시계열 데이터 선택
    /// </summary>
    public void OnClick()
    {
        if (ui_TimeseriesPanel.activeInHierarchy) return;
        ui_TimeseriesPanel.SetActive(true);
    }
}
