using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this door script is attached 
// to the door mesh that has a box trigger
// in front of it 

public class Door : MonoBehaviour
{
    [Tooltip("The status of the door")]
    public bool isLocked = false;

    [Tooltip("Condition if it can only be passed if the player is a spirit")]
    public bool isSpiritOnly = false;

    [Tooltip("Is the door swung open right now??")]
    // is the door already swung open?
    public bool isOpen = false;

    [Tooltip("The box collider that keeps the player from teleporting past it")]
    public GameObject DoorMeshBoxCollider;

    [Tooltip("The box collider in front of the door")]
    public GameObject FrontDoorBoxCollider;

    [Tooltip("The box collider behind the door")]
    public GameObject BackDoorBoxCollider;

    private void Start()
    {
        if (isOpen)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Open", true);

            DoorMeshBoxCollider.GetComponent<BoxCollider>().enabled = false;
        }

        else if (isOpen == false)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Open", false);

            DoorMeshBoxCollider.GetComponent<BoxCollider>().enabled = true;
        }

    }

    private void Update()
    {
        if (isOpen)
        {
            DoorMeshBoxCollider.GetComponent<BoxCollider>().enabled = false;
        }

        else if (isOpen == false)
        {
            DoorMeshBoxCollider.GetComponent<BoxCollider>().enabled = true;
        }
    }
}