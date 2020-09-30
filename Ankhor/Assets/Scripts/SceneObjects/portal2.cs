using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal2 : MonoBehaviour
{
    private GameObject Player;

    // is the block you have on the player's head
    // keeps player from switching places too fast to process 

    public GameObject ViveEye;

    public Transform netherLocation;

    public Transform physicalLocation;


    public GameObject spiritLeft;
    public GameObject spiritRight;
    public GameObject humanLeft;
    public GameObject humanRight;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Portal");
    
        if (other.tag == "PlayerBodyVR")
        {
            Player = GameObject.FindGameObjectWithTag("Player");

            Debug.Log("Player went in Portal");
            // if you are a spirit return to physical world
            Player = GameObject.FindGameObjectWithTag("Player");

            if (!Player.GetComponent<Player>().isSpirit)
            {
                Player.GetComponent<Player>().isSpirit = true;

                ViveEye.SetActive(true);

                Invoke("TeleportFade", 2);

                Player.transform.position = netherLocation.position;

                Player.GetComponent<Player>().isSpirit = true;
                //GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().Hide();
                //GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().Hide();
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(spiritLeft);
                GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(spiritRight);
            }

            // else go to spirit world

            else
            {
                Player.GetComponent<Player>().isSpirit = false;

                ViveEye.SetActive(true);

                Invoke("TeleportFade", 2);

                Player.transform.position = physicalLocation.position;

                
                Player.GetComponent<Player>().isSpirit = false;
                //GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().Hide();
                //GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().Hide();
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(humanLeft);
                GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(humanRight);
            }
        }
    }


    void TeleportFade()
    {
        ViveEye.SetActive(false);
    }

}
