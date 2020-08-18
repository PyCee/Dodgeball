using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKController))]
[RequireComponent(typeof(IKCatchController))]
[RequireComponent(typeof(IKDeflectController))]
[RequireComponent(typeof(IKThrowController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Animator))]
public class MovementTestController : MonoBehaviour
{
    public GameObject ballPrefab;
    private IKController ikController;
    private IKCatchController ikCatchController;
    private IKDeflectController ikDeflectController;
    private IKThrowController ikThrowController;
    private MovementController movementController;
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

        movementController = GetComponent<MovementController>();
        movementController.setActive(true);
        
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        
        if(Input.GetMouseButtonDown(0) && ikCatchController.hasCaughtBall()){
            ikThrowController.StartAim();
            
            
        } else if(Input.GetMouseButtonUp(0) && ikThrowController.IsAiming()){
            // Throw a ball if the bot has one
            ikThrowController.ReadyThrow();
            animator.SetTrigger("Throw");
        }
        if(Input.GetMouseButtonDown(1)){
            // Supply the bot with a new ball
            Vector3 ballPos = transform.position + transform.forward * 0.5f;
            ballPos.y += 1.5f;
			GameObject throwable = Instantiate(ballPrefab, ballPos, Quaternion.identity);
        }
    }
}
