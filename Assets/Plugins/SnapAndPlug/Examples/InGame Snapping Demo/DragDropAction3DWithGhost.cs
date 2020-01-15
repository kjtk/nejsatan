using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SnapAndPlug;

public class DragDropAction3DWithGhost : DragDropAction3D
 {
  public DragDropAction3DWithGhost(Material m, DragDropSnapMode sm, float maxSnapDist, bool unGroupOnDrag = false) : base(m, sm, maxSnapDist, unGroupOnDrag )
  {
   
  }
  
  public override void OnCreatedPreviewObject(GameObject previewObject)
  {
   SnapGhostable ghostable = previewObject.AddComponent<SnapGhostable>();
   ghostable.ghostRecursively = true;
   ghostable.materialsForPlainGhosts = new Material[] { materialForGhostedObjects };
   ghostable.MakeGhost();		
  }
}
