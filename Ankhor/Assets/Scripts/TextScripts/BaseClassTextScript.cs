using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClassTextScript : MonoBehaviour
{
    [Tooltip("The text mesh on the sceneyou want the player to see when they are on the collider")]
    public TextMesh TextMesh;

    [Tooltip("The text that will be used during run-time")]
    [TextArea]
    public string Notification;

    private void Start()
    {
        TextMesh.text = "";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            TextMesh.text = Notification;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            TextMesh.text = "";
        }
    }
}
