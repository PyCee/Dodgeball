using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotIKController))]
public class BotIKTestController : MonoBehaviour
{
	public Transform rightHandBall;
	public Transform leftFootBall;

	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand, AvatarIKGoal.LeftFoot};
	private BotIKController ikController;
    void Start()
    {
        ikController = GetComponent<BotIKController>();
		ikController.setIKActive(true, enabledIKGoals);
		ikController.setLookAt(rightHandBall);
		
		ikController.rightHandObj = rightHandBall;
		ikController.leftFootObj = leftFootBall;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
			ikController.setIKActive(true, enabledIKGoals);
		}
        if(Input.GetMouseButtonDown(1)){
			ikController.setIKActive(false);
		}
    }
}
