using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHint : MonoBehaviour
{
    public TextMesh FlashlightTextMesh;

    // tell them to grab the flashlight
    [TextArea]
    public string FlashlightHintText;

    private void Start()
    {
        FlashlightTextMesh.text = "";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            FlashlightTextMesh.text = FlashlightHintText;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            FlashlightTextMesh.text = "";
        }
    }

}
