using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnsureFPSControllerPresent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	    Camera foundCamera = GameObject.FindObjectOfType<Camera>();
	    
	    if( foundCamera == null )
		    Debug.LogError("This scene requires a first-person camera. Please install Unity's free 'Standard Assets' package and drag/drop the free FirstPersonController prefab into this scene before hitting Play");
    }
}
