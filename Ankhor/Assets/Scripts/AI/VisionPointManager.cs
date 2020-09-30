using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionPointManager : MonoBehaviour
{
    private Transform startingTransform;

    public Transform headTransform;

    private bool isDemond = false;

    // Start is called before the first frame update
    void Start()
    {
        startingTransform = transform;

        isDemond = this.tag.Equals("Demond");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetStatus(bool isInvestigating)
    {
        if (isInvestigating)
        {
            if(!isDemond) {
                transform.rotation = headTransform.rotation;
            } else {
                Quaternion lookRoation0 = Quaternion.AngleAxis(70f, new Vector3(0, 1, 0));
                Quaternion lookRoation1 = Quaternion.AngleAxis(-140f, new Vector3(0, 1, 0));

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRoation0, Time.deltaTime * 2f);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRoation1, Time.deltaTime * 2f);
            }
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

}
