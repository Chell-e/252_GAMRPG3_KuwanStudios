//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ApproachState : EnemyState
//{
//    SpriteRenderer spriteRenderer;

//    public ApproachState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
//    {
//    }

//    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
//    {
//        base.AnimationTriggerEvent(triggerType);
//    }

//    public override void EnterState()
//    {
//        base.EnterState();

//        spriteRenderer = enemy.GetComponent<SpriteRenderer>();
//    }

//    public override void ExitState()
//    {
//        base.ExitState();
//    }

//    public override void FrameUpdateState()
//    {
//        base.FrameUpdateState();

//        if (PlayerController.Instance == null)
//            return;

//        // Flips sprite to always face the player
//        var playerPosition = PlayerController.Instance.transform.position;
//        spriteRenderer.flipX = playerPosition.x < enemy.transform.position.x;
//    }

//    public override void PhysicsUpdateState()
//    {
//        base.PhysicsUpdateState();
//    }
//}
