using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Updated:
 * 
 * v2.1: Made the ghosting automatically recursive: this also means you SHOULD place your SnapGhostable on the highest-up parent you want to ghost - unlike prevrious versions, it WILL NOT ghost its parents/grandparents/etc!
 */
namespace SnapAndPlug
{
	public class SnapGhostable : MonoBehaviour
	{
		public bool isGhosted;
		public bool ghostRecursively = false;
		private bool _wasGhostedRecursively; // Users/devs may change the recursive setting in a live project mid-test/playthrough; we'll make sure that unghosting STILL works even when they do this
		public Material[] materialsForPlainGhosts, materialsForSnappedGhosts;

		private bool _isRenderingSnapped;
		private Dictionary<Renderer,Material[]> _originalMaterialsPerChildRenderer;

		public void MakeGhost()
		{
			if( isGhosted )
			{
				Debug.LogWarning("Can't ghost, already ghosted: "+name );
			}
			else
			{
				
				isGhosted = true;
				_isRenderingSnapped = false;

				if( _originalMaterialsPerChildRenderer == null )
					_originalMaterialsPerChildRenderer = new Dictionary<Renderer, Material[]>();
				else
					_originalMaterialsPerChildRenderer.Clear();

				/** Remove the physics so it doesnt affect the world */
				foreach( Collider c in GetComponentsInChildren<Collider>() )
				{
					c.enabled = false;
				}

				/** Make it spooky */
				_wasGhostedRecursively = ghostRecursively;
				Renderer[] rs = _wasGhostedRecursively ? GetComponentsInChildren<Renderer>() : new Renderer[] { GetComponent<Renderer>() };
				foreach( Renderer _renderer in rs )
				{
					Material[] savingMats = _renderer.materials;
					_originalMaterialsPerChildRenderer[_renderer] = savingMats;
					_renderer.materials = materialsForPlainGhosts;
				}
			}
		}

		public void SetSnapped( bool isSnapped )
		{
			if( _isRenderingSnapped == isSnapped )
				return;

			if( !isGhosted )
			{
				Debug.LogError("You can't set a ghostable to be displaying in snap-ready mode when it's not ghosted yet! Call MakeGhost() first" );
				return;
			}

			if( isSnapped && (materialsForSnappedGhosts == null || materialsForSnappedGhosts.Length < 1 ) )
			{
				Debug.LogError("You have no materials assigned for snapped ghosts, cannot set rendering mode to snapped-ghost" );
				return;
			}

			_isRenderingSnapped = isSnapped;

			Renderer _renderer = GetComponent<Renderer>();
				
			if( _renderer != null )
			{
				if( _isRenderingSnapped )
					_renderer.materials = materialsForSnappedGhosts;
				else
					_renderer.materials = materialsForPlainGhosts;
			}
		}

		public void UnGhost()
		{
			if( !isGhosted )
			{
				Debug.LogWarning("Can't un-ghost, not currently ghosted: "+name );
			}
			else
			{
				isGhosted = false;

				/** UNDO: Remove the physics so it doesnt affect the world */
				GameObject root = gameObject;
				while( root.transform.parent != null )
					root = root.transform.parent.gameObject;

				foreach( Collider c in root.GetComponentsInChildren<Collider>() )
					c.enabled = true;

				/** UNDO: Make it spooky */
				Renderer[] rs = _wasGhostedRecursively ? GetComponentsInChildren<Renderer>() : new Renderer[] { GetComponent<Renderer>() };
				foreach( Renderer _renderer in rs )
				{
					_renderer.materials = _originalMaterialsPerChildRenderer[_renderer];
					_originalMaterialsPerChildRenderer.Remove( _renderer );
				}
			}
		}
	}
}