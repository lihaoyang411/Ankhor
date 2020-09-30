using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBackgroundText : MonoBehaviour
{
    public TextMesh BackgroundText;

    [TextArea]
    public string StoryBackground;

    private void Start()
    {
        BackgroundText.text = "";   
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            BackgroundText.text = StoryBackground;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            BackgroundText.text = "";
        }
    }
}
