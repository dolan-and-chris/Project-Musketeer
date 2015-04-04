using UnityEngine;
using System.Collections;

public class Initialisation : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GameObject RTSCamera = (GameObject) Instantiate (Resources.Load ("RTSCamera"),transform.position,Quaternion.identity);
		RTSCamera.AddComponent("CameraController");
		UnitManager unitManager = new UnitManager();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
