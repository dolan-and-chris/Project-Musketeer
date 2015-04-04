using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour {
	
	GameObject floor;
	// Use this for initialization
	void Start () {
		
		floor = new GameObject("floor");
		
		GameObject tile = initTile(5);
		GameObject tile2 = initTile(10);
		
	    tile2.transform.position = new Vector3(0,10,0);
	}
	
	//Acts as the "constructor" for a single Tile.
	GameObject initTile(int tilesize) {
		
		GameObject obj = new GameObject("tile");
		obj.AddComponent<MeshFilter>();
		obj.AddComponent<MeshRenderer>();
		Mesh mesh = createMesh (tilesize);
		obj.GetComponent<MeshFilter>().mesh = mesh;
		obj.transform.parent = floor.transform;
		
		return obj;
		
	}
	
	//Forms the mesh from the vertices specified in generateMeshVertices
	Mesh createMesh(int size) {
		
		Mesh floorMesh = new Mesh();
		floorMesh.name = "floorMesh";
		
		floorMesh.vertices = generateMeshVertices (size);
		int number_of_vertices = floorMesh.vertices.Length;
		int[] triangles = new int[number_of_vertices];
		
		for(int i=0; i<number_of_vertices; i++) triangles[i] = i;
		//floorMesh.uv = new Vector2[]{Vector3.up, Vector3.up, Vector3.up};
		floorMesh.triangles = triangles;

		return floorMesh;
	}

	//Generates a flat square mesh, with perimeter 'size', measured in NUMBER OF SQUARES
	//where NUMBER OF SQUARES = 2*(NUMBER OF POLYGONS)
	
	Vector3[] generateMeshVertices(int size) 
	{
		Vector3[] vertices = new Vector3[size*size*6];
		
		int i = 0;
		int j = 0;
		int currentindex = 0;

			while(j < size) {
				while(i < size) {
				vertices[currentindex++] = new Vector3(i,0,j);
				vertices[currentindex++] = new Vector3(i,0,j+1);
				vertices[currentindex++] = new Vector3(i+1,0,j);
			
				vertices[currentindex++] = new Vector3(i,0,j+1);
				vertices[currentindex++] = new Vector3(i+1,0,j);
				vertices[currentindex++] = new Vector3(i+1,0,j+1);
				i++;
				}
			i=0;
			j++;
		}
		return vertices;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
