using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotIKCatchController))]
public class BotIKCatchTestController : MonoBehaviour
{
	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand};
	private BotIKCatchController catchController;
    void Start()
    {
        catchController = GetComponent<BotIKCatchController>();
		catchController.setActive(true);
    }
    void Update()
    {
	
    }
}
