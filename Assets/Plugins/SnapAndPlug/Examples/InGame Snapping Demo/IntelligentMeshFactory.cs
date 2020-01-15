using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0642 // This warning is almost always wrong (and is wrong in every time it appears in this class)

/**
 * Convenience methods on Unity Meshes that ought to be in Unity's Mesh class
 * but are missing.
 */
public class IntelligentMeshFactory
{
	public List<Vector3> vs = new List<Vector3>();
	public List<Vector2> uvs = new List<Vector2>();
	public List<int> triIndices = new List<int>();

	public Mesh newMesh
	{
		get
		{
			Mesh m = new Mesh();
			OverwriteMesh( m );

			return m;
		}
	}

	public void Clear()
	{
		vs.Clear();
		uvs.Clear();
		triIndices.Clear();
	}

	public void OverwriteMesh( Mesh m )
	{
		m.vertices = vs.ToArray();
		m.triangles = triIndices.ToArray();
		if( uvs.Count > 0 )
			m.uv = uvs.ToArray();
	}
	public void AddVertex( Vector3 v )
	{
		vs.Add( v );
		if( uvs != null )
			uvs.Add( Vector2.zero );
	}

	public void AddVertex( Vector3 v, Vector2 uv )
	{
		vs.Add( v );
		uvs.Add( uv );
	}

	public void AddVertexUVFromWorldSpace( Vector3 v )
	{
		Vector2 _uv =  new Vector2( v.x, v.z );

		AddVertex( v, _uv );
	}

	public void AddTriangle( Vector3 v1, Vector3 v2, Vector3 v3 )
	{
		triIndices.Add( vs.Count );
		vs.Add( v1 );
		triIndices.Add( vs.Count );
		vs.Add( v2 );
		triIndices.Add( vs.Count );
		vs.Add( v3 );
	}

	public void AddLine( Vector3 start, Vector3 end, float lineWidth, Vector3 normalVector, bool UVs0to1 = false )
	{
		List<Vector3> outers = new List<Vector3>();
		List<Vector3> inners = new List<Vector3>();

		Vector3 orthVector = (Vector3.Cross( (end - start), normalVector /**up makes lines flat in Y; right makes them vertical*/ )).normalized;
		int numSamples = 2;
		for( int i = 0; i < numSamples; i++ )
		{
			outers.Add( i / (float) (numSamples - 1) * (end - start) + start + lineWidth * 0.5f * orthVector );
			inners.Add( i / (float) (numSamples - 1) * (end - start) + start - lineWidth * 0.5f * orthVector );
		}

		//List<int> generatedTris = HexMeshUtilities.AddStripBetweenLinesOfPoints( vs, uvs, outers, inners );
		AddRibbon( inners, outers, !UVs0to1 );
	}

	public void AddRibbon( List<Vector3> innerPoints, List<Vector3> outerPoints, bool UVsInWorldSpace = true )
	{
		List<int> tris = new List<int>();
		int trisOffsetIntoVerts = vs.Count;

		int iMultiplier = UVsInWorldSpace ? 2 : 4; // to make UVs be in local space, we have to duplicate two points extra each time around, i.e. add "2xi", i.e. add to to i-multiplier

		if( UVsInWorldSpace )
			AddVertexUVFromWorldSpace( outerPoints[0] );
		else
			AddVertex( outerPoints[0], new Vector2( 0, 0 ) );


		if( UVsInWorldSpace )
			AddVertexUVFromWorldSpace( innerPoints[0] );
		else
			AddVertex( innerPoints[0], new Vector2( 1, 0 ) );

		for( int i = 0; i<outerPoints.Count-1; i++ )
		{
			if( UVsInWorldSpace )
				AddVertexUVFromWorldSpace( outerPoints[i+1] );
			else
				AddVertex( outerPoints[i+1], new Vector2( 0, 1 ) );

			triIndices.Add( trisOffsetIntoVerts + iMultiplier*i+1 );
			triIndices.Add( trisOffsetIntoVerts + iMultiplier*i );
			triIndices.Add( trisOffsetIntoVerts + iMultiplier*i+2 );

			if( UVsInWorldSpace )
				AddVertexUVFromWorldSpace( innerPoints[i+1] );
			else
				AddVertex( innerPoints[i+1], new Vector2( 1, 1 ) );


			triIndices.Add( trisOffsetIntoVerts + iMultiplier*i+1 );
			triIndices.Add( trisOffsetIntoVerts + iMultiplier*i+2 );
			triIndices.Add( trisOffsetIntoVerts + iMultiplier*i+3 );

			if( UVsInWorldSpace )
				; // No more work needed
			else
			{
				/** Stupid UVs: we have to duplicate each pair of end-points to give them unique UV coords */

				AddVertex( outerPoints[ i+1 ], new Vector2( 0, 0 ) );
				AddVertex( innerPoints[ i+1 ], new Vector2( 1, 0 ) );

				// i will now be 2 larger than expected with simpler algorithm :(
			}
		}
	}
}