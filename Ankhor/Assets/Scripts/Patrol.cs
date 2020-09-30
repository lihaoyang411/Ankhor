using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public float speed;
    private float waitTime;
    public float startWaitTime;

    public Transform[] moveSpots;
    private int spot;
    public float lookRadius = 10f;
    Transform target = null;
    NavMeshAgent agent;
    public bool stingerPlayed = false;
    private bool seenFlipped = true;
    private bool rayFound = false;
    private bool radiusIncrease = false;



    void TurnCorner()
    {
        int newSpot = spot + 1;
        Vector3 direction = (moveSpots[(newSpot % 5)].position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    // Start is called before the first frame update
    void Start()
    {
        waitTime = startWaitTime;
        spot = 0;
        target = GameObject.FindWithTag("MainCamera").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(transform.position.x, 0, transform.position.z));

        RaycastHit hit1, hit2, hit3, hit4;

        //Draw ray
        Vector3 forward = transform.forward * 20;
        Vector3 middle = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        Debug.DrawRay(middle, forward, Color.green);

        Vector3 adder = new Vector3(forward.x + 1, forward.y, forward.z + 1) * 20;
        Debug.DrawRay(middle, adder, Color.red);

        Vector3 adder2 = new Vector3(forward.x - 1, forward.y, forward.z - 1) * 20;
        Debug.DrawRay(middle, adder2, Color.blue);

        Vector3 adder3 = new Vector3(forward.x, forward.y-2, forward.z) * 20;
        Debug.DrawRay(middle, adder3, Color.yellow);

        //Ray sightRay = new Ray();
        if (Physics.Raycast(middle, forward, out hit1) && !rayFound)
        {
            if (hit1.collider.tag == "Player")
            {
                Debug.Log("here");
                rayFound = true;
                agent.SetDestination(target.position);
                if (distance <= 2)
                {
                    FaceTarget();
                }
                if (!stingerPlayed)
                {
                    GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().seen = true;
                    stingerPlayed = true;
                    seenFlipped = false;
                }
                radiusIncrease = true;
                lookRadius *= 5;
            }
        }
        if (Physics.Raycast(middle, adder, out hit2) && !rayFound)
        {
            if (hit2.collider.tag == "Player")
            {
                Debug.Log("here");
                rayFound = true;
                agent.SetDestination(target.position);
                if (distance <= 2)
                {
                    FaceTarget();
                }
                if (!stingerPlayed)
                {
                    GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().seen = true;
                    stingerPlayed = true;
                    seenFlipped = false;
                }
                radiusIncrease = true;
                lookRadius *= 5;
            }
        }
        if (Physics.Raycast(middle, adder2, out hit3) && !rayFound)
        {
            if (hit3.collider.tag == "Player")
            {
                Debug.Log("here");
                rayFound = true;
                agent.SetDestination(target.position);
                if (distance <= 2)
                {
                    FaceTarget();
                }
                if (!stingerPlayed)
                {
                    GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().seen = true;
                    stingerPlayed = true;
                    seenFlipped = false;
                }
                radiusIncrease = true;
                lookRadius *= 5;
            }
        }
        if (Physics.Raycast(middle, adder3, out hit4) && !rayFound)
        {
            if (hit4.collider.tag == "Player")
            {
                Debug.Log("here");
                rayFound = true;
                agent.SetDestination(target.position);
                if (distance <= 2)
                {
                    FaceTarget();
                }
                if (!stingerPlayed)
                {
                    GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().seen = true;
                    stingerPlayed = true;
                    seenFlipped = false;
                }
                radiusIncrease = true;
                lookRadius *= 5;
            }
        }
        if (distance <= lookRadius && !rayFound)
        {
            rayFound = true;
            agent.SetDestination(target.position);
            if (distance <= 2)
            {
                FaceTarget();
            }
            if (!stingerPlayed)
            {
                GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().seen = true;
                stingerPlayed = true;
                seenFlipped = false;
            }
            radiusIncrease = true;
            lookRadius *= 5;
        }
        if (distance <= lookRadius && rayFound)
        {
            agent.SetDestination(target.position);
        }
        else if(distance >= lookRadius + 2)
        {
            rayFound = false;
            if (!seenFlipped)
            {
                GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().seen = false;
                seenFlipped = true;
            }
            stingerPlayed = false;
            //transform.position = Vector3.MoveTowards(transform.position, moveSpots[spot].position, speed * Time.deltaTime);
            agent.SetDestination(moveSpots[spot].position);

            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(moveSpots[spot].position.x, 0, moveSpots[spot].position.z)) < 1.0f)
            {
                if (waitTime <= 0)
                {
                    spot++;
                    spot = spot % 5;
                    waitTime = startWaitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                    TurnCorner();
                }
            }
            if (radiusIncrease)
            {
                lookRadius /= 5;
                radiusIncrease = false;
            }
        }



        // Attacking
        if(distance <= 1)
        {
            GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAI>().Health -= .01f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 ball = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        Gizmos.DrawWireSphere(ball, lookRadius);
    }
}
