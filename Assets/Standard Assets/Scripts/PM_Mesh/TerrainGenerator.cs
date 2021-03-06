using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
	
	Random random;
	public static float[,] HeightMap;
	float TERRAIN_HEIGHT;
	TerrainData td;
	
	public TerrainGenerator(Terrain terrain) 
	{

		td = terrain.terrainData;
		terrain.detailObjectDistance = 2000;
		//Creates a new 2D array, the same size resolution as the heightmap.
		HeightMap = new float[td.heightmapWidth, td.heightmapHeight];
		//Sets the renderer as the texture
		//renderer.material.mainTexture = texture;
		

		td.splatPrototypes = initialiseSplatPrototypes();
		initialiseHills ();
		Average (45, td.heightmapWidth, td.heightmapHeight);
		
		
		int[][] patchcoords = Cliff.setCliffPatch(200,200,240,270, 0.7f, 0.71f); 
		Cliff.setHillPatch (500,700, -0.3f,100);
		setPondPatch(800,400, 0.3f,100);
		//cliffCircleCutout (patchcoords, 25);
		
		int[] circlecoords = Cliff.setCircularCliffPatch(700,400,100, 2048, 0.75f,0.74f);
		TERRAIN_HEIGHT = HeightMap[1,1];
		
		//setQuadraticCliffPatch(400,300,350,450);
		
		Cliff.quadraticCliffPatchController(400,300,370,430,88,0.1f,0.9f,15,-1*0.005f);
		
		//createPathToCliff (700,400,120,Mathf.PI,HeightMap[5,5],true);
		//createPathToCliff (700,400,100,Mathf.PI*2,HeightMap[5,5],true);
		Average (4, td.heightmapWidth, td.heightmapHeight);

		// Sets the heightmap of the terrain equal to the height map we just generated.
		td.SetHeights (0,0,HeightMap);
	
		
	}
	
	private SplatPrototype[] initialiseSplatPrototypes() 
	{
		SplatPrototype[] terrainSplat = new SplatPrototype[4];
		terrainSplat[0] = new SplatPrototype();
		terrainSplat[1] = new SplatPrototype();
		terrainSplat[2] = new SplatPrototype();
		terrainSplat[3] = new SplatPrototype();
		
		//Sets the "SplatProrotype" as unity like to call it (bascially texture) with the noise texture generated by the function
		//I got help from http://answers.unity3d.com/questions/50958/adding-a-texture-to-a-code-generated-terrain-throu.html
		

		terrainSplat[0].texture = createNoiseTexture(td.heightmapWidth, td.heightmapHeight, 0.69f, 6.18f, 8.379f, 0.75f, 0.9f);
		terrainSplat[0].tileSize = new Vector2(15, 15); 
		terrainSplat[0].texture.Apply(true);	
		
		return terrainSplat;
	}
	
	private void initialiseHills() 
	{
		//Iterate through each coordinate of the height map
		for(int x = 0; x < td.heightmapWidth; x++) 
		{	
			for(int y=0; y<td.heightmapHeight; y++) 
			{
				// Generate a random number (between 0 and 1)
				
				float val = Random.value;
				HeightMap[x,y] = val;
				/*
				  This block of code is optional - clamps the range of possible height map values between 
				  some upper bound and lower bound. You can mess with these values, or remove them, to get
				  various different results. Generally, having the numbers closer together, will make the world
				  flatter. Changing this value, along with the value in the average function coming next, can massively
				  alte r the look of the terrain.
				*/
				
				const float UPPER_BOUND = 0.7f;
				const float LOWER_BOUND = 0.3f;
				
				if(val>UPPER_BOUND) HeightMap[x,y] = UPPER_BOUND;
				if(val<LOWER_BOUND) HeightMap[x,y] = LOWER_BOUND;
				
			}
		}		
	}
	
	
	// Use this for initialization

	
	/*
	 * Sets a patch of terrain to a different height to the rest of the terrain. Useful for creating mountains and valleys.
	 * The core code is to be used for splitting up the terrain into segments, so that different terrain details
	 * can be applied to different parts of the map
	 * LIMITATION: Only allows for squares or rectangles. 
	 * */
	

	
	void setPondPatch (int x, int z, float depth, int spread)
	{
		Vector3 waterVector = new Vector3(x,z,0);
		Cliff.setHillPatch (x,z,depth,spread);

		Vector3 position = new Vector3(x+530,281,z+520);
		GameObject pond = (GameObject) Instantiate (Resources.Load ("waterPond"),position,Quaternion.identity);

	}
	
	
    Texture2D createNoiseTexture(int heightmapWidth, int heightmapHeight, float h, float lacunarity, float octaves, float offset, float scale)
	{
		//Creates a 2D Texture
		Texture2D texture = new Texture2D(heightmapWidth, heightmapHeight, TextureFormat.RGB24, false);
		//Perlin and FractalNoise are a custom class in Perlin.cs
		Perlin perlin = new Perlin();
		FractalNoise fractal = new FractalNoise(h, lacunarity, octaves, perlin);
		
		for (int y = 0; y<heightmapHeight; y++)
		{
			for (int x = 0; x<heightmapWidth; x++)
			{
				float value = fractal.HybridMultifractal(x*scale, y * scale, offset);
				Color colour = new Color(value/10, value, value/3, value);
				texture.SetPixel(x, y, colour);
			}	
		}
		texture.Apply();
		return texture;
	}
	
	
     void paintGrass(TerrainData terrainData, int x, int y)
	 {
		WWW www = new WWW("file://"+Application.dataPath+"/Standard Assets/Terrain Assets/Terrain Grass/Grass.png");
		//yield return www;
		Texture2D grass = www.texture; 

		DetailPrototype[] details = new DetailPrototype[1];
		details[0] = new DetailPrototype();
		details[0].prototypeTexture = grass;
		details[0].minHeight = 5;
		details[0].minWidth = 5;
		details[0].renderMode = DetailRenderMode.Grass;
		
		terrainData.detailPrototypes = details; //This is good i think
		
		//int[,] detailLayer = terrainData.GetDetailLayer(0,0,terrainData.detailWidth,terrainData.detailHeight,0);
		//detailLayer[10,10] = 8;
		int[,] detailLayer = new int[terrainData.detailWidth, terrainData.detailHeight];
		
		for(int i=0; i<terrainData.detailWidth; i++) 
		{
			for(int j=0; j<terrainData.detailHeight; j++)
			{
				if(i%25 == 0 || j%25 == 0 || i % 13 == 0 || j % 13 == 0) detailLayer[i,j] = 1;
			}
		}

		terrainData.SetDetailLayer(0,0,0,detailLayer);
		
		
		
		
		/*
		// read all detail layers into a 3D int array:
		int numDetails = terrainData.detailPrototypes.Length; //this is also ok
		print (numDetails);
		int [,,] detailMapData = new int[terrainData.detailWidth, terrainData.detailHeight, numDetails];
		print (detailMapData);
		for (int layerNum=0; layerNum < numDetails; layerNum++) 
		{
    		int[,] detailLayer = terrainData.GetDetailLayer(x, y, terrainData.heightmapWidth, terrainData.heightmapHeight, layerNum);
		}

		// write all detail data to terrain data:
		for (int n = 0; n < detailMapData.Length; n++)
		{
    		terrainData.SetDetailLayer(0, 0, 1, detailMapData);
		}
		*/
	}
	
	
	/*
	 * 
	 This function averages (smooths) the terrain. The arguments are the number of iterations on which to run the average operation,
     the original (unaveraged) height map, along with the width and height of the terrain's heightmap object.
	
	 This function works by finding the MATHEMATICAL AVERAGE of each element of the HeightMap. For example, for a given coordinate
	 [x,y], the values of [x+1,y], [x-1,y], [x,y+1], [x, y-1], [x+1,y+1], [x-1,y-1], [x+1,y-1], [x-1,y+1] IF they exist (ie, for the
	 top left square, [x-1,y] will not exist. The value at the current square is then set to the mathematical average of surrounding
	 squares, and this process is repeated for each square.
	 
	 This WHOLE PROCESS is then repeated the number of times specified by the 'iterations' argument - finding the average of the average,
	 then the average of the average of the average etc. This is why more iterations = smoother, flatter terrain.
	 *
	 */
	float[,] Average(int iterations, int heightmapWidth, int heightmapHeight) 
	{
		// While there are still iterations to process...
		while(iterations>0){
			
			// For each value of height map
			for(int x = 0; x < heightmapWidth; x++)
			{
				for(int y=0; y<heightmapHeight; y++) 
				{
					//[x+1,y], [x-1,y]
					float nextx = 0;
					float prevx = 0;

					//[x,y+1], [x,y-1]
					float nexty = 0;
					float prevy = 0;
				
					//[x+1,y+1], [x-1,y-1]
					float nextxnexty = 0;
					float prevxprevy = 0;
					
					//[x+1,y-1], [x-1,y+1]
					float nextxprevy = 0;
					float prevxnexty = 0;
				
					//The count of the number of values specified above which exist for a given [x,y]. Initially 0, as
					//we haven't checked any of the surrounding values yet.
					int count = 0;
				
					//Populates the variables above, increases the value of count if each condition is true.
					if(x>0){ prevx = HeightMap[x-1,y]; count++;}
					if(y>0){ prevy = HeightMap[x,y-1]; count++;}
				
					if(x<heightmapWidth-1) { nextx = HeightMap[x+1,y]; count++;}
					if(y<heightmapHeight-1){nexty = HeightMap[x,y+1]; count++;}
				
			   	    if(x>0 && y > 0){ prevxprevy = HeightMap[x-1,y-1]; count++;}
				    if(x<heightmapWidth-1 && y < heightmapHeight-1){ nextxnexty = HeightMap[x+1,y+1]; count++;}
				
				    if(x>0 && y<heightmapHeight-1) {prevxnexty = HeightMap[x-1,y+1]; count++;}
				    if(x<heightmapWidth-1 && y >0) {nextxprevy = HeightMap[x+1,y-1]; count++;}
			
					//Calculates the mathematical average of all surrounding values (count variable is needed so we know
					//which number by which to divide the average).
					float sum = nextx + prevx + nexty + prevy + nextxnexty + prevxprevy + nextxprevy + prevxnexty;
					float average = sum/count;
				
					//Sets the current [x,y] equal to the average of all surrounding values.
					HeightMap[x,y] = average;
				}
			}
		iterations--;
		}
		
		return HeightMap;
	}

	// Update is called once per frame
	void Update () {
	
	}
	
	void Start() { }
}




/*
 * 
 **********************************************************************************************************************
 CODE BELOW THIS IS DUMPED CODE - NOT NEEDED ANYMORE, BUT KEPT INCASE SOME PARTS OF IT CAN BE REUSED FOR FUTURE WORK  *
 **********************************************************************************************************************
 *
 */



	/*
	 * 
	 * Generates a path of traversable terrain from the flat ground terrain to a given cliff. RECURSIVE, UNFINISHED.
	 * 								(FAIL - Keep for reference)
	 */	
	 /*
	void createPathToCliff(int x, int z, int radius, float theta, float groundheight, bool firstrun) 
	{	
		
		//Gets the height of the ground at the top center of the cliff.
		float cliffheight = HeightMap[x,z];
		
		//Sets the height of the new cliff, equal to this percentage of the original cliff
		float newcliffheight = (cliffheight-0.02f);

		//Gives the center of a new circle, on the edge of the original circle, in a direction determined by theta.
		// 0 <= theta <= 2*PI. Theta is generated randomly.
		
		float val = Random.value;
		float newtheta = 0;
		newtheta = theta > Mathf.PI ? theta - (Mathf.PI/2)*val : theta + (Mathf.PI/2)*val;

		int xpoint = (int) (x + radius*(Mathf.Cos (newtheta)));
		int zpoint = (int) (z + radius*(Mathf.Sin (newtheta)));
		float newrs = radius*1.2f;
		int newradius = (int) newrs;
		
		
		if(firstrun) {newradius = 20;
			//setCircularCliffPatch ((int)(xpoint+70*Mathf.Cos (newtheta)), (int)(zpoint+70*Mathf.Sin (newtheta)), newradius, 400, newcliffheight, newcliffheight-0.01f);
		}
		
		
		setCircularCliffPatch(xpoint, zpoint, newradius, 400, newcliffheight,newcliffheight-0.01f);
		if(newcliffheight < groundheight + 0.05 && newcliffheight > groundheight - 0.05) return;
		
		createPathToCliff (xpoint,zpoint,newradius,newtheta,groundheight, false);
		createPathToCliff (xpoint,zpoint,newradius,(2*Mathf.PI)-newtheta,groundheight,false);
	}
	
	
	
	
	
	*/
	/*
	 * 
	 * Cuts out a circular segment of cliff, given each of the coordinates of the corners of the cliff.
	 * This currently only works on Square cliffs, but will be changed in the future.
	 * 
	 */

/*
 *
	void cliffCircleCutout(int[][] coordinates, int radius) 
	{
		int pointx = coordinates[0][0];
		int pointz = coordinates[0][1];
		float cliffheight;
		
		int height1x = coordinates[0][0] + 10;
		int height1z = coordinates[0][1] + 10;
		
		int height2x = coordinates[0][0] - 10;
		int height2z = coordinates[0][1] - 10;
		
		if(HeightMap[height1x,height1z] > HeightMap[height2x,height2z]) 
		{ 
			cliffheight = HeightMap[height1x,height1z];
		} 
		else 
		{ 
			cliffheight = HeightMap[height2x,height2z];
		}
		while(radius > 0) {
			for(float theta = 0; theta < 2*(Mathf.PI); theta += 2*(Mathf.PI/128)) 
			{
				int x = (int)(pointx + radius*(Mathf.Cos (theta)));
				int z = (int)(pointz + radius*(Mathf.Sin (theta)));
			
				HeightMap[x,z] = (cliffheight/100)*98;
			}
			radius--;
	   }
		
		
	}
	*/