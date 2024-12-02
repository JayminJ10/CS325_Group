using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAnimation : MonoBehaviour
{
    private Animation anim;               //Refernce to Animation component
    private SimpleState currentState;           //Refernce to current state

    
    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        currentState = GetComponentInParent<SpiderState>().state;
        
        //Let attack animation finish
        if (anim.IsPlaying("Attack")) { return; }
        //Let death animation finish
        if (anim.IsPlaying("Death")) { return; }

        //Death state
        if (!GetComponentInParent<SpiderMovement>().IsAlive() )
        {
            if (!anim.IsPlaying("Death"))
            {
                anim.Play("Death");
            }
        }

        //Idle states
        else if (currentState == SimpleState.IDLE)
        {
            //Let attack animation finish
            //if (anim.IsPlaying("Attack")) { return; }
            if (!anim.IsPlaying("Idle"))
            {
                anim.CrossFade("Idle");
            }

        }
        //Idle states
        else if (currentState == SimpleState.CHASE)
        {
            //Let attack animation finish
            //if (anim.IsPlaying("Attack")) { return; }
            if (!anim.IsPlaying("Chase"))
            {
                anim.CrossFade("Chase");
            }

        }

        else if (currentState == SimpleState.ATTACK)
        {
            if (!anim.IsPlaying("Attack"))
            {
                anim.Play("Attack");
            }
        }

    }
}
