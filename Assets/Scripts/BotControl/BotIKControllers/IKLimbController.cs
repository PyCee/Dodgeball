using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLimbController : Object
{
    private float jointOffsetAngle;
    private float endEffectorThickness;
    private float startReachDuration;
    private float endReachDuration;
    public bool ikActive;
    
    private AvatarIKGoal ikGoal;
    private HumanBodyBones baseJointBone;
    private HumanBodyBones endEffectorBone;
    private float reachProgress = 0.0f;
    private AvatarIKHint ikHint;
    private Vector3 ikHintPosition;
    
    public IKLimbController(AvatarIKGoal goal,
        float jointOffset, float effectorThickness, float startDuration,
        float endDuration){
        ikGoal = goal;
        switch(ikGoal){
            case AvatarIKGoal.RightHand:
                baseJointBone = HumanBodyBones.RightLowerArm;
                endEffectorBone = HumanBodyBones.RightHand;
                ikHint = AvatarIKHint.RightElbow;
                break;
            case AvatarIKGoal.LeftHand:
                baseJointBone = HumanBodyBones.LeftLowerArm;
                endEffectorBone = HumanBodyBones.LeftHand;
                ikHint = AvatarIKHint.LeftElbow;
                break;
            case AvatarIKGoal.RightFoot:
                baseJointBone = HumanBodyBones.RightUpperLeg;
                endEffectorBone = HumanBodyBones.RightFoot;
                ikHint = AvatarIKHint.RightKnee;
                break;
            case AvatarIKGoal.LeftFoot:
                baseJointBone = HumanBodyBones.LeftUpperLeg;
                endEffectorBone = HumanBodyBones.LeftFoot;
                ikHint = AvatarIKHint.LeftKnee;
                break;
            default:
                break;
        }
        jointOffsetAngle = jointOffset;
        endEffectorThickness = effectorThickness;
        startReachDuration = startDuration;
        endReachDuration = endDuration;
    }
    public void activeReset(){
        if(ikActive)
            reachProgress = 0.0f;
    }
    private float getBallGrabRange(Transform ball){
        return (ball.localScale.x * 0.5f) + endEffectorThickness;
    }
    public bool isLimbOnTarget(Animator animator, Transform ball){
        if(ikActive){
            Vector3 handPosition = animator.GetBoneTransform(endEffectorBone).position;
            float distanceHandBall = (handPosition - ball.position).magnitude;
            
            // The 0.01f is for fp-error
            bool inRange = distanceHandBall <= (getBallGrabRange(ball) + 0.01f);
            return (reachProgress == 1.0f) && inRange;
        }
        return true;
    }
    public void setIKHintPosition(Vector3 pos){
        ikHintPosition = pos;
    }
    public void updateIK(Animator animator, Transform ball){
        if(ball == null){
            return;
        }
        if(ikActive) {
            // LERP weight to reach over time
            reachProgress += Time.deltaTime / startReachDuration;
            reachProgress = Mathf.Min(reachProgress, 1.0f);
        } else {
            // LERP weight to end reach over time
            reachProgress -= Time.deltaTime / endReachDuration;
            reachProgress = Mathf.Max(reachProgress, 0.0f);
        }
        
        animator.SetIKPositionWeight(ikGoal, reachProgress);
        animator.SetIKRotationWeight(ikGoal, reachProgress);
        
        // Offset ik position by ball radius in direction of ball --> hand
        Transform boneTransform = animator.GetBoneTransform(baseJointBone);
        Vector3 offset = ball.position - boneTransform.position;
        offset = Vector3.Normalize(offset);
        
        Vector3 objectOffset = offset * getBallGrabRange(ball);
        Vector3 goalPosition = ball.position - objectOffset;

        // offsetRot is so the hand is rotated appropriately to hold the ball
        Quaternion offsetRot = Quaternion.AngleAxis(jointOffsetAngle, Vector3.right);
        Quaternion goalRotation = Quaternion.LookRotation(offset) * offsetRot;

        animator.SetIKPosition(ikGoal, goalPosition);
        animator.SetIKRotation(ikGoal, goalRotation);

        if(ikHintPosition == new Vector3(0.0f, 0.0f, 999.0f)){
            // Always false
            MonoBehaviour.print("This is to get rid of the warning when we don't use ikHintPosition");
            MonoBehaviour.print(ikHint);
        }
    }
}