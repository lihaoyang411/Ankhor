using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private bool hasPostedHeathbeatEvent = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHeatbeatSound(float health) { 
        if(health <= 0.25f) { 
            if(!hasPostedHeathbeatEvent) {
                AkSoundEngine.PostEvent("Heartbeat", gameObject);
                hasPostedHeathbeatEvent = true;
            }
            AkSoundEngine.SetState("Player_State", "Danger");
        }
        else {
            AkSoundEngine.SetState("Player_State", "Normal");
        }
    }
}
