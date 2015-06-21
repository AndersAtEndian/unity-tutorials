using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour 
{
	public int width = 10;
	public int height = 20;

	public GameObject[] tetrominoes;
	public GameObject block;

	private GameObject[,] grid;
	
	void Start () 
	{
		grid = new GameObject[width, height];

		CreatePlayingField();
	}
	
	void Update () 
	{
	
	}

	private void AddTileToGrid(GameObject tile, Vector3 position)
	{
		int x = Mathf.RoundToInt(position.x);
		int y = Mathf.RoundToInt(position.y);

		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			grid[x,y] = tile;
		}
	}

	private void CreatePlayingField()
	{
		int x, y;

		for (y=-1; y<height; y++)
		{
			Instantiate(block, new Vector3(-1, y, 0), Quaternion.identity);
			Instantiate(block, new Vector3(width+1, y, 0), Quaternion.identity);
		}
		
		for (x=-1; x<(width+1); x++)
		{
			Instantiate(block, new Vector3(x, -1, 0), Quaternion.identity);
		}
	}
}
