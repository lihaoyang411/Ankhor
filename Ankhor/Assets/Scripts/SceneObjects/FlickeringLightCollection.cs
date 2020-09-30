using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLightCollection : MonoBehaviour
{
    public float minWait = 0.05f;
    public float maxWait = 0.5f;
    public float minIntensity = 0.75f;
    public float maxIntensity = 1.5f;
    Component[] lightList;
    bool bDimmed = false;

    // Start is called before the first frame update
    void Start()
    {
        lightList = this.GetComponentsInChildren<Light>();
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            foreach (Light l in lightList)
            {
                if (!bDimmed) 
                {
                    l.intensity = minIntensity;
                    bDimmed = true;
                }

                else
                {
                    l.intensity = maxIntensity;
                    bDimmed = false;
                }
            }
        }
    }
}
