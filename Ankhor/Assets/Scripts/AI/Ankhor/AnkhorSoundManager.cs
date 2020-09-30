using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkhorSoundManager : MonoBehaviour
{
    private Transform playerHead;

    private bool hasDetectedPlayer = false;
    private bool isInvestigating = false;

    private bool hasPostedDetectedStinger = false;
    private bool hasPostedCuriousStinger = false;
    private bool postedEnemyChaseEvent = false;

    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAnkhorStates(bool hasDetectedPlayer, bool isInvestigating) {
        this.hasDetectedPlayer = hasDetectedPlayer;
        this.isInvestigating = isInvestigating;
    }

    public void PlayAnkhorDetectedStinger() {
        if (hasDetectedPlayer && !hasPostedDetectedStinger) {
            AkSoundEngine.PostEvent("DetectedStinger_Ankhor", gameObject);
            hasPostedDetectedStinger = true;
        }
    }

    public void PlayAnkhorCuriousStinger() {
        if (hasDetectedPlayer && !hasPostedCuriousStinger) {
            AkSoundEngine.PostEvent("CuriousStinger_Ankhor", gameObject);
            hasPostedCuriousStinger = true;
        }
    }

    //public void PlayAnkhorFootstepSound(float currentSpeed, float MAX_SPEED) {
    //    if (currentSpeed < MAX_SPEED) {
    //        AkSoundEngine.SetSwitch("MoveType", "Walk_Switch", gameObject);
    //    } else {
    //        AkSoundEngine.SetSwitch("MoveType", "Run_Switch", gameObject);
    //    }

    //    float rtpc = Vector3.Distance(transform.position, playerHead.position);
    //    AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);
    //}

    // Maybe also set postedEnemyChaseEvent to true for cultists
    public void PlayEnemyChaseSound() {
        if (hasDetectedPlayer) {
            if (!postedEnemyChaseEvent) {
                AkSoundEngine.PostEvent("EnemyChase", gameObject);
                postedEnemyChaseEvent = true;

                //GameObject[] demonds = GameObject.FindGameObjectsWithTag("Demond");

                //foreach (GameObject demond in demonds)
                //{
                //    demond.GetComponent<DemondSoundManager>().postedEnemyChaseEvent = true;
                //}

            }

            AkSoundEngine.SetState("Enemy_State", "Chase");
        } else if (!isInvestigating) {
            //  lost player, so it is no longer chasing
            AkSoundEngine.SetState("Enemy_State", "Normal");
        }
    }

    public void ResetAnkhorDetectedStinger() {
        hasDetectedPlayer = false;
    }

    public void ResetAnkhorCuriousStinger()
    {
        hasPostedCuriousStinger = false;
    }
}
