using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainMenuCultistsAIManager : MonoBehaviour
{
    private bool hasPlayerSeenAI = false;

    private Transform player;
    private NavMeshAgent agent;

    private MainMenuCultistLabelManager labelManager;
    private MainMenuCultistsSoundManager soundManager;
    private MainMenuCultistAnimationManager animationManager;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        agent = GetComponent<NavMeshAgent>();

        labelManager = GetComponent<MainMenuCultistLabelManager>();
        soundManager = GetComponent<MainMenuCultistsSoundManager>();
        animationManager = GetComponent<MainMenuCultistAnimationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ChaseTarget();
    }


    private void ChaseTarget()
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentPlayerPosition;

        if (hasPlayerSeenAI)
        {
            currentPlayerPosition = player.position;

            currentPosition.y *= 0;
            currentPlayerPosition.y *= 0;

            if (Vector3.Distance(currentPosition, currentPlayerPosition) < agent.stoppingDistance)
            {
                FaceTarget();
                AttackPlayer();
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y *= 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void AttackPlayer()
    {
        animationManager.UpdateState(CultistAnimationStates.ATTAKING);
        GameObject.FindWithTag("Player").GetComponentInChildren<Player>().health -= 0.01f;

    }

    public void StartChasingPlayer()
    {
        if (!hasPlayerSeenAI)
        {
            hasPlayerSeenAI = true;
            transform.LookAt(player);
            labelManager.UpdateStatus(CultistStatus.CHASING);
            soundManager.PlayCultistDetectedStinger();
            soundManager.PlayCultistFootstepRunSwitch();
            soundManager.PlayCultistChaseSound();
            animationManager.UpdateState(CultistAnimationStates.CHASING);

            GameObject[] mainMenuCultists = GameObject.FindGameObjectsWithTag("MainMenuCultist");

            foreach (GameObject mainMenuCultist in mainMenuCultists)
            {
                if (!mainMenuCultist.GetComponent<MainMenuCultistsAIManager>().hasPlayerSeenAI)
                {
                    mainMenuCultist.GetComponent<MainMenuCultistsAIManager>().StartChasingPlayer();
                }
            }
        }
    }
}
