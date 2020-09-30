using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.XR;

public class EnemyBaseClass : MonoBehaviour
{

    // Start is called before the first frame update
    public Transform enemy;
    public Transform enemyVisionPoint;

    private Transform target;
    public Transform targetVR;
    public Transform targetNonVr;

    // Might contain the Vr objects, if pre-testing is successfull
    public Transform[] playerSteamVRObjects;
    // Might contain the Non Vr objects, if pre-testing is successfull
    public Transform[] playerNoSteamVRObjects;
    NavMeshAgent agen;

    public const float MAX_ANGLE = 180.0f;
    public const float MAX_RADIUS = 10.0f;
    public const float MAX_SPEED = 3.0f;

    public float frontAngle = 65.0f;
    public float frontRadius = 7.0f;
    public float frontCloseRadius = 3.0f;

    public float backRadius = 2.0f;

    private float baseFrontAngle;
    private float baseFrontRadius;
    private float baseSpeed;
    private float patrollingStopDistance = 0.5f;
    private const float followingStopDistance = 1.5f;

    public Transform[] patrolSpots;
    private int spot = 0;

    //public Transform hideSpot;

    public float speed = 2.0f;
    public float waitTime = 3.0f;
    private float startWaitTime;
    private bool searched = false;

    private Vector3 lastSeenPosition;
    private bool wasInFieldOfView = false;
    private bool isInFieldOfView = false;

    private const float MAX_SHOUTING_RADIUS = 10.0f;
    private const float MAX_HEARING_RADIUS = 8.0f;

    private bool usingVR = false;

    private string otherTag;

    private bool hasPlayedGrunt = false;
    private bool hasSwitchedToWalk = false;

    private GameObject clone = null;

    void Start()
    {
        baseFrontAngle = frontAngle;
        baseFrontRadius = frontRadius;
        baseSpeed = speed;

        startWaitTime = waitTime;
        agen = GetComponent<NavMeshAgent>();

        usingVR = IsUsingVR();

        // if player != spirit form
        //Debug.Log("isSpirit: " + GameObject.FindWithTag("Player").GetComponent<Player>().isSpirit);

        if (!GameObject.FindWithTag("Player").GetComponent<Player>().isSpirit
            && this.tag.Equals("SpiritEnemy"))
        {
            //Debug.Log("Disabling Spirit");


            this.gameObject.SetActive(false);
            otherTag = "Cultist";
        }

        if (GameObject.FindWithTag("Player").GetComponent<Player>().isSpirit
            && this.tag.Equals("Cultist"))
        {
            //Debug.Log("Disabling Cult");

            this.gameObject.SetActive(false);
            otherTag = "SpiritEnemy";
        }

        if(this.tag.Equals("MainMenuCultist")) {
            this.gameObject.SetActive(false);
        }

        if (usingVR) {
            target = targetVR;
        } else {
            target = targetNonVr;
        }

        AkSoundEngine.SetState("Enemy_State", "Normal");
        // AkSoundEngine.SetRTPCValue("RTCP", 0);
        AkSoundEngine.PostEvent("EnemyChase", gameObject);

    }

    // Update is called once per frame
    void Update()
    {

        // if VR
        if (usingVR)
        {
            isInFieldOfView = InFieldOfView(enemyVisionPoint, playerSteamVRObjects[0], frontAngle, frontRadius, true);
            if (!isInFieldOfView)
            {
                isInFieldOfView = InFieldOfView(enemyVisionPoint, playerSteamVRObjects, frontAngle, frontCloseRadius, true);

                if (!isInFieldOfView)
                {
                    isInFieldOfView = InFieldOfView(enemyVisionPoint, playerSteamVRObjects, frontAngle, backRadius, false);
                }
            }
        }
        else
        {
            isInFieldOfView = InFieldOfView(enemyVisionPoint, playerNoSteamVRObjects, frontAngle, frontRadius, true);
            if (!isInFieldOfView)
            {
                isInFieldOfView = InFieldOfView(enemyVisionPoint, playerNoSteamVRObjects, frontAngle, backRadius, false);
            }
        }

        // Debug.Log(this.name + " is in Field Of View: " + isInFieldOfView);
        if(isInFieldOfView && !wasInFieldOfView) {
            hasSwitchedToWalk = false;
            Debug.LogWarning("Will be switched to run");
        }
        PlayEnemyFootstepsSound();

        if (isInFieldOfView)
        {
            // Debug.Log(this.name +  " In Field Of View");


            PlayEnemyGruntSound();
            ChaseTarget();
            CallBackup();
        } else if (wasInFieldOfView) {
            // Debug.Log(this.name +  " Was in Field Of View");
            SearchForRecentlyLostTarget();
        } else {
            Patrol();
        }

        PlayEnemyFootstepSound();
        //PlayEnemyChasingSound();
        PlayEnemyChaseSound();

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemyVisionPoint.position, frontRadius);
        Gizmos.DrawWireSphere(enemyVisionPoint.position, frontCloseRadius);
        Gizmos.DrawWireSphere(enemyVisionPoint.position, backRadius);

        Gizmos.color = Color.white;

        Vector3 fieldOfViewLine0 = Quaternion.AngleAxis(frontAngle, enemyVisionPoint.up) * enemyVisionPoint.forward * frontRadius;
        Vector3 fieldOfViewLine1 = Quaternion.AngleAxis(-frontAngle, enemyVisionPoint.up) * enemyVisionPoint.forward * frontRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(enemyVisionPoint.position, fieldOfViewLine0);
        Gizmos.DrawRay(enemyVisionPoint.position, fieldOfViewLine1);

        Vector3 fieldOfViewLine2 = Quaternion.AngleAxis(frontAngle, enemyVisionPoint.up) * -enemyVisionPoint.forward * backRadius;
        Vector3 fieldOfViewLine3 = Quaternion.AngleAxis(-frontAngle, enemyVisionPoint.up) * -enemyVisionPoint.forward * backRadius;

        Gizmos.DrawRay(enemyVisionPoint.position, fieldOfViewLine2);
        Gizmos.DrawRay(enemyVisionPoint.position, fieldOfViewLine3);

        if (isInFieldOfView)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(enemyVisionPoint.position, MAX_SHOUTING_RADIUS);

            Gizmos.color = Color.green;
        }

        if (usingVR)
        {
            Gizmos.DrawRay(enemyVisionPoint.position, (playerSteamVRObjects[0].position - enemyVisionPoint.position).normalized * frontRadius);

            for (int i = 1; i < playerSteamVRObjects.Length; i++)
            {
                Gizmos.DrawRay(enemyVisionPoint.position, (playerSteamVRObjects[i].position - enemyVisionPoint.position).normalized * frontCloseRadius);
            }
        }
        else
        {
            for (int j = 0; j < playerNoSteamVRObjects.Length; j++)
            {
                Gizmos.DrawRay(enemyVisionPoint.position, (playerNoSteamVRObjects[j].position - enemyVisionPoint.position).normalized * frontRadius);
            }
        }

        Gizmos.color = Color.black;
        Gizmos.DrawRay(enemyVisionPoint.position, enemyVisionPoint.forward * frontRadius);

    }

    private bool InFieldOfView(Transform visionPoint, Transform[] targetPlayerObjects, float maxAngle, float maxRadius, bool isFront)
    {
        Collider[] colliders = new Collider[500];
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);
        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = false;
        }

        int count = Physics.OverlapSphereNonAlloc(visionPoint.position, maxRadius, colliders);

        for (int i = 0; i < count; i++)
        {
            if (colliders[i] != null)
            {

                //Debug.Log("Colider Tag: " + colliders[i].tag);

                for (int j = 0; j < targetPlayerObjects.Length; j++)
                {
                    if (colliders[i].tag.Equals(targetPlayerObjects[j].tag))//
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
                                    for (int index = 0; index < enemies.Length; index++)
                                    {
                                        enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
        }

        return false;
    }

    private bool InFieldOfView(Transform visionPoint, Transform targetPlayerObject, float maxAngle, float maxRadius, bool isFront)
    {
        Collider[] colliders = new Collider[500];

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);
        for(int index = 0; index < enemies.Length; index++) {
            enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = false;
        }

        int count = Physics.OverlapSphereNonAlloc(visionPoint.position, maxRadius, colliders);

        for (int i = 0; i < count; i++)
        {
            if (colliders[i] != null)
            {

                //Debug.Log("Colider Tag: " + colliders[i].tag);

                if (colliders[i].tag.Equals(targetPlayerObject.tag))//
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

                                for (int index = 0; index < enemies.Length; index++)
                                {
                                    enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
                                }

                                return true;
                            }
                        }
                    }
                }
            }
        }

        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
        }

        return false;
    }

    private void Patrol()
    {
        // Not as aware.
        frontAngle = baseFrontAngle;
        frontRadius = baseFrontRadius;
        speed = baseSpeed;
        agen.stoppingDistance = patrollingStopDistance;
        // hasSwitchedToWalk = false;

        agen.SetDestination(patrolSpots[spot].position);
        agen.speed = speed;

        Vector3 currentPosition = transform.position;
        Vector3 currentPatrolPosition = patrolSpots[spot].position;

        currentPosition.y *= 0;
        currentPatrolPosition.y *= 0;

        if (Vector3.Distance(currentPosition, currentPatrolPosition) <= agen.stoppingDistance)
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

                float splitTime = startWaitTime / 3.0f;

                if ((splitTime * 2.0f) <= waitTime && !searched)
                {
                    waitTime -= Time.deltaTime;
                    LookAround(true);
                }
                else if (0.0f < waitTime && !searched)
                {
                    waitTime -= Time.deltaTime;
                    LookAround(false);
                    if (waitTime <= 0.0f)
                    {
                        waitTime = startWaitTime;
                        searched = true;
                    }
                }
                else
                {
                    waitTime -= Time.deltaTime;
                    TurnConer();
                }
            }
        }
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
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y *= 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void ChaseTarget()
    {
        // Increase Awarness
        frontAngle = MAX_ANGLE;
        frontRadius = MAX_RADIUS;
        speed = MAX_SPEED;
        agen.stoppingDistance = followingStopDistance;

        agen.SetDestination(target.position);
        agen.speed = speed;

        Vector3 currentPosition = transform.position;
        Vector3 currentTargetPosition = target.position;

        currentPosition.y *= 0;
        currentTargetPosition.y *= 0;

        if (Vector3.Distance(currentPosition, currentTargetPosition) <= agen.stoppingDistance)
        {

            FaceTarget();
            AttackTarget();
        }

        wasInFieldOfView = isInFieldOfView;
    }

    private void SearchForRecentlyLostTarget()
    {
        agen.SetDestination(lastSeenPosition);

        Vector3 currentPosition = transform.position;
        Vector3 lastSeenTargetPosition = lastSeenPosition;

        currentPosition.y *= 0;
        lastSeenTargetPosition.y *= 0;

        // Turning needs to be fine tunned so that it does not turn too quick
        if (Vector3.Distance(currentPosition, lastSeenTargetPosition) <= agen.stoppingDistance)
        {

            float splitTime = startWaitTime / 3.0f;

            if ((splitTime * 2.0f) <= waitTime)
            {
                waitTime -= Time.deltaTime;
                LookAround(true);
            }
            else if (0.0f <= waitTime)
            {
                waitTime -= Time.deltaTime;
                LookAround(false);
            }
            else
            {
                wasInFieldOfView = false;
                hasPlayedGrunt = false;
                hasSwitchedToWalk = false;
                Debug.LogWarning("Will be switched to walk soon");
                waitTime = startWaitTime;
                GoToNearestPatrolSpot();
            }
        }
    }

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

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
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

    private void AttackTarget()
    {
        GameObject.FindWithTag("Player").GetComponentInChildren<Player>().health -= 0.01f;
    }

    private void CallBackup()
    {

        // Debug.Log(this.name + " is calling backup");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (transform != enemies[i].transform)
            {
                enemies[i].GetComponentInChildren<EnemyBaseClass>().HearBackupCall(enemyVisionPoint);
            }
        }
    }

    public void HearBackupCall(Transform otherTeammate)
    {
        float distance = Vector3.Distance(otherTeammate.position, enemyVisionPoint.position);
        if (distance <= MAX_SHOUTING_RADIUS || distance <= (MAX_SHOUTING_RADIUS + MAX_HEARING_RADIUS))
        {

            if(!isInFieldOfView) { 
                wasInFieldOfView = true;
                lastSeenPosition = target.position;
                agen.stoppingDistance = followingStopDistance;
            } else if(isInFieldOfView && wasInFieldOfView && !hasPlayedGrunt) {
                PlayEnemyGruntSound();
            }
            // In hearing radius, can go assist and chase target
            // Maybe don't increase awarness till this enemy sees it
            //Debug.Log(this.name + ": Hear, and is now Chasing and bools are: (" + isInFieldOfView + "," + wasInFieldOfView + ")");
            //if(isInFieldOfView) {
            //    wasInFieldOfView = false;
            //    PlayEnemyGruntSound();
            //}

            ////ChaseTarget();
            //Debug.Log(this.name + ": Hear, and is now Chasing and bools are: (" + isInFieldOfView + "," +wasInFieldOfView + ")");
        }
    }

    // Test!!! Might only happen for a frame...
    public bool HeardTargetSound(float soundRadius)
    {
        float distance = Vector3.Distance(target.position, enemyVisionPoint.position);
        if (distance <= soundRadius || (distance - MAX_HEARING_RADIUS) <= soundRadius)
        {
            // Heard weird sound, will go investigate
            lastSeenPosition = target.position;
            SearchForRecentlyLostTarget();
        }

        return false;
    }

    public void PlayerChangedForm()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag);

        // diable all of them and hide AI
        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].SetActive(false);
        }

        enemies = GameObject.FindGameObjectsWithTag(otherTag);

        // enable other AI
        for (int index = 0; index < enemies.Length; index++)
        {
            enemies[index].SetActive(true);
            enemies[index].transform.position = patrolSpots[0].position;
        }
    }

    public void Reset() {
        isInFieldOfView = false;
        wasInFieldOfView = false;
        transform.position = patrolSpots[0].position;
        AkSoundEngine.SetState("Enemy_State", "Normal");
    }

    public void MainMenuStartGamePressed() {

        if(clone == null) {
            clone = Instantiate(gameObject, new Vector3(-13, 2, 10), new Quaternion(0, 0, 0, 0));
            clone.transform.localScale = gameObject.transform.lossyScale;
            clone.GetComponent<EnemyBaseClass>().clone = clone;
        }

    }

    public void PostPlayerDeathInMainMenuRoom() {
        if(clone != null && clone != gameObject) {
            Destroy(clone);
            clone = null;
        }

    }

    /* 
     * System Settings for Debuging
    */

    private bool IsUsingVR()
    {
        for (int i = 0; i < XRSettings.supportedDevices.Length; i++)
        {
            if (XRSettings.supportedDevices[i].Equals(XRSettings.loadedDeviceName))
            {
                return true;
            }
        }

        return false;
    }



    /*
     * Enemey Sounds are below
    */

    private void PlayEnemyGruntSound()
    {
        if (isInFieldOfView && !wasInFieldOfView && !hasPlayedGrunt)
        {
            AkSoundEngine.PostEvent("CultGrunt_1", gameObject);
            hasPlayedGrunt = true;
        } else if (isInFieldOfView && wasInFieldOfView && !hasPlayedGrunt) {
            AkSoundEngine.PostEvent("CultGrunt_1", gameObject);
            hasPlayedGrunt = true;
        }

    }

    private void PlayEnemyFootstepsSound() {
        if (isInFieldOfView && !wasInFieldOfView && !hasSwitchedToWalk) {
            AkSoundEngine.SetSwitch("MoveType", "Run_Switch", gameObject);
            AkSoundEngine.PostEvent("Footsteps_1", gameObject);
            Debug.LogWarning(this.name + ": Setting back to Run");
            hasSwitchedToWalk = true;
        } else if (!isInFieldOfView && !wasInFieldOfView && !hasSwitchedToWalk) {
            AkSoundEngine.SetSwitch("MoveType", "Walk_Switch", gameObject);
            // AkSoundEngine.PostEvent("Footsteps_1", gameObject);
            Debug.LogWarning(this.name + ": Setting back to Walk");
            hasSwitchedToWalk = true;
        }

        float rtpc = Vector3.Distance(transform.position, target.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);

    }

    private void PlayEnemyChaseSound() { 
        // bool haveOtherEnemiesSeenPlayer???
        if(isInFieldOfView) {
            AkSoundEngine.SetState("Enemy_State", "Chase");
        } else if (!wasInFieldOfView) {
            AkSoundEngine.SetState("Enemy_State", "Normal");
        }
    }

    private void PlayEnemyFootstepSound() { 
        if(isInFieldOfView || wasInFieldOfView) {
            //AkSoundEngine.SetSwitch("MoveType", "Run_Switch", gameObject);
            //AkSoundEngine.PostEvent("FootStepsRun_1", gameObject);
        }
        else {
            //AkSoundEngine.SetSwitch("MoveType", "Walk_Switch", gameObject);
            //AkSoundEngine.PostEvent("FootStepsWalk_1", gameObject);
        }

        //AkSoundEngine.PostEvent("MoveType", gameObject);
    }

}