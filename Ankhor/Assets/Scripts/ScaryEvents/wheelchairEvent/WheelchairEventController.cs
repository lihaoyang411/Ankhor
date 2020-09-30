using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairEventController : MonoBehaviour
{
    Animator anim;
    bool ActivatedLight = false;
    bool ActivatedChair = false;
    float rtpc = 0.0f;
    GameObject playerHead;


    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameObject.FindWithTag("PlayerHeadVR");
        rtpc = Vector3.Distance(GameObject.Find("SpookyWheelchair").transform.position, playerHead.transform.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerObject_RTPC", rtpc);
        transform.GetChild(1).gameObject.SetActive(false);
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rtpc = Vector3.Distance(GameObject.Find("SpookyWheelchair").transform.position, GameObject.FindWithTag("PlayerHeadVR").transform.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerObject_RTPC", rtpc);
        if (transform.GetChild(2).GetComponent<WheelchairActivateLight>().lightActivate && !ActivatedLight)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            ActivatedLight = true;
        }
        else if (transform.GetChild(3).GetComponent<WheelchairActivate>().chairActivate && !ActivatedChair)
        {
            AkSoundEngine.PostEvent("GhostLaugh", gameObject);
            anim.SetTrigger("ShouldMove");
            transform.GetChild(1).gameObject.SetActive(false);
            ActivatedChair = true;
        }
    }
}
