using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadCollisionCheck : MonoBehaviour
{
    public GameObject ViveEye;
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
        if (other.CompareTag("NoPlayerPass"))
        {
            Debug.Log("Entered Wall");
            ViveEye.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoPlayerPass"))
        {
            Debug.Log("Exited Wall");
            ViveEye.SetActive(false);
        }
    }
}
