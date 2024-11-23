using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private float wanderSpeed = 1.1f;     //Playback speed of "Wander" state clip
    private float searchSpeed = 1f;       //Playback speed of "Search" state clip
    private float chaseSpeed = 3f;        //Playback speed of "Chase" state clip
    private float attackSpeed = 1f;       //Playback speed of "Attack" state clip

    private Animation anim;               //Refernce to Animation component
    private State currentState;           //Refernce to current state

    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play("Wander");

        //Set up animation payback speeds
        anim["Wander"].speed = wanderSpeed;
        anim["Search"].speed = searchSpeed;
        anim["Attack"].speed = attackSpeed;
    }

    void Update()
    {
        currentState = GetComponentInParent<EnemyState>().state;

        //Wander and Seek states
        if (currentState == State.WANDER || currentState == State.SEEK)
        {
            if (!anim.IsPlaying("Wander"))
            {
                anim.CrossFade("Wander");
            }
            anim["Wander"].speed = wanderSpeed;

        }
        //Search state
        else if (!anim.IsPlaying("Search") && currentState == State.SEARCH)
        {
            anim.CrossFade("Search");
        }
        //Chase state
        else if (currentState == State.CHASE)
        {
            if (!anim.IsPlaying("Wander"))
            {
                anim.CrossFade("Wander");
            }
           anim["Wander"].speed = chaseSpeed;
            
        }
        //Attack state
        else if (!anim.IsPlaying("Attack") && currentState == State.ATTACK)
        {
            anim.CrossFade("Attack");
        }
    }
}
