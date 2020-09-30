using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Tooltip("The keypad for this gate")]
    public GameObject KeyPad;

    public GameObject SlidingAnim;

    public bool isLocked;

    public bool isOpen;

    private void Start()
    {
        if (this.SlidingAnim.GetComponent<Animator>() == null)
        {
            Debug.Log("Animator does not exist on " + this.gameObject.name + "!");
        }

        else
        {
            Animator GateAnim = SlidingAnim.GetComponent<Animator>();

            if (isOpen)
            {
                if (GateAnim.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                {
                    // var KeyPadScript = gameObject.GetComponent<KeyPad>();

                    isOpen = true;

                    // play the open animation 

                    GateAnim.SetBool("Open", true);

                    Debug.Log(gameObject.name + "Opened!");

                }
            }

            else if (isLocked)
            {
                if (GateAnim.GetCurrentAnimatorStateInfo(0).IsName("GateOpen"))
                {
                    // var KeyPadScript = gameObject.GetComponent<KeyPad>();

                    // door is now open

                    isOpen = false;

                    // play the open animation 

                    GateAnim.SetBool("Open", false);

                    Debug.Log(gameObject.name + " closed!");
                }
            }
        }
    }
}
