using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableHandle : MonoBehaviour
{
    [Tooltip("The GameObject with a collider used to detec if player is nearby")]
    public GameObject OpenableCollider;

    [Tooltip("The Left Human Hand GameObject positioned on the Handle")]
    public GameObject HumanHandPositionL;

    [Tooltip("The Right Human Hand GameObject positioned on the Handle")]
    public GameObject HumanHandPositionR;

    [Tooltip("The Left Spirit Hand GameObject positioned on the Handle")]
    public GameObject SpiritHandPositionL;

    [Tooltip("The Right Spirit Hand GameObject positioned on the Handle")]
    public GameObject SpiritHandPositionR;

    [Tooltip("Amount of time it takes until animation finishes")]
    public int WaitAnimTime = 2;

    [Tooltip("The name of the sound to be played in Wwise")]
    public string WwiseSoundName;

    private bool isWwiseSet = false;

    private float WaitTimeRemaning;

    public bool isPlayingAnimation = false;

    // used to keep only one hand 
    // on the door and not two

    public bool isHandOnOpenable;

    // for testing in editor purposes

    // public bool test;

    private void Start()
    {
        if (OpenableCollider == null)
        {
            Debug.Log("No Openable Collider");
        }
        if (WwiseSoundName != "")
        {
            isWwiseSet = true;
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

        /*
        if (test)
        {
            Open_Close_Door(false);
        }
        */
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

    public void HideHumanRightHand()
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
        var OpenableColliderScript = OpenableCollider.GetComponent<OpenableCollider>();

        // Openable is OpenableCollider's parent
        GameObject Openable = OpenableColliderScript.Openable;

        var OpenableScript = Openable.GetComponent<Openable>();

        // if the Openable was not open

        if (!isPlayingAnimation)
        {
            // the player will attempt to open
            // or close the door

            isPlayingAnimation = true;
       
            // if the Openable was not open

            if (!OpenableScript.isOpen)
            {
                // try to open it

                bool openSuccessful = OpenableColliderScript.AttemptToOpen(isSpirit);

                if (openSuccessful)
                {

                    if (Openable.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                    {

                        if (WwiseSoundName != "")
                        {
                            AkSoundEngine.PostEvent(WwiseSoundName, gameObject);
                        }

                        OpenableColliderScript.Open();

                        // let the player see the hand open the door for like 1 sec
                    }
                }
                else
                {
                    // play a little animation to show that 
                    // the Openable cannot open

                    if (WwiseSoundName != "")
                    {
                        AkSoundEngine.PostEvent(WwiseSoundName, gameObject);
                    }

                    OpenableColliderScript.OpenFailed();


                }
            }

            else
            {
                // Openable was open so close it

                bool closeSuccessful = OpenableColliderScript.AttemptToClose(isSpirit);

                if (closeSuccessful)
                {
                    if (Openable.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("OpenableOpen"))
                    {
                        if (WwiseSoundName != "")
                        {
                            AkSoundEngine.PostEvent(WwiseSoundName, gameObject);
                        }

                        OpenableColliderScript.Close();
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
