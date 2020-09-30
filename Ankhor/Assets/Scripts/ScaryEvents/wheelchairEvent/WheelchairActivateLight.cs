using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairActivateLight : MonoBehaviour
{
    public bool lightActivate = false;
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
        if (other.tag == "PlayerBodyVR")
        {
            lightActivate = true;
        }
    }
}
