using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKController))]
[RequireComponent(typeof(IKCatchController))]
[RequireComponent(typeof(IKDeflectController))]
[RequireComponent(typeof(IKThrowController))]
[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(Animator))]
public class IKThrowTestController : MonoBehaviour
{
    public GameObject ballPrefab;

    private IKController ikController;
    private IKCatchController ikCatchController;
    private IKDeflectController ikDeflectController;
    private IKThrowController ikThrowController;
    private PlayerCameraController playerCameraController;
    private Animator animator;
	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand, AvatarIKGoal.LeftHand};

    void Start()
    {
        ikController = GetComponent<IKController>();
        ikController.setIKActive(true, enabledIKGoals);
        
        ikCatchController = GetComponent<IKCatchController>();
        ikCatchController.setActive(true);

        ikDeflectController = GetComponent<IKDeflectController>();
        ikDeflectController.setActive(true);

        ikThrowController = GetComponent<IKThrowController>();

        playerCameraController = GetComponent<PlayerCameraController>();

        animator = GetComponent<Animator>();

        
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0) &&
            ikThrowController.GetThrowState() == IKThrowController.ThrowState.None &&
            ikCatchController.HasCaughtBall()){
            ikThrowController.StartAim();
            ikThrowController.SetThrowPathState(IKThrowController.ThrowPathState.Straight);
            playerCameraController.UseAimCamera();
        } else if(Input.GetMouseButtonUp(0) && ikThrowController.IsAiming()){
            // Throw a ball if the bot has one
            ikThrowController.ReadyThrow();
            playerCameraController.StopUsingAimCamera();
            animator.SetTrigger("Throw");
        }

        if(ikThrowController.IsAiming()){
            if(Input.GetKeyDown(KeyCode.A)){
                ikThrowController.MoveTargetLeft();
            }
            if(Input.GetKeyDown(KeyCode.D)){
                ikThrowController.MoveTargetRight();
            }
            if(Input.GetKeyDown(KeyCode.W)){
                //target = GetNextTarget();
            }
            if(Input.GetKeyDown(KeyCode.S)){
                //target = GetNextTarget(false);
            }
            if(Input.GetKeyDown(KeyCode.Q)){
                ikThrowController.SetThrowPathState(IKThrowController.ThrowPathState.Straight);
            }
            if(Input.GetKeyDown(KeyCode.E)){
                ikThrowController.SetThrowPathState(IKThrowController.ThrowPathState.CurveFromLeft);
            }
            if(Input.GetKeyDown(KeyCode.R)){
                ikThrowController.SetThrowPathState(IKThrowController.ThrowPathState.CurveFromAbove);
            }
            if(Input.GetKeyDown(KeyCode.F)){
                ikThrowController.SetThrowPathState(IKThrowController.ThrowPathState.CurveFromRight);
            }
        }

        if(Input.GetMouseButtonDown(1)){
            // Supply the bot with a new ball
            Vector3 ballPos = transform.position + transform.forward * 0.5f;
            ballPos.y += 1.5f;
			GameObject throwable = Instantiate(ballPrefab, ballPos, Quaternion.identity);
        }
    }
}
