using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject PickUpHandPositionL;

    public GameObject PickUpHandPositionR;

    public GameObject LightSource;

    // this is used for enemies to detect 
    // if thesre is a flashlight activated

    public float LightSourceRange = 10.0f;

    public bool isOn = false;


    private void Start()
    {
        if (this.GetComponent<Rigidbody>() == null)
        {
            this.gameObject.AddComponent<Rigidbody>();

            this.GetComponent<Rigidbody>().useGravity = false;

            this.GetComponent<Rigidbody>().isKinematic = true;
        }

        else
        {
            this.GetComponent<Rigidbody>().useGravity = false;
        }

        isOn = false;

        LightSource.GetComponent<Light>().range = 0;
    }

    private void Update()
    {
        // if it is on then spawn a ray

        if (isOn)
        {

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.up, out hit, 45.0f))
            {

                Debug.Log("Hit");

                if (hit.transform.tag == "Cultist")
                {
                    Debug.Log("Hit Cultist");

                    //hit.transform.gameObject.GetComponent<EnemyBaseClass>().HeardTargetSound(LightSourceRange);
                    // New script vvv
                    hit.transform.gameObject.GetComponent<CultistsAIManager>().DetectedFlashLight(transform.position , LightSourceRange);
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.up * LightSourceRange);
        Gizmos.DrawRay(transform.position, direction);
    }

    public void ActivateFlashlight()
    {
        if (isOn)
        {
            LightSource.GetComponent<Light>().range = 0;

            isOn = false;
            AkSoundEngine.PostEvent("Flashlight_OFF", gameObject);
        }
        else
        {
            LightSource.GetComponent<Light>().range = LightSourceRange;

            isOn = true;
            AkSoundEngine.PostEvent("Flashlight_ON", gameObject);
        }
    }



    public void ShowLeftHand(bool isHoldingFlashlight)
    {
        if (!isHoldingFlashlight)
        {
            PickUpHandPositionL.SetActive(true);
        }
    }

    public void HideLeftHand()
    {
        PickUpHandPositionL.SetActive(false);
    }

    public void ShowRightHand(bool isHoldingFlashlight)
    {
        if (!isHoldingFlashlight)
        {
            PickUpHandPositionR.SetActive(true);
        }
    }

    public void HideRightHand()
    {
        PickUpHandPositionR.SetActive(false);
    }

}
