using GCRR.VirtualMap;
using TMPro;
using UnityEngine;

public class Option_LatLonText : MonoBehaviour
{
    [SerializeField] private TMP_Text text_LatLon;

    private void Awake()
    {
        if (text_LatLon == null)
        {
            text_LatLon = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        if (text_LatLon == null) return;

        string strLatLon = $"À§°æµµ - {GameManager.Instance.StartCoordinates.latitude}, {GameManager.Instance.StartCoordinates.longitude}";

        if (text_LatLon.text.Equals(strLatLon)) return;

        text_LatLon.text = strLatLon;
    }
}
