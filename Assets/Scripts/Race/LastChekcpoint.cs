using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastChekcpoint : MonoBehaviour
{
    public static bool PassedLastCheckpointPlayer = false;
    public bool FirstPass;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle")
        {
            if (!FirstPass)
                FirstPass = true;
            
            if(FirstPass)
                PassedLastCheckpointPlayer = true;
        }
    }
}