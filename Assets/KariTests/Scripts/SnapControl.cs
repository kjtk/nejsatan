using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapControl : MonoBehaviour
{

    Vector3 posThisParent;
    Vector3 posThisSnapPoint;
    Vector3 posOtherSnapPoint;

    // Detect collision with other SnapPoint
    void OnTriggerEnter(Collider other) {
        Debug.Log("SnapPoint Collision Detected.\nParent Name: " + other.transform.parent.name);

        // Simple check for target and source (object that will be attached)
        // Which one is moving (both could be moving though...)
        // This "works" for one pair of objects...
        //if(other.transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero) {
        if(other.transform.parent.GetComponent<PartControl>().MasterPart) {

            // Disable gravity from parent part
            this.transform.parent.GetComponent<Rigidbody>().useGravity = false;
            this.transform.parent.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            Debug.Log(posThisParent);
            Debug.Log(other.transform.parent.name + " is master.");

            // Get other SnapPoint's position and prepare new pos. for this part
            //posThisSnapPoint = other.transform.position;

            // Get parent's pos.
            //posThisParent = this.GetComponentInParent<Transform>().position;

            // Change the parent's rotation/position. posThisSnapPoint is already in new pos.
            //posThisParent -= posThisSnapPoint;
            //this.transform.parent.position = posThisParent;

            Debug.Log(other.transform.position + "\n" + this.transform.position);


            this.transform.parent.position = other.transform.position + (this.transform.parent.position - this.transform.position);

            this.transform.parent.GetComponent<Rigidbody>().useGravity = true;

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

    void Awake() {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;    
    }
}
