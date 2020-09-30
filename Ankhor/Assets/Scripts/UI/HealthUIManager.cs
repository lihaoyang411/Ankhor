using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{

    public Image HealthBarImage;
    public float Health;

    // Start is called before the first frame update
    void Start()
    {
        Health = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // if attcked
        Health-= Time.deltaTime * .01f;
        HealthBarImage.fillAmount = Health;

    }
}
