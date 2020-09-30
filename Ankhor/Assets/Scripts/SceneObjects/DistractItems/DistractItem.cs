using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractItem : MonoBehaviour
{
    [Tooltip("The hand script that this gameobject is referencing to")]
    public GameObject Hand = null;

    [Tooltip("The hand that it is currently holding this game object")]
    public GameObject HandHoldingPos = null;

    [Tooltip("The name of the sound for the item")]
    public string SoundName;

    [Tooltip("The sound radius for the cultist to get attracted to the object")]
    public float soundRadius = 5.0f;

    [Tooltip("The amount of force needed to destroy the object (ex. 4 is for a coffee cup)")]
    public float ImpactLimit = 3.5f;

    private GameObject[] cultists;

    private bool wasThrown = false;

    private void Start()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            Debug.Log("There is no ridgid body in " + gameObject.name + "!");
        }
        else
        {
            // GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = true;
        }

        cultists = GameObject.FindGameObjectsWithTag("Cultist");
    }

    public void Throw(GameObject Hand)
    {
        this.gameObject.GetComponent<DistractItem>().Hand = Hand;

        // throw it

        // unparent it 
        transform.parent = null;

        Debug.Log("Unparented " + gameObject.transform.parent);

        Hand.GetComponent<PlayerHand>().holdingObject = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;

        // send the object flying 
        GetComponent<Rigidbody>().AddForce(Hand.GetComponent<PlayerHand>().handForce * 5000);

        wasThrown = true;

        /*
        // remove it from the inventory 
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        List<GameObject> PlayerInventory = Player.GetComponent<InventorySwap>().PlayerInventory;
        Hand.GetComponent<PlayerHand>().holdingObject = false;
        Hand.GetComponent<PlayerHand>().ItemOnHand = PlayerInventory[0].gameObject;

        PlayerInventory.Remove(gameObject);
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Other Collider name " + collision.gameObject.name);

        Debug.Log("Collision Impulse " + collision.impulse);

        Debug.Log("Collision Impulse Sum " + collision.relativeVelocity);

        Debug.Log("Magnitude Impulse Sum " + collision.impulse.magnitude);

        // soundRadius = collision.impulse.magnitude / 2;

        if (collision.impulse.magnitude >= ImpactLimit)
        {
            // soundRadius = collision.impulse.magnitude / 2;

            Debug.Log(gameObject.name + " broke!");

            RaycastHit hit;
            foreach (GameObject cultist in cultists)
            {
                if (Physics.Raycast(gameObject.transform.position, cultist.transform.position, out hit))
                {
                    if (hit.collider.tag.Equals("Cultist"))
                    {
                        Debug.LogWarning("Ray hits Cultist");
                        hit.collider.gameObject.GetComponent<CultistsAIManager>().InHearingRadiusCheck(soundRadius, gameObject.transform, true);
                    }
                }
            }

            // also if it was thrown remove it from the inventory 

            if (wasThrown)
            {
                GameObject Player = GameObject.FindGameObjectWithTag("Player");
                List<GameObject> PlayerInventory = Player.GetComponent<InventorySwap>().PlayerInventory;
                Hand.GetComponent<PlayerHand>().holdingObject = false;
                Hand.GetComponent<PlayerHand>().ItemOnHand = PlayerInventory[0].gameObject;

                PlayerInventory.Remove(gameObject);

                AkSoundEngine.PostEvent(SoundName, gameObject);

                Destroy(gameObject);
            }
        }
    }

    /*
    // when this object breaks against something 
    private void OnDestroy()
    {
        Debug.Log(gameObject.name + " broke!");

        RaycastHit hit;
        foreach (GameObject cultist in cultists)
        {
            if (Physics.Raycast(gameObject.transform.position, cultist.transform.position, out hit))
            {

                // New Code
                if (hit.collider.tag.Equals("Cultist"))
                {
                    Debug.LogWarning("Ray hits Cultist");
                    hit.collider.gameObject.GetComponent<CultistsAIManager>().InHearingRadiusCheck(soundRadius, gameObject.transform, true);
                }
            }
        }
    }
    */


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, soundRadius);
    }

}
