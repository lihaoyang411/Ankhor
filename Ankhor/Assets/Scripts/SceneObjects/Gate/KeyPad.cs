using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    [Tooltip("This is the GameObject that contains the parent GameObject's animator")]
    public GameObject Gate;

    private Animator GateAnim;
   
    private void Start()
    {
        CheckOutline();

        GetGateAnim();
    }

    private void CheckOutline()
    {

        if ((gameObject.GetComponent<Outline>() as Outline) == null)
        {
            // Debug.Log(this.gameObject.name + "is missing a Outline script!");

            gameObject.AddComponent<Outline>();

            gameObject.GetComponent<Outline>().OutlineColor = Color.yellow;

            gameObject.GetComponent<Outline>().enabled = false;
        }

    }

    private Animator GetGateAnim()
    {
        if (Gate.GetComponent<Gate>().SlidingAnim.GetComponent<Animator>() != null)
        {
            GateAnim = Gate.GetComponent<Gate>().SlidingAnim.GetComponent<Animator>();

            return GateAnim;
        }
        else
        {
            Debug.Log(Gate.name + "does not have an animator!");

            return null;
        }

       
    }

    public virtual void Open()
    {
        // Debug.LogWarning("Open Animation" + (GateAnim.GetCurrentAnimatorStateInfo(0)));

        GateAnim = GetGateAnim();

        if (GateAnim.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
        {
            var GateScript = Gate.GetComponent<Gate>();

            GateScript.isOpen = true;

            // play the open animation 

            GateAnim.SetBool("Open", true);

            Debug.Log(Gate.name + "Opened!");

        }
    }

    public bool AttemptToOpen(bool playerIsSpirit)
    {
        var GateScript = Gate.GetComponent<Gate>();

        // check if the player state (human or spirit)
        // is the same as door required state

        // door is not locked and is not restricted to spirits
        if (!GateScript.isLocked)
        {
            Debug.Log("The " + Gate.name + " is not locked");

            return true;
        }

        // door not locked but restricted to spirits so check player
        else if (!GateScript.isLocked)
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

    public virtual void OpenFailed()
    {
        Debug.Log("Player cannot open " + Gate.name + "!");

        GateAnim.SetTrigger("Locked");
    }

    public virtual bool AttemptToClose(bool playerIsSpirit)
    {
        Debug.Log("Player attempting to close " + Gate.name + "!");

        // check if the player state (human or spirit)
        // is the same as door required state

        var GateScript = Gate.GetComponent<Gate>();

        /*
        if (!playerIsSpirit == !GateScript.isSpiritOnly)
        {
            return true;
        }
        */

        if (!playerIsSpirit)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public virtual void Close()
    {
        if (GateAnim.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            var GateScript = Gate.GetComponent<Gate>();

            // door is now open

            GateScript.isOpen = false;

            // play the open animation 

            GateAnim.SetBool("Open", false);

            Debug.Log(Gate.name + " closed!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if they hit the keypad with electricity from the 
        // artifact then turn this keypad on 

        if (other.tag == "ElectricityArtifact")
        {
            // make sure that the door is not already on

            if (!Gate.GetComponent<Gate>().isOpen)
            {
                gameObject.GetComponent<KeyPad>().Open();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.tag == "PlayerRightHandVR") || (other.tag == "PlayerLeftHandVR"))
        {
            if (Gate.GetComponent<Gate>().isLocked && other.GetComponent<PlayerHand>().ItemOnHand.tag == "ElectricityArtifact")
            {
                gameObject.GetComponent<Outline>().OutlineColor = Color.yellow;

                gameObject.GetComponent<Outline>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<Outline>().OutlineColor = Color.red;

                gameObject.GetComponent<Outline>().enabled = true;
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "PlayerRightHandVR") || (other.tag == "PlayerLeftHandVR"))
        {
            gameObject.GetComponent<Outline>().enabled = false;
        }
    }
}