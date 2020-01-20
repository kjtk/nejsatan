using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PartControl : MonoBehaviour {

    [Header("Do something on attach.")]
    public UnityEvent OnPartAttach;

    

    void Start() {
        //Debug.Log("PartControl:Start");

    }
}
