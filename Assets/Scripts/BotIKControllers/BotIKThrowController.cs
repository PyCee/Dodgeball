using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotIKController))]
[RequireComponent(typeof(BotIKCatchController))]
[RequireComponent(typeof(BotIKDeflectController))]
[RequireComponent(typeof(Animator))]
public class BotIKThrowController : MonoBehaviour
{
    public Vector3 throwForce;

    private BotIKController ikController;
    private BotIKCatchController ikCatchController;
    private BotIKDeflectController ikDeflectController;
    private Animator animator;
    private Transform target;

	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand, AvatarIKGoal.LeftHand};
    void Start()
    {
        ikController = GetComponent<BotIKController>();
        ikCatchController = GetComponent<BotIKCatchController>();
        ikDeflectController = GetComponent<BotIKDeflectController>();
        animator = GetComponent<Animator>();
    }
    public void PrepareThrow(Transform target){
        ikController.setIKActive(false, enabledIKGoals);
        ikCatchController.setActive(false);
        ikDeflectController.setActive(false);

        // Parent ball to hand
        Transform rHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        ikCatchController.caughtBall.SetParent(rHand);

        this.target = target;
        ikController.setLookAt(this.target);
    }
    public void ReleaseBall(){
        // Release ball from bot
        ikCatchController.caughtBall.SetParent(null);
        Rigidbody rb = ikCatchController.caughtBall.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Apply throw force to the ball
        Quaternion towards = transform.rotation;
        if(target != null){
            Transform rHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            towards = Quaternion.LookRotation(target.position - rHand.position);
        }
        rb.AddForce(towards * throwForce, ForceMode.Impulse);

        ikCatchController.caughtBall = null;
    }
    public void PostThrow(){
        ikController.setIKActive(true, enabledIKGoals);
        ikCatchController.setActive(true);
        ikDeflectController.setActive(true);
    }
}