
using UnityEngine;
using UnityEngine.UI;

public class BearingPanel : MonoBehaviour
{
    [SerializeField] private Scrollbar rotate_Scrollbar;

    [SerializeField] private GameObject north;
    [SerializeField] private GameObject south;
    [SerializeField] private GameObject east;
    [SerializeField] private GameObject west;

    void Update()
    {
        Transform trPlayer = GameObject.FindGameObjectWithTag("Player").transform;

        if (rotate_Scrollbar == null) return;

        float rot = trPlayer.localRotation.eulerAngles.y;
        float value = -rotate_Scrollbar.value * 360 + rot;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, value));

        if (north != null) { north.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -value)); }        
        if (south != null) { south.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -value)); }        
        if (east != null) { east.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -value)); }        
        if (west != null) { west.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -value)); }        
    }
}
