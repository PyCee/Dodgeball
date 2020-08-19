using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

[RequireComponent(typeof(IKController))]
[RequireComponent(typeof(IKCatchController))]
[RequireComponent(typeof(IKDeflectController))]
[RequireComponent(typeof(Animator))]
public class IKThrowController : MonoBehaviour
{
    public float throwSpeed;
    public float cosLowerLimit;
    public Transform mainCamera;

    public enum ThrowPathState {
        Straight,
        CurveFromRight,
        CurveFromLeft,
        CurveFromAbove
    };
    private ThrowPathState throwPathState = ThrowPathState.Straight;
    public void SetThrowPathState(ThrowPathState t){throwPathState = t;}

    public enum ThrowState {
        None,
        Aiming,
        Throwing
    };
    private ThrowState throwState;
    public ThrowState GetThrowState(){return throwState;}
    public bool IsAiming(){return throwState == ThrowState.Aiming;}

    private IKController ikController;
    private IKCatchController ikCatchController;
    private IKDeflectController ikDeflectController;
    private Animator animator;
    private Transform target;
    public Transform GetTarget(){return target;}

	AvatarIKGoal[] enabledIKGoals = {AvatarIKGoal.RightHand, AvatarIKGoal.LeftHand};
    void Start()
    {
        throwState = ThrowState.None;
        ikController = GetComponent<IKController>();
        ikCatchController = GetComponent<IKCatchController>();
        ikDeflectController = GetComponent<IKDeflectController>();
        animator = GetComponent<Animator>();
    }
    public void StartAim(){
        SetTarget(GetInitialTarget(mainCamera.position, mainCamera.forward));
        throwState = ThrowState.Aiming;
    }
    public void ReadyThrow(){
        // Ready for throw animation
        
        Assert.IsTrue(ikCatchController.hasCaughtBall());

        ikController.setIKActive(false, enabledIKGoals);
        ikCatchController.setActive(false);
        ikDeflectController.setActive(false);

        // Parent ball to hand
        Transform rHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        ikCatchController.caughtBall.SetParent(rHand);

        ikController.setLookAt(target);
        throwState = ThrowState.Throwing;
    }
    public void ReleaseBall(){
        // Ran as an animation event

        // Apply throw force to the ball
        Quaternion towards = transform.rotation;
        if(target != null){
            Transform rHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            towards = Quaternion.LookRotation(target.position - rHand.position);
        }
        Transform thrownBall = ikCatchController.caughtBall;
        ikCatchController.ReleaseCaughtBall();
        

        ThrownController ballThrownController = thrownBall.gameObject.AddComponent<ThrownController>() as ThrownController;
        
        Vector3 beginningPoint = thrownBall.position;
        Vector3 endPoint = target.position;
        Vector3 direction = endPoint - beginningPoint;
        Vector3 tmpPoint = beginningPoint + direction * 0.9f;
        Vector3 controlPoint;
        ThrowPath throwPath = null;
        switch(throwPathState){
            case ThrowPathState.Straight:
                throwPath = new StraightPath(beginningPoint, endPoint, throwSpeed);
                break;
            case ThrowPathState.CurveFromRight:
                controlPoint = tmpPoint + (Quaternion.AngleAxis(-90.0f, direction.normalized) * Vector3.up * 3);
                throwPath = new CurvePath(beginningPoint, controlPoint, endPoint, throwSpeed);
                break;
            case ThrowPathState.CurveFromLeft:
                controlPoint = tmpPoint + (Quaternion.AngleAxis(90.0f, direction.normalized) * Vector3.up * 3);
                throwPath = new CurvePath(beginningPoint, controlPoint, endPoint, throwSpeed);
                break;
            case ThrowPathState.CurveFromAbove:
                controlPoint = tmpPoint + (Quaternion.AngleAxis(0.0f, direction.normalized) * Vector3.up * 3);
                throwPath = new CurvePath(beginningPoint, controlPoint, endPoint, throwSpeed);
                break;
            default:
                print("Unrecognized ThrowPathState");
                break;
        }
        ballThrownController.SetThrowPath(throwPath);

        RemoveTarget();

        throwState = ThrowState.None;
    }
    public void PostThrow(){
        ikController.setIKActive(true, enabledIKGoals);
        ikCatchController.setActive(true);
        ikDeflectController.setActive(true);
    }

    public void MoveTargetLeft(){
        SetTarget(GetNextTarget());
    }

    public void MoveTargetRight(){
        SetTarget(GetNextTarget(false));
    }

    private void SetTarget(Transform target){
        this.target = target;
    }
    private void RemoveTarget(){
        this.target = null;
    }

    private Transform GetNextTarget(bool left=true){
        Vector3 targetDiff = target.position - mainCamera.position;
        float targetSin = Vector3.Cross(targetDiff.normalized, mainCamera.forward).y;
        Transform nextTarget = null;
        float nextTargetSin = 0.0f;
        foreach(Transform transf in GetValidTargets(mainCamera.position, mainCamera.forward)){
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
        foreach(Transform transf in GetValidTargets(position, forward)){
            Vector3 diff = transf.position - position;
            float dir = Vector3.Dot(diff.normalized, forward);
            if(dir > largestDir){
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
            if(dir > cosLowerLimit){
                targets.Add(transf);
            }
        }
        return targets.ToArray();
    }
    private Transform[] GetTargets(){
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Target");
        Transform[] targets = new Transform[gameObjects.Length];
        for(int i = 0; i < gameObjects.Length; ++i){
            targets[i] = gameObjects[i].transform;
        }
        return targets;
    }
}