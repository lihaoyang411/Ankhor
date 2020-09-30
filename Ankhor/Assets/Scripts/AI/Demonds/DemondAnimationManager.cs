using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemondAnimationManager : MonoBehaviour
{
    private bool isFrozen = false;

    public float frozenTime = 3.0f;
    public float currentFrozenTime;

    // R, G, B is set for when the Cultist is Frozen
    private float[] RGB = new float[3];
    private float[] currentRGB = new float[3];

    // Start is called before the first frame update
    void Start()
    {
        currentFrozenTime = frozenTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFrozen)
        {
            Defrost();
        }
    }

    public void Freeze()
    {
        //animator.enabled = false;
        FrozenRGB(0, 195, 255);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.material.color = new Color(0, 195, 255);
        }

        isFrozen = true;
    }

    private void Defrost()
    {
        if (currentFrozenTime <= 0.0f)
        {
            //animator.enabled = true;
            isFrozen = false;

            this.GetComponent<DemondsAIManager>().DeFrosted();

            currentFrozenTime = frozenTime;

            // change the entire cultist back to it's original color

            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (var r in renderers)
            {
                r.material.color = Color.white;
            }
        }
        else
        {
            currentFrozenTime -= Time.deltaTime;

            // at 1/5 of the orignal freezing time, it will change to it's regular color
            for (int index = 0; index < RGB.Length; index++)
            {
                currentRGB[index] = Remap(currentFrozenTime - (frozenTime / 5), 0, frozenTime - (frozenTime / 5), 0, RGB[index]);

                if (currentRGB[index] <= 0.0f)
                {
                    currentRGB[index] = 0.0f;
                }
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (var r in renderers)
            {
                if (currentRGB[0] <= 0 && currentRGB[1] <= 0 && currentRGB[2] <= 0)
                {
                    r.material.color = Color.white;
                }
                else
                {
                    r.material.color = new Color(currentRGB[0], currentRGB[1], currentRGB[2]);
                }
            }
        }
    }

    // the rgb the projectile passes to turn into 
    public void FrozenRGB(float r, float g, float b)
    {
        RGB[0] = r;
        RGB[1] = g;
        RGB[2] = b;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}