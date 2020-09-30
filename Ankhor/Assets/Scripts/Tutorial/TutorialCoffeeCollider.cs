using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCoffeeCollider : MonoBehaviour
{
    public bool buttonPressed = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerLeftHandVR" ||
            other.tag == "PlayerRightHandVR" &&
            GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().phase == 6)
        {
            buttonPressed = true;
        }
    }
}
