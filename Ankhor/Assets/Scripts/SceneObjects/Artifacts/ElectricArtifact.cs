using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricArtifact : MonoBehaviour
{

    [Tooltip("Needs a gameobject to reference it's other collider")]
    public GameObject ElectricityCollider;

    public GameObject ElectricityParticleSystem;

    [Tooltip("Needs to be taken to spirit before being able to activate it")]
    public bool isSealed = true;

    private bool isElectricityEnabled = false;

    private void Start()
    {
        isSealed = true;

        if (gameObject.GetComponent<BoxCollider>() == null)
        {
            Debug.Log(gameObject.name + " does not have a box collider!");
        }

        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            Debug.Log(gameObject.name + " does not have a rigidbody!");
        }
    }

    private void Update()
    {
        if (isElectricityEnabled)
        {
            ElectricityParticleSystem.SetActive(true);
        }
        else
        {
            ElectricityParticleSystem.SetActive(false);
        }
    }

    // when the player 
    // hits the trigger 
    // activate the artifact 

    public void Activate()
    {

        if (!isSealed)
        {
            // turn on the box collider for this gameobject

            ElectricityCollider.GetComponent<BoxCollider>().enabled = true;

            isElectricityEnabled = true;

            // turn off the artifact after 1 second
            Invoke("TurnOff", 1);
        }

    }

    public void TurnOff()
    {
        // turn off the box collider for this game object

        ElectricityCollider.GetComponent<BoxCollider>().enabled = false;

        isElectricityEnabled = false;
    }

}
