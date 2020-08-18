using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKCatchController))]
[RequireComponent(typeof(IKDeflectController))]
[RequireComponent(typeof(Animator))]
public class IKPickupTestController : MonoBehaviour
{
    private IKCatchController ikCatchController;
    private IKDeflectController ikDeflectController;
    private Animator animator;
    void Start()
    {
        ikCatchController = GetComponent<IKCatchController>();
        ikCatchController.setActive(true);

        ikDeflectController = GetComponent<IKDeflectController>();
        ikDeflectController.setActive(true);

        animator = GetComponent<Animator>();    
    }
    void Update()
    {
        animator.SetBool("Crouch", Input.GetKey(KeyCode.LeftControl));
    }
}
