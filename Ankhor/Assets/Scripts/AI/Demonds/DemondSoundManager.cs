using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemondSoundManager : MonoBehaviour
{
    private bool hasDetectedPlayer = false;
    private bool isInvestigating = false;

    private bool hasPostedDetectedStinger = false;
    private bool hasPostedCuriousStinger = false;
    private bool postedEnemyChaseEvent = false;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void UpdateDemondStates(bool hasDetectedPlayer, bool isInvestigating) {
        this.hasDetectedPlayer = hasDetectedPlayer;
        this.isInvestigating = isInvestigating;
    }

    public void PlayDemondDetectedStinger() {
        if (hasDetectedPlayer && !hasPostedDetectedStinger)
        {
            AkSoundEngine.PostEvent("DetectedStinger_Demond", gameObject);
            hasPostedDetectedStinger = true;
        }
    }

    public void PlayDemondCuriousStinger()
    {
        if (hasDetectedPlayer && !hasPostedCuriousStinger)
        {
            AkSoundEngine.PostEvent("CuriousStinger_Demond", gameObject);
            hasPostedCuriousStinger = true;
        }
    }

    // Maybe also set postedEnemyChaseEvent to true for cultists
    public void PlayEnemyChaseSound() {
        if (hasDetectedPlayer) {
            if (!postedEnemyChaseEvent) {
                AkSoundEngine.PostEvent("EnemyChase", gameObject);
                postedEnemyChaseEvent = true;

                GameObject[] demonds = GameObject.FindGameObjectsWithTag("Demond");

                foreach (GameObject demond in demonds) {
                    demond.GetComponent<DemondSoundManager>().postedEnemyChaseEvent = true;
                }

            }

            AkSoundEngine.SetState("Enemy_State", "Chase");
        } else if (!isInvestigating) {
            // Lost player, so it is no longer chasing
            AkSoundEngine.SetState("Enemy_State", "Normal");
        }
    }

    public void ResetDemondDetectedStinger() {
        hasDetectedPlayer = false;
    }

    public void ResetDemondCuriousStinger() {
        hasPostedCuriousStinger = false;
    }

}
