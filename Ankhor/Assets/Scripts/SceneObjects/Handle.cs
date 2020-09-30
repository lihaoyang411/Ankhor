using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    // door with large box trigger
    // there are two handles, one if for the front
    // the other is for the back 

    public GameObject Door_w_Script;

    public GameObject HumanHandPositionL;

    public GameObject HumanHandPositionR;

    public GameObject SpiritHandPositionL;

    public GameObject SpiritHandPositionR;

    [Tooltip("Amount of time it takes until animation finishes")]
    public int WaitAnimTime = 2;

    public float WaitTimeRemaning;

    public bool isPlayingAnimation = false;

    // used to keep only one hand 
    // on the door and not two

    public bool isHandOnDoor;

    private void Start()
    {
        if (Door_w_Script == null)
        {
            Debug.Log("No GameObject Door with script attached");
        }

        else if (Door_w_Script.GetComponent<Door>() == null)
        {
            Debug.Log("Attached door does not have script attached");
        }

        HumanHandPositionL.SetActive(false);

        HumanHandPositionR.SetActive(false);

        SpiritHandPositionL.SetActive(false);

        SpiritHandPositionR.SetActive(false);

        WaitTimeRemaning = WaitAnimTime;

        isPlayingAnimation = false;

        // Open_Close_Door(true);
    }

    private void Update()
    {
        // if the player has attemepted to open the door
        // and the animation is still playing
        // don't allow them to try to continue 
        // closing / opening the door 

        if (isPlayingAnimation)
        {
            WaitTimeRemaning -= Time.deltaTime;
        }

        if (WaitTimeRemaning <= 0)
        {
            // reset wait time to WaitAnimTime

            WaitTimeRemaning = WaitAnimTime;

            // the animation is no longer playing

            isPlayingAnimation = false;
        }


    }

    // to show / hide hands on handle

    public void ShowHumanLeftHand()
    {
        HumanHandPositionL.SetActive(true);
    }

    public void HideHumanLeftHand()
    {
        HumanHandPositionL.SetActive(false);
    }

    public void ShowHumanRightHand()
    {
        HumanHandPositionR.SetActive(true);
    }

    public void HideRightHand()
    {
        HumanHandPositionR.SetActive(false);
    }

    public void ShowSpiritLeftHand()
    {
        SpiritHandPositionL.SetActive(true);
    }

    public void HideSpiritLeftHand()
    {
        SpiritHandPositionL.SetActive(false);
    }

    public void ShowSpiritRightHand()
    {
        SpiritHandPositionR.SetActive(true);
    }

    public void HideSpiritRightHand()
    {
        SpiritHandPositionR.SetActive(false);
    }

    // when the player hits the squeeze 
    // depending on the state of the door and 
    // if the player is a spirit or not
    // open it (or not)

    public void Open_Close_Door(bool isSpirit)
    {
        // if it hasn't been less than 2 seconds
        // since they last opened the door
        // try to open it

        if (!isPlayingAnimation)
        {
            // the player will attempt to open
            // or close the door
            isPlayingAnimation = true;

            var door_script = Door_w_Script.GetComponent<DoorCollider>();

            GameObject Door = door_script.Door_w_Animator;

            // if the door was not open

            if (!Door.GetComponent<Door>().isOpen)
            {
                // try to open it

                bool openSuccessful = door_script.AttemptToOpen(isSpirit);

                if (openSuccessful)
                {

                    if (Door.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                    {
                        AkSoundEngine.PostEvent("Doorknob", gameObject);

                        door_script.Open();

                        // let the player see the hand open the door for like 1 sec
                    }
                }
                else
                {
                    // play a little animation to show that 
                    // the door cannot open

                    AkSoundEngine.PostEvent("Doorknob", gameObject);

                    door_script.OpenFailed();


                }
            }

            else
            {
                // door was open so close it

                bool closeSuccessful = door_script.AttemptToClose(isSpirit);

                if (closeSuccessful)
                {
                    if (Door.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("DoorAnimation"))
                    {
                        AkSoundEngine.PostEvent("Doorknob", gameObject);

                        door_script.Close();
                    }
                }
                else
                {
                    Debug.Log("Cannot close door!");
                }
            }
        }
    }
}
