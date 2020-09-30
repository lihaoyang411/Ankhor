using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritQuestZone : MonoBehaviour
{
    public Text QuestText;

    public Text NarrativeText;

    [TextArea]
    public string Quest0Instructions;

    [TextArea]
    public string Narrative0Text;

    [TextArea]
    public string Quest1Instructions;

    [TextArea]
    public string Narrative1Text;

    [TextArea]
    public string Quest2Instructions;

    [TextArea]
    public string Narrative2Text;

    [TextArea]
    public string Quest3Instructions;

    [TextArea]
    public string Narrative3Text;

    [TextArea]
    public string Quest4Instructions;

    [TextArea]
    public string Narrative4Text;

    [TextArea]
    public string Quest5Instructions;

    [TextArea]
    public string Narrative5Text;


    public int QuestPhase = 0;

    public bool isTesting = false;

    private bool bKeycard = false;

    private bool bElectricArtifact = false;

    private bool bDistractArtifact = false;

    private bool bFreezeArtifact = false;

    private bool bFirstTime = true;

    private bool bPlayerWithin = false;

    private bool bNewItemFound = false;

    float rtpc = 0.0f;
    GameObject spirit;

    GameObject Electric;
    GameObject Distract;
    GameObject Freeze;

    private void Start()
    {
        spirit = GameObject.Find("docSpirit");
        rtpc = Vector3.Distance(this.transform.position, spirit.transform.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);

        Electric = GameObject.FindWithTag("ElectricityArtifact");
        Distract = GameObject.FindWithTag("DistractArtifact");
        Freeze = GameObject.FindWithTag("FreezeArtifact");

        Distract.SetActive(false);
        Freeze.SetActive(false);
    }

    // only keep this on update for testing purposes
    private void Update()
    {
        rtpc = Vector3.Distance(this.transform.position, spirit.transform.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);
    }

    private void OnTriggerEnter(Collider other)
    {
        bPlayerWithin = true;
        if (other.tag == "PlayerBodyVR")
        {
            GameObject Player = GameObject.FindGameObjectWithTag("Player");

            // for dummy player test
            //Player = other.gameObject;

            List<GameObject> PlayerInventory = Player.GetComponent<InventorySwap>().PlayerInventory;

            foreach (GameObject item in PlayerInventory)
            {
                Debug.Log("Player Inventory" + item.name);

                if (item.tag == "KeyCard" && !bKeycard)
                {
                    QuestPhase = 1;
                    bKeycard = true;
                }
                if (item.tag == "ElectricityArtifact" && !bElectricArtifact)
                {
                    QuestPhase = 2;
                    item.GetComponent<ElectricArtifact>().isSealed = false;
                    bElectricArtifact = true;
                    bNewItemFound = true;
                    AkSoundEngine.SetSwitch("Dialogue", "AnkhorsKeyFound_Switch", gameObject);
                    AkSoundEngine.PostEvent("SpiritVoice_Key_01", gameObject);

                    Distract.SetActive(true);
                }
                if (item.tag == "DistractArtifact" && !bDistractArtifact)
                {
                    QuestPhase = 3;
                    item.GetComponent<DistractArtifact>().isSealed = false;
                    bDistractArtifact = true;
                    bNewItemFound = true;
                    AkSoundEngine.SetSwitch("Dialogue", "AnkhorsCallFound_Switch", gameObject);
                    AkSoundEngine.PostEvent("SpiritVoice_Call_01", gameObject);
                    Freeze.SetActive(true);
                }
                if (item.tag == "FreezeArtifact" && !bFreezeArtifact)
                {
                    QuestPhase = 4;
                    item.GetComponent<FreezeArtifact>().isSealed = false;
                    bFreezeArtifact = true;
                    bNewItemFound = true;
                    AkSoundEngine.SetSwitch("Dialogue", "AnkhorsMightFound_Switch", gameObject);
                    AkSoundEngine.PostEvent("SpiritVoice_Might_01", gameObject);
                }
            }
            if (QuestPhase > 1)
            {
                GameObject.Find("SpiritDialogue_Text").GetComponent<SpiritDialogue>().questPhase = QuestPhase;
                GameObject.Find("SpiritDialogue_Text").GetComponent<SpiritDialogue>().NextText();
            }

            if (!bFirstTime && !(bElectricArtifact && bDistractArtifact) && !bNewItemFound)
            {
                AkSoundEngine.SetSwitch("Dialogue", "FindTheRestArtifacts_Switch", gameObject);
                AkSoundEngine.PostEvent("SpiritVoice_FindTheRest_01", gameObject);
            }
            else if (!bFirstTime && bElectricArtifact && bDistractArtifact && !bNewItemFound)
            {
                AkSoundEngine.SetSwitch("Dialogue", "FindTheLastArtifact_Switch", gameObject);
                AkSoundEngine.PostEvent("SpiritVoice_FindTheLast_01", gameObject);
            }
            else if(bFirstTime)
            {
                AkSoundEngine.SetSwitch("Dialogue", "SpiritIntro_Switch", gameObject);
                AkSoundEngine.PostEvent("SpiritVoice_Intro_01", gameObject);
                bFirstTime = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        bNewItemFound = false;
        bPlayerWithin = false;
    }
}
