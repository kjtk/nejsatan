using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SimpleAttach : MonoBehaviour {

    Interactable interactable;

    void Start() {
        interactable = GetComponent<Interactable>();
    }

    void OnHandHoverBegin(Hand hand) {
        hand.ShowGrabHint();
    }

    void OnHandHoverEnd(Hand hand) {
        hand.HideGrabHint();
    }

    void HandHoverUpdate(Hand hand) {
        GrabTypes grabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(gameObject);

        // Grab
        if (interactable.attachedToHand == null && grabType != GrabTypes.None) {
            hand.AttachObject(gameObject, grabType);
            hand.HoverLock(interactable);
            hand.HideGrabHint();
        }
        // Release
        else if(isGrabEnding) {
            hand.DetachObject(gameObject);
            hand.HoverUnlock(interactable);
        }
    }

}
