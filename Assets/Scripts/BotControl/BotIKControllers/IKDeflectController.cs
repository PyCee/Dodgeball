using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKCatchController))]
[RequireComponent(typeof(Animator))]
public class IKDeflectController : MonoBehaviour
{
    private Animator animator;
	private IKCatchController ikCatchController;
	private bool deflectActive = false;

    public Vector3 defaultHoldOffset;
    public float holdDistance;
    public float ballMoveSpeed;
    public float closeAngle;
	
	public void setActive(bool active){deflectActive = active;}
	
    void Start()
    {
        animator = GetComponent<Animator>();
        ikCatchController = GetComponent<IKCatchController>();
    }
	
    void Update()
    {
        if(deflectActive && ikCatchController.hasCaughtBall()){
            // If deflect is active and catch controller has a ball

            Transform bone = animator.GetBoneTransform(HumanBodyBones.Chest);

            // Set chestOffset to defaultHoldOffset by default
            Vector3 chestOffset = transform.rotation * defaultHoldOffset;

            GameObject[] balls = (GameObject[])ikCatchController.findNearbyBalls().ToArray(typeof(GameObject));
            if((balls.Length >= 2)){
                // If there is a catchable ball

                for(int i = 1; i < balls.Length; ++i){

                    Rigidbody rb = balls[i].GetComponent<Rigidbody>();
                    Vector3 posDisplacementDir = bone.position - balls[1].transform.position;
                    posDisplacementDir = posDisplacementDir.normalized;
                    if(Vector3.Dot(posDisplacementDir, rb.velocity.normalized) > Mathf.Cos(closeAngle * Mathf.Deg2Rad)){
                        // If the ball is moving in a direction towards the bot
                        Transform ballToDeflect = balls[i].transform;

                        // Calculate target ball position from direction of the ball to deflect at holdDistance
                        chestOffset = -1.0f * posDisplacementDir * holdDistance;
                        break;
                    }
                }


            }

            Vector3 holdPosition = bone.position + chestOffset;

            // TODO: move ball to position over time (dotween?)
            Vector3 ballDisplacementDir = holdPosition - ikCatchController.caughtBall.position;
            Vector3 intermediatePosition = ikCatchController.caughtBall.position + ballDisplacementDir * ballMoveSpeed * Time.deltaTime;
            ikCatchController.caughtBall.position = intermediatePosition;
        }
    }
}
