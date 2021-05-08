using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(IKController))]
public class IKCatchController : MonoBehaviour
{
	public float stopSpeed;
	public Transform caughtBall = null;
	
	public float maxCatchConeAngle;
	
	// RANGE OF THE ARMS = 0.65f
	public float reach;
	public float reachAwareness;

	[Tooltip("The distance form the body that the ball should end up when caught")]
	public float minCatchDistance;

	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand, AvatarIKGoal.LeftHand};
	private IKController ikController;
	private int lastBallID = -1;
	private Animator animator;
	private bool catchActive = false;
	
	public bool HasCaughtBall(){
		return caughtBall != null;
	}
	public void setActive(bool active){
		catchActive = active;
	}
	private float GetMaxCatchDistance(){
		return reach + reachAwareness;
	}
	
    void Start()
    {
        animator = GetComponent<Animator>();
        ikController = GetComponent<IKController>();
		ikController.setIKActive(false, enabledIKGoals);
    }

    void FixedUpdate()
    {
		// TODO: when aiming at thigh, the ball will be caught, but ik is disabled. Fix
		if(catchActive && !HasCaughtBall()){
			// Scan for catchable ball
			ArrayList balls = FindNearbyBalls();
			if(balls.Count == 0){
				ikController.fullReset(enabledIKGoals);
				lastBallID = -1;
			} else {
				GameObject ball = (GameObject) balls[0];
				// If the bot is not holding a ball
				int newBallID = ball.GetInstanceID();
				if(newBallID != lastBallID){
					// If we selected a new ball, reset ik
					lastBallID = newBallID;
					ikController.setIKActive(true, enabledIKGoals);
					ikController.rightHandObj = ball.transform;
					ikController.leftHandObj = ball.transform;
					ikController.setLookAt(ball.transform);
					ikController.activeReset();
				}
				if(ikController.areLimbsOnTargets()){
					// If the hands are on the ball
					CatchBall(ball.transform);
				}
			}
		}
    }
	public void CatchBall(Transform ball){
		caughtBall = ball;
		
		Transform hipBone = animator.GetBoneTransform(HumanBodyBones.Hips);
		caughtBall.SetParent(hipBone);
		
		Rigidbody rb = ball.GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
	}
	public void ReleaseCaughtBall(){
		// Release ball from bot
        caughtBall.SetParent(null);
        Rigidbody rb = caughtBall.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        caughtBall = null;
	}
	
	public ArrayList FindNearbyBalls(){
		// returns an arraylist of gameobjects in order of ball catchableness
		ArrayList nearbyBalls = new ArrayList();
		foreach(GameObject gameObj in GameObject.FindGameObjectsWithTag("Ball")){
			float currCatchableness = GetBallCatchableness(gameObj.transform.position);
			
			//print("Ball:");
			//print("Position: " + gameObj.transform.position.ToString());
			//print("Catchableness: " + currCatchableness.ToString());\
			
			if(currCatchableness > 0.0f){
				bool added = false;
				for(int i = 0; i < nearbyBalls.Count; ++i){
					GameObject listBall = (GameObject) nearbyBalls[i];
					if(currCatchableness > GetBallCatchableness(listBall.transform.position)){
						nearbyBalls.Insert(i, gameObj);
						added = true;
						break;
					}
				}
				if(!added){
					nearbyBalls.Add(gameObj);
				}
			}
		}
		return nearbyBalls;
	}
	private float GetBallCatchableness (Vector3 ballPosition)
	{
		float catchableness = 0.0f;
		// Do math to see if ball is:
		//   in front of bot (find angle with vector math)
		//   is near enough
		//   is moving towards the bot
		
		Vector3 chestPosition = animator.GetBoneTransform(HumanBodyBones.Chest).position;
		Vector3 difference = ballPosition - chestPosition;
		float dot = Vector3.Dot(transform.forward, difference.normalized);
		if(dot < Mathf.Cos(maxCatchConeAngle * Mathf.Deg2Rad)){
			dot = 0.0f;
		}
		
		// TODO: mult the distanceViability to make this have a stronger effect in calculation
		float distance = (ballPosition - chestPosition).magnitude;
		float distanceViability = 1.0f - (distance / GetMaxCatchDistance());
		if(distanceViability < 0.0){
			distanceViability = 0.0f;
		}

		catchableness = dot * distanceViability;
		return catchableness;
	}
}
