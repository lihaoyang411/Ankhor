using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InventorySwap : MonoBehaviour
{

    // when the player activates the grip button
    // swap out their current item in their hand 
    // so something else in their inventory
    // note: empty hand is considered inventory to swap to
    //private GameObject Player;

    public List<GameObject> PlayerInventory;

    private GameObject PlayerLHand;

    private GameObject PlayerRHand;

    private int InventorySize;

    public SteamVR_Action_Boolean GrabLGripActionDef;
    public SteamVR_Action_Boolean GrabLGripActionLoco;

    public SteamVR_Action_Boolean GrabRGripActionDef;
    public SteamVR_Action_Boolean GrabRGripActionLoco;

    private void Start()
    {
        //Player = GameObject.FindGameObjectWithTag("Player");

        PlayerLHand = GameObject.FindGameObjectWithTag("PlayerLeftHandVR");

        PlayerRHand = GameObject.FindGameObjectWithTag("PlayerRightHandVR");

        GameObject EmptyGameObject = new GameObject("EmptyInventoryObject");

        PlayerInventory = GameObject.FindWithTag("Player").GetComponent<Player>().PlayerInventory;

        // add the empty game object as the first thing in the player inventory 

        //PlayerInventory.Add(EmptyGameObject);

        foreach (GameObject item in PlayerInventory)
        {
            Debug.Log("Player Inventory" + item.name);
        }

    }

    private void Update()
    {
        InventorySize = PlayerInventory.Count;

        // when the user presses the grip
        // and they have a item on their hand swap it

        // swap left object
        if (PressedLGrip())
        {
            Debug.Log("Pressed L Grip");
            var PlayerLHandScript = PlayerLHand.GetComponent<PlayerHand>();

            // get the index of the item they are holding
            int currentHoldItemIndex = getLCurrentItemInventoryIndex();

            // exception
            // if the item they are using right now is the distract artifact don't swap
            if (PlayerInventory[currentHoldItemIndex].tag == "DistractArtifact")
            {
                var artifact_script = GameObject.FindGameObjectWithTag("DistractArtifact").GetComponent<DistractArtifact>();

                if (artifact_script.isThrown || artifact_script.isActive)
                {
                    return;
                }
            }

            // get the index of the other hand item
            int RightItemIndex = getRCurrentItemInventoryIndex();

            // if the next item in the list is the one that the right hand it is holding
            // skip it, add +1

            // if they reached the end of the list, go back to the beggining
            /*
            if (InventorySize <= currentHoldItemIndex)
            {
                currentHoldItemIndex = 0;

                // now let them switch 
                PlayerLHandScript.ItemOnHand = PlayerInventory[currentHoldItemIndex];

                return;
            }*/

            if (((currentHoldItemIndex + 1) % InventorySize) != 0 && currentHoldItemIndex + 1 == RightItemIndex)
            {
                currentHoldItemIndex++;
            }

            // switch what they have in their hand with whatever is next in their inventory 
            currentHoldItemIndex = (currentHoldItemIndex + 1) % InventorySize;

            Debug.Log("Trying to get " + currentHoldItemIndex);
            Debug.Log("Name of gameObject trying to get " + PlayerInventory[currentHoldItemIndex]);
            PlayerLHandScript.ItemOnHand.SetActive(false);
            PlayerLHandScript.ItemOnHand = PlayerInventory[currentHoldItemIndex].gameObject;

            GameObject newItem = PlayerLHandScript.ItemOnHand;
            newItem.SetActive(true);
            newItem.transform.position = PlayerLHandScript.HoldItemPos.transform.position;

            newItem.transform.rotation = PlayerLHandScript.HoldItemPos.transform.rotation;
            newItem.transform.SetParent(PlayerLHandScript.HoldItemPos.transform);
            if (currentHoldItemIndex == 0)
                PlayerLHandScript.holdingObject = false;
            else
                PlayerLHandScript.holdingObject = true;
        }

        // swap right object

        if (PressedRGrip())
        {
            Debug.Log("Pressed R Grip");
            var PlayerRHandScript = PlayerRHand.GetComponent<PlayerHand>();

            // get the index of the item they are holding
            int currentHoldItemIndex = getRCurrentItemInventoryIndex();

            // exception
            // if the item they are using right now is the distract artifact don't swap
            if (PlayerInventory[currentHoldItemIndex].tag == "DistractArtifact")
            {
                var artifact_script = GameObject.FindGameObjectWithTag("DistractArtifact").GetComponent<DistractArtifact>();

                if (artifact_script.isThrown || artifact_script.isActive)
                {
                    return;
                }
            }

            // get the index of the other hand item
            int leftItemIndex = getLCurrentItemInventoryIndex();

            // if the next item in the list is the one that the right hand it is holding
            // skip it, add +1

            // if they reached the end of the list, go back to the beggining
            /*
            if (InventorySize <= currentHoldItemIndex)
            {
                currentHoldItemIndex = 0;

                // now let them switch 
                PlayerRHandScript.ItemOnHand = PlayerInventory[currentHoldItemIndex];

                return;
            }*/

            if (((currentHoldItemIndex + 1) % InventorySize) != 0 && currentHoldItemIndex + 1 == leftItemIndex)
            {
                currentHoldItemIndex++;
            }

            currentHoldItemIndex = (currentHoldItemIndex+1) % InventorySize;

            // switch what they have in their hand with whatever is next in their inventory 
            Debug.Log("Trying to get " + currentHoldItemIndex);
            Debug.Log("Name of gameObject trying to get " + PlayerInventory[currentHoldItemIndex]);
            PlayerRHandScript.ItemOnHand.SetActive(false);
            PlayerRHandScript.ItemOnHand = PlayerInventory[currentHoldItemIndex].gameObject;

            GameObject newItem = PlayerRHandScript.ItemOnHand;
            newItem.SetActive(true);
            newItem.transform.position = PlayerRHandScript.HoldItemPos.transform.position;

            newItem.transform.rotation = PlayerRHandScript.HoldItemPos.transform.rotation;
            newItem.transform.SetParent(PlayerRHandScript.HoldItemPos.transform);

            if (currentHoldItemIndex == 0)
                PlayerRHandScript.holdingObject = false;
            else
                PlayerRHandScript.holdingObject = true;
        }


    }


    private int getLCurrentItemInventoryIndex()
    {
        var PlayerLHandScript = PlayerLHand.GetComponent<PlayerHand>();

        // find the index of the item you are holding

        for (int itemIndex = 0; itemIndex < PlayerInventory.Count; itemIndex++)
        {
            // get the index of what you are holding 
            if (PlayerInventory[itemIndex] == PlayerLHandScript.ItemOnHand)
            {

                Debug.Log("Got the index of the Left Item Hand " + itemIndex);

                return itemIndex;

            }
        }

        // if the item they are holding did not 
        // get added to the inventory we have a problem 

        return 0;

    }

    private int getRCurrentItemInventoryIndex()
    {
        var PlayerRHandScript = PlayerRHand.GetComponent<PlayerHand>();

        // find the index of the item you are holding

        for (int itemIndex = 0; itemIndex < PlayerInventory.Count; itemIndex++)
        {
            // get the index of what you are holding 
            if (PlayerInventory[itemIndex] == PlayerRHandScript.ItemOnHand)
            {
                Debug.Log("Got the index of the Right Item Hand " + itemIndex);

                return itemIndex;

            }
        }

        // if the item they are holding did not 
        // get added to the inventory we have a problem 

        return 0;

    }



    public bool PressedLGrip()
    {
        // get the value from the left grabGrip action
        // note: we need to make sure we take either default
        // or locomotion action sets

        bool gripLValueDef = GrabLGripActionDef.GetStateDown(SteamVR_Input_Sources.Any);
        bool gripLValueLoco = GrabLGripActionLoco.GetStateDown(SteamVR_Input_Sources.Any);

        if (gripLValueDef || gripLValueLoco)
        {

            Debug.Log("Player pressed Left Grip Button!");

            //Debug.Log("Trigger Value: " + triggerValue);

            return true;
        }

        else
        {
            return false;
        }
    }

    public bool PressedRGrip()
    {
        // get the value from the left grabGrip action
        // note: we need to make sure we take either default
        // or locomotion action sets

        bool gripRValueDef = GrabRGripActionDef.GetStateDown(SteamVR_Input_Sources.Any);
        bool gripRValueLoco = GrabRGripActionLoco.GetStateDown(SteamVR_Input_Sources.Any);

        if (gripRValueDef || gripRValueLoco)
        {

            Debug.Log("Player pressed Right Grip Button!");

            //Debug.Log("Trigger Value: " + triggerValue);

            return true;
        }

        else
        {
            return false;
        }
    }

}
