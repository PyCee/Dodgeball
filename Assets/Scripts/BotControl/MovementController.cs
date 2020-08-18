using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(IKThrowController))]
public class MovementController : MonoBehaviour
{
    public float moveSpeed;
    public float strafeSpeed;
    public float backpedalSpeed;
    public float rotationSpeed;

    private CharacterController cc;
    private bool isActive = false;
    private Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);
    private IKThrowController iKThrowController;
    private Sequence turnSequence;

    enum MovementState {
        Standard,
        Aiming
        /*
        LeftSidestep // dodge a ball by stepping to the side in slow motion
        RightSidestep
        Slide // Dodge a ball by sliding under it forwards
        Jump
        */
    };
    private MovementState movementState;

    void Start()
    {
        movementState = MovementState.Standard;
        iKThrowController = GetComponent<IKThrowController>();
        cc = GetComponent<CharacterController>();
        turnSequence = DOTween.Sequence();
    }
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 flatCameraForward = Camera.main.transform.forward;
        flatCameraForward.y = 0.0f;
        Vector3 flatCameraRight = Camera.main.transform.right;
        flatCameraRight.y = 0.0f;
        Vector3 controlledMovement = flatCameraForward.normalized * z + flatCameraRight.normalized * x;
        controlledMovement *= moveSpeed;
        

        if(isActive){
            switch(movementState){
                case MovementState.Standard:
                    TurnToDir(controlledMovement.normalized);
                    movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                    
                    if(iKThrowController.GetThrowState() != IKThrowController.ThrowState.None){
                        movementState = MovementState.Aiming;
                    }
                    break;
                case MovementState.Aiming:
                    if(iKThrowController.GetTarget()){
                        Vector3 targetDir = iKThrowController.GetTarget().position - transform.position;
                        Vector3 flatTargetDir = new Vector3(targetDir.x, 0.0f, targetDir.z);
                        TurnToDir(flatTargetDir.normalized);
                    }
                    movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);

                    if(iKThrowController.GetThrowState() == IKThrowController.ThrowState.None){
                        movementState = MovementState.Standard;
                    }
                    break;
            }
            movement += Physics.gravity * Time.deltaTime;
            cc.Move(movement * Time.deltaTime);
        }
        
    }
    public void setActive(bool active){
        isActive = active;
    }
    private void TurnToDir(Vector3 normalizedDir){
        
        Vector3 flatForward = transform.forward;
        flatForward.y = 0.0f;
        flatForward = flatForward.normalized;
        Vector3 cross = Vector3.Cross(flatForward, normalizedDir);
        float diff = cross.magnitude;
        if(Vector3.Dot(flatForward, normalizedDir) < 0.0f){
            diff = 1.0f;
        }
        if(cross.y < 0.0){
            diff *= -1.0f;
        }
        
        turnSequence.Kill();
        Vector3 DORotation = Vector3.up * diff * rotationSpeed;
        turnSequence.Append(transform.DOLocalRotate(DORotation, 0.75f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart));

    }
}
