using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCultistAnimationManager : MonoBehaviour {
    private Animator animator;

    private CultistAnimationStates state;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponentInChildren<Animator>();
        state = CultistAnimationStates.SEARCHING;
    }

    // Update is called once per frame
    void Update() {
        AnimateState();
    }

    private void AnimateState() {

        switch (state) {
            case CultistAnimationStates.SEARCHING:
                animator.SetBool("isSearching", true);
                break;
            case CultistAnimationStates.WALKING:
                animator.SetBool("isSearching", false);
                animator.SetBool("isAttacking", false);
                break;
            case CultistAnimationStates.ATTAKING:
                animator.SetBool("isAttacking", true);
                break;
            default:
                animator.SetBool("isSearching", false);
                animator.SetBool("isAttacking", false);
                break;
        }
    }

    public void UpdateState(CultistAnimationStates state) {
        this.state = state;
    }
}
