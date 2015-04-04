using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGrid : MonoBehaviour {
	
	
	private TerrainNode[,] grid;
	
	public TerrainGrid(Terrain terrain) 
	{
		Vector3 size = terrain.terrainData.size;
	 	grid = new TerrainNode[(int)size.x, (int)size.z];
		
		for(int i=0; i<size.x; i++) 
		{
			for(int j=0; j<size.z; j++) 
			{
				//We are initially saying ALL terrain nodes are traversable.
				grid[i,j] = new TerrainNode(i,j,1);
				
				//Code here to see whether the terrain can be traversed at this point
			}
		}
		
	}
	
	//Unfinished - commented out so it compiles
	
	public List<TerrainNode> AStar(int x0, int z0, int x1, int z1) 
	{
		List<TerrainNode> closedset = new List<TerrainNode>();
		List<TerrainNode> openset = new List<TerrainNode>();
		List<TerrainNode> camefrom = new List<TerrainNode>();
		
		TerrainNode start = grid[x0,z0];
		TerrainNode goal = grid[x1, z1];
		
		openset.Add(start);
		
		start.setGScore(0);
		start.setFScore(start.getGScore() + heuristic_cost_estimate(start,goal));
		
		//While not empty
		while(openset.Count != 0) 
		{
			TerrainNode current = null;
			
			for(int i=0; i<openset.Count; i++) 
			{
				TerrainNode tn = openset[i];
				if(current==null || tn.getFScore() < current.getFScore()) {current = tn; }
				
			}

			if(current == goal) return reconstruct_path(camefrom, goal);
			openset.Remove(current);
			closedset.Add (current);
			
			List<TerrainNode> neighbours = getNeighbours (current.getX(),current.getZ());
			
			for(int i=0; i<neighbours.Count; i++) 
			{
				if(closedset.Contains (neighbours[i])) continue;
				
				int tenative_g_score = current.getGScore() + 1;
				
				if(!openset.Contains(neighbours[i]) || tenative_g_score < neighbours[i].getGScore()) 
				{
					if(!closedset.Contains(neighbours[i])) openset.Add (neighbours[i]);
					
					neighbours[i].setCameFrom (current);
					neighbours[i].setGScore(tenative_g_score);
					neighbours[i].setFScore(neighbours[i].getGScore() + heuristic_cost_estimate(neighbours[i],goal));
				}

			}
			
		}
		return null;

	}
	
	
	private int heuristic_cost_estimate(TerrainNode node1, TerrainNode node2) 
	{
		int x0 = node1.getX();
		int z0 = node1.getZ();
		
		int x1 = node2.getX();
		int z1 = node2.getX();
		
		return (int) Mathf.Abs(Mathf.Sqrt((x1-x0)*(x1-x0) + (z1-z0)*(z1-z0)));
	}
	
	private List<TerrainNode> getNeighbours(int x, int z) 
	{
		List<TerrainNode> neighbours = new List<TerrainNode>();
	    
		try {neighbours.Add(grid[x,z+1]);}
		catch(UnityException e){}
		
		try {neighbours.Add(grid[x+1,z]);}
		catch(UnityException e){}		
		
		try {neighbours.Add(grid[x+1,z+1]);}
		catch(UnityException e){}
		
		try {neighbours.Add(grid[x-1,z-1]);}
		catch(UnityException e){}
		
		try {neighbours.Add(grid[x-1,z+1]);}
		catch(UnityException e){}
		
		try {neighbours.Add(grid[x+1,z-1]);}
		catch(UnityException e){}	
		
		try {neighbours.Add(grid[x,z-1]);}
		catch(UnityException e){}		
		
		try {neighbours.Add(grid[x-1,z]);}
		catch(UnityException e){}		
		
		return neighbours;
	}

	
	
	public List<TerrainNode> reconstruct_path(List<TerrainNode> came_from, TerrainNode current_node) 
	{
		if(current_node.getCameFrom() != null) 
		{
			List<TerrainNode> p = reconstruct_path(came_from, current_node.getCameFrom());
			p.Add (current_node);
			return p;
		} else 
		{
			List<TerrainNode> p = new List<TerrainNode>();
			p.Add (current_node);
			return p;
		}
	}
	
	void start() { }
	void update() { }
	

	

}
