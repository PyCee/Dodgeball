using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKController : MonoBehaviour
{
	public Transform rightHandObj;
	public Transform leftHandObj;
	public Transform rightFootObj;
	public Transform leftFootObj;
	
	public float handThickness;
	public float handJointAngleOffset;
	public float footThickness;
	public float footJointAngleOffset;
	public float startReachDuration;
	public float endReachDuration;
	public Vector3 rightArmIKHint;
	public Vector3 leftArmIKHint;
	
	// TODO: better implement looking at objects
	private Transform lookAtObj = null;
	private float lookAtWeight;
	public void setLookAt(Transform look)
	{
		lookAtObj = look;
		lookAtWeight = 0.0f;
	}
	private Animator animator;
	
	public LimbIKController rArm;
	public LimbIKController lArm;
	public LimbIKController rLeg;
	public LimbIKController lLeg;
	
	void Awake()
	{
		rArm = new LimbIKController(AvatarIKGoal.RightHand,
			handJointAngleOffset, handThickness, startReachDuration, endReachDuration);
		lArm = new LimbIKController(AvatarIKGoal.LeftHand,
			handJointAngleOffset, handThickness, startReachDuration, endReachDuration);
		rLeg = new LimbIKController(AvatarIKGoal.RightFoot,
			footJointAngleOffset, footThickness, startReachDuration, endReachDuration);
		lLeg = new LimbIKController(AvatarIKGoal.LeftFoot,
			footJointAngleOffset, footThickness, startReachDuration, endReachDuration);
	}
    void Start()
    {
        animator = GetComponent<Animator>();
    }
	public void fullReset(AvatarIKGoal[] ikGoals = null)
	{
		setLookAt(null);
		setIKActive(false, ikGoals);
		foreach(AvatarIKGoal ikGoal in ikGoals)
		{
			switch(ikGoal){
				case AvatarIKGoal.RightHand:
					rightHandObj = null;
					break;
				case AvatarIKGoal.LeftHand:
					leftHandObj = null;
					break;
				case AvatarIKGoal.RightFoot:
					rightFootObj = null;
					break;
				case AvatarIKGoal.LeftFoot:
					leftFootObj = null;
					break;
				default:
					break;
			}
		}
	}
	public void activeReset()
	{
		rArm.activeReset();
		lArm.activeReset();
		rLeg.activeReset();
		lLeg.activeReset();
	}
	public bool areLimbsOnTargets()
	{
		return rArm.isLimbOnTarget(animator, rightHandObj) &&
			lArm.isLimbOnTarget(animator, leftHandObj) &&
			rLeg.isLimbOnTarget(animator, rightFootObj) &&
			lLeg.isLimbOnTarget(animator, leftFootObj);
	}
	public void setIKActive(bool active, AvatarIKGoal[] ikGoals = null)
	{
		if(ikGoals == null){
			rArm.ikActive = active;
			lArm.ikActive = active;
			rLeg.ikActive = active;
			lLeg.ikActive = active;
		} else{
			foreach(AvatarIKGoal ikGoal in ikGoals)
			{
				switch(ikGoal){
					case AvatarIKGoal.RightHand:
						rArm.ikActive = active;
						break;
					case AvatarIKGoal.LeftHand:
						lArm.ikActive = active;
						break;
					case AvatarIKGoal.RightFoot:
						rLeg.ikActive = active;
						break;
					case AvatarIKGoal.LeftFoot:
						lLeg.ikActive = active;
						break;
					default:
						break;
				}
			}
		}
	}
	void OnAnimatorIK(int layerIndex)
    {
		/*
		rightArmIKHint = transform.right * 0.5f;
		leftArmIKHint = transform.right * -1.0f * 0.5f;
		rArm.setIKHintPosition(rightArmIKHint);
		lArm.setIKHintPosition(leftArmIKHint);
		*/
		
		rArm.updateIK(animator, rightHandObj);
		lArm.updateIK(animator, leftHandObj);
		rLeg.updateIK(animator, rightFootObj);
		lLeg.updateIK(animator, leftFootObj);
		
		if(lookAtObj != null) {
			// TODO: do this better
			lookAtWeight += Time.deltaTime / startReachDuration;
			lookAtWeight = Mathf.Min(lookAtWeight, 1.0f);
			// TODO: use more than 1 parameter for SetLookAtWeight to aim body
            animator.SetLookAtWeight(lookAtWeight * 0.5f, lookAtWeight);
            animator.SetLookAtPosition(lookAtObj.position);
        } else{
			animator.SetLookAtWeight(0);
		}
    }
	public class LimbIKController
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
		
		public LimbIKController(AvatarIKGoal goal,
			float jointOffset, float effectorThickness, float startDuration,
			float endDuration)
		{
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
		public void activeReset()
		{
			if(ikActive)
				reachProgress = 0.0f;
		}
		public bool isLimbOnTarget(Animator animator, Transform ball)
		{
			if(ikActive){
				Vector3 handPosition = animator.GetBoneTransform(endEffectorBone).position;
				float distanceHandBall = (handPosition - ball.position).magnitude;
				
				// The 0.01f is for fp-error
				bool inRange = distanceHandBall <= (getBallGrabRange(ball) + 0.01f);
				return (reachProgress == 1.0f) && inRange;
			}
			return true;
		}
		public void setIKHintPosition(Vector3 pos)
		{
			ikHintPosition = pos;
		}
		public void updateIK(Animator animator, Transform ball)
		{
			if(ball != null){
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
				
				if (reachProgress != 0.0f){
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
						print("This is to get rid of the warning when we don't use ikHintPosition");
						print(ikHint);
					}
				}
			}
		}
		private float getBallGrabRange(Transform ball)
		{
			return (ball.localScale.x * 0.5f) + endEffectorThickness;
		}
	}
}