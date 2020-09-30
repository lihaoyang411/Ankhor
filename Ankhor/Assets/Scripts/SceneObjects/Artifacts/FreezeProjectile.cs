using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeProjectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cultist")
        {
            // freeze the enemy 
            other.GetComponent<CultistsAIManager>().Freeze();
        }
        if (other.tag == "Demond")
        {
            // freeze the enemy 
            other.GetComponent<DemondsAIManager>().Freeze();
        }
        if (other.tag == "Ankhor")
        {
            // freeze the enemy 
            other.GetComponent<AnkhorAIManager>().Freeze();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
