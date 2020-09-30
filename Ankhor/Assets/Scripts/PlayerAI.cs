using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

using UnityEngine.UI;

public class PlayerAI : MonoBehaviour
{
    public AudioClip stinger;
    public AudioClip baby;
    public AudioSource[] audio;
    public Transform startLocation;
    public Transform cultistStartLocation;
    public GameObject VivePlayer;
    public GameObject Cultist;

    public float Health;
    public Image HealthBarImage;

    public bool seen = false;
    private bool musicFlip = false;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponents<AudioSource>();
        Health = 1.0f;
    }

    public void RestartGame()
    {
        Health = 1.0f;
        //GameObject.Find("FollowHead").GetComponent<AudioListener>().enabled = false;
        audio[0].Pause();
        audio[1].Play();
        seen = false;
        musicFlip = false;

        VivePlayer.transform.position = startLocation.position;
        Cultist.transform.position = cultistStartLocation.position;

        //SteamVR_LoadLevel.Begin(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Health);

        //HealthBarImage.fillAmount = Health;

        if (seen == true && musicFlip == false)
        {
            audio[0].PlayOneShot(stinger, 0.7f);
            audio[1].Pause();
            audio[0].Play();
            musicFlip = true;
        }
        if (seen == false && musicFlip == true)
        {
            audio[0].Pause();
            audio[1].Play();
            musicFlip = false;
        }

        if (Health <= 0)
        {
            RestartGame();
        }
    }
}
