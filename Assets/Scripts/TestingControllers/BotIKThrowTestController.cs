using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BotIKController))]
[RequireComponent(typeof(BotIKCatchController))]
[RequireComponent(typeof(BotIKDeflectController))]
[RequireComponent(typeof(BotIKThrowController))]
[RequireComponent(typeof(Animator))]
public class BotIKThrowTestController : MonoBehaviour
{
    public GameObject prefab;
    public Transform mainCamera;
    public GameObject aimingCamera;
    public GameObject cameraTargetGroup;
    private BotIKController ikController;
    private BotIKCatchController ikCatchController;
    private BotIKDeflectController ikDeflectController;
    private BotIKThrowController ikThrowController;
    private Animator animator;
	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand, AvatarIKGoal.LeftHand};
    private Transform target;

    void Start()
    {
        ikController = GetComponent<BotIKController>();
        ikController.setIKActive(true, enabledIKGoals);
        
        ikCatchController = GetComponent<BotIKCatchController>();
        ikCatchController.setActive(true);

        ikDeflectController = GetComponent<BotIKDeflectController>();
        ikDeflectController.setActive(true);

        ikThrowController = GetComponent<BotIKThrowController>();

        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && ikCatchController.hasCaughtBall()){
            aimingCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 2;
            target = GetInitialTarget(mainCamera.position, mainCamera.forward);
            cameraTargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[1].target = target;
        } else if(Input.GetMouseButtonUp(0) && ikCatchController.hasCaughtBall()){
            // Throw a ball if the bot has one
            // Record the target and prepare throw
            ikThrowController.PrepareThrow(target);
            // Play the animation
            animator.SetTrigger("Throw");
            aimingCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 0;
        }

        if(Input.GetMouseButton(0)){
            if(Input.GetKeyDown(KeyCode.A)){
                // Compare target to list of GetTargets() and find next left one and set to target
                target = GetNextTarget();
                cameraTargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[1].target = target;
            }
            if(Input.GetKeyDown(KeyCode.D)){
                // Compare target to list of GetTargets() and find next right one and set to target
                target = GetNextTarget(false);
                cameraTargetGroup.GetComponent<CinemachineTargetGroup>().m_Targets[1].target = target;
            }
        }

        if(Input.GetMouseButtonDown(1)){
            // Supply the bot with a new ball
            Vector3 ballPos = transform.position + transform.forward * 0.5f;
            ballPos.y += 1.5f;
			GameObject throwable = Instantiate(prefab, ballPos, Quaternion.identity);
        }
    }
    private Transform GetNextTarget(bool left=true){
        Vector3 targetDiff = target.position - mainCamera.position;
        float targetSin = Vector3.Cross(targetDiff.normalized, mainCamera.forward).magnitude;
        Transform nextTarget = null;
        float nextTargetSin = 0.0f;
        foreach(Transform transf in GetValidTargets(mainCamera.position, mainCamera.forward)){
            if(transf == target){
                continue;
            }
            Vector3 tmpDiff = transf.position - mainCamera.position;
            float sin = Vector3.Cross(tmpDiff.normalized, mainCamera.forward).y;

            bool betterThanOriginal = left ? sin > targetSin : sin < targetSin;
            bool betterThanNext = nextTarget == null || (left ? sin < nextTargetSin : sin > nextTargetSin);
            if(betterThanOriginal && betterThanNext){
                nextTarget = transf;
                nextTargetSin = sin;
            }
        }
        return nextTarget == null ? target : nextTarget;
    }

    public Transform GetInitialTarget(Vector3 position, Vector3 forward){
        // Returns valid target to throw at
        Transform target = null;
        float largestDir = Mathf.NegativeInfinity;
        foreach(Transform transf in GetTargets()){
            Vector3 diff = transf.position - position;
            float dir = Vector3.Dot(diff.normalized, forward);
            if(dir > 0.5 && dir > largestDir){
                target = transf;
                largestDir = dir;
            }
        }
        return target;
    }
    private Transform[] GetValidTargets(Vector3 position, Vector3 forward){
        // Returns valid targets to throw at
        List<Transform> targets = new List<Transform>();
        foreach(Transform transf in GetTargets()){
            Vector3 diff = transf.position - position;
            float dir = Vector3.Dot(diff.normalized, forward);
            if(dir > 0.5){
                targets.Add(transf);
            }
        }
        return targets.ToArray();
    }
    private Transform[] GetTargets(){
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Target");
        Transform[] results = new Transform[gos.Length];
        for(int i = 0; i < gos.Length; ++i){
            results[i] = gos[i].transform;
        }
        return results;
    }
}
