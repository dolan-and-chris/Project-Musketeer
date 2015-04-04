using UnityEngine;
using System.Collections;

public class Cliff : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static void setHillPatch (int x, int z, float height, int spread) 
	{
		float absHeight = Mathf.Abs(height);
		float terrainhHeight = TerrainGenerator.HeightMap[1,1];
		//Critical value, try changing this. It will change the resolution of the hill. This took a ton of thinking to work out lol.
		float resolution = 50;
		float radiusReduction = spread/resolution;
		float heightDifference = Mathf.Abs(absHeight - terrainhHeight);
		float cakeSliceSize = heightDifference/resolution;
		//The spread of a hill is how much it is spread across. It first sets the current radius to be the spread, then decreases the redius, bringing the hill upwwards.
		int currentRadius =  spread;
		
		for(float i = 0f; i<absHeight; i+=cakeSliceSize) {
			setCircularCliffPatch(x,z,currentRadius,2048,terrainhHeight,terrainhHeight-0.01f);
			if(height < 0) 
			{ 
				terrainhHeight -= cakeSliceSize; ;
			} 
			if(height > 0) 
			{
				terrainhHeight += cakeSliceSize;
			}
			currentRadius -= (int)radiusReduction;
		}
	}
	
	/*
	 * Sets a patch of terrain to a different height to the rest of the terrain. Useful for creating mountains and valleys.
	 * The core code is to be used for splitting up the terrain into segments, so that different terrain details
	 * can be applied to different parts of the map
	 * LIMITATION: Only allows for squares or rectangles. 
	 * */	
	public static int[][] setCliffPatch (int x1, int y1, int x2, int y2, float lowerBound, float upperBound) 
	{	
		int[][] coordinates = new int[4][];
		coordinates[0] = new int[] {x1,y1};
		coordinates[1] = new int[] {x2, y2};
		coordinates[2] = new int[] {x1, y2};
		coordinates[3] = new int[] {x2, y1};
		
		for(int y = y1; y <= y2; y++) {
			for (int x = x1; x <= x2; x++) {
				float val = Random.value;
				TerrainGenerator.HeightMap[x,y] = val;
				if(val>upperBound) TerrainGenerator.HeightMap[x,y] = upperBound;
				if(val<lowerBound) TerrainGenerator.HeightMap[x,y] = lowerBound;
			}
		}
		return coordinates;
	}		
	
	// This does the same job as the function above, but generates a circular cliff instead of square.
	// x and z coordinates specify the centre of the circle.
	// High radii might cause visual issues. This is because of the line:
	// 			for(float theta = 0; theta < 2*(Mathf.PI); theta += 2*(Mathf.PI/1024))
	// The step of PI/1024 may still be too big, so might need smaller values for large radii, such as
	// PI/2048, PI/4096. Can add this in eventually	
	public static int[] setCircularCliffPatch(int x, int z, int radius, int subdivisions, float upperBound, float lowerBound) 
	{
		//Many circles need to be generated - with a radius between zero and the argument radius.
		while(radius > 0) 
		{
			//For angles between 0 and 2PI radians, incrementing by PI/subdivision argument each time.
			for(float theta = 0; theta < 2*(Mathf.PI); theta += 2*(Mathf.PI/subdivisions)) 
			{
				float randomseed1 = Random.value*10;
				float randomseed2 = Random.value*10;
				int xpoint = (int)(x + radius*(Mathf.Cos (theta)) + randomseed1);
				int zpoint = (int)(z + radius*(Mathf.Sin (theta))+ randomseed2);
				
				float val = Random.value;
				if(val > upperBound) val = upperBound;
				if(val < lowerBound) val = lowerBound;
				
				
				TerrainGenerator.HeightMap[xpoint,zpoint] = val;
				
			}
			radius--;
		}
		return new int[]{x,z};
	}
	
	

	

	

	
	/*
	 * This function CONTROLS the drawing of a series of quadratic curves, which form a cliff. Eventually, it'd be cool to introduce
	 * cubic curves, or even higher power curves as this gives the possibility for more complex structures. At the moment it is not
	 * possible to rotate the cliffs (in the x-z plane) generated here, but this will be easilyimplemented in the future using the 
	 * trigonometric functions. The random map generator could use some random combination of the arguments listed below to generate
	 * natural looking cliffs. 
	 * 
	 * Bear in mind, currently, SMALL CHANGES to the arguments below cause BIG changes on screen. This is because of the complexity
	 * of sketching many quadratic curves, each of which is slightly differet. Try to imagine the current cliff you see on the scene, 
	 * if it was to be tessalated, rotated, and perhaps linked to cliffs made with the other functions (circular patch and square patch)
	 * 
	 * 
	 * 
	 * Arguments:
	 * 
	 * x0: The X position of the bottom of the U shaped curve
	 * z0: The Z Position of the bottom of the U shaped curve
	 * x: The initial x value plugged into the equation
	 * maxx: The maximum possible x value. This defines the domain of the function.
	 * iterations: The number of individual curves to draw
	 * multiplier: Transformations to the x axis. For example y = (0.5*x)^2, is a stretch by x2.
	 * height: The initial height of the cliff
	 * falloff: After drawing 'falloff' number of curves, the height of each curve after this, begins to decrease.
	 * gradient: The gradient of the slope between the top of the cliff, and the bottom. Negative for downward slopes.
	 * 
	 */
	public static void quadraticCliffPatchController(int x0, int z0, int x, int maxx, int iterations, float multiplier, float height, int falloff, float gradient) 
	{
		//Count the total number of times the while loop below has executed
		int count = 0;
		
		//iterations decreases on each iteration of the loop.
		while(iterations > 0) 
		{
			//If the counter is higher than falloff, start in/de/creasing the height of each subsequent curve by the value of gradient.
			if(count > falloff)   height+=gradient;
			
			//Draws a single curve, with "bottom" coordinates x0,z0, etc.
			setQuadraticCliffPatch (x0,z0,x,maxx,multiplier,height);
			
			//Decrease the multiplier each time, resulting in wider (more stretched) curves
			if(multiplier>0.1) multiplier-=0.005f;
			
			//Increase the z0 value of the next curve - making it 3 units further in z.
			z0+=3;
			
			//Increase the max possible x value by 1
			maxx+=1;
			
			// <captainObvious> Decrement iterations, increment count, </captainObvious>
			iterations--;
			count++;
			
		}
	}
	
	/*
	 * 
	 * This function is called by the quadraticCliffPatchController. NOT TO BE CALLED DIRECTLY.	Recursive.
	 * Recursively draws each section of a particular curve. Arguments have the same meaning as for
	 * the controller, but should change on each iteration of controller, which is why this function should
	 * not be called directly. 
	 * 
	 * MAKE _SURE_ this function is always called such that x < xmax, otherwise there will be an infinite
	 * loop.
	 * 
	 */
	
	public static void setQuadraticCliffPatch(int x0, int z0, int x, int maxx, float multiplier, float height) 
	{
		//If we have reached our max possible xvalue, give up. Prevents infinite loop.
		if(x==maxx) return;

		int z=-1;
		
		//Looks complex, but is actually just the quadratic forumla, of the form: y = (z-a)^2 + b
        z = (int)((multiplier*(x-x0))*(multiplier*(x-x0)))+z0;

		//Set the height at the current point to the height specified in the argument. 
		TerrainGenerator.HeightMap[x,z] = height;
		
		//Recurse, this time with x=x+1. 
		setQuadraticCliffPatch (x0,z0,x+1,maxx,multiplier,height);
	
	}	

	
	
}
