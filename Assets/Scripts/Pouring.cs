﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

public class Pouring : MonoBehaviour
{
    public float angle = 90;
    public FlexSourceActor spawn;
    public AudioSource pouringsound;

    // Start is called before the first frame update
    void Start()
    {
        spawn.isActive = false;
    }

    void Update() {
        if (Vector3.Angle(Vector3.up, transform.forward) > angle) {
            if (spawn.isActive == false) {
                pouringsound.Play();
            }
            spawn.isActive = true;
            
        } else {
            spawn.isActive = false;
            pouringsound.Stop();
        }
    }
}
//https://devtalk.nvidia.com/default/topic/1047717/general/flex-unity-v1-0-5-steamvr-fluid-error/
//NIVIDIAn koodissa jotain mätää joten fiksattu oudosti