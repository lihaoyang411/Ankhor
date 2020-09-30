using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightManager : MonoBehaviour
{
    GameObject vrcamera;

    private void Awake()
    {
        vrcamera = GameObject.FindGameObjectWithTag("PlayerHeadVR");
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(new Vector3(vrcamera.transform.position.x, vrcamera.transform.position.y + 4, vrcamera.transform.position.z), Quaternion.Euler(new Vector3(vrcamera.transform.rotation.x + 90, 0, 0)));
    }
}
