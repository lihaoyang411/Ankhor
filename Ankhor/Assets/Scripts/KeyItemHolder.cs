using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemHolder : MonoBehaviour
{
    public GameObject ControlPanel;
    
    public void TakeObject()
    {
        ControlPanel.GetComponent<ControlPanel>().CloseDoors();
    }
}
