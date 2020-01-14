//#define ERROR_ON_DETECTING_WHEEL_COLLIDER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SnapAndPlug
{
	enum PointerSnapMode
	{
		ABSOLUTE_PROJECTION,
		RELATIVE_TO_LAST_SNAP
	}


	/**
	 * 
	 * 
	 * EXAMPLE CLASS
	 * 
	 *    Licence: MIT
	 * 
	 * SUMMARY
	 * 
	 *    This is ONE EXAMPLE of how you can implement in-game snapping with a 3D preview.
	 *
	 ****************************************************************************************
	 *    As of SnapAndPlug version 2.3.0, THIS CLASS IS NO LONGER NEEDED (there is a better
	 *     built-in snapping solution provided by SnP v2.3 onwards).
	 *    Look at "DragDropAction3D" and the documentation / examples for it.
	 ****************************************************************************************
	 *
	 *    You can use this class directly, but I recommend you alter it and create your own
	 *     that fits with the style and look and feel of your game.
	 * 
	 *    Note: this class is designed to be generic and simple - it lets you snap anything,
	 *     in front, behind, upside-down, etc. Most games work better if you limit the player's 
	 *     options to what makes sense in your game at the current moment.
	 * 
	 * USAGE
	 * 
	 *    When the player purchases an item, or picks something up, call SpawnGhostToMouse()
	 *     and this will create an object attached to the mouse.
	 *    As the player moves the mouse around the screen, the object will follow.
	 *    When it gets close to a snappable object (another SnapPiece), it will auto-snap to
	 *     the nearest SnapSocket.
	 *    
	 *    Use the mousewheel to spin the object, cycling through all its possible SnapSockets
	 */
	public class SnappableSpawner : MonoBehaviour
	{
		public GameObject prefabSnapPiece;
		public float initialDistanceToSpawnAt = 100f;
		private float _currentDistanceToPositionAt;

		public bool removeWCRigidBodiesToo = false;
		public bool showDebugMessages = false;

		private SnapPiece _currentPiece;

		private int _spawnSuffix = 1;
		public void SpawnGhostToMouse()
		{
			if( _currentPiece == null )
			{
				_snapMode = PointerSnapMode.ABSOLUTE_PROJECTION;
				_currentDistanceToPositionAt = initialDistanceToSpawnAt;
				_currentPiece = GameObject.Instantiate( prefabSnapPiece ).GetComponent<SnapPiece>();
				_currentPiece.name = "Spawned-"+_spawnSuffix;
				_spawnSuffix++;

#pragma warning disable 219
				foreach( WheelCollider c in _currentPiece.GetComponentsInChildren<WheelCollider>() )
				{
#if ERROR_ON_DETECTING_WHEEL_COLLIDER
					Debug.LogError(
#else
						Debug.LogWarning(
#endif
						"WheelCollider in Unity 5 is a bit broken. Until Unity fixes the bugs in their integration of Havok, you are strongly recommended not to use WheelCollider. It works fine in this demo, but you CANNOT rotate it (bug added in Unity 5.x)" );
#if ERROR_ON_DETECTING_WHEEL_COLLIDER
					return;
#endif
				}
#pragma warning restore 219

				foreach( SnapGhostable ghostable in _currentPiece.GetComponentsInChildren<SnapGhostable>() )
				{
					ghostable.MakeGhost();
				}
			}
			else
				Debug.LogWarning("Can't spawn something; there's a spawned piece attached to the mouse. Click somewhere to finish placing, then you can spawn a new thing" );
		}

		private bool ignorePos;
		private PointerSnapMode _snapMode;
		private SnapSocket _snapModeLastSnappedToSocket;
		private Vector3 _snapModeOriginalPosition;
		private Vector3 _snapModeOriginalJoinPosition;
		private Vector3 _snapModeOriginalMousePointNearSocket;
		private float _snapModeOrignalJoinDistanceFromCamera;
		private Dictionary<SnapSocket,Ray> _lastRayForEachSocket;
		void Update ()
		{
			Vector3 mWorld = Camera.main.ScreenToWorldPoint( Input.mousePosition + Vector3.forward * _currentDistanceToPositionAt );

			if( _currentPiece != null )
			{

				/** Decide the new position to place in world, from which we'll calculate a POTENTIAL snap position */
				switch( _snapMode )
				{
				case PointerSnapMode.ABSOLUTE_PROJECTION:
					_currentPiece.transform.position = mWorld;
					break;

				case PointerSnapMode.RELATIVE_TO_LAST_SNAP:
					/** See how far the mouse has moved since the snap position */
					Ray currentMouseRay = Camera.main.ScreenPointToRay( Input.mousePosition );
					Vector3 currentMousePointCloseToSocket = currentMouseRay.GetPoint( _snapModeOrignalJoinDistanceFromCamera );

					Vector3 mouseOffsetCloseToSocket = currentMousePointCloseToSocket - _snapModeOriginalMousePointNearSocket;

					_currentPiece.transform.position = _snapModeOriginalPosition + mouseOffsetCloseToSocket;
					break;
				}




				/** Now that we've repositioned in world, calculate the rays for each socket.
					 * 
					 * On next frame-update, this will be used to try and match a socket to a nearby other-piece
					 */
				if( _lastRayForEachSocket == null )
					_lastRayForEachSocket = new Dictionary<SnapSocket, Ray>();
				
				foreach( SnapSocket localSocket in _currentPiece.GetAllUnconnectedSockets() )
				{
					_lastRayForEachSocket[localSocket] = new Ray( Camera.main.transform.position, localSocket.transform.position - Camera.main.transform.position );
				}


				/***************************************************************//***************************************************************/
							// MAJOR bug in the Mono compiler that Unity is shipping: these variabels ARE USED below!
							#pragma warning disable 219
				Vector3 nearestRoomHitPoint = Vector3.zero;
				float nearestSocketSocketDistance = 999999f; // MUST calculate this in screen space, not world space!
				SnapPiece nearestRoom = null;
				SnapSocket nearestSocket = null;
				SnapSocket localSocketForConnection = null;
							#pragma warning restore 219

				/** 
				 * 1. Check each unconnected socket
				 * 2. For each, find the piece it is nearest to
				 * 3. On that piece, for ALL hits on the ray (front and back) find the unconnected socket that is nearest to the hit
				 * 4. Stick with whichever socket was nearest
				 */
				foreach( SnapSocket localSocket in _currentPiece.GetAllUnconnectedSockets() )
				{
					Ray rayForSocket = _lastRayForEachSocket[localSocket];
					
								List<RaycastHit> hitsOnSnapPiece;
								SnapPiece hitRoom = _currentPiece.FirstSnapPieceAlongRayWithAvailableSockets( rayForSocket, out hitsOnSnapPiece, localSocket );
				
								if( hitRoom != null )
								{

									foreach( RaycastHit hit in hitsOnSnapPiece )
									{
							
							SnapSocket hitSocket = hitRoom.NearestUnconnectedSocketScreenSpace( Camera.main.WorldToScreenPoint( hit.point ), Camera.main, 0f, localSocket, SnapAndPlugScene.sceneData.showDebugMessages );

										if( hitSocket != null )
										{
											Vector3 screenForHitSocket = Camera.main.WorldToScreenPoint( hitSocket.attachPoint );
											Vector3 screenForLocalSocket = Camera.main.WorldToScreenPoint( localSocket.attachPoint );

											float socketDistance = (screenForHitSocket - screenForLocalSocket).magnitude;
											if( socketDistance < nearestSocketSocketDistance )
											{
												nearestSocketSocketDistance = socketDistance;
												nearestRoom = hitRoom;
												nearestSocket = hitSocket;
												nearestRoomHitPoint = hit.point;

												localSocketForConnection = localSocket;
											}

								if( SnapAndPlugScene.sceneData.showDebugMessages )
											Debug.Log("Found "+hitsOnSnapPiece.Count+" hits on target piece; this one has nearest socket: "+hitSocket+" near hit: "+hit.point+" ("+socketDistance+" away)" );
										}
										else
								if( SnapAndPlugScene.sceneData.showDebugMessages )
											Debug.Log("HIT on the room, but no unconnected sockets within range of point: "+hit.point );

									}


								}
				}

				/********* DEBUG HELPER * /
				if( nearestRoom != null )
				{
					foreach( SnapSocket s in _currentPiece.GetAllUnconnectedSockets() )
					{
						foreach( SnapSocket t in nearestRoom.GetAllUnconnectedSockets() )
						{
							Color c = Color.gray;

							if( s.Equals( localSocketForConnection ) )
								c = Color.yellow;

							if( s.Equals( localSocketForConnection ) && t.Equals( nearestSocket ) )
								c = Color.green;

							Debug.DrawLine( s.transform.position, t.transform.position, c );
						}
					}
				}*/

				switch( _snapMode )
				{
				case PointerSnapMode.ABSOLUTE_PROJECTION:
					if( nearestSocket != null )
					{
						_snapModeOriginalJoinPosition = nearestSocket.transform.position;
						_snapModeOriginalPosition = _currentPiece.transform.position;
						_snapModeOrignalJoinDistanceFromCamera = Vector3.Distance( Camera.main.transform.position, _snapModeOriginalJoinPosition );
						_snapModeOriginalMousePointNearSocket = Camera.main.ScreenPointToRay( Input.mousePosition ).GetPoint( _snapModeOrignalJoinDistanceFromCamera );

						_snapMode = PointerSnapMode.RELATIVE_TO_LAST_SNAP;
						//Debug.Log("Switching to RELATIVE SNAP positioning mode" );
					}
					break;

				case PointerSnapMode.RELATIVE_TO_LAST_SNAP:
					if( nearestSocket == null )
					{
						// Change the default projected distance for absolute mode so it matches the new local depth from last round of snapping
						_currentDistanceToPositionAt = _snapModeOrignalJoinDistanceFromCamera;

						_snapMode = PointerSnapMode.ABSOLUTE_PROJECTION;
						//Debug.Log("Switching to ABSOLUTE snap positioning mode" );
					}
					break;
				}


				if( nearestSocket != null )
				{
					/*if( ! nearestSocket.Equals( _debug_currentSnapSocket ) )
						Debug.Log("-- changing to new snap socket (old: "+_debug_currentSnapSocket+"), new = "+nearestSocket );*/

					///////////////// 
					localSocketForConnection.SnapTo( nearestSocket, false );

					foreach( SnapGhostable ghostable in _currentPiece.GetComponentsInChildren<SnapGhostable>() )
						ghostable.SetSnapped( true );

					if( ! nearestSocket.Equals( _snapModeLastSnappedToSocket ) )
					{
						_snapModeOriginalJoinPosition = nearestSocket.transform.position;
					_snapModeOriginalPosition = _currentPiece.transform.position;
					_snapModeOrignalJoinDistanceFromCamera = Vector3.Distance( Camera.main.transform.position, _snapModeOriginalJoinPosition );
					_snapModeOriginalMousePointNearSocket = Camera.main.ScreenPointToRay( Input.mousePosition ).GetPoint( _snapModeOrignalJoinDistanceFromCamera );
				}
					_snapModeLastSnappedToSocket = nearestSocket;
				
					/** Allow user to change the chosen socket, moving the piece to FORCE it into being "nearest" */
				float scrollInput = Input.GetAxis("Mouse ScrollWheel");
				if( scrollInput != 0 )
				{
						int deltaIndex = (scrollInput > 0) ? 1 : -1;
					int indexCurrent = _currentPiece.GetAllUnconnectedSockets().IndexOf( localSocketForConnection );
						int indexNew = indexCurrent + deltaIndex;
						Debug.Log("SCROLL: "+scrollInput+" -- changing index of local socket from "+indexCurrent+" to "+indexNew+" (total socks on this object = "+_currentPiece.GetAllUnconnectedSockets().Count );
						if( indexNew >= _currentPiece.GetAllUnconnectedSockets().Count )
							indexNew = 0;
						if( indexNew < 0 )
							indexNew = _currentPiece.GetAllUnconnectedSockets().Count-1;


						_currentPiece.GetAllUnconnectedSockets()[indexNew].SnapTo( nearestSocket, false );

						// redo it
						_snapModeOriginalJoinPosition = nearestSocket.transform.position;
						_snapModeOriginalPosition = _currentPiece.transform.position;
						_snapModeOrignalJoinDistanceFromCamera = Vector3.Distance( Camera.main.transform.position, _snapModeOriginalJoinPosition );
						_snapModeOriginalMousePointNearSocket = Camera.main.ScreenPointToRay( Input.mousePosition ).GetPoint( _snapModeOrignalJoinDistanceFromCamera );
				}

				}
				else
				{
					foreach( SnapGhostable ghostable in _currentPiece.GetComponentsInChildren<SnapGhostable>() )
						ghostable.SetSnapped( false );
				}

				if( Input.GetMouseButtonDown( 0 ) )
				{
					//Debug.Log(" mouse clicked " );

					// Clear the socket/ray dictionry since we'll have a new piece next
					if( _lastRayForEachSocket != null )
					{
						_lastRayForEachSocket.Clear();
					}

					// Finalise the connection, if it is snappably-close (if not, leave it where it is)
					if( nearestSocket != null )
					{

						//Debug.Log(" ... snapping: "+localSocketForConnection+" to: "+nearestSocket );
						localSocketForConnection.SnapTo( nearestSocket, true );

						if( removeWCRigidBodiesToo )
						{
										if( showDebugMessages )
							Debug.Log("Removing Unity fake RBs" );
							
								foreach( WheelCollider wc in nearestSocket.parentPiece.GetComponentsInChildren<WheelCollider>() )
								{
											if( showDebugMessages )
									Debug.Log("Removing fake RB from WC = "+wc.gameObject.name );
									Rigidbody rbwc = wc.GetComponent<Rigidbody>();
									
									if( rbwc != null )
									{
												if( showDebugMessages )
										Debug.Log("Destroying!" );
										GameObject.Destroy( rbwc );
									}
								}
								
								

						}
					}

					// If it was ghosted, reset the object materials back to what they should be
					foreach( SnapGhostable ghostable in _currentPiece.GetComponentsInChildren<SnapGhostable>() )
					{
						ghostable.UnGhost();
					}

					_currentPiece = null;
				}
			}
		}
	}
}