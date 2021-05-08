using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DodgeController))]
[RequireComponent(typeof(Animator))]
public class DodgeTestController : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 ballSpawnPosition;
    public float throwForce;
    public float ballDelay;

    private DodgeController dodgeController;
    private Animator animator;
    private float timeUntilBall;
    void Start()
    {
        dodgeController = GetComponent<DodgeController>();
        animator = GetComponent<Animator>();

        timeUntilBall = ballDelay;
    }
    void Update()
    {
        HandleBallSpawn();
        if(Input.GetKeyDown(KeyCode.W)){
            dodgeController.Dodge(DodgeController.DodgeAction.Jump);
        }
        if(Input.GetKeyDown(KeyCode.A)){
            dodgeController.Dodge(DodgeController.DodgeAction.StepLeft);
        }
        if(Input.GetKeyDown(KeyCode.S)){
            dodgeController.Dodge(DodgeController.DodgeAction.Duck);
        }
        if(Input.GetKeyDown(KeyCode.D)){
            dodgeController.Dodge(DodgeController.DodgeAction.StepRight);
        }
    }
    private void HandleBallSpawn()
    {
        timeUntilBall -= Time.deltaTime;
        while(timeUntilBall < 0.0){
            timeUntilBall += ballDelay;
            SpawnBall();
        }
    }
    private void SpawnBall()
    {
        GameObject throwable = Instantiate(ballPrefab, ballSpawnPosition, Quaternion.identity);
        throwable.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 0.0f, - throwForce), ForceMode.Impulse);
    }
}
