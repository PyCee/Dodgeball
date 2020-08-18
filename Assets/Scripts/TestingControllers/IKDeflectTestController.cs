using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKCatchController))]
[RequireComponent(typeof(IKDeflectController))]
public class IKDeflectTestController : MonoBehaviour
{
    public GameObject ballPrefab;

    private IKCatchController ikCatchController;
    private IKDeflectController ikDeflectController;
    void Start()
    {
        ikCatchController = GetComponent<IKCatchController>();
        ikCatchController.setActive(true);

        ikDeflectController = GetComponent<IKDeflectController>();
        ikDeflectController.setActive(true);
        
        // Supply the bot with a new ball
        Vector3 ballPos = transform.position + transform.forward * 0.5f;
        ballPos.y += 1.5f;
		Instantiate(ballPrefab, ballPos, Quaternion.identity);
    }
}
