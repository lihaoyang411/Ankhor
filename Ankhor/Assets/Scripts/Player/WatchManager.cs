using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WatchManager : MonoBehaviour
{

    private Canvas UICanvas;
    private Image healthLabel;
    private Image staminaLabel;

    private const float MAXHealthStamina = 1.0f;

    private float health;
    private float stamina;


    // Start is called before the first frame update
    void Start() {
        health = MAXHealthStamina;
        stamina = MAXHealthStamina;

        InitSetup();
    }

    private void InitSetup()
    {
        UICanvas = GetComponentInChildren<Canvas>();

        Transform child, childChild;
        for (int index = 0; index < UICanvas.transform.childCount; index++) {
            child = UICanvas.transform.GetChild(index);

            for(int jndex = 0; jndex < child.childCount; jndex++) {
                childChild = child.GetChild(jndex);

                if(childChild.name.Equals("HealthImage")) {
                    healthLabel = childChild.GetComponent<Image>();
                } else if (childChild.name.Equals("StaminaImage")) {
                    staminaLabel = childChild.GetComponent<Image>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
        healthLabel.fillAmount = health;
        staminaLabel.fillAmount = stamina;
    }

    public void UpdateHealth(float health) {
        this.health = health;
    }

    public void UpdateStamina(float stamina) {
        this.stamina = stamina;
    }
}
