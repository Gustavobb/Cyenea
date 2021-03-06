using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    public DeathState(ConditionUpdateDelegate func, OnEnterStateDelegate funcEnter, OnExitStateDelegate funcExit, Entity entity, GameObject entitygo, StateMachine stateMachine, string animBoolName) : base(func, funcEnter, funcExit, entity, entitygo, stateMachine, animBoolName) { }

    public override void EnterState() => base.EnterState();

    public override void ExitState() => base.ExitState();

    public override void LogicUpdate()
    {
    }

    public override void PhysicsUpdate() 
    {

    } 
}
