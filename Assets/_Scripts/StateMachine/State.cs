using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{   
    protected Animator animator;
    protected StateMachine stateMachine;
    protected float startTime;
    protected Entity entity;
    string animBoolName;

    public delegate void ConditionUpdateDelegate();
    public ConditionUpdateDelegate conditionUpdateDelegate;

    public delegate void OnEnterStateDelegate();
    public OnEnterStateDelegate onEnterStateDelegate;

    public delegate void OnExitStateDelegate();
    public OnExitStateDelegate onExitStateDelegate;

    public State(ConditionUpdateDelegate func, OnEnterStateDelegate funcEnter, OnExitStateDelegate funcExit, Entity entity, GameObject entitygo, StateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.conditionUpdateDelegate = func;
        this.onEnterStateDelegate = funcEnter;
        this.onExitStateDelegate = funcExit;
        this.animator = entitygo.GetComponent<Animator>();
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void EnterState()
    {
        animator.SetBool(animBoolName, true);
        startTime = Time.time;
        onEnterStateDelegate();
    }

    public virtual void ExitState()
    {
        animator.SetBool(animBoolName, false);
        onExitStateDelegate();
    }

    public virtual void LogicUpdate()
    {
        conditionUpdateDelegate();
    }

    public virtual void PhysicsUpdate()
    {

    }
}
