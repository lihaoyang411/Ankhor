using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.SetState("Menu_State", "inMainMenu");
        AkSoundEngine.SetSwitch("MenuMusic", "MenuMusicOn_Switch", gameObject);
        AkSoundEngine.PostEvent("Music_Menu", gameObject);
    }
}
