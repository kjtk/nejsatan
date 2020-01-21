using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SnapAndPlug;
using UnityEngine.EventSystems;

//using Valve.VR.InteractionSystem;

#pragma warning disable 0642 // This warning is almost always wrong (and is wrong in every time it appears in this class)

public class InGameSnappableNejSatan : MonoBehaviour, IDragActionSource
{
    public AudioSource attachSound;

    public void PlayAttachSound() {
        attachSound.Play(0);
        Debug.Log("Attach audio played.");
    }

    // SteamVR
    //Hand hand;

    public DragDropAction3D currentDrag;
	public bool draggingRemovesFromGroup = false;

	public virtual DragDropAction3D CreateNewDragAction()
	{
		//e.g. 3D world snap: return new DragDropBlockAction3D( materialForPreviewGhosts, DragDropSnapMode.METERS_3D, 1f );
		//e.g. 2D screen snap: return new DragDropBlockAction3D( materialForPreviewGhosts, DragDropSnapMode.PIXELS_2D, 15f );
		return new DragDropAction3DWithGhost( materialForPreviewGhosts, DragDropSnapMode.METERS_3D, 0.5f, draggingRemovesFromGroup );
	}

	public Material materialForPreviewGhosts;
	public Material materialForHilightingMesh;

	public void Update ()
	{
		if (currentDrag != null)
		{
			currentDrag.UpdateDrag ();
            //Debug.Log("::: UpdateDrag :::");
		}
	}
	
	public virtual void DropCompletedOrCancelled()
	{
        Debug.Log("::: DropCompletedOrCancelled :::" + this.name);

    }

    public virtual void SnappedOntoButNotDroppedYet( SnapSocket localSocket, SnapPiece pieceSnappedOnto, SnapSocket socketSnappedOnto )
	{
        Debug.Log("::: SnappedOntoButNotDroppedYet :::" + this.name);


    }

    
	public void OnMouseDown ()
	{
		if( EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() ) // NB: this will BREAK if you ever add a PhysicsCaster to your Camera; Unity 5's Events are tricky
				;
		else
		{
			ClickStarted();
		}
	}
    


    // For VR environment...
    public void OnMouseDownVR() {
        ClickStarted();
    }


	/** we use this instead of copy/pasting into OnMouseDown so that subclasses can override this
	 *  - rather than overriding OnMouseDown, which has low-level significance, and MIGHT get replaced
	 * in future (since Unity's implementation of OnMouseDown is a bit weird and likely to change)
	 */
	public virtual void ClickStarted()
	{
        Debug.Log("::: ClickStarted :::" + this.name);
        
        // SteamVR
        //hand = GetComponentInParent<Hand>();
        //Debug.Log("::: SteamVR Hand = " + hand.name + " :::");

        currentDrag = CreateNewDragAction();

		currentDrag.StartDrag( this.gameObject, this );

		_RemoveHilightMesh ();
	}

    
	public void OnMouseUp ()
	{
        Debug.Log("::: OnMouseUp :::" + this.name);
        if ( currentDrag != null )
		{
			currentDrag.TriggerDrop();

            Invoke("PlayAttachSound", 0);

        }

        currentDrag = null;

		_RemoveHilightMesh (); // MouseUp may happen when you're NOT over the object, because of snapping, so need to pre-emptively de-hilight here
	}
    


    /*
    // For VR environment...
    private void OnParentHandHoverEnd(Interactable other) {
        Debug.Log("::: OnParentHandHoverEnd :::" + this.name);
        if (currentDrag != null) {
            currentDrag.TriggerDrop();

            Invoke("PlayAttachSound", 0);

        }

        currentDrag = null;

        _RemoveHilightMesh(); // MouseUp may happen when you're NOT over the object, because of snapping, so need to pre-emptively de-hilight here
    }
    */

    #region Showing a hilight mesh when the mouse is over it

    private GameObject _mouseOverMesh;
	private IntelligentMeshFactory _localMeshGenerator = new IntelligentMeshFactory ();


    /*
    // For VR environment...
    void OnParentHandHoverBegin(Interactable other) {
        Debug.Log("::: OnParentHandHoverBegin :::" + this.name);
        if (currentDrag == null
                 || !currentDrag._isBeingDragged)
            _AddHilightMesh();
    }
    */

    
    public void OnMouseEnter ()
	{
        Debug.Log("::: OnMouseEnter :::" + this.name);
        if (currentDrag == null
		    || !currentDrag._isBeingDragged)
			_AddHilightMesh ();
	}
    


	/**
	 * Required because this class needs to check mouse position in the constructor, but Unity doesn't allow that.
	 * 
	 * Additionally required because there's no reliable, non-buggy, way in Unity 5.x to ignore a MouseEnter while the UI is doing something else e.g. a popup.
	 */
	public void OnMouseOver ()
	{
        Debug.Log("::: OnMouseOver :::" + this.name);
        if (currentDrag != null
		    && !currentDrag._isBeingDragged
		    && _mouseOverMesh == null)
			_AddHilightMesh ();
	}


    
    public void OnMouseExit ()
	{
        Debug.Log("::: OnMouseExit :::" + this.name);
        _RemoveHilightMesh();
	}
    


	private void _RemoveHilightMesh ()
	{
		if (_mouseOverMesh != null)
			GameObject.Destroy (_mouseOverMesh);
	}

	public static Bounds BoundsForGameObjectAndChildColliders( GameObject go )
	{
		Bounds objectBounds = default(Bounds);
		bool boundsIsNull = true;

		foreach (var r in go.GetComponentsInChildren<Collider>())
		{
			if (boundsIsNull)
			{
				objectBounds = r.bounds;
				boundsIsNull = false;
			}
			else
				objectBounds.Encapsulate (r.bounds);
		}

		return objectBounds;
	}

	public static Bounds BoundsForGameObjectAndChildRenderers( GameObject go )
	{
		Bounds objectBounds = default(Bounds);
		bool boundsIsNull = true;

		foreach (var r in go.GetComponentsInChildren<Renderer>())
		{
			if (boundsIsNull)
			{
				objectBounds = r.bounds;
				boundsIsNull = false;
			}
			else
				objectBounds.Encapsulate (r.bounds);
		}

		return objectBounds;
	}

	private void _AddHilightMesh ()
	{
		if (_mouseOverMesh != null)
			GameObject.DestroyImmediate (_mouseOverMesh);

		_mouseOverMesh = new GameObject ("Mouse-over mesh (temporary)");

		SnapGroup containedInGroup = GetComponentInParent<SnapGroup> ();


		GameObject initialObject = null;

		bool autoDetectGroupsAndTreatDifferently = false;
		if (autoDetectGroupsAndTreatDifferently)
			initialObject = containedInGroup != null ? containedInGroup.gameObject : this.gameObject;
		else
			initialObject = gameObject;

		Bounds objectBounds = BoundsForGameObjectAndChildColliders( initialObject );

		/** Expand the bounds very slightly to prevent z-fighting on objects whose faces are right on the bounds */
		objectBounds.extents *= 1.05f;

		Vector3 x = new Vector3 (objectBounds.extents.x, 0, 0);
		Vector3 y = new Vector3 (0, objectBounds.extents.y, 0);
		Vector3 z = new Vector3 (0, 0, objectBounds.extents.z);

		Vector3 xy = new Vector3 (objectBounds.extents.x, objectBounds.extents.y, 0);
		Vector3 yz = new Vector3 (0, objectBounds.extents.y, objectBounds.extents.z);
		Vector3 xz = new Vector3 (objectBounds.extents.x, 0, objectBounds.extents.z);

		Vector3 xyz = objectBounds.extents;
		float lineWidth = 0.1f;
		Vector3 n = Vector3.zero;

		_localMeshGenerator.Clear();
		
		// left (YZ plane, pointing in negative X)
		n = -1f * Vector3.right;
		_localMeshGenerator.AddLine (-xyz,
			-xy + z, lineWidth, n);
		_localMeshGenerator.AddLine (-xyz,
			-xz + y, lineWidth, n);
		_localMeshGenerator.AddLine (-xz + y,
			-x + yz, lineWidth, n);
		_localMeshGenerator.AddLine (-xy + z,
			-x + yz, lineWidth, n);
		// right (YZ plane, pointing in positive X)
		n = Vector3.right;
		_localMeshGenerator.AddLine (-yz + x,
			-y + xz, lineWidth, n);
		_localMeshGenerator.AddLine (-yz + x,
			-z + xy, lineWidth, n);
		_localMeshGenerator.AddLine (-z + xy,
			-Vector3.zero + xyz, lineWidth, n);
		_localMeshGenerator.AddLine (-y + xz,
			-Vector3.zero + xyz, lineWidth, n);

		// top (XZ plane, pointing in positive Y)
		n = Vector3.up;
		_localMeshGenerator.AddLine (-xz + y,
			-x + yz, lineWidth, n);
		_localMeshGenerator.AddLine (-xz + y,
			-z + xy, lineWidth, n);
		_localMeshGenerator.AddLine (-x + yz,
			-Vector3.zero + xyz, lineWidth, n);
		_localMeshGenerator.AddLine (-z + xy,
			-Vector3.zero + xyz, lineWidth, n);
		// bottom (XZ plane, pointing in negative Y)
		n = -1f * Vector3.up;
		_localMeshGenerator.AddLine (-xyz,
			-xy + z, lineWidth, n);
		_localMeshGenerator.AddLine (-xyz,
			-yz + x, lineWidth, n);
		_localMeshGenerator.AddLine (-xy + z,
			-y + xz, lineWidth, n);
		_localMeshGenerator.AddLine (-yz + x,
			-y + xz, lineWidth, n);

		// back (XY plane, pointing in positive Z)
		n = Vector3.forward;
		_localMeshGenerator.AddLine (-xy + z,
			-x + yz, lineWidth, n);
		_localMeshGenerator.AddLine (-xy + z,
			-y + xz, lineWidth, n);
		_localMeshGenerator.AddLine (-x + yz,
			-Vector3.zero + xyz, lineWidth, n);
		_localMeshGenerator.AddLine (-y + xz,
			-Vector3.zero + xyz, lineWidth, n);
		// front (XY plane, pointing in negative Z)
		n = -1f * Vector3.forward;
		_localMeshGenerator.AddLine (-xyz,
			-xz + y, lineWidth, n);
		_localMeshGenerator.AddLine (-xyz,
			-yz + x, lineWidth, n);
		_localMeshGenerator.AddLine (-xz + y,
			-z + xy, lineWidth, n);
		_localMeshGenerator.AddLine (-yz + x,
			-z + xy, lineWidth, n);

		MeshFilter mf = _mouseOverMesh.AddComponent<MeshFilter> ();
		MeshRenderer mr = _mouseOverMesh.AddComponent<MeshRenderer> ();

		mr.sharedMaterial = materialForHilightingMesh;

		Mesh m = _localMeshGenerator.newMesh;
		m.RecalculateNormals ();
		mf.sharedMesh = m;
		_mouseOverMesh.transform.position = objectBounds.center;

	}

	#endregion
}