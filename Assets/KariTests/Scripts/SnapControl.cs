using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapControl : MonoBehaviour
{

    //void OnCollisionEnter(Collision other) {
    //    Debug.Log("SnapPoint Collision Detected With: " + other.gameObject.name);
    //    //transform.parent.GetComponent<PartControl>().SnapPointCollisionDetected(this);
    //}

    // Detect collision with other SnapPoint
    void OnTriggerEnter(Collider other) {
        Debug.Log("SnapPoint Collision Detected.\nParent Name: " + other.transform.parent.name);

        // Simple check for target and source (object that will be attached)
        // Which one is moving (both could be moving though...)
        // This "works" for one pair of objects...
        //if(other.transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero) {
        if(true) {
            Debug.Log(other.transform.parent.name + " was moving.");
            this.transform.position = other.transform.position;
        }

        //transform.parent.GetComponent<PartControl>().SnapPointCollisionDetected(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
