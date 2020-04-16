using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BotIKController))]
public class BotIKCatchController : MonoBehaviour
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
	private BotIKController ikController;
	private int lastBallID = -1;
	private Animator animator;
	private bool catchActive = false;
	
	public bool hasCaughtBall(){
		return caughtBall != null;
	}
	public void setActive(bool active){
		catchActive = active;
	}
	private float getMaxCatchDistance(){
		return reach + reachAwareness;
	}
	
    void Start()
    {
        animator = GetComponent<Animator>();
        ikController = GetComponent<BotIKController>();
		ikController.setIKActive(false, enabledIKGoals);
    }

    void FixedUpdate()
    {
		// TODOTODOTODO: when aiming at thigh, the ball will be caught, but ik is disabled. Fix
		if(catchActive && !hasCaughtBall()){
			// Scan for catchable ball
			ArrayList balls = findNearbyBalls();
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
					Rigidbody rb = ball.GetComponent<Rigidbody>();
					
					// Slowing down the ball is slow, just catch it immediatly and apply impulse while controlling it
					caughtBall = ball.transform;
					
					Transform hipBone = animator.GetBoneTransform(HumanBodyBones.Hips);
					caughtBall.SetParent(hipBone);
					rb.velocity = Vector3.zero;
					rb.isKinematic = true;
					/*	
					if(rb.velocity.magnitude <= stopSpeed){
						// If ball is not moving, ball has been caught
						caughtBall = ball.transform;
						
						Transform hipBone = animator.GetBoneTransform(HumanBodyBones.Hips);
						caughtBall.SetParent(hipBone);
						rb.velocity = Vector3.zero;
						rb.isKinematic = true;
					} else{
						// Otherwise, slow it down
						Vector3 diff = ball.transform.position - transform.position;
						diff.y = 0.0f;
						float dist = diff.magnitude;
						float velocityMultiplier = (dist - minCatchDistance) / 
												(reach - minCatchDistance);
						if(dist < minCatchDistance){
							velocityMultiplier = 0.0f;
						}
						velocityMultiplier = Mathf.Min(1.0f, velocityMultiplier);
						velocityMultiplier = Mathf.Max(0.0f, velocityMultiplier);
						rb.velocity *= velocityMultiplier;
					}
					*/
				}
			}
		}
    }
	
	public ArrayList findNearbyBalls(){
		// returns an arraylist of gameobjects in order of ball catchableness
		ArrayList nearbyBalls = new ArrayList();
		foreach(GameObject gameObj in GameObject.FindGameObjectsWithTag("Ball")){
			float currCatchableness = getBallCatchableness(gameObj.transform.position);
			
			//print("Ball:");
			//print("Position: " + gameObj.transform.position.ToString());
			//print("Catchableness: " + currCatchableness.ToString());\
			
			if(currCatchableness > 0.0f){
				bool added = false;
				for(int i = 0; i < nearbyBalls.Count; ++i){
					GameObject listBall = (GameObject) nearbyBalls[i];
					if(currCatchableness > getBallCatchableness(listBall.transform.position)){
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
	private float getBallCatchableness (Vector3 ballPosition)
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
		float distanceViability = 1.0f - (distance / getMaxCatchDistance());
		if(distanceViability < 0.0){
			distanceViability = 0.0f;
		}

		catchableness = dot * distanceViability;
		return catchableness;
	}
}
