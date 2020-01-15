using UnityEngine;
using System.Collections;

namespace SnapAndPlug
{
	public class AnyObjectSpawner : MonoBehaviour
	{
		public GameObject prefabSnapPiece;
		public float distanceToSpawnAt = 100f;

		private GameObject _currentPiece;

		public void SpawnToMouse()
		{
			if( _currentPiece == null )
			_currentPiece = GameObject.Instantiate( prefabSnapPiece );
			else
				Debug.LogWarning("Can't spawn something; there's a spawned piece attached to the mouse. Click somewhere to finish placing, then you can spawn a new thing" );
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			Vector3 mWorld = Camera.main.ScreenToWorldPoint( Input.mousePosition + Vector3.forward * distanceToSpawnAt );

			if( _currentPiece != null )
			{
				_currentPiece.transform.position = mWorld;

				if( Input.GetMouseButtonDown( 0 ) )
				{
					_currentPiece = null;
				}
			}
		}
	}
}