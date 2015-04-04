using UnityEngine;
using System.Collections;

public class TerrainNode : Object {
	
	private int x;
	private int z;
	private int cost {set; get;}
	
	private int fscore;
	private int gscore;
	private TerrainNode came_from;
	
	public TerrainNode(int x, int z, int cost) 
	{
		this.x = x;
		this.z = z;
		this.cost = cost;
	}
	
	public void setFScore(int v) 
	{
		this.fscore = v;
	}
	
	public void setGScore(int v) 
	{
		this.gscore = v;
	}
	
	public int getX() { return x; }
	public int getZ() { return z; }
	
	
	public int getFScore() { return fscore;}
	public int getGScore() { return gscore;}
	
	public void setCameFrom(TerrainNode node) { this.came_from = node; }
	public TerrainNode getCameFrom() { return this.came_from; }
	
	
	
	
	

}
