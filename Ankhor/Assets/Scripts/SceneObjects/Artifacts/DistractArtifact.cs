using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractArtifact : MonoBehaviour
{
    [Tooltip("If the artifact is active return it to the player, if not throw it and activate it's distraction")]
    public bool isActive = false;

    [Tooltip("If the artifact was thrown")]
    public bool isThrown = false;

    [Tooltip("The hand script that this gameobject is referencing to")]
    public GameObject Hand = null;

    [Tooltip("The hand that it is currently holding this game object")]
    public GameObject HandHoldingPos;

    [Tooltip("The sound radius for the cultist to get attracted to the object")]
    public float soundRadius = 3.0f;

    [Tooltip("The delay until the artifact activates")]
    public float ActivateDelay = 3.0f;

    [Tooltip("The durration of the artifact after it activates")]
    public float DistractTime = 5.0f;

    [Tooltip("The artifact needs to be taken the spirit in order to unlock it")]
    public bool isSealed = true;

    [Tooltip("The sound the artifact will produce")]
    public string ArtiactSoundName;

    private float CurrentDelayTimeRemaining;

    private float CurrentDistractTime;

    private bool BeginCountDown = false;

    private bool playedSFX = false;

    private GameObject[] cultists;

    private void Start()
    {
        isSealed = true;

        // AkSoundEngine.PostEvent(ArtiactSoundName, gameObject);

        if (GetComponent<Rigidbody>() == null)
        {
            Debug.Log("There is no ridgid body in " + gameObject.name + "!");
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = true;
        }

        CurrentDelayTimeRemaining = ActivateDelay;

        CurrentDistractTime = DistractTime;

        cultists = GameObject.FindGameObjectsWithTag("Cultist");
    }

    private void Update()
    {
        if (BeginCountDown)
        {
            CurrentDelayTimeRemaining -= Time.deltaTime;
        }

        if (CurrentDelayTimeRemaining <= 0)
        {
            BeginCountDown = false;

            CurrentDelayTimeRemaining = ActivateDelay;

            isActive = true;
        }

        // if its done distracting return it to hand
        if (CurrentDistractTime <= 0)
        {
            isActive = false;

            playedSFX = false;

            CurrentDistractTime = DistractTime;

            if (Hand != null)
            {
                Hand.GetComponent<PlayerHand>().returnDistractArtifact();
            }
        }

        if (isActive)
        {
            Distract();
        }
    }

    public void Activate(GameObject Hand)
    {
        this.gameObject.GetComponent<DistractArtifact>().Hand = Hand;

        // this.gameObject.GetComponent<DistractArtifact>().HandHoldingPos = Hand.GetComponent<PlayerHand>().HoldItemPos;

        if (!isSealed)
        {
            // when the gameobject is not active

            // activate it the distract artifact
            if (!isThrown && !isActive)
            {
                // throw it
                isThrown = true;

                // unparent it 
                transform.parent = null;
                Hand.GetComponent<PlayerHand>().holdingObject = false;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().AddForce(Hand.GetComponent<PlayerHand>().handForce * 5000);

                // call the distract function in the next _ seconds 
                BeginCountDown = true;

            }

            // if it is already active and the player hits the trigger again
            // allow them to bring it back 
            // restart everything 

            else if (isThrown && !isActive)
            {
                // stop counting down for the activation 
                // reset the current delay time 

                BeginCountDown = false;

                CurrentDelayTimeRemaining = DistractTime;

                Hand.GetComponent<PlayerHand>().returnDistractArtifact();

                isThrown = false;
            }
        }
    }

    public void Unseal()
    {
        gameObject.GetComponent<DistractArtifact>().isSealed = false;
    }
    

    private void Distract()
    {
        if (!playedSFX)
        {
            AkSoundEngine.PostEvent(ArtiactSoundName, gameObject);

            playedSFX = true;
        }

        RaycastHit hit;
        foreach (GameObject cultist in cultists)
        {
            if (Physics.Raycast(gameObject.transform.position, cultist.transform.position, out hit))
            {

                // New Code
                if (hit.collider.tag.Equals("Cultist"))
                {
                    Debug.LogWarning("Ray hits Cultist");
                    hit.collider.gameObject.GetComponent<CultistsAIManager>().InHearingRadiusCheck(soundRadius, gameObject.transform, true);
                }

                if (hit.collider.tag.Equals("Demond"))
                {
                    Debug.LogWarning("Ray hits Demond");
                    hit.collider.gameObject.GetComponent<DemondsAIManager>().InHearingRadiusCheck(soundRadius, gameObject.transform, true);
                }

                if (hit.collider.tag.Equals("Ankhor"))
                {
                    Debug.LogWarning("Ray hits Ankhor!!");
                    hit.collider.gameObject.GetComponent<AnkhorAIManager>().InHearingRadiusCheck(soundRadius, gameObject.transform, true);
                }

                //float distance = Vector3.Distance(Player.transform.position, cultist.transform.position);
                //if (hit.collider.tag == "Cultist" && distance <= soundRadius)
                //{
                //    //Debug.Log("A cultist can hear");
                //    //hit.collider.gameObject.GetComponent<EnemyBaseClass>()..HeardTargetSound(soundRadius)
                //}
            }
        }

        CurrentDistractTime -= Time.deltaTime;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, soundRadius);
    }

}
