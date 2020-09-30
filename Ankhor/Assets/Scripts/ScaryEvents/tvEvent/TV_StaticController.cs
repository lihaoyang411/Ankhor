using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV_StaticController : MonoBehaviour
{
    bool bIsOn = false;
    float rtpc = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rtpc = Vector3.Distance(this.transform.position, GameObject.FindWithTag("PlayerHeadVR").transform.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerTV_RTPC", rtpc);
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsOn)
        {
            rtpc = Vector3.Distance(this.transform.position, GameObject.FindWithTag("PlayerHeadVR").transform.position);
            AkSoundEngine.SetRTPCValue("Distance_PlayerTV_RTPC", rtpc);
            Debug.Log("Distance static:" + rtpc);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            this.GetComponentInParent<MeshRenderer>().enabled = true;
            bIsOn = true;
            BeginSound();
            StartCoroutine(TurnOff(60.185f));
        }
    }

    void BeginSound()
    {
        AkSoundEngine.PostEvent("TVStatic", gameObject);
    }

    IEnumerator TurnOff(float time)
    {
        yield return new WaitForSeconds(time);

        this.GetComponentInParent<MeshRenderer>().enabled = false;
        bIsOn = false;
    }
}
