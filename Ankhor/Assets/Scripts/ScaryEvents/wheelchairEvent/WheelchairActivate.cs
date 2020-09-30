using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairActivate : MonoBehaviour
{
    public bool chairActivate = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            chairActivate = true;
        }
    }
}
