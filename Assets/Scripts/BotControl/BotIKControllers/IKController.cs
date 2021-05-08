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
	
	public IKLimbController rArm;
	public IKLimbController lArm;
	public IKLimbController rLeg;
	public IKLimbController lLeg;
	
	void Awake()
	{
		rArm = new IKLimbController(AvatarIKGoal.RightHand,
			handJointAngleOffset, handThickness, startReachDuration, endReachDuration);
		lArm = new IKLimbController(AvatarIKGoal.LeftHand,
			handJointAngleOffset, handThickness, startReachDuration, endReachDuration);
		rLeg = new IKLimbController(AvatarIKGoal.RightFoot,
			footJointAngleOffset, footThickness, startReachDuration, endReachDuration);
		lLeg = new IKLimbController(AvatarIKGoal.LeftFoot,
			footJointAngleOffset, footThickness, startReachDuration, endReachDuration);
	}
    void Start()
    {
        animator = GetComponent<Animator>();
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
	
}