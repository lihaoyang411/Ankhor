using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AnkhorLabelManager : MonoBehaviour
{
    private Canvas statusCanvas;
    private Image suspiciousStatusLabel;
    private Image chasingStatusLabel;

    private AnkhorStatus status;

    // Start is called before the first frame update
    void Start()
    {
        InitSetup();
        status = AnkhorStatus.PATROLLING;
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
            case AnkhorStatus.PATROLLING:
                suspiciousStatusLabel.fillAmount = 0.0f;
                chasingStatusLabel.fillAmount = 0.0f;
                break;
            case AnkhorStatus.SUSPICIOUS:
                suspiciousStatusLabel.fillAmount = 1.0f;
                chasingStatusLabel.fillAmount = 0.0f;
                break;
            case AnkhorStatus.CHASING:
                suspiciousStatusLabel.fillAmount = 0.0f;
                chasingStatusLabel.fillAmount = 1.0f;
                break;
        }
    }

    public void UpdateStatus(AnkhorStatus status)
    {
        this.status = status;
    }
}

public enum AnkhorStatus
{
    PATROLLING, SUSPICIOUS, CHASING
}
