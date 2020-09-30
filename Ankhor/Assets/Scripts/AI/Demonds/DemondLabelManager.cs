using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DemondLabelManager : MonoBehaviour
{
    private Canvas statusCanvas;
    private Image suspiciousStatusLabel;
    private Image chasingStatusLabel;

    private DemondStatus status;

    // Start is called before the first frame update
    void Start() {
        InitSetup();
        status = DemondStatus.PATROLLING;
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
    void Update()
    {
        UpdateStatusLabel();
    }

    private void UpdateStatusLabel() {

        switch (status) {
            case DemondStatus.PATROLLING:
                suspiciousStatusLabel.fillAmount = 0.0f;
                chasingStatusLabel.fillAmount = 0.0f;
                break;
            case DemondStatus.SUSPICIOUS:
                suspiciousStatusLabel.fillAmount = 1.0f;
                chasingStatusLabel.fillAmount = 0.0f;
                break;
            case DemondStatus.CHASING:
                suspiciousStatusLabel.fillAmount = 0.0f;
                chasingStatusLabel.fillAmount = 1.0f;
                break;
        }
    }

    public void UpdateStatus(DemondStatus status)
    {
        this.status = status;
    }

}

public enum DemondStatus
{
    PATROLLING, SUSPICIOUS, CHASING
}
