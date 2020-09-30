using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject recentSavePoint;

    public void PlayerDied()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        // fade the screen to black

        //Player.GetComponent<Player>().

        Player.transform.position = recentSavePoint.transform.position;


    }


}
