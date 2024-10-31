using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private int spawnerID = 0;
    public int spID => spawnerID;

    private bool isEnable = false;
    public bool Enable => isEnable;

    public void EnableSpawner()
    {
        isEnable = true;
    }

    public void DisableSpawner()
    {
        isEnable = false;
    }
}
