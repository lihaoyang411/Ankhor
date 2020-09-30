using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScareTrigger : MonoBehaviour
{
    bool bIsOn = false;
    float rtpc = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rtpc = Vector3.Distance(this.transform.position, GameObject.FindWithTag("PlayerHeadVR").transform.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerSkeleton_RTPC", rtpc);
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsOn)
        {
            rtpc = Vector3.Distance(this.transform.position, GameObject.FindWithTag("PlayerHeadVR").transform.position);
            AkSoundEngine.SetRTPCValue("Distance_PlayerSkeleton_RTPC", rtpc);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            Animator anim = GameObject.Find("SpookySkeleton").GetComponent<Animator>();
            anim.SetTrigger("SkellyScare");
            bIsOn = true;
            BeginSound();
            StartCoroutine(TurnOff(6.65f));
        }
    }

    void BeginSound()
    {
        AkSoundEngine.PostEvent("SkeletonScream", gameObject);
    }

    IEnumerator TurnOff(float time)
    {
        yield return new WaitForSeconds(time);

        bIsOn = false;
    }
}
