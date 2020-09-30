using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorCollider : MonoBehaviour
{
    public TextMesh ExitText;

    [TextArea]
    public string ExitRequirements;

    [TextArea]
    public string Win;

    [Tooltip("Time it will take to restart game once they won")]
    public int secondsTillRestartGame = 10;

    // public List<string> KeyTagRequirements;

    private Animator ExitDoorAnim;

    private GameObject Player;

    private void Start()
    {
        ExitText.text = "";
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "PlayerBodyVR")
        {
            Player = GameObject.FindGameObjectWithTag("Player");

            // for dummy player test
            //Player = other.gameObject;

            List<GameObject> PlayerInventory = Player.GetComponent<InventorySwap>().PlayerInventory;

            int physicalKeyCount = 0;

            int netherKeyCount = 0;

            foreach (GameObject item in PlayerInventory)
            {
                Debug.Log("Player Inventory" + item.name);

                if (item.tag == "physicalKey")
                {
                    physicalKeyCount++;
                }
                if (item.tag == "netherKey")
                {
                    netherKeyCount++;
                }
            }

            if((physicalKeyCount == 1) && (netherKeyCount == 1))
            {
                ExitText.text = Win;

                Invoke("RestartGame", secondsTillRestartGame);
            }

            else
            {
                ExitText.text = ExitRequirements;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ExitText.text = "";
    }


    private void RestartGame()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ResetKeys();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Reset();
    }
}
