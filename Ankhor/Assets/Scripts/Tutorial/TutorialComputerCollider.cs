using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialComputerCollider : MonoBehaviour
{
    public bool recordsLogged = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Equals("PatientRecords"))
            recordsLogged = true;
    }
}
