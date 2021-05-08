using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

[RequireComponent(typeof(IKThrowController))]
[RequireComponent(typeof(Animator))]
public class PlayerCameraController : MonoBehaviour
{
    public GameObject targetMarkerPrefab;
    public CinemachineVirtualCamera aimingCamera;
    public CinemachineTargetGroup aimingCameraTargetGroup;
    public Transform canvas;

    private GameObject targetMarker = null;
    private IKThrowController iKThrowController;
    private Animator animator;

    private Transform lastTarget = null;

    void Start()
    {
        iKThrowController = GetComponent<IKThrowController>();
        animator = GetComponent<Animator>();
        Transform neckTransform = animator.GetBoneTransform(HumanBodyBones.Neck);
        aimingCameraTargetGroup.AddMember(neckTransform, 1, 3);
    }
    void LateUpdate(){
        Transform currTarget = iKThrowController.GetTarget();

        if(lastTarget != currTarget){
            if(aimingCameraTargetGroup.m_Targets.Length > 1){
                aimingCameraTargetGroup.RemoveMember(lastTarget);
            }
            aimingCameraTargetGroup.AddMember(currTarget, 1.5f, 1.0f);
            lastTarget = currTarget;
        }
        UpdateTargetMarkerPosition();
    }
    public void UseAimCamera(){
        if(targetMarker == null){
            targetMarker = Instantiate(targetMarkerPrefab);
            targetMarker.transform.SetParent(canvas, false);
        }
        aimingCamera.m_Priority = 2;
    }
    public void StopUsingAimCamera(){
        if(targetMarker != null){
            Destroy(targetMarker);
            targetMarker = null;
        }
        aimingCamera.m_Priority = 0;
    }
    private void UpdateTargetMarkerPosition(){
        if(iKThrowController.GetTarget() != null &&
            targetMarker != null){
            // Positioning a UI element over a world-space object
            //   https://forum.unity.com/threads/create-ui-health-markers-like-in-world-of-tanks.432935/
            // With an offset
            //   https://forum.unity.com/threads/get-ui-placed-right-over-gameobjects-head.489464/
            Vector3 targetWorldPoint = iKThrowController.GetTarget().position + new Vector3(0.0f, 1.5f, 0.0f);
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(targetWorldPoint);
            targetMarker.transform.position = screenPoint;
        }
    }
}