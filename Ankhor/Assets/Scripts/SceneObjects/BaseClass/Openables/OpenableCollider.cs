using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableCollider : MonoBehaviour
{
    [Tooltip("This is the GameObject that contains the parent GameObject's animator")]
    public GameObject Openable;

    [Tooltip("This is the child GameObject that contains the Handle that the player 'grabs'")]
    public GameObject OpenableHandle;

    private Animator OpenableAnim;

    private void Start()
    {
        CheckOutline();

        GetOpenableAnim();
    }

    private void CheckOutline()
    {

        if((OpenableHandle.GetComponent<Outline>() as Outline) == null)
        {
            // Debug.Log(this.gameObject.name + "is missing a Outline script!");

            OpenableHandle.AddComponent<Outline>();

            OpenableHandle.GetComponent<Outline>().OutlineColor = Color.yellow;

            OpenableHandle.GetComponent<Outline>().enabled = false;
        }

    }

    private void GetOpenableAnim()
    {
        if(Openable.GetComponent<Animator>() != null)
        {
            OpenableAnim = Openable.GetComponent<Animator>();
        }
        else
        {
            Debug.Log(Openable.name + "does not have an animator!");
        }
    }

    public virtual void Open()
    {
        if (OpenableAnim.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
        {
            var OpenableScript = Openable.GetComponent<Openable>();

            OpenableScript.isOpen = true;

            // play the open animation 

            OpenableAnim.SetBool("Open", true);

            Debug.Log(Openable.name + "Opened!");

        }
    }

    public bool AttemptToOpen(bool playerIsSpirit)
    {
        var OpenableScript = Openable.GetComponent<Openable>();

        // check if the player state (human or spirit)
        // is the same as door required state

        // door is not locked and is not restricted to spirits
        if (!OpenableScript.isLocked && !OpenableScript.isSpiritOnly)
        {
            Debug.Log("The " + this.gameObject.name + " is not locked and or the player is not a spirit");

            return true;
        }

        // door not locked but restricted to spirits so check player
        else if (!OpenableScript.isLocked && OpenableScript.isSpiritOnly)
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
        Debug.Log("Player cannot open " + Openable.name + "!");

        OpenableAnim.SetTrigger("Locked");
    }

    public virtual bool AttemptToClose(bool playerIsSpirit)
    {
        Debug.Log("Player attempting to close " + Openable.name + "!");

        // check if the player state (human or spirit)
        // is the same as door required state

        var OpenableScript = Openable.GetComponent<Openable>();

        if (!playerIsSpirit == !OpenableScript.isSpiritOnly)
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
        if (OpenableAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenableOpen"))
        {
            var OpenableScript = Openable.GetComponent<Openable>();

            // door is now open

            OpenableScript.isOpen = false;

            // play the open animation 

            OpenableAnim.SetBool("Open", false);

            Debug.Log(Openable.name + " closed!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.tag == "PlayerRightHandVR") || (other.tag == "PlayerLeftHandVR"))
        {
            OpenableHandle.GetComponent<Outline>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "PlayerRightHandVR") || (other.tag == "PlayerLeftHandVR"))
        {
            OpenableHandle.GetComponent<Outline>().enabled = false;
        }
    }
}
