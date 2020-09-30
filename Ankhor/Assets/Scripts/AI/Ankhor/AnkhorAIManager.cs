using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnkhorAIManager : MonoBehaviour
{
    private Transform cultist;
    private Transform visionPoint;

    private Transform playerHead;
    private Transform[] playerHeadHands;
    NavMeshAgent agent;

    public const float MAX_RADIUS = 15.0f;
    public const float MAX_SPEED = 5.0f;

    public float frontAngle = 180.0f;
    public float frontRadius = 5.0f;
    public float frontCloseRadius = 8.0f;

    private float baseSpeed;
    private float baseFrontRadius;
    private float baseFrontCloseRadius;
    private Quaternion baseRotation;

    private const float patrollingStopDistance = 0.5f;
    private const float followingStopDistance = 1.5f;

    private Transform[] patrolSpots;

    public float speed = 3.0f;
    public float waitTime = 3.0f;
    private float startWaitTime;
    private bool searched = false;

    private Vector3 lastSeenPosition;
    private bool hasSeenPlayerLongRange = false;
    private bool hasSeenPlayerShortRange = false;
    private bool hasHeardSound = false; // Probably don't need it ... can be hasSeenPlayerLongRange insted

    private bool isFrozen = false;

    private const float MAX_HEARING_RADIUS = 10.0f;

    private AnkhorAnimationManager animationManager;
    private AnkhorLabelManager labelManager;
    private AnkhorSoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        InitSetUp();

        baseFrontRadius = frontRadius;
        baseFrontCloseRadius = frontCloseRadius;
        baseSpeed = speed;

        baseRotation = transform.rotation;

        startWaitTime = waitTime;
    }

    private void InitSetUp()
    {
        SetPlayer();
        SetGameObjectChildren();
        SetPatrolPoints();
        SetScrips();
    }

    public void SetPlayer()
    {
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        playerHeadHands = new Transform[] {
            GameObject.FindGameObjectWithTag("PlayerHead").transform,
            GameObject.FindGameObjectWithTag("PlayerLeftHandVR").transform,
            GameObject.FindGameObjectWithTag("PlayerRightHandVR").transform};

        //Debug.LogWarning("Player was set in CultistAIManager");
    }

    private void SetGameObjectChildren()
    {
        for (int index = 0; index < this.transform.childCount; index++)
        {
            Transform child = this.transform.GetChild(index);

            //Debug.LogWarning("ChildName: " + child.name);

            if (child.name.Equals("VisionPoint"))
            {
                visionPoint = child;
                //Debug.LogWarning("VisionPoint was set in CultistAIManager");
            }
            else if (child.name.Equals("cultAnimation"))
            {
                cultist = child;
                //Debug.LogWarning("cultAnimation was set in CultistAIManager");
            }
        }
    }

    private void SetPatrolPoints()
    {
        Transform enemyPatrolRoutes = GameObject.FindGameObjectWithTag("EnemyPatrolRoutes").transform;

        for (int index = 0; index < enemyPatrolRoutes.childCount; index++)
        {
            Transform child = enemyPatrolRoutes.GetChild(index);
            Transform childChild;

            if (child.name.Equals("AnkhorPatrolRoutes"))
            {
                for (int jndex = 0; jndex < child.childCount; jndex++)
                {
                    childChild = child.GetChild(jndex);
                    
                    //Debug.LogWarning(childChild.name + " length: " + childChild.childCount);

                    if (childChild.name[childChild.name.Length - 1] == this.name[this.name.Length - 1])
                    {
                        Transform[] childSpots = childChild.GetComponentsInChildren<Transform>();

                        //Debug.LogWarning("Cultist: " + this.name);
                        //Debug.LogWarning("patrolSpots were set in CultistAIManager");
                        //Debug.LogWarning("Name = " + childChild.name);
                        //Debug.LogWarning("Count = " + childChild.childCount);
                        //Debug.LogWarning("Patrol Points Count = " + childSpots.Length);

                        patrolSpots = new Transform[childSpots.Length - 1];
                        for (int kndex = 1; kndex < childSpots.Length; kndex++)
                        {
                            patrolSpots[kndex - 1] = childSpots[kndex];
                        }

                        return;
                    }
                }
            }
        }
    }

    private void SetScrips()
    {
        agent = GetComponent<NavMeshAgent>();

        soundManager = GetComponent<AnkhorSoundManager>();
        labelManager = GetComponent<AnkhorLabelManager>();
        animationManager = GetComponent<AnkhorAnimationManager>();
        animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);
    }

    // Update is called once per frame
    void Update()
    {
        CheckFieldOfView();

        soundManager.UpdateAnkhorStates(hasSeenPlayerShortRange,
            hasSeenPlayerLongRange);

        if (isFrozen)
        {
            animationManager.Freeze();

        }
        else if (hasSeenPlayerShortRange)
        {
            //Debug.LogWarning("hasSeenPlayerShortRange: "+ hasSeenPlayerShortRange);

            soundManager.PlayAnkhorDetectedStinger();
            labelManager.UpdateStatus(AnkhorStatus.CHASING);
            ChaseTarget();

            //Debug.LogWarning("Ankhor Detected");
        }
        else if ((hasSeenPlayerLongRange || hasHeardSound))
        {
            //Debug.LogWarning("hasSeenPlayerLongRange: " + hasSeenPlayerLongRange);
            //Debug.LogWarning("hasHeardSound: " + hasHeardSound);

            // Play enemyQuestionSound
            soundManager.PlayAnkhorCuriousStinger();
            labelManager.UpdateStatus(AnkhorStatus.SUSPICIOUS);
            Investigate();

            //Debug.LogWarning("Ankhor Investigating");
            //Debug.LogWarning("Ankhor Investigating hasSeenPlayerLongRange: " + hasSeenPlayerLongRange);
            //Debug.LogWarning("Ankhor Investigating hasHeardSound: " + hasHeardSound);

        }
        else
        {
            //Debug.LogWarning("Ankhor Patrolling");

            labelManager.UpdateStatus(AnkhorStatus.PATROLLING);
            Patrol();

        }

        // play sounds here
        //soundManager.PlayAnkhorFootstepSound(speed, MAX_SPEED);
        soundManager.PlayEnemyChaseSound();
    }

    private void CheckFieldOfView()
    {
        if (hasSeenPlayerShortRange)
        {
            waitTime = startWaitTime;
        }

        hasSeenPlayerShortRange = InFieldOfView(playerHeadHands, frontAngle, frontCloseRadius);
        if (!hasSeenPlayerShortRange && !hasHeardSound && !hasSeenPlayerLongRange)
        {
            hasSeenPlayerLongRange = InFieldOfView(playerHeadHands, frontAngle, frontRadius);
        }
    }

    private bool InFieldOfView(Transform[] targetPlayerObjects, float maxAngle, float maxRadius)
    {
        DisableCultistsCollisionBoxes();

        Collider[] colliders = new Collider[500];

        int count = Physics.OverlapSphereNonAlloc(visionPoint.position, maxRadius, colliders);

        for (int i = 0; i < count; i++)
        {
            if (colliders[i] != null)
            {

                for (int j = 0; j < targetPlayerObjects.Length; j++)
                {
                    if (colliders[i].tag.Equals(targetPlayerObjects[j].tag))
                    {
                        Vector3 directionBetween = (colliders[i].transform.position - visionPoint.position).normalized;
                        directionBetween.y *= 0;

                        // Making sure it is within the Angle
                        float angle0 = Vector3.Angle(visionPoint.forward, directionBetween);
                        float angle1 = Vector3.Angle(-visionPoint.forward, directionBetween);

                        // In Field Of Vision
                        if (angle0 <= maxAngle || angle1 <= maxAngle)
                        {
                            Ray ray = new Ray(visionPoint.position, targetPlayerObjects[j].position - visionPoint.position);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit, maxRadius))
                            {
                                if (hit.transform == targetPlayerObjects[j])
                                {
                                    lastSeenPosition = targetPlayerObjects[j].position;
                                    EnableCultistsCollisionBoxes();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        EnableCultistsCollisionBoxes();

        return false;
    }

    private bool InFieldOfView(Transform targetPlayerObject, float maxAngle, float maxRadius)
    {

        DisableCultistsCollisionBoxes();

        Collider[] colliders = new Collider[500];

        int count = Physics.OverlapSphereNonAlloc(visionPoint.position, maxRadius, colliders);

        for (int i = 0; i < count; i++)
        {
            if (colliders[i] != null)
            {

                if (colliders[i].tag.Equals(targetPlayerObject.tag))
                {
                    Vector3 directionBetween = (colliders[i].transform.position - visionPoint.position).normalized;
                    directionBetween.y *= 0;

                    float angle0 = Vector3.Angle(visionPoint.forward, directionBetween);
                    float angle1 = Vector3.Angle(-visionPoint.forward, directionBetween);

                    // In Field Of Vision
                    if (angle0 <= maxAngle || angle1 <= maxAngle)
                    {
                        Ray ray = new Ray(visionPoint.position, targetPlayerObject.position - visionPoint.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == targetPlayerObject)
                            {
                                lastSeenPosition = targetPlayerObject.position;
                                EnableCultistsCollisionBoxes();
                                return true;
                            }
                        }
                    }
                }
            }
        }

        EnableCultistsCollisionBoxes();
        return false;
    }

    public void InHearingRadiusCheck(float soundRadius, Transform soundTransform, bool areSoundsAndWallsAccounted)
    {
        float distance = Vector3.Distance(soundTransform.position, visionPoint.position);
        if (distance <= soundRadius || (distance - MAX_HEARING_RADIUS) <= soundRadius)
        {
            if (areSoundsAndWallsAccounted)
            {
                hasHeardSound = true;
                lastSeenPosition = soundTransform.position;
            }
            else
            {
                hasHeardSound = InFieldOfView(soundTransform, MAX_HEARING_RADIUS, soundRadius);

                // hasSeenPlayerLongRange = hasHeardSound;
            }
        }
    }

    public void DetectedFlashLight(Vector3 flashlightPosition, float lightRange)
    {
        float distance = Vector3.Distance(flashlightPosition, visionPoint.position);

        if (distance <= lightRange)
        {
            hasSeenPlayerLongRange = true;
            lastSeenPosition = playerHead.position;
        }

    }

    private void ChaseTarget()
    {
        SetValuesToAwareState();
        Vector3 currentPosition = transform.position;
        Vector3 currentTargetPosition = playerHead.position;

        currentPosition.y *= 0;
        currentTargetPosition.y *= 0;

        if (Vector3.Distance(currentPosition, currentTargetPosition) <= agent.stoppingDistance)
        {
            FaceTarget();
            AttackTarget();
            animationManager.UpdateState(AnkhorAnimationStates.ATTAKING);
        }
        else
        {
            agent.SetDestination(lastSeenPosition);
            animationManager.UpdateState(AnkhorAnimationStates.WALKING);
        }

        hasSeenPlayerLongRange = hasSeenPlayerShortRange;
    }

    private void Investigate()
    {
        Vector3 currentPosition = transform.position;
        Vector3 lastSeenTargetPosition = lastSeenPosition;

        currentPosition.y *= 0;
        lastSeenTargetPosition.y *= 0;

        //agen.stoppingDistance = followingStopDistance;

        if (Vector3.Distance(currentPosition, lastSeenTargetPosition) <= agent.stoppingDistance)
        {
            //Debug.LogWarning("1st if in Investigate");

            float splitTime = startWaitTime / 3.0f;
            visionPoint.GetComponent<VisionPointManager>().SetStatus(true);

            if ((splitTime * 2.0f) <= waitTime)
            {
                //Debug.LogWarning("2nd if in Investigate");

                waitTime -= Time.deltaTime;
                animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);
            }
            else if (0.0f <= waitTime)
            {
                //Debug.LogWarning("3rd if in Investigate");

                waitTime -= Time.deltaTime;
                animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);
            }
            else
            {
                //Debug.LogWarning("4th if in Investigate");

                hasHeardSound = false;
                hasSeenPlayerLongRange = false;
                waitTime = startWaitTime;
                soundManager.ResetAnkhorDetectedStinger();
                soundManager.ResetAnkhorCuriousStinger();
                animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);

            }
        }
        else
        {
            //Debug.LogWarning("Last Else in Investigate");

            animationManager.UpdateState(AnkhorAnimationStates.WALKING);
            agent.SetDestination(lastSeenPosition);
            visionPoint.GetComponent<VisionPointManager>().SetStatus(false);
        }

    }

    private void Patrol()
    {
        //Debug.LogWarning("In Patroll");

        SetValuesToUnawareState();

        //Debug.LogWarning("In Patroll");

        Vector3 currentPosition = transform.position;
        Vector3 currentPatrolPosition = patrolSpots[0].position;

        currentPosition.y *= 0;
        currentPatrolPosition.y *= 0;

        if (Vector3.Distance(currentPosition, currentPatrolPosition) <= agent.stoppingDistance)
        {
            //Debug.LogWarning("Patrol, 1st if");

            if (waitTime <= 0 && searched)
            {
                //Debug.LogWarning("Patrol, 2nd if");

                waitTime = startWaitTime;
                searched = false;
            }
            else
            {
                //Debug.LogWarning("Patrol, 1st else");

                if (patrolSpots.Length == 1)
                {
                    //Debug.LogWarning("Patrol, 3rd if");

                    transform.rotation = baseRotation;
                }

                float splitTime = startWaitTime / 3.0f;
                visionPoint.GetComponent<VisionPointManager>().SetStatus(true);
                if ((splitTime * 2.0f) <= waitTime && !searched)
                {
                    //Debug.LogWarning("Patrol, 4th if");

                    waitTime -= Time.deltaTime;
                    animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);
                }
                else if (0.0f < waitTime && !searched)
                {
                    //Debug.LogWarning("Patrol, 2nd else");

                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0.0f)
                    {
                        //Debug.LogWarning("Patrol, 5th if");

                        waitTime = startWaitTime;
                        searched = true;
                    }
                    animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);

                }
                else
                {
                    //Debug.LogWarning("Patrol, 3rd else");

                    waitTime -= Time.deltaTime;
                    visionPoint.GetComponent<VisionPointManager>().SetStatus(false);
                    animationManager.UpdateState(AnkhorAnimationStates.SEARCHING);

                }
            }
        }
        else
        {
            //Debug.LogWarning("Patrol, 4th else");
            animationManager.UpdateState(AnkhorAnimationStates.WALKING);
            agent.SetDestination(patrolSpots[0].position);
            visionPoint.GetComponent<VisionPointManager>().SetStatus(false);
        }

    }

    public void Freeze()
    {
        if (!isFrozen)
        {
            animationManager.Freeze();
            agent.isStopped = true;
        }
    }

    public void DeFrosted()
    {
        isFrozen = false;
        agent.isStopped = false;
    }

    private void SetValuesToAwareState()
    {
        frontRadius = MAX_RADIUS;
        frontCloseRadius = MAX_RADIUS;
        speed = MAX_SPEED;
        agent.stoppingDistance = followingStopDistance;

        agent.speed = speed;
    }

    private void SetValuesToUnawareState()
    {
        frontRadius = baseFrontRadius;
        frontCloseRadius = baseFrontCloseRadius;
        speed = baseSpeed;
        agent.stoppingDistance = patrollingStopDistance;

        agent.speed = speed;
    }

    private void AttackTarget()
    {
        GameObject.FindWithTag("Player").GetComponentInChildren<Player>().health -= 0.1f;
    }

    private void FaceTarget()
    {
        Vector3 direction = (playerHead.position - transform.position).normalized;
        direction.y *= 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void DisableCultistsCollisionBoxes()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);
        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = false;
        }
    }

    private void EnableCultistsCollisionBoxes()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);
        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
        }
    }

    public void Reset()
    {
        hasSeenPlayerShortRange = false;
        hasSeenPlayerLongRange = false;
        hasHeardSound = false;
        transform.position = patrolSpots[0].position;
        soundManager.ResetAnkhorCuriousStinger();
        soundManager.ResetAnkhorDetectedStinger();
        //AkSoundEngine.SetState("Enemy_State", "Normal");
    }

    // Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(visionPoint.position, frontRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(visionPoint.position, frontCloseRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(visionPoint.position, MAX_HEARING_RADIUS);

        Vector3 fieldOfViewLine0 = Quaternion.AngleAxis(frontAngle, visionPoint.up) * visionPoint.forward * frontRadius;
        Vector3 fieldOfViewLine1 = Quaternion.AngleAxis(-frontAngle, visionPoint.up) * visionPoint.forward * frontRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(visionPoint.position, fieldOfViewLine0);
        Gizmos.DrawRay(visionPoint.position, fieldOfViewLine1);

        if (hasSeenPlayerShortRange)
        {
            Gizmos.color = Color.red;
        }
        else if (hasSeenPlayerLongRange)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Vector3 vec = playerHead.position - visionPoint.position;
        Gizmos.DrawRay(visionPoint.position, vec);

        for (int i = 0; i < playerHeadHands.Length; i++)
        {
            float distance = Vector3.Distance(visionPoint.position, playerHeadHands[i].position);
            if (distance <= frontCloseRadius)
            {
                Gizmos.DrawRay(visionPoint.position, (playerHeadHands[i].position - visionPoint.position).normalized * distance);
            }
            else
            {
                Gizmos.DrawRay(visionPoint.position, (playerHeadHands[i].position - visionPoint.position).normalized * frontCloseRadius);

            }
        }

        Gizmos.color = Color.black;
        Gizmos.DrawRay(visionPoint.position, visionPoint.forward * frontRadius);

    }
}