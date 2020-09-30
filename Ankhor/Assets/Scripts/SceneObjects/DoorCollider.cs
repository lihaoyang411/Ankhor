using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this door script is attached 
// to the door mesh that has a box trigger
// in front of it 

public class DoorCollider : MonoBehaviour
{
    // Entire door is parent with animator 
    public GameObject Door_w_Animator;

    // Make sure handle has box trigger
    public GameObject Handle;

    public bool playerNearDoor = false;

    /*
    public bool isLocked = false;

    public bool isSpiritOnly = false;

    // is the door already swung open?
    public bool isOpen = false;
    */

    // public bool testOpen = false;

    // public bool testClose = false;


    private void Start()
    {
        // give the handle it's Outline script

        Handle.AddComponent<Outline>();

        Handle.GetComponent<Outline>().OutlineColor = Color.yellow;

        Handle.GetComponent<Outline>().enabled = false;


    }

    private void Update()
    {
        /*
        if (testOpen)
        {
            bool isSuccessful = AttemptToOpen(false);

            if (isSuccessful)
            {
                Open();

                var door_script = Door_w_Animator.GetComponent<Door>();

                door_script.isOpen = true;
            }

            testOpen = false;
        }

        if (testClose)
        {
            bool isSuccessful = AttemptToClose(false);

            if (isSuccessful)
            {
                Close();
            }
            testClose = false;
        }
        */

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBodyVR") {

            Debug.Log("Player is near the door");

            playerNearDoor = true;

            if (Door_w_Animator.GetComponent<Door>().isLocked)
            {
                // highlight the handle if player is near door

                Handle.GetComponent<Outline>().enabled = true;

                Handle.GetComponent<Outline>().OutlineColor = Color.red;
            }

            else
            {
                // highlight the handle if player is near door

                Handle.GetComponent<Outline>().enabled = true;

                Handle.GetComponent<Outline>().OutlineColor = Color.yellow;
            }


        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            Debug.Log("Player is still near the door");

            // highlight the handle if player is near door

            Handle.GetComponent<Outline>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            Debug.Log("Player is not near the door anymore");
            playerNearDoor = false;

            // remove highlight if away from door

            Handle.GetComponent<Outline>().enabled = false;
        }
    }

    // if the player attempts to open the door
    // check conditions

    public bool AttemptToOpen(bool playerIsSpirit)
    {
        var door_script = Door_w_Animator.GetComponent<Door>();

        // check if the player state (human or spirit)
        // is the same as door required state

        // door is not locked and is not restricted to spirits
        if (!door_script.isLocked && !door_script.isSpiritOnly)
        {
            Debug.Log("The door is locked and or the player is not a spirit");

            return true;
        }

        // door not locked but restricted to spirits so check player
        else if (!door_script.isLocked && door_script.isSpiritOnly)
        {
            if (playerIsSpirit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // the door is locked
        else
        {
            Debug.Log("Player attempted to open the door but failed");

            return false;
        }
    }

    public void Open()
    {
        if (Door_w_Animator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed"))
        {
            Debug.Log("Door opened!");
            // door is now open

            var door_script = Door_w_Animator.GetComponent<Door>();

            door_script.isOpen = true;

            // play the open animation 

            Door_w_Animator.GetComponent<Animator>().SetBool("Open", true);

            
        }


    }

    public void OpenFailed()
    {
        Debug.Log("Player cannot open the door");

        Door_w_Animator.GetComponent<Animator>().SetTrigger("Locked");
    }

    public bool AttemptToClose(bool playerIsSpirit)
    {
        Debug.Log("Player attempting to close the door");

        // check if the player state (human or spirit)
        // is the same as door required state

        var door_script = Door_w_Animator.GetComponent<Door>();

        if (!playerIsSpirit == !door_script.isSpiritOnly)
        {
            return true;
        }

        else
        {
            return false;
        }
    }


    public void Close()
    {

        if (Door_w_Animator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("DoorAnimation"))
        {
            var door_script = Door_w_Animator.GetComponent<Door>();

            // door is now open

            door_script.isOpen = false;

            // play the open animation 

            Door_w_Animator.GetComponent<Animator>().SetBool("Open", false);

            Debug.Log("Door closed!");
        }

    }


}
