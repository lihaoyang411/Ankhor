using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TutorialManager : MonoBehaviour
{
    public int phase = 0;
    public bool tutorialFinished = false;
    public SteamVR_Action_Boolean runAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Run");
    public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

    private GameObject[] mainMenuCultists;
    private GameObject playerHead;

    // Start is called before the first frame update
    void Start() {
        mainMenuCultists = GameObject.FindGameObjectsWithTag("MainMenuCultist");
        foreach(GameObject mainMenuCultist in mainMenuCultists) {
            mainMenuCultist.SetActive(false);
        }
        playerHead = GameObject.FindWithTag("PlayerHeadVR");

    }

    /*************************************************************************************
     * Update will handle the tutorial events in order. Below is an event description:
     * 
     * Phase Zero: Set up initial controls image based on configuration
     * Phase One: Instruct player to move to door
     * Phase Two: Show graphic for interacting with door
     * Phase Three: Instruct player to find records
     * Phase Four: Take records to computer
     * 
     *************************************************************************************/
    void Update()
    {
        Debug.Log("Phase: " + phase);
        if (tutorialFinished)
        {
            this.enabled = false;
        }
        else
        {
            switch (phase)
            {
                case 0:
                    phaseZero();
                    break;
                case 1:
                    phaseOne();
                    break;
                case 2:
                    phaseTwo();
                    break;
                case 3:
                    phaseThree();
                    break;
                case 4:
                    phaseFour();
                    break;
                case 5:
                    phaseFive();
                    break;
                case 6:
                    phaseSix();
                    break;
                case 7:
                    phaseSeven();
                    break;
                default:
                    break;
            }
        }
    }

    void phaseZero()
    {
        Debug.Log("Checking phase zero");
        if(GameObject.Find("Actions").GetComponent<Locomotion>().moveMode == Locomotion.PlayerMovementMode.Teleportation)
        {
            GameObject.Find("TutorialUI_TV").transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial/ViveControllers_Teleport");
        }
        else
        {
            GameObject.Find("TutorialUI_TV").transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial/ViveControllers_Locomotion");
            GameObject.Find("MoveReminder").transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial/ViveControllers_Locomotion");
            GameObject.Find("TutorialUI_Stamina").SetActive(false);
        }
        GameObject.FindGameObjectWithTag("TutorialDoor").GetComponent<Door>().isLocked = true;
        phase = 1;
    }


    void phaseOne()
    {
        Debug.Log("Checking phase 1");
        if (GameObject.Find("Actions").GetComponent<Locomotion>().moveMode == Locomotion.PlayerMovementMode.Teleportation)
        {
            if (teleportAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                phase = 2;
                AkSoundEngine.SetSwitch("Dialogue", "AnotherDayOfWork_Switch", gameObject);
                AkSoundEngine.PostEvent("DoctorVoice_Tutorial_01", gameObject);
                GameObject.FindGameObjectWithTag("TutorialDoor").GetComponent<Door>().isLocked = false;
            }
        }
        else
        {
            if (runAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                phase = 2;
                AkSoundEngine.SetSwitch("Dialogue", "AnotherDayOfWork_Switch", gameObject);
                AkSoundEngine.PostEvent("DoctorVoice_Tutorial_01", gameObject);
                GameObject.FindGameObjectWithTag("TutorialDoor").GetComponent<Door>().isLocked = false;
            }
        }
    }

    void phaseTwo()
    {
        if (GameObject.FindGameObjectWithTag("TutorialDoorCollider").GetComponent<DoorCollider>().playerNearDoor)
        {
            phase = 3;
        }
    }

    void phaseThree()
    {
        if (GameObject.FindGameObjectWithTag("TutorialDoor").GetComponent<Door>().isOpen)
        {
            GameObject.Find("TutorialUI_TV").SetActive(false);
            phase = 4;
            GameObject.Find("MoveReminder").SetActive(false);
        }
    }

    void phaseFour()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PlayerInventory.IndexOf(GameObject.Find("PatientRecords")) != -1)
        {
            AkSoundEngine.SetSwitch("Dialogue", "AhHereTheyAre_Switch", gameObject);
            AkSoundEngine.PostEvent("DoctorVoice_Tutorial_02", gameObject);
            GameObject.Find("TutorialComputer").transform.GetChild(2).gameObject.SetActive(true);
            if (GameObject.Find("Actions").GetComponent<Locomotion>().moveMode == Locomotion.PlayerMovementMode.Teleportation)
                GameObject.Find("TutorialUI_Stamina").SetActive(false);
            phase = 5;
        }
    }

    void phaseFive()
    {
        if (GameObject.FindGameObjectWithTag("TutorialComputer").GetComponent<TutorialComputerCollider>().recordsLogged)
        {
            AkSoundEngine.PostEvent("KeyboardTyping", gameObject);
            GameObject.Find("TutorialComputer").transform.GetChild(2).gameObject.SetActive(false);
            List<GameObject> PlayerInventory = GameObject.FindWithTag("Player").GetComponent<Player>().PlayerInventory;
            GameObject.FindWithTag("PlayerLeftHandVR").GetComponent<PlayerHand>().ItemOnHand = PlayerInventory[0];
            PlayerInventory.RemoveAt(1);
            GameObject.Find("PatientRecords").SetActive(false);

            StartCoroutine(phaseFiveWait(3f));

            phase = 6;
        }
    }

    IEnumerator phaseFiveWait(float time)
    {
        yield return new WaitForSeconds(time);
        AkSoundEngine.SetSwitch("Dialogue", "ExcellentTheyreLogged_Switch", gameObject);
        AkSoundEngine.PostEvent("DoctorVoice_Tutorial_03", gameObject);
    }

    void phaseSix()
    {
        if (GameObject.Find("TutorialCoffeeButton").GetComponent<TutorialCoffeeCollider>().buttonPressed)
        {
            AkSoundEngine.PostEvent("CoffeeMaker", gameObject);
            GameObject.Find("Coffee-Maker").GetComponent<Animator>().SetTrigger("CoffeeButtonMove");

            foreach (GameObject mainMenuCultist in mainMenuCultists) {
                mainMenuCultist.SetActive(true);
            }

            foreach(GameObject l in GameObject.FindGameObjectsWithTag("TutorialLight"))
            {
                l.SetActive(false);
            }
            GameObject.Find("HallwayLights").GetComponent<FlickeringLightCollection>().enabled = true;

            AkSoundEngine.SetSwitch("Dialogue", "WhatWasThat_Switch", gameObject);
            AkSoundEngine.PostEvent("DoctorVoice_Tutorial_04", gameObject);
            phase = 7;
        }
    }


    void phaseSeven()
    {
        if(CheckForCultists())
            phase++;
    }

    private bool CheckForCultists()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerHead.transform.position, playerHead.transform.forward.normalized, out hit, 20.0f))
        {
            if (hit.collider.tag.Equals("MainMenuCultist"))
            {
                Debug.LogWarning("Ray hits Cultist");
                hit.collider.gameObject.GetComponent<MainMenuCultistsAIManager>().StartChasingPlayer();
                return true;
            }
        }
        return false;
    }
}
