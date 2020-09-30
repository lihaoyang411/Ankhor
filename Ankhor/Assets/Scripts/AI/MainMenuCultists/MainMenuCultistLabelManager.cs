using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCultistLabelManager : MonoBehaviour
{

    private Canvas statusCanvas;
    private Image suspiciousStatusLabel;
    private Image chasingStatusLabel;

    private CultistStatus status;

    // Start is called before the first frame update
    void Start() {
        InitSetup();
        status = CultistStatus.SUSPICIOUS;
    }

    private void InitSetup() {
        statusCanvas = GetComponentInChildren<Canvas>();

        for (int index = 0; index < statusCanvas.transform.childCount; index++) {
            Transform child = statusCanvas.transform.GetChild(index);

            if (child.name.Equals("SuspiciousStatusLabel")) {
                suspiciousStatusLabel = child.GetComponent<Image>();
            } else if (child.name.Equals("ChasingStatusLabel")) {
                chasingStatusLabel = child.GetComponent<Image>();
            }
        }
    }

    // Update is called once per frame
    void Update() {
        UpdateStatusLabel();
    }

    private void UpdateStatusLabel() {

        switch (status)
        {
            case CultistStatus.SUSPICIOUS:
                statusCanvas.GetComponent<Canvas>().enabled = true;
                suspiciousStatusLabel.fillAmount = 1;
                chasingStatusLabel.fillAmount = 0;
                break;
            case CultistStatus.CHASING:
                statusCanvas.GetComponent<Canvas>().enabled = true;
                suspiciousStatusLabel.fillAmount = 0;
                chasingStatusLabel.fillAmount = 1;
                break;
            default:
                break;
        }
    }

    public void UpdateStatus(CultistStatus status) {
        this.status = status;
    }

}
