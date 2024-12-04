using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpiderSound : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField]
    private AudioClip deathSound;          //Death audio file reference
    [SerializeField]
    private AudioClip attackSound;         //Attack audio file reference
    [SerializeField]
    private AudioClip chaseSound;          //Chase audio file reference
    [SerializeField]
    private float defaultVolume = 1f;      //Universal volume
    private AudioSource audioSource;       //Audio Source component reference

    private SimpleState currentState;
    private SimpleState newState;
    private SimpleState prevState;
    private bool isAlive;
    private bool wasAlive;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentState = GetComponent<SpiderState>().state;
        wasAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool isAlive = GetComponent<SpiderMovement>().IsAlive();
        //Death occurred
        if (isAlive != wasAlive)
        {
            wasAlive = isAlive;
            DeathSound();
            return;
        }

        newState = GetComponent<SpiderState>().state;

        //Change of states
        if (currentState != newState)
        {
            prevState = currentState;
            currentState = newState;

            //Chase state
            if (currentState == SimpleState.CHASE)
            {
                if (prevState == SimpleState.IDLE) 
                {
                    ChaseSound();
                }
            }

            //Attack state
            if (currentState == SimpleState.ATTACK)
            {
                AttackSound();
            }
        }
    }

    private void AttackSound()
    {
        audioSource.Pause();
        audioSource.clip = attackSound;
        audioSource.volume = defaultVolume;
        audioSource.loop = false;
        audioSource.Play();
    }

    private void ChaseSound()
    {
        audioSource.Pause();
        audioSource.clip = chaseSound;
        audioSource.volume = defaultVolume;
        audioSource.loop = false;
        audioSource.Play();
    }

    private void DeathSound()
    {
        audioSource.Pause();
        audioSource.clip = deathSound;
        audioSource.volume = defaultVolume;
        audioSource.loop = false;
        audioSource.Play();
    }
}
