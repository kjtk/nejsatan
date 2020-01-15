using UnityEngine;
using System.Collections;

using SnapAndPlug;

public class ManageRigidBodies : MonoBehaviour
{
	public float defaultMass = 1000;

	public void EnablePhysicsObject()
	{
		SnapGroup[] groups = FindObjectsOfType<SnapGroup>();

		if( groups.Length > 0 )
		{
			Rigidbody rb = groups[0].GetComponent<Rigidbody>();
			if( rb == null )
			{
				rb = groups[0].gameObject.AddComponent<Rigidbody>();
				rb.mass = defaultMass;
			}

		}
	}
}
