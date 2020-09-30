using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManger : MonoBehaviour
{
    public GameObject[] savePoints;

    public GameObject RecentCheckpoint;

    private void Start()
    {
        // get all checkpoints

        savePoints = GameObject.FindGameObjectsWithTag("SavePoint");

    }



}
