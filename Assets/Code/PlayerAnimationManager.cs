﻿//------------------------------------------------------------------------------
//
// File Name:	PlayerAnimationManager.cs
// Author(s):	Jeremy Kings (j.kings) - Unity Project
//              Nathan Mueller - original Zero Engine project
// Project:		Endless Runner
// Course:		WANIC VGP
//
// Copyright © 2021 DigiPen (USA) Corporation.
//
//------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationStates
{
    Run,
    Jump,
    Slam,
    Slide,
    Attack,
    Hurt
};

public class PlayerAnimationManager : MonoBehaviour
{   
    [Tooltip("Invulnerable time")]
    public float InvulnTime = 0.25f;
    [Tooltip("The current amount of time in attack animation")]
    public float TimeInAttack = 0.7f;
    [Tooltip("Current state")]
    public PlayerAnimationStates CurrentState = PlayerAnimationStates.Run;
    [Tooltip("Last state")]
    public PlayerAnimationStates PreviousState = PlayerAnimationStates.Run;
    [Tooltip("The current amount of time invonerable (for scripts)")]
    public float CurrInvulnTime = 0;

    Animator animator;
    float AttackCooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        SwitchTo(CurrentState);
        animator = GetComponent<Animator>();
        animator.Play("Run");
    }

    // Update is called once per frame
    void Update()
    {
        // Override the animation that should be happening if we are hurt
        CurrInvulnTime -= Time.deltaTime;
        AttackCooldownTimer -= Time.deltaTime;
        if (CurrInvulnTime > 0 && CurrentState != PlayerAnimationStates.Hurt)
        {
            SwitchTo(PlayerAnimationStates.Hurt);
        }
        // Switch back to the correct animation if the player is done with the hurt animation.
        if (CurrInvulnTime <= 0 && CurrentState == PlayerAnimationStates.Hurt)
        {
            SwitchTo(PreviousState);
        }

        PlayCurrentAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.otherCollider.gameObject.CompareTag("Floor"))
        {
            SwitchTo(PlayerAnimationStates.Run);
        }
    }

    public void SwitchTo(PlayerAnimationStates state)
    {
        // Ignore any animation changes if the player was hurt, we are playing that animation.
        if (CurrInvulnTime > 0)
        {
            return;
        }
      
        if (CurrentState != PlayerAnimationStates.Hurt && state == PlayerAnimationStates.Hurt)
        {
            CurrInvulnTime = InvulnTime;
        }

        //ignore if attacking
        if (AttackCooldownTimer >= 0 && state != PlayerAnimationStates.Hurt)
        {
            return;
        }

        if (CurrentState != PlayerAnimationStates.Attack && state == PlayerAnimationStates.Attack && state != PlayerAnimationStates.Hurt)
        {
            AttackCooldownTimer = TimeInAttack;
        }

        PreviousState = CurrentState;
        CurrentState = state;
    }

    public void PlayCurrentAnimation()
    {
        // Don't bother if we haven't
        // switched states recently
        if (CurrentState == PreviousState)
            return;

        // Debug log for testing purposes - feel free to remove
        //Debug.Log("PrevState: " + PreviousState.ToString()
            //+ ", CurrentState: " + CurrentState.ToString());

        // Play state based on value of CurrentState
        switch (CurrentState)
        {
            case PlayerAnimationStates.Run:
                animator.Play("Run");
                break;
            case PlayerAnimationStates.Jump:
                animator.Play("Jump");
                break;
            case PlayerAnimationStates.Slam:
                animator.Play("Slam");
                break;
            case PlayerAnimationStates.Slide:
                animator.Play("Slide");
                break;
            case PlayerAnimationStates.Attack:
                animator.Play("Attack");
                break;
            case PlayerAnimationStates.Hurt:
                animator.Play("Hurt");
                break;
        }
    }
}
