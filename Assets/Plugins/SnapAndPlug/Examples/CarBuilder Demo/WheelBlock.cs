using UnityEngine;
using System.Collections;

using SnapAndPlug;

public class WheelBlock : MonoBehaviour
{
	//public WheelCollider wheelPhysics;
	public float powerReceived;
	public GameObject childWheelToSpinFast;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		SnapGroup group = GetComponentInParent<SnapGroup>();

		if( group != null )
		{
			//EngineBlock myEngine = null;
			float totalPowerFromEngines = 0f;
			foreach( EngineBlock engine in group.GetComponentsInChildren<EngineBlock>() )
			{
				totalPowerFromEngines += engine.currentPowerSentToEachComponent;
				break;
			}

			if( totalPowerFromEngines > 0f )
			{
				powerReceived = totalPowerFromEngines;

				//Debug.Log(" Have engine, driving! Engine gives torque = "+myEngine.currentPowerSentToEachComponent );
				WheelCollider wc = GetComponentInChildren<WheelCollider>();

				if( wc != null )
				{
					wc.motorTorque = powerReceived;

					float dir = Vector3.Angle( transform.forward, group.transform.forward ) < 20f ? 1f : -1f;

					childWheelToSpinFast.transform.localRotation *= Quaternion.AngleAxis( 360f * (dir * Time.deltaTime * wc.rpm / 60f), new Vector3(1f,0,0));
				}

			}
			else
			{
				powerReceived = 0f;
				Debug.Log(" :( :( no engine :( :(" );
			}
		}
	}

	/** Old code leftover from MANY MANY attempts of trying to workaround all the horrible bugs in Unity's WheelCollider. I gave up!
	void FixedUpdate()
	{
		WheelCollider wc = GetComponentInChildren<WheelCollider>();
		
		if( wc == null )
		if( powerReceived > 0 )
		{
			Debug.Log("powerreceived = "+powerReceived+" transform.forward = "+transform.forward );
			Rigidbody rb = gameObject.GetComponentInParent<Rigidbody>();


			if( rb != null )
				rb.AddForce( new Vector3(0f,0,1f) * powerReceived );
			else
				Debug.Log("No rigidbody to give force to" );
		}
	}
	*/
}
