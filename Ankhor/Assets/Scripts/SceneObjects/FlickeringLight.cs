using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public float minWait = 0.05f;
    public float maxWait = 0.5f;
    Light l;
    // Start is called before the first frame update
    void Start()
    {
        l = GetComponent<Light>();
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            l.enabled = !l.enabled;
        }
    }
}
