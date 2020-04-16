using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotIKCatchController))]
[RequireComponent(typeof(BotIKDeflectController))]
[RequireComponent(typeof(Animator))]
public class BotIKPickupTestController : MonoBehaviour
{
    private BotIKCatchController ikCatchController;
    private BotIKDeflectController ikDeflectController;
    private Animator animator;
    void Start()
    {
        ikCatchController = GetComponent<BotIKCatchController>();
        ikCatchController.setActive(true);

        ikDeflectController = GetComponent<BotIKDeflectController>();
        ikDeflectController.setActive(true);

        animator = GetComponent<Animator>();    
    }
    void Update()
    {
        animator.SetBool("Crouch", !Input.GetKey(KeyCode.LeftControl));
    }
}
