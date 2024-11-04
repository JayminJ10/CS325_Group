using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField]
    private AudioClip wanderSound;         //Wander audio file reference
    [SerializeField]
    private AudioClip screechSound;        //Screech audio file reference
    [SerializeField]
    private AudioClip chaseSound;          //Chase audio file reference
    [SerializeField]
    private float defaultVolume = 0.7f;    //Universal volume
    private AudioSource audioSource;       //Audio Source component reference

    private State currentState;            
    private State newState;
    private bool playedScreech;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentState = GetComponent<EnemyState>().state;
        playedScreech = false;
    }

    void Update()
    {
        newState = GetComponent<EnemyState>().state;

        //Change sounds if state is different
        if (currentState != newState)
        {
            currentState = newState;
            //Handle nonaggro states
            if (currentState == State.WANDER || 
                currentState == State.SEARCH ||
                currentState == State.SEEK)
            {
                playedScreech = false;
                WanderSound();
            }

            //Handle aggro states
            if (currentState == State.CHASE || currentState == State.ATTACK)
            {
                //Play screech only at beginning of chase
                if (!playedScreech)
                {
                    playedScreech = true;
                    ScreechSound();
                }
            }
        }

        //Play chase sounds if screech sound is over
        if (playedScreech && !audioSource.isPlaying)
        {
            ChaseSound();
        }
    }

    //Update audio source settings to handle wander audio
    void WanderSound()
    {
        audioSource.Pause();
        audioSource.clip = wanderSound;
        audioSource.volume = defaultVolume;
        audioSource.loop = true;
        audioSource.Play();
    }

    //Update audio source settings to handle screech audio
    void ScreechSound()
    {
        audioSource.Pause();
        audioSource.clip = screechSound;
        audioSource.volume = defaultVolume * 0.75f;
        audioSource.loop = false;
        audioSource.Play();
    }

    //Update audio source settings to handle chase audio
    void ChaseSound()
    {
        audioSource.Pause();
        audioSource.clip = chaseSound;
        audioSource.volume = defaultVolume;
        audioSource.loop = true;
        audioSource.Play();
    }
}
