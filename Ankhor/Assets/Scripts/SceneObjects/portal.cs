using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal : MonoBehaviour
{

    //public TextMesh PlayerState;

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            bool isSpirit = GetComponent<Player>().isSpirit;

            if (!isSpirit)
            {
                isSpirit = true;

                GetComponent<Player>().isSpirit = true;

                //PlayerState.text = "You are now a spirit!";
                GetComponent<EnemyBaseClass>().PlayerChangedForm();

            }
            else
            {
                isSpirit = false;

                GetComponent<Player>().isSpirit = false;

                //PlayerState.text = "You are now a doctor!";
                GetComponent<EnemyBaseClass>().PlayerChangedForm();

            }
        }
    }
}
