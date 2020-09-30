using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

public class Player : MonoBehaviour
{
    public int physicalKeys;
    public int netherKeys;

    public bool isSpirit;

    public List<GameObject> PlayerInventory;

    public float health;
    public GameObject emptyHandObject;

    private bool fadeOut = true;
    private const float FADE_TIME = 2.0f;

    public Vector3 startingPoint;

    private Vector3 recentSavePoint;

    private bool usingVR = true;
    private bool isPlaying = false;
    private int currentLevel = 0;

    private GameObject tutorialCheck;

    private PlayerSoundManager soundManager;

    private bool bTeleportationChosen = true;

    private void Awake()
    {
        //PlayerInventory.Add(new GameObject("EmptyGameObject"));
        PlayerInventory.Add(emptyHandObject);
        startingPoint = new Vector3(-10, 0, 1);
        tutorialCheck = GameObject.Find("TutorialCheck");

        soundManager = this.GetComponent<PlayerSoundManager>();
    }


    void Start() {
        health = 1.0f;
        usingVR = IsUsingVR();

        recentSavePoint.x = startingPoint.x;
        recentSavePoint.y = transform.position.y;
        recentSavePoint.z = startingPoint.z;

        //AkSoundEngine.SetState("Player_State", "Normal");
        //AkSoundEngine.PostEvent("Heartbeat", gameObject);

        if (tutorialCheck)
            tutorialCheck.SetActive(false);
    }

    void Update() {

        GameObject.FindWithTag("Watch").GetComponent<WatchManager>().UpdateHealth(health);

        if(currentLevel == 1)
            SetPlayerToEnemies();


        soundManager.PlayHeatbeatSound(health);
        //if (health <= 0.25f) {
        //    AkSoundEngine.SetState("Player_State", "Danger");
        //}
        //else {
        //    AkSoundEngine.SetState("Player_State", "Normal");
        //}

        if (health <= 0) {
            PostStartGameMenuRoom();
            Reset();
        }

    }

    public void Reset() {
        if (!gameObject.GetComponent<TutorialManager>().tutorialFinished)
        {
            SteamVR_LoadLevel.Begin("Scenes/Ankhor_Sprints/Ankhor_Sprint5Scenes/Ankhor_Sprint5");
            //SetPlayerToEnemies();
            gameObject.GetComponent<TutorialManager>().tutorialFinished = true;

        }
        Vector3 position = startingPoint;

        if (recentSavePoint != null) {
            transform.position = recentSavePoint;
        }
        else {
            position.y = transform.position.y;
            transform.position = position;
        }

        if(!isPlaying) { 
            isPlaying = true;
        }

        health = 1.0f;
        fadeOut = false;

        GameObject[] enemies;
        if (isSpirit) { 
            enemies = GameObject.FindGameObjectsWithTag("SpiritEnemy");
        } else {
            enemies = GameObject.FindGameObjectsWithTag("Cultist");
        }

        for(int index = 0; index < enemies.Length; index++) {
            enemies[index].GetComponent<CultistsAIManager>().Reset();
        }

    }

    private bool IsUsingVR()
    {
        for (int i = 0; i < XRSettings.supportedDevices.Length; i++)
        {
            if (XRSettings.supportedDevices[i].Equals(XRSettings.loadedDeviceName))
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SavePoint")
        {
            recentSavePoint = other.transform.position;
        }
    }

    public void PhysicalKeyCollected() {
    }

    public void SpiritlKeyCollected() {
    }

    public void ResetKeys() { 
        if(usingVR) {

            foreach (GameObject item in PlayerInventory)
            {
                Debug.Log("Player Inventory" + item.name);

                if (item.tag == "physicalKey")
                {
                    Destroy(item);
                }
                if (item.tag == "netherKey")
                {
                    Destroy(item);
                }
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger");

        if (other.tag == "physicalKey")
        {
            Debug.Log("On physicalKey");

            if (!isSpirit)
            {
                Destroy(other.gameObject);

                physicalKeys++;
            }
        }

        else if (other.tag == "netherKey")
        {
            Debug.Log("On netherKey");

            if (isSpirit)
            {
                Destroy(other.gameObject);

                netherKeys++;
            }
        }
    }
    */

    private void PostStartGameMenuRoom() {
        GameObject[] cultists = GameObject.FindGameObjectsWithTag("Cultist");

        for (int index = 0; index < cultists.Length; index++)
        {
            cultists[index].GetComponent<CultistsAIManager>().PostPlayerDeathInMainMenuRoom();
        }
    }

    private void SetPlayerToEnemies() {
        GameObject[] cultists = GameObject.FindGameObjectsWithTag("Cultist");
        foreach (GameObject cultist in cultists) {
            cultist.GetComponent<CultistsAIManager>().SetPlayer();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        currentLevel = level;
        if(level == 2)
        {
            Debug.Log("Level loaded: " + level);
            this.transform.position = new Vector3(-20, 0, 20);
            this.GetComponent<TutorialManager>().enabled = true;
        }
        if(level == 1)
        {
            this.transform.position = startingPoint;
            AkSoundEngine.SetSwitch("Dialogue", "PlayerSpawn_Switch", gameObject);
            AkSoundEngine.PostEvent("DoctorVoice_PlayerSpawn", gameObject);
        }
    }

    /*
     * 
     * 
     *  BELOW ARE MAIN MENU FUNCTIONS
     * 
     * 
     * 
     * 
     */

    public void OnClickStartGameMainMenu()
    {
        Debug.Log("Pressed on Start game");
        GameObject.Find("MainMenu").SetActive(false);
        tutorialCheck.SetActive(true);
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
    }

    public void YesTutorial()
    {
        if(!bTeleportationChosen)
            GameObject.Find("Actions").GetComponent<ActionSetter>().setMovementLocomotion();
        SteamVR_LoadLevel.Begin("Scenes/Ankhor_Sprints/Ankhor_Sprint5Scenes/Ankhor_Sprint5_Tutorial");
        AkSoundEngine.PostEvent("UI_PressStart", gameObject);
    }
    public void NoTutorial()
    {
        if (!bTeleportationChosen)
            GameObject.Find("Actions").GetComponent<ActionSetter>().setMovementLocomotion();
        SteamVR_LoadLevel.Begin("Scenes/Ankhor_Sprints/Ankhor_Sprint5Scenes/Ankhor_Sprint5");
        //SteamVR_LoadLevel.Begin("Scenes/CalebTestScenes/Ankhor_Sprint6_caleb");
        gameObject.GetComponent<TutorialManager>().tutorialFinished = true;
        AkSoundEngine.PostEvent("UI_PressStart", gameObject);
    }

    public void PressToStart()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        Animator anim = GameObject.Find("MainMenu").transform.GetChild(3).GetComponent<Animator>();
        anim.SetTrigger("MoveTitle");
        GameObject.Find("MainMenu").transform.GetChild(2).gameObject.SetActive(false);
        GameObject.Find("MainMenu").transform.GetChild(4).gameObject.SetActive(true);
    }

    public void MainMenuQuit()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        Application.Quit();
    }

    public void MainMenuCredits()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        GameObject.Find("MainMenu").transform.GetChild(4).gameObject.SetActive(false);
        GameObject.Find("MainMenu").transform.GetChild(5).gameObject.SetActive(true);
    }

    public void MainMenuCreditsBack()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        GameObject.Find("MainMenu").transform.GetChild(5).gameObject.SetActive(false);
        GameObject.Find("MainMenu").transform.GetChild(4).gameObject.SetActive(true);
    }

    public void MainMenuSettings()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        GameObject.Find("MainMenu").transform.GetChild(4).gameObject.SetActive(false);
        GameObject.Find("MainMenu").transform.GetChild(6).gameObject.SetActive(true);
    }

    public void MainMenuSettingsTeleport()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        GameObject.Find("SettingsDisplay").transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
        GameObject.Find("SettingsDisplay").transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
        bTeleportationChosen = true;
    }

    public void MainMenuSettingsLocomotion()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        GameObject.Find("SettingsDisplay").transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
        GameObject.Find("SettingsDisplay").transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
        bTeleportationChosen = false;
    }

    public void MainMenuSettingsBack()
    {
        AkSoundEngine.PostEvent("UI_BasicClick", gameObject);
        GameObject.Find("MainMenu").transform.GetChild(6).gameObject.SetActive(false);
        GameObject.Find("MainMenu").transform.GetChild(4).gameObject.SetActive(true);
    }
}