using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKCatchController))]
public class IKCatchTestController : MonoBehaviour
{
	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand};
	private IKCatchController catchController;
    void Start()
    {
        catchController = GetComponent<IKCatchController>();
		catchController.setActive(true);
    }
    void Update()
    {
	
    }
}
