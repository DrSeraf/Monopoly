using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSide : MonoBehaviour
{

    bool onGround;
    public bool OnGround => onGround;

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public int SideValue()
    {
        
            int value = Int32.Parse(name);
            return value;
    }

}
