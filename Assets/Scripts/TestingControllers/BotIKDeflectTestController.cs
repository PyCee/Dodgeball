using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotIKCatchController))]
[RequireComponent(typeof(BotIKDeflectController))]
public class BotIKDeflectTestController : MonoBehaviour
{
    private BotIKCatchController ikCatchController;
    private BotIKDeflectController ikDeflectController;
    void Start()
    {
        ikCatchController = GetComponent<BotIKCatchController>();
        ikCatchController.setActive(true);

        ikDeflectController = GetComponent<BotIKDeflectController>();
        ikDeflectController.setActive(true);
        
    }
}
