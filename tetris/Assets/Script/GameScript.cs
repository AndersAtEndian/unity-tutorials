using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour 
{
	public int width = 10;
	public int height = 20;

	public GameObject[] tetrominoes;
	public GameObject block;

	private GameObject[,] grid;

	private GameObject player;
	
	void Start () 
	{
		grid = new GameObject[width, height];

		CreatePlayingField();

		SpawnPlayer();

		InvokeRepeating("MovePlayerDown", 0.3f, 0.3f);

	}
	
	void Update () 
	{
	
	}

	private bool PlayerPositionValid()
	{
		return true;
	}

	private bool MovePlayer(Vector3 movement)
	{
		bool result = true;
		player.transform.position += movement;
		if (!PlayerPositionValid())
		{
			player.transform.position -= movement;
			result = false;
		}

		return result;
	}

	private void MovePlayerDown()
	{
		if (!MovePlayer (new Vector3(0, -1, 0)))
		{
			/* Spawn next... */

		}
	}

	private void SpawnPlayer()
	{
		int i = Random.Range(0, tetrominoes.Length);
		
		player = (GameObject)Instantiate(tetrominoes[i], new Vector3(width / 2, height, 0), Quaternion.identity);
		
		OffsetScript tileScript = player.GetComponent<OffsetScript>();
		if (tileScript.offsetX != 0 || tileScript.offsetY != 0)
		{
			var move = new Vector3(tileScript.offsetX, tileScript.offsetY, 0);
			player.transform.position += move;
		}
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
