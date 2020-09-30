using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerCollider : OpenableCollider
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerRightHandVR" || other.tag == "PlayerLeftHandVR")
        {
            OpenableHandle.GetComponent<Outline>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerRightHandVR" || other.tag == "PlayerLeftHandVR")
        {
            OpenableHandle.GetComponent<Outline>().enabled = false;
        }
    }
}
