using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Terrain terrain = gameObject.GetComponent<Terrain>();
		TerrainGenerator generator = new TerrainGenerator(terrain);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
