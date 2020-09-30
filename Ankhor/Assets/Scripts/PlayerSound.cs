using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{

    public GameObject Player;
    private Transform playerHead;
    float soundRadius = 7.0f;
    private GameObject[] cultists;
    Vector3 previous;
    float velocity;
    Locomotion.PlayerStatus status;
    
    //Component locomotionComponent = GameObject.Find("Actions").GetComponent<Locomotion>();


    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;

        cultists = GameObject.FindGameObjectsWithTag("Cultist");
        previous = Player.transform.position;
        status = GameObject.Find("Actions").GetComponent<Locomotion>().status;
    }

    // Update is called once per frame
    void Update()
    {
        status = GameObject.Find("Actions").GetComponent<Locomotion>().status;
        velocity = Vector3.Distance(Player.transform.position, previous);
        previous = Player.transform.position;

        if (status == Locomotion.PlayerStatus.Walking)
            soundRadius = 8.0f;
        else if (status == Locomotion.PlayerStatus.Running)
            soundRadius = 12.0f;
        else if (status == Locomotion.PlayerStatus.Crouching)
            soundRadius = 2.0f;

        foreach (GameObject cultist in cultists)
        {
            //Debug.Log(cultist.transform.position);
            //Debug.DrawLine(Player.transform.position, cultist.transform.position, Color.green);
        }
        if (velocity >= .01f)
        {
            RaycastHit hit;
            foreach (GameObject cultist in cultists)
            {
                if (Physics.Linecast(playerHead.position, cultist.transform.position, out hit))
                {

                    // New Code
                    if(hit.collider.tag.Equals("Cultist")) {
                        Debug.LogWarning("Ray hits Cultist");
                        hit.collider.gameObject.GetComponent<CultistsAIManager>().InHearingRadiusCheck(soundRadius, playerHead, true);
                    }

                    //float distance = Vector3.Distance(Player.transform.position, cultist.transform.position);
                    //if (hit.collider.tag == "Cultist" && distance <= soundRadius)
                    //{
                    //    //Debug.Log("A cultist can hear");
                    //    //hit.collider.gameObject.GetComponent<EnemyBaseClass>()..HeardTargetSound(soundRadius)
                    //}
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.transform.position, soundRadius);
    }
}
