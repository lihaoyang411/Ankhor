using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{

    public bool testFreeze = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (testFreeze)
        {
            TriggerAIFreeze();
        }
    }

    private void TriggerAIFreeze()
    {
        testFreeze = false;

        GameObject[] cultists = GameObject.FindGameObjectsWithTag("Cultist");

        GameObject[] demonds = GameObject.FindGameObjectsWithTag("Demond");

        GameObject ankhor = GameObject.FindGameObjectWithTag("Ankhor");

        foreach(GameObject cultist in cultists)
        {
            cultist.GetComponent<CultistsAIManager>().Freeze();
        }

        foreach (GameObject demond in demonds)
        {
            demond.GetComponent<DemondsAIManager>().Freeze();
        }

        ankhor.GetComponent<AnkhorAIManager>().Freeze();


    }
}
