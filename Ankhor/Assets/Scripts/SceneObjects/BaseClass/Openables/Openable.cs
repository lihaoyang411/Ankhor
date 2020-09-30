using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openable : MonoBehaviour
{
    public bool isLocked;

    public bool isSpiritOnly;

    public bool isOpen;

    private void Start()
    {
        if(this.gameObject.GetComponent<Animator>() == null)
        {
            Debug.Log("Animator does not exist on " + this.gameObject.name + "!");
        }
    }

}
