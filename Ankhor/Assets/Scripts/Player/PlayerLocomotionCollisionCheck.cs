using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionCollisionCheck : MonoBehaviour
{
    //public GameObject VivePlayer;
    //bool playerDontMove = GameObject.Find("Actions").GetComponent<Locomotion>().playerDontMove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NoPlayerPass") || other.CompareTag("NoPlayerPassObject"))
        {
            GameObject.Find("Actions").GetComponent<Locomotion>().playerDontMove = true;
            GameObject.Find("Actions").GetComponent<Locomotion>().badDir = other.transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NoPlayerPass") || other.CompareTag("NoPlayerPassObject"))
        {
            GameObject.Find("Actions").GetComponent<Locomotion>().playerDontMove = false;
        }
    }
}
