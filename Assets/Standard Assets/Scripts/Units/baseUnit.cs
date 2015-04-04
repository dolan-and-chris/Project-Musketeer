using UnityEngine;
using System.Collections;

public class baseUnit : MonoBehaviour {
	public bool selected = false;
	public int team = 0;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (selected) {
			gameObject.renderer.material.color = Color.red;
		} else {
			gameObject.renderer.material.color = Color.white;
		}
	}
}
