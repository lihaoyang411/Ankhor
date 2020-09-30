using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerCollider : MonoBehaviour
{
    // this script is for both left and right hand controllers
    // to detect what they are on 

    public int physicalKeys;

    public int netherKeys;

    public SteamVR_Action_Single squeezeAction;

    private void OnTriggerEnter(Collider other)
    {
        float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);

        // if the player is in a handle trigger

        if (other.tag == "physicalHandle")
        {
            if (triggerValue > 0)
            {
                if (physicalKeys > 0)
                {
                    // open the door
                    other.GetComponentInParent<Animator>().SetTrigger("Open");

                    // set the class to open

                    // reduce the number of keys you have by 1

                    physicalKeys--;

                }
                else
                {
                    Debug.Log("Door is locked. Find a hospital key in order to open it");
                }
            }
        }

        if (other.tag == "netherHandle");
        {
            if (triggerValue > 0)
            {
                if (netherKeys > 0)
                {
                    // open the door
                    other.GetComponentInParent<Animator>().SetTrigger("Open");

                    // set the class to open

                    // reduce the number of keys you have by 1

                    netherKeys--;

                }
                else
                {
                    Debug.Log("Door is locked. Find a hospital key in order to open it");
                }
            }
        }

        if (other.tag == "physicalKey")
        {
            Destroy(other.gameObject);

            physicalKeys++;
        }

        if (other.tag == "netherKey")
        {
            Destroy(other.gameObject);

            netherKeys++;
        }
        if (other.tag == "TutorialItem")
        {
            Destroy(other.gameObject);
        }
    }

}
