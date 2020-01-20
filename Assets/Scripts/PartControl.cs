using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PartControl : MonoBehaviour {

    [Header("Do something on attach.")]
    public UnityEvent OnPartAttach;

    public AudioSource attachSound;

    public void PlayAttachSound() {
        attachSound.Play(0);
        Debug.Log("Attach audio played.");
    }

    void Start() {
        //Debug.Log("PartControl:Start");

    }
}
