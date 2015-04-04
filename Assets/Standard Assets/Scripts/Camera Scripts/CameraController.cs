using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	
	int team = 0;                         	//Team the player resides on
	int alliance = 0;					  	//The alliance player is on
	Vector3 selection_Start;  //The World position when left mouse down
	RaycastHit hit_Mouse;	  //Used for box seelction, position when dragged
	Vector2 start_Mouse;      //Screen coordinates when left mouse down
	//float xUnitper3DUnit;     //Number of X Screen coordinates per World coordinate
	//float yUnitper3DUnit;     //Number of Y Screen coordinates per World coordinate
	bool selectionEnded = false;
	bool dragging = false;
	UnitManager unitManager;
	
	// Use this for initialization
	void Start () 
	{
		unitManager = new UnitManager();
		//when the game starts, the camera will be pointing at the Town Center
		Vector3 townCenter = new Vector3(530,900,520);
		Quaternion rotation = Quaternion.Euler(45f, 45f, 0f);
		transform.position = townCenter;
		transform.rotation = rotation;
	}
	
	// Update is called once per frame
	void Move(float x, float y, float z)
	{
		Vector3 dir = new Vector3(x,y,z);
        transform.Translate(dir, Space.World);
	}
	
	void Update () 
	{
		if (!dragging)
		{
			checkPanning(15, 500);
		}
		checkZooming(500,1200);				//first and second args are the min and max zooms respectively.
		checkRotating(45,30);				//first arg is the viewing angle, second arg is how much it will rotate by. However, ATM it only works for 45 and 30.
		trackMouse();
		if(Input.GetMouseButtonDown(0))
		{
			singleUnitSelection();
		}
		multipleUnitSelection();
	}
	
	void checkPanning(int scrollDistance, float scrollSpeed)
	{
		float totalSpeed = scrollSpeed * Time.deltaTime;
		float mousePosX = Input.mousePosition.x;
		float mousePosY = Input.mousePosition.y;
		Vector3 right = new Vector3(1,0,0);

		if (mousePosX < scrollDistance) 
		{ 
			Move(-totalSpeed,0,totalSpeed);
		} 
		if (mousePosX >= Screen.width - scrollDistance) 
		{
			Move(totalSpeed,0,-totalSpeed);
		}

		if (mousePosY < scrollDistance) 
		{ 
			Move(-totalSpeed,0,-totalSpeed);
		} 
		if (mousePosY >= Screen.height - scrollDistance) 
		{ 
			Move(totalSpeed,0,totalSpeed);
		}
	}
	
	void checkZooming(int minZoom, int maxZoom) 
	{
		float minZoomAmount = 0.5f;       	//A buffer variable to clean the end of Zooming
		int zoomFactor = 4;               	//Imitates decelerating by making delta smaller
		float pos = 0;
		if (Input.GetAxis("Mouse ScrollWheel") > 0) 
		{
			if(transform.position.y <= minZoom+minZoomAmount)
			{    													//If height less than buffer amount + minimum height
            	pos = minZoom-transform.position.y;               	//Then set height to minimum height
			}
			else
			{
				pos = (minZoom-transform.position.y)/zoomFactor;
			}
		}
		
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{               											//Same as zoom in except it checks the maximum height
            if(transform.position.y >= maxZoom-minZoomAmount)
			{
                pos = maxZoom-transform.position.y;
            }
            else
			{
            	pos = (maxZoom-transform.position.y)/zoomFactor;  //Multiplied by 2 so that the player reaches the top quickly
            }
		}
        //Translate Y amount
        Move(0,pos,0);
        //Recalculate screen conversions
        //calculateScreenConversion();			
		
	}
	
	void checkRotating(int viewAngle, int rotRange)
	{
		int angularSpeedFactor = 10;     	 	//Angular Speed to Rotate
		RaycastHit rot_hit;
		int groundLayermask = 1 << 8; //8 is the Layer of which the ground is set at. 1-7 is Unity pre-made layers.
        if(Physics.Raycast(transform.position, transform.forward, out rot_hit, Mathf.Infinity,groundLayermask))
		{
        	//Cast a Ray to get a point to Rotate around
            Vector3 rotPoint = rot_hit.point;
            float rotAmount = 0;
            float angle = transform.eulerAngles.y;    //Current Angle in Euler coordinates
            if(angle > viewAngle-1 && angle < viewAngle+1)
			{    								//If Angle is somehow negative and small set it to 45
            	angle = viewAngle;
            }
			float tempangle1 = angle;
			if(Input.GetKey("q"))
			{
            	if(tempangle1 >= (viewAngle-rotRange))
				{    																	//If less than rotRange than rotate towards it
               		rotAmount = (viewAngle-rotRange-tempangle1)/angularSpeedFactor;    	//Decelerating amount
                }
            }
            else if(tempangle1 <= viewAngle && !Input.GetKey("e"))
			{    																		//If it is not idle and E is not pressed
				rotAmount = (viewAngle-tempangle1)/angularSpeedFactor;                	//Then Rotate back to start
            }

       
            //Right
            float tempangle2 = angle;
            if(Input.GetKey("e"))
			{
            	if(tempangle2 <= viewAngle+rotRange)
				{
                	rotAmount = (105-rotRange-tempangle2)/angularSpeedFactor;
                }
            }
            else if(tempangle2 >= viewAngle && !Input.GetKey("q"))
			{
            	rotAmount = (viewAngle-tempangle2)/angularSpeedFactor;
            }
                
            //Rotate the camera around the given (point, axis, amount)
            transform.RotateAround(rotPoint, Vector3.up, rotAmount);
        }
		
		
	}
	
	void trackMouse() 
	{
		if(Input.GetMouseButtonDown(0))
		{
			selectionEnded = false;
			start_Mouse = Input.mousePosition;
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			int groundLayermask = 1 << 8;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayermask))
			{
				selection_Start = hit.point;    //Store initial hit
				clearSelected();
			}
			
		}
		if(Input.GetMouseButtonUp(0))
		{
			dragging = false;
			selectionEnded = true;
		}
	}
	
	void singleUnitSelection()
	{
		unitManager.singleUnitSelection(camera.ScreenPointToRay(Input.mousePosition));
	}
	
	void multipleUnitSelection() 
	{
		if(Input.GetMouseButton(0)) {
        	/*if(Vector2.Distance(start_Mouse, Input.mousePosition) > 10) {    //Buffer added so it doesn't interfere wih single Selection
            	selectionEnded = false;
                //Cast Ray of current mouse position on Ground
            	Ray ray_Mouse = camera.ScreenPointToRay(Input.mousePosition);
            	RaycastHit hit_Mouse;
           		if(Physics.Raycast(ray_Mouse, out hit_Mouse, Mathf.Infinity, 1<<8)) {
                	clearSelected();
					
                	//Set size of Selection Box
					Vector3 localScale = new Vector3(100,1,hit_Mouse.point.z-selection_Start.z);
                	transform.Find("Selection_Box").localScale = localScale;
					
                	//Set position of Selection Box to account for the size
					Vector3 position = new Vector3(selection_Start.x, selection_Start.y, selection_Start.z+(transform.Find("Selection_Box").transform.lossyScale.z/2));
                	transform.Find("Selection_Box").position = position;
					
                	//Sweep the Selection Box in the direction and get colliders
					Vector3 sweep = new Vector3(hit_Mouse.point.x-selection_Start.x,0,0);
                	RaycastHit[] temp = transform.Find("Selection_Box").rigidbody.SweepTestAll(sweep, Mathf.Abs(hit_Mouse.point.x-selection_Start.x));
					
                	for(int i=0;i<temp.Length;i++) {
                		//Test if every collider is a Unit and on the same Team
                		if(temp[i].transform.GetComponent<baseUnit>() != null && temp[i].transform.GetComponent<baseUnit>().team == team) {
                			addSelectedUnit(temp[i].collider.gameObject);
                			temp[i].transform.GetComponent<baseUnit>().selected = true;
                		}
                	}
           		}
        	}*/
			//hit_Mouse = unitManager.multipleUnitSelecton(camera.ScreenPointToRay(Input.mousePosition),selection_Start, hit_Mouse.point);
			Ray ray_Mouse = camera.ScreenPointToRay(Input.mousePosition);
           	if(Physics.Raycast(ray_Mouse, out hit_Mouse, Mathf.Infinity, 1<<8)) {
				unitManager.SelectUnitsInArea(selection_Start, hit_Mouse.point);
			}
        }
		
		/*if(Input.GetMouseButtonUp(0)) {
        	//Hide Selection Box off screen
          	transform.Find("Selection_Box").localScale = Vector3.up;
          	transform.Find("Selection_Box").position = Vector3.up;
        }*/
	}
	
	void drawPhysicalSelectionBox () {
		//transform.Find("Selection_Box");
		//Vector3 localScale = new Vector3(100,1,hit_Mouse.point.z-selection_Start.z);
        //transform.Find("Selection_Box").localScale = localScale;
	}
	
	void addSelectedUnit (GameObject unit) {
		unitManager.selectedUnits.Add(unit);
	}
	
	void clearSelected() {
		unitManager.clearSelected();
	}
	
	void OnGUI() {
		if(Input.GetMouseButton(0) && Vector2.Distance(start_Mouse, Input.mousePosition) > 10) {
            //Screen coordinates are bottom-left is (0,0) and top-right is (Screen.width, Screen.height)
			dragging = true;
            GUI.Box(new Rect(start_Mouse.x, Screen.height-start_Mouse.y, Input.mousePosition.x-start_Mouse.x, -(Input.mousePosition.y-start_Mouse.y)), "");//, playerGUISkin.customStyles[0]);
        }
		
		GUI.Label(new Rect(10, 10, 300, 100), "Click & drag to make a selection box");
		GUI.Label(new Rect(10, 30, 300, 100), "Selected Units:");
		for (int i = 0; i<unitManager.selectedUnits.Count; i++) {
			string tempUnit = unitManager.selectedUnits[i].ToString();
			GUI.Label(new Rect(10, (i*20+45), 300, 100), tempUnit);
		}
		GUI.Label(new Rect(10, 200, 300, 100), "All Units:");
		for (int i = 0; i<unitManager.allUnits.Count; i++) {
			string tempUnit = unitManager.allUnits[i].ToString();
			GUI.Label(new Rect(10, (i*20+215), 300, 100), tempUnit);
		}
		
		GUI.Label(new Rect(Screen.width-200, 10, 300, 100), "Selection box corners:");
		string point1 = selection_Start.ToString();
		string point2 = hit_Mouse.point.ToString();
		GUI.Label(new Rect(Screen.width-200, 30, 300, 100), point1);
		GUI.Label(new Rect(Screen.width-200, 50, 300, 100), point2);
	}
}
