using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistSoundManager : MonoBehaviour
{
    private Transform playerHead;

    private bool hasDetectedPlayer = false;
    private bool isInvestigating = false;

    private bool hasPostedDetectedStinger = false;
    private bool hasPostedCuriousStinger = false;
    private bool postedEnemyChaseEvent = false;
    
    // Start is called before the first frame update
    void Start() {
        playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
    }

    // Update is called once per frame
    void Update() {

    }

    public void UpdateCultistStates(bool hasDetectedPlayer, bool isInvestigating) {
        this.hasDetectedPlayer = hasDetectedPlayer;
        this.isInvestigating = isInvestigating;
    }

    public void PlayCultistDetectedStinger() {
        if (hasDetectedPlayer && !hasPostedDetectedStinger) {
            AkSoundEngine.PostEvent("DetectedStinger_Cultist", gameObject);
            hasPostedDetectedStinger = true;
        }
    }

    public void PlayCultistCuriousStinger() {
        if (hasDetectedPlayer && !hasPostedCuriousStinger) {
            AkSoundEngine.PostEvent("CuriousStinger_Cultist", gameObject);
            hasPostedCuriousStinger = true;
        }
    }

    public void PlayCultistFootstepSound(float currentSpeed, float MAX_SPEED) {
        if (currentSpeed < MAX_SPEED) {
            AkSoundEngine.SetSwitch("MoveType", "Walk_Switch", gameObject);
        } else {
            AkSoundEngine.SetSwitch("MoveType", "Run_Switch", gameObject);
        }

        float rtpc = Vector3.Distance(transform.position, playerHead.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);
    }

    // Maybe also set postedEnemyChaseEvent to true for cultists
    public void PlayEnemyChaseSound() {
        if (hasDetectedPlayer) {
            if(!postedEnemyChaseEvent) {
                AkSoundEngine.PostEvent("EnemyChase", gameObject);
                postedEnemyChaseEvent = true;

                GameObject[] cultists = GameObject.FindGameObjectsWithTag("Cultist");
            
                foreach(GameObject cultist in cultists) {
                    cultist.GetComponent<CultistSoundManager>().postedEnemyChaseEvent = true;
                }
            }

            AkSoundEngine.SetState("Enemy_State", "Chase");
        } else if (!isInvestigating) {
            // Cultist lost player, so it is no longer chasing
            AkSoundEngine.SetState("Enemy_State", "Normal");
        }
    }

    public void ResetCultistDetectedStinger() {
        hasDetectedPlayer = false;
    }

    public void ResetCultistCuriousStinger() {
        hasPostedCuriousStinger = false;
    }
}
