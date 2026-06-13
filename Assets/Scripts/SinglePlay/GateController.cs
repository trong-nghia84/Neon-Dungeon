using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D gateCollider;
    private bool isOpen = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        gateCollider = GetComponent<BoxCollider2D>();
    }

   
    public void OpenGate()
    {
        if (!isOpen)
        {
            isOpen = true;
            anim.SetTrigger("IsOpen"); 
            gateCollider.enabled = false; 
        }
    }

    public void CloseGate()
    {
        if (isOpen)
        {
            isOpen = false;
            anim.SetTrigger("IsClose"); 
            gateCollider.enabled = true; 
        }
    }
}
