using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal3 : MonoBehaviour
{
    public GameObject Button;

    public GameObject particleEffect;

    public bool CloseButtonPressed;

    public Vector3 targetScale;

    public float shrinkSpeed = 0.3f;

    private void Start()
    {

    }

    private void Update()
    {
        if (CloseButtonPressed)
        {
            Closeing();
        }
    }

    private void Closeing()
    {
        gameObject.transform.localScale -= Vector3.one * Time.deltaTime * shrinkSpeed;

        if (gameObject.transform.localScale.magnitude <= targetScale.magnitude)
        {
            Debug.Log("Hit target scale");

            GameObject particleEff = Instantiate(particleEffect, transform);

            particleEff.transform.localPosition = new Vector3(0, 0, 0);

            particleEff.transform.parent = null;

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBodyVR")
        {
            CloseButtonPressed = true;
        }
    }
}
