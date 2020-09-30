using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritDialogue : MonoBehaviour
{
    int textNum = 0;
    public int questPhase = 1;

    public void NextText()
    {
        Debug.Log("InNextText");
        if (questPhase == 1)
        {
            switch (textNum)
            {
                case 0:
                    gameObject.GetComponent<Text>().text = "For now, know this: I am you. Rather, I am your spirit. I was pulled from my own dimension, by these cultists. I need your help to stop them from merging my dimension with yours, and releasing Ankhor into the world.";
                    break;
                case 1:
                    gameObject.GetComponent<Text>().text = "In order to stop this, I need your help. The cultists should have several artifacts around this place. Please - Find them and bring them to me. Both of us depend on it. One looks like a staff and the other is a red capsule";
                    break;
                case 2:
                    gameObject.GetComponent<Text>().text = "Please - Find the artifacts.";
                    break;
                default:
                    break;
            }
            textNum++;
        }
        else
        {
            switch (questPhase)
            {
                case 0:
                    gameObject.GetComponent<Text>().text = "";
                    break;
                case 1:
                    gameObject.GetComponent<Text>().text = "Keycard";
                    break;
                case 2:
                    gameObject.GetComponent<Text>().text = "I see that you have found the Electricity Artifact. Excellent. It can be used to open some doors that may have previously been inaccessible.";
                    break;
                case 3:
                    gameObject.GetComponent<Text>().text = "Ah, the Distraction Artifact. Use it to your advantage to lure away cultists.";
                    break;
                case 4:
                    gameObject.GetComponent<Text>().text = "Finally, the Freeze Artifact. This is the last artifact I need. Please, go and close the portal.";
                    break;
                    /*
                case 5:
                    QuestText.text = Quest5Instructions;
                    NarrativeText.text = Narrative5Text;
                    break;
                case 6:
                    QuestText.text = Quest6Instructions;
                    NarrativeText.text = Narrative6Text;
                    break;
                case 7:
                    QuestText.text = Quest7Instructions;
                    NarrativeText.text = Narrative7Text;
                    break;
                    */
            }
        }
    }
}
