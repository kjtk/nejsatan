using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartControl : MonoBehaviour {

    public bool MasterPart;

    //public void SnapPointCollisionDetected(SnapControl childScript) {
        //Debug.Log("Snap point collided");
    //}

    void OnCollisionEnter(Collision collision) {
        //Debug.Log("PartControl:OnCollisionEnter");
        //Debug.Log(collision.gameObject.name);
        if(collision.gameObject.name == "SnapPoint") {
            //Debug.Log("!!SnapPoint!!");
        }
    }

    void Start() {
        //Debug.Log("PartControl:Start");
    }
}
