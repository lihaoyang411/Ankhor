using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    [Tooltip("Attach this to the corresponding door(s) with a Door Script")]
    public GameObject[] LockedDoors;

    [Tooltip("The place where the key object will be placed where it will be stored")]
    public Transform ItemPosition;

    [Tooltip("The tag of the item that can open the locked door")]
    public string ItemTag = "KeyCard";

    public GameObject KeyItem; 

    public bool keyItemInPlace = false;

    private bool hasLockedDoors = false;

    // public bool test;

    private void Start()
    {
        foreach (GameObject LockedDoor in LockedDoors)
        {
            if ((LockedDoor.GetComponent<Door>() as Door) == null)
            {
                Debug.LogError("The Control Panel has 'Doors' that do not contain the Door Script");
            }
        }
    }

    private void Update()
    {
        /*
        if (test)
        {
            foreach (GameObject LockedDoor in LockedDoors)
            {

                LockedDoor.GetComponent<Door>().isLocked = false;
                LockedDoor.GetComponent<Door>().FrontDoorBoxCollider.GetComponent<DoorCollider>().Open();

            }
            test = false;
        }
        */
    }

    public void Activate(GameObject PlayerHand)
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        List<GameObject> PlayerInventory = Player.GetComponent<InventorySwap>().PlayerInventory;

        foreach (GameObject item in PlayerInventory)
        {
            // Debug.Log("Player Inventory" + item.name);

            if (item.tag == ItemTag)
            {
                // for each door in the locked doors
                // unlock them 

                foreach (GameObject LockedDoor in LockedDoors){

                    LockedDoor.GetComponent<Door>().isLocked = false;
                    LockedDoor.GetComponent<Door>().FrontDoorBoxCollider.GetComponent<DoorCollider>().Open();

                }

                /*
                item.transform.parent = null;
                item.transform.position = ItemPosition.position;
                item.GetComponent<Rigidbody>().isKinematic = true;
                item.GetComponent<Rigidbody>().useGravity = false;

                PlayerHand.GetComponent<PlayerHand>().holdingObject = false;
                PlayerHand.GetComponent<PlayerHand>().ItemOnHand = PlayerInventory[0].gameObject;

                item.GetComponent<KeyItem>().isBeingUsed = true;

                KeyItem = item;

                PlayerInventory.Remove(item);
                */               
            }
        }
    }

    public void CloseDoors()
    {
        foreach (GameObject LockedDoor in LockedDoors)
        {
            LockedDoor.GetComponent<Door>().FrontDoorBoxCollider.GetComponent<DoorCollider>().Close();
            LockedDoor.GetComponent<Door>().isLocked = true;
        }

        hasLockedDoors = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerLeftHandVR" || other.tag == "PlayerRightHandVR")
            Activate(other.gameObject);

    }
}
