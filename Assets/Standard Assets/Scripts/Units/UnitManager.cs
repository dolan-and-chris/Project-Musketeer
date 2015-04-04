using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : Object {
	
	//WE MAY NEED A MORE ELEGANT METHOD TO COUNT SELECTED UNITS. I have an idea of cycling through all the units with 'selected=true', and counting them up this way with a for loop or something?
	public List<GameObject> selectedUnits;
	public List<GameObject> allUnits;
	
	// Use this for initialization
	public UnitManager () {
		selectedUnits = new List<GameObject>();
		allUnits = new List<GameObject>();
		searchUnits();
	}
	
	// Update is called once per frame
	void Update () {
		/*for (int i = 0; i<selectedUnits.Count; i++) {
			selectedUnits[i].GetComponent<baseUnit>().selected = true;
		}*/
	}
	
	public void searchUnits () {
		foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject))) {
			if (obj.GetComponent<baseUnit>() == true) {
				allUnits.Add(obj);
			}
		}
	}
	
	public void addAllUnit(GameObject unit) {
		allUnits.Add (unit);	
	}
	
	public void addSelectedUnit(GameObject unit) {
		selectedUnits.Add (unit);
	}
	
	public void clearSelected()
	{
		if(!Input.GetKey("left shift") && !Input.GetKey("right shift")) {
        	for(var i=0; i<selectedUnits.Count;i++) {
  	        	selectedUnits[i].GetComponent<baseUnit>().selected = false;
            }
			selectedUnits.Clear();
		}
	}
	
	public RaycastHit singleUnitSelection(Ray cameraToScene)
	{
		RaycastHit hit;
		//Cast Ray to Select Unit
		int unit_Layermask = 1 << 9; //9 is the layer of which the units are set at.
		if(Physics.Raycast(cameraToScene, out hit, Mathf.Infinity, unit_Layermask))
		{
			//Store Unit in Selectables
			clearSelected();
			hit.collider.GetComponent<baseUnit>().selected = true;
			addSelectedUnit(hit.collider.gameObject);
		}
		//Didn't click on Ground
		else
		{
			clearSelected();
		}
		return hit;
	}
	
	
	public RaycastHit multipleUnitSelecton(Ray cameraToScene, Vector3 point1, Vector3 point2) {
		RaycastHit hit;
		if(Physics.Raycast(cameraToScene, out hit, Mathf.Infinity, 1<<8)) {
			SelectUnitsInArea(point1, point2);
		}
		return hit;
	}
	
	public void SelectUnitsInArea(Vector3 point1, Vector3 point2) {
		if (point2.x < point1.x) {
			// swap x positions. Selection rectangle is being drawn from right to left
			float x1 = point1.x;
			float x2 = point2.x;
			point1.x = x2;
			point2.x = x1;
		}
	
		if (point2.z > point1.z) {
			// swap z positions. Selection rectangle is being drawn from bottom to top
			float z1 = point1.z;
			float z2 = point2.z;
			point1.z = z2;
			point2.z = z1;
		}
	
		for (int i = 0; i<allUnits.Count; i++) {
			Vector3 unitPos = allUnits[i].transform.position;
			if (unitPos.x > point1.x && unitPos.x < point2.x && unitPos.z < point1.z && unitPos.z > point2.z) { //if in the box bounds, which is wrong cos its not a diagonal box
			//if (unitPos.x < point1.x && unitPos.x > point2.x && unitPos.z < point1.z && unitPos.z > point2.z) {
				if (!selectedUnits.Contains(allUnits[i])) {
					selectedUnits.Add(allUnits[i]);
					allUnits[i].GetComponent<baseUnit>().selected = true;
				}
			}
		}
	}
}
