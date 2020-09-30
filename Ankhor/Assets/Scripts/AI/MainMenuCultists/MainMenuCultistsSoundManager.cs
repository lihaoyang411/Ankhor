using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCultistsSoundManager : MonoBehaviour
{
    private Transform player;

    private bool postedEnemyChaseEvent = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHead").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCultistCuriousStinger()
    {
        AkSoundEngine.PostEvent("CuriousStinger_Cultist", gameObject);
    }

    public void PlayCultistDetectedStinger()
    {
        AkSoundEngine.PostEvent("DetectedStinger_Cultist", gameObject);
    }

    public void PlayCultistChaseSound()
    {
        if (!postedEnemyChaseEvent) {
            AkSoundEngine.PostEvent("EnemyChase", gameObject);
            postedEnemyChaseEvent = true;

            GameObject[] mainMenuCultists = GameObject.FindGameObjectsWithTag(this.tag);
            foreach (GameObject cultist in mainMenuCultists) {
                cultist.GetComponent<MainMenuCultistsSoundManager>().postedEnemyChaseEvent = true;
            }
        }

        AkSoundEngine.SetState("Enemy_State", "Chase");
    }

    public void StopCultistChaseSound()
    {
        AkSoundEngine.SetState("Enemy_State", "Normal");
    }


    public void PlayCultistFootstepRunSwitch()
    {
        AkSoundEngine.SetSwitch("MoveType", "Run_Switch", gameObject);
        float rtpc = Vector3.Distance(transform.position, player.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);
    }

    public void PlayCultistFootstepWalkSwitch()
    {
        AkSoundEngine.SetSwitch("MoveType", "Walk_Switch", gameObject);
        float rtpc = Vector3.Distance(transform.position, player.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);
    }

    public void PlayCultistFootstepIdleSwitch()
    {
        AkSoundEngine.SetSwitch("MoveType", "Idle_Switch", gameObject);
        float rtpc = Vector3.Distance(transform.position, player.position);
        AkSoundEngine.SetRTPCValue("Distance_PlayerEnemy_RTPC", rtpc);
    }

}
