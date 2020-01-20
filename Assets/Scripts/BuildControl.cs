using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SnapAndPlug;

public class BuildControl : MonoBehaviour {

    /*Parameters
    m	            Unity3D material to use to render the preview of where the dragged 
                    object will be snapped to (eg use a transparent shader)
    sm	            Whether to measure snap-tolerance in 3D (meters) or 2D (pixels)
    maxSnapDist	    The snap-tolerance; within this range (meters or pixels) the dragged 
                    object will snap to nearby objects
    unGroupOnDrag	Default: false. If true, dragging a SnapPiece that's inside a SnapGroup 
                    will automatically take it out of the group, disconnecting it. This was 
                    legacy default behaviour, but most games want the opposite: dragging a 
                    SnapPiece drags the whole group.
*/
    //public Material m;
    //public DragDropSnapMode dragDropSnapMode;
    //public float maxSnapDist;
    //public bool unGroupOnDrag;

    [Header("Do something on attach.")]
    public UnityEvent OnPartAttach;

    public AudioSource attachSound;
    //public UnityEvent OnAttachPlaySound;

    public void PlayAttachSound() {
        //attachSound = GetComponent<AudioSource>();
        attachSound.Play(0);
        Debug.Log("Attach audio played.");
    }

    void Start() {

        //var myBuildAction3D = new DragDropAction3D(m, dragDropSnapMode, maxSnapDist, unGroupOnDrag);

    }

    void Update() {
        
    }
}
