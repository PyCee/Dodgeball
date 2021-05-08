using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(IKThrowController))]
public class DodgeController : MonoBehaviour
{
    public float ballIncomingRadius;

    public enum DodgeAction {
        StepRight,
        StepLeft,
        Duck,
        Jump
    }

    public void Dodge(DodgeAction dodgeAction)
    {
        if(GetIncomingBalls().Count > 0){
            switch(dodgeAction){
                case DodgeAction.StepRight:
                    print("step right");
                    break;
                case DodgeAction.StepLeft:
                    print("step left");
                    break;
                case DodgeAction.Duck:
                    print("duck");
                    break;
                case DodgeAction.Jump:
                    print("jump");
                    break;
                default:
                    break;
            }
        }
    }

    private ArrayList GetIncomingBalls()
    {
		ArrayList incomingBalls = new ArrayList();
		foreach(GameObject ball in GameObject.FindGameObjectsWithTag("Ball")){
            float distance = (transform.position - ball.transform.position).sqrMagnitude;
            // TODO: and if ball is moving towards player at speed above threshold
            if(distance < Mathf.Pow(ballIncomingRadius, 2)){
                incomingBalls.Add(ball);
            }
        }
        return incomingBalls;
    }
}
