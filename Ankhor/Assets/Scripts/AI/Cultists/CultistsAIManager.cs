using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CultistsAIManager : MonoBehaviour
{
    private Transform cultist;
    private Transform visionPoint;

    private Transform playerHead;
    private Transform[] playerHeadHands;
    NavMeshAgent agent;

    public const float MAX_ANGLE = 180.0f;
    public const float MAX_RADIUS = 10.0f;
    public const float MAX_SPEED = 3.0f;

    public float frontAngle = 65.0f;
    public float frontRadius = 7.0f;
    public float frontCloseRadius = 3.0f;

    public float backRadius = 2.0f;

    private float baseFrontAngle;
    private float baseFrontRadius;
    private float baseFrontCloseRadius;
    private float baseSpeed;
    private Quaternion baseRotation;

    private const float patrollingStopDistance = 0.5f;
    private const float followingStopDistance = 1.5f;

    private Transform[] patrolSpots;
    private int spot = 0;

    public float speed = 2.0f;
    public float waitTime = 3.0f;
    private float startWaitTime;
    private bool searched = false;

    private Vector3 lastSeenPosition;
    private bool hasSeenPlayerLongRange = false;
    private bool hasSeenPlayerShortRange = false;
    private bool hasHeardSound = false; // Probably don't need it ... can be hasSeenPlayerLongRange insted

    private bool isFrozen = false;

    private const float MAX_SHOUTING_RADIUS = 10.0f;
    private const float MAX_HEARING_RADIUS = 8.0f;

    private GameObject clone = null;

    private CultistAnimationManager animationManager;
    private CultistLabelManager labelManager;
    private CultistSoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        InitSetUp();

        baseFrontAngle = frontAngle;
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

            if (child.name.Equals("CultistPatrolRoutes"))
            {
                for (int jndex = 0; jndex < child.childCount; jndex++)
                {
                    childChild = child.GetChild(jndex);
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

        soundManager = GetComponent<CultistSoundManager>();
        labelManager = GetComponent<CultistLabelManager>();
        animationManager = GetComponent<CultistAnimationManager>();
        animationManager.UpdateState(CultistAnimationStates.WALKING);
    }

    // Update is called once per frame
    void Update()
    {
        CheckFieldOfView();

        soundManager.UpdateCultistStates(hasSeenPlayerShortRange,
            hasSeenPlayerLongRange);

        if (isFrozen)
        {

        }
        else if (hasSeenPlayerShortRange)
        {
            //Debug.LogWarning("hasSeenPlayerShortRange: "+ hasSeenPlayerShortRange);

            soundManager.PlayCultistDetectedStinger();
            labelManager.UpdateStatus(CultistStatus.CHASING);
            ChaseTarget();
            CallCultistBackup();
        }
        else if (hasSeenPlayerLongRange || hasHeardSound)
        {
            //Debug.LogWarning("hasSeenPlayerLongRange: " + hasSeenPlayerLongRange);
            //Debug.LogWarning("hasHeardSound: " + hasHeardSound);

            // Play enemyQuestionSound
            soundManager.PlayCultistCuriousStinger();
            labelManager.UpdateStatus(CultistStatus.SUSPICIOUS);
            Investigate();
        }
        else
        {
            //Debug.LogWarning("Patroling ");

            labelManager.UpdateStatus(CultistStatus.PATROLLING);
            Patrol();
        }

        // play sounds here
        soundManager.PlayCultistFootstepSound(speed, MAX_SPEED);
        soundManager.PlayEnemyChaseSound();

    }

    private void CheckFieldOfView()
    {
        if (hasSeenPlayerShortRange)
        {
            waitTime = startWaitTime;
        }

        hasSeenPlayerShortRange = InFieldOfView(playerHeadHands, frontAngle, frontCloseRadius, true);
        if (!hasSeenPlayerShortRange)
        {
            hasSeenPlayerShortRange = InFieldOfView(playerHeadHands, frontAngle, backRadius, false);
            if (!hasSeenPlayerShortRange && !hasHeardSound && !hasSeenPlayerLongRange)
            {
                hasSeenPlayerLongRange = InFieldOfView(playerHeadHands, frontAngle, frontRadius, true);
            }
        }
    }

    private bool InFieldOfView(Transform[] targetPlayerObjects, float maxAngle, float maxRadius, bool isFront)
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
                        float angle;
                        if (isFront)
                        {
                            angle = Vector3.Angle(visionPoint.forward, directionBetween);
                        }
                        else
                        {
                            angle = Vector3.Angle(-visionPoint.forward, directionBetween);
                        }

                        // In Field Of Vision
                        if (angle <= maxAngle)
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

    private bool InFieldOfView(Transform targetPlayerObject, float maxAngle, float maxRadius, bool isFront)
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

                    // Making sure it is within the Angle
                    float angle;
                    if (isFront)
                    {
                        angle = Vector3.Angle(visionPoint.forward, directionBetween);
                    }
                    else
                    {
                        angle = Vector3.Angle(-visionPoint.forward, directionBetween);
                    }

                    // In Field Of Vision
                    if (angle <= maxAngle)
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
                hasHeardSound = InFieldOfView(soundTransform, MAX_HEARING_RADIUS, soundRadius, true);

                // hasSeenPlayerLongRange = hasHeardSound;
            }
        }
    }

    public void HeardBackupCall(Transform otherCultist)
    {
        float distance = Vector3.Distance(otherCultist.position, visionPoint.position);
        if (distance <= MAX_SHOUTING_RADIUS || distance <= (MAX_SHOUTING_RADIUS + MAX_HEARING_RADIUS))
        {
            if (!hasSeenPlayerShortRange)
            {
                hasHeardSound = true;
                lastSeenPosition = playerHead.position;
                agent.stoppingDistance = followingStopDistance;
            }

            // else if(isInFieldOfView && wasInFieldOfView && !hasPlayedGrunt) {
            // PlayEnemyGruntSound();
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
            animationManager.UpdateState(CultistAnimationStates.ATTAKING);
        }
        else
        {
            agent.SetDestination(lastSeenPosition);
            animationManager.UpdateState(CultistAnimationStates.WALKING);
        }

        hasSeenPlayerLongRange = hasSeenPlayerShortRange;
    }

    private void CallCultistBackup()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (transform != enemies[i].transform)
            {
                enemies[i].GetComponentInChildren<CultistsAIManager>().HeardBackupCall(visionPoint);
            }
        }
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

            float splitTime = startWaitTime / 3.0f;
            visionPoint.GetComponent<VisionPointManager>().SetStatus(true);

            if ((splitTime * 2.0f) <= waitTime)
            {
                waitTime -= Time.deltaTime;
                animationManager.UpdateState(CultistAnimationStates.SEARCHING);
            }
            else if (0.0f <= waitTime)
            {
                waitTime -= Time.deltaTime;
                animationManager.UpdateState(CultistAnimationStates.SEARCHING);
            }
            else
            {
                hasHeardSound = false;
                hasSeenPlayerLongRange = false;
                waitTime = startWaitTime;
                soundManager.ResetCultistDetectedStinger();
                soundManager.ResetCultistCuriousStinger();
                animationManager.UpdateState(CultistAnimationStates.SEARCHING);

                GoToNearestPatrolSpot();
            }
        }
        else
        {
            animationManager.UpdateState(CultistAnimationStates.WALKING);
            agent.SetDestination(lastSeenPosition);
            visionPoint.GetComponent<VisionPointManager>().SetStatus(false);
        }

    }

    private void Patrol()
    {
        SetValuesToUnawareState();

        Vector3 currentPosition = transform.position;
        Vector3 currentPatrolPosition = patrolSpots[spot].position;

        currentPosition.y *= 0;
        currentPatrolPosition.y *= 0;

        if (Vector3.Distance(currentPosition, currentPatrolPosition) <= agent.stoppingDistance)
        {
            if (waitTime <= 0 && searched)
            {
                spot++;
                if (spot % patrolSpots.Length == 0)
                {
                    spot = 0;
                    ShufflePatrolSpots();
                }
                waitTime = startWaitTime;
                searched = false;
            }
            else
            {
                if (patrolSpots.Length == 1)
                {
                    transform.rotation = baseRotation;
                }

                float splitTime = startWaitTime / 3.0f;
                visionPoint.GetComponent<VisionPointManager>().SetStatus(true);
                if ((splitTime * 2.0f) <= waitTime && !searched)
                {
                    waitTime -= Time.deltaTime;
                    animationManager.UpdateState(CultistAnimationStates.SEARCHING);
                }
                else if (0.0f < waitTime && !searched)
                {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0.0f)
                    {
                        waitTime = startWaitTime;
                        searched = true;
                    }
                    animationManager.UpdateState(CultistAnimationStates.SEARCHING);

                }
                else
                {
                    waitTime -= Time.deltaTime;
                    visionPoint.GetComponent<VisionPointManager>().SetStatus(false);
                    if (patrolSpots.Length > 1)
                    {
                        TurnConer();
                    }
                    animationManager.UpdateState(CultistAnimationStates.SEARCHING);

                }
            }
        }
        else
        {
            animationManager.UpdateState(CultistAnimationStates.WALKING);
            agent.SetDestination(patrolSpots[spot].position);
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
        frontAngle = MAX_ANGLE;
        frontRadius = MAX_RADIUS;
        frontCloseRadius = MAX_RADIUS;
        speed = MAX_SPEED;
        agent.stoppingDistance = followingStopDistance;

        agent.speed = speed;
    }

    private void SetValuesToUnawareState()
    {
        frontAngle = baseFrontAngle;
        frontRadius = baseFrontRadius;
        frontCloseRadius = baseFrontCloseRadius;
        speed = baseSpeed;
        agent.stoppingDistance = patrollingStopDistance;

        agent.speed = speed;

    }

    private void AttackTarget()
    {
        GameObject.FindWithTag("Player").GetComponentInChildren<Player>().health -= 0.01f;
    }

    // Move head/vision point insted of the whole body. Animation aswell
    private void LookAround(bool searchRight)
    {
        Quaternion rotation;
        if (searchRight)
        {
            rotation = Quaternion.AngleAxis(frontAngle, transform.up);
        }
        else
        {
            rotation = Quaternion.AngleAxis(-frontAngle, transform.up);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    private void ShufflePatrolSpots()
    {
        for (int i = patrolSpots.Length - 1; 0 < i; i--)
        {
            int j = Random.Range(0, i);
            Transform l = patrolSpots[i], r = patrolSpots[j];
            patrolSpots[i] = r;
            patrolSpots[j] = l;
        }
    }

    private void GoToNearestPatrolSpot()
    {
        float minPatroldistance = Vector3.Distance(transform.position, patrolSpots[0].position);
        for (int index = 1; index < patrolSpots.Length; index++)
        {
            if (Vector3.Distance(transform.position, patrolSpots[index].position) < minPatroldistance)
            {
                spot = index;
            }
        }
    }

    private void TurnConer()
    {
        int nextSpot = spot + 1;
        Vector3 direction = (patrolSpots[(nextSpot % patrolSpots.Length)].position - transform.position).normalized;
        direction.y *= 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
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

    /* Methods for Game States bellow vvv*/

    public void Reset()
    {
        hasSeenPlayerShortRange = false;
        hasSeenPlayerLongRange = false;
        hasHeardSound = false;
        transform.position = patrolSpots[0].position;
        soundManager.ResetCultistCuriousStinger();
        soundManager.ResetCultistDetectedStinger();
        //AkSoundEngine.SetState("Enemy_State", "Normal");
    }

    // Call once Start Game on the main menu has been preseed
    public void MainMenuStartGamePressed()
    {
        if (clone == null)
        {
            clone = Instantiate(gameObject, new Vector3(-16, 2, 11), new Quaternion(0, 0, 0, 0));
            clone.transform.localScale = gameObject.transform.lossyScale;
            clone.GetComponent<CultistsAIManager>().clone = clone;
            clone.transform.LookAt(playerHead);
        }
    }

    // Call once the player had died in the Main Menu room
    public void PostPlayerDeathInMainMenuRoom()
    {
        if (clone != null && clone != gameObject)
        {
            Destroy(clone);
            clone = null;
        }
    }

    // Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(visionPoint.position, frontRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(visionPoint.position, frontCloseRadius);
        Gizmos.DrawWireSphere(visionPoint.position, backRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(visionPoint.position, MAX_HEARING_RADIUS);

        Vector3 fieldOfViewLine0 = Quaternion.AngleAxis(frontAngle, visionPoint.up) * visionPoint.forward * frontRadius;
        Vector3 fieldOfViewLine1 = Quaternion.AngleAxis(-frontAngle, visionPoint.up) * visionPoint.forward * frontRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(visionPoint.position, fieldOfViewLine0);
        Gizmos.DrawRay(visionPoint.position, fieldOfViewLine1);

        Vector3 fieldOfViewLine2 = Quaternion.AngleAxis(frontAngle, visionPoint.up) * -visionPoint.forward * backRadius;
        Vector3 fieldOfViewLine3 = Quaternion.AngleAxis(-frontAngle, visionPoint.up) * -visionPoint.forward * backRadius;

        Gizmos.DrawRay(visionPoint.position, fieldOfViewLine2);
        Gizmos.DrawRay(visionPoint.position, fieldOfViewLine3);

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
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(visionPoint.position, MAX_SHOUTING_RADIUS);

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