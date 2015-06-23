using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameScript : MonoBehaviour 
{
	public int width = 10;
	public int height = 20;

	public GameObject[] tetrominoes;
	public GameObject block;
	
	public GameObject startGamePanel;

	public Text scoreText;

	/* Grid of blocks - used for collision detection and basically holds the game world
	 * (apart from the player tetromino. */
	private GameObject[,] grid;

	private GameObject player;
	
	private bool gameRunning = false;

	private int score = 0;

	/* Speed of player tetromino. */
	private float speed = 0.3f;

	void Start () 
	{
		/* +1 - room to spawn */
		grid = new GameObject[width, height+1];

		CreatePlayingField();
	}

	private void StartGame()
	{
		score = 0;
		gameRunning = true;
		startGamePanel.gameObject.SetActive(false);
		
		SpawnPlayer();

		speed = 0.3f;
		Invoke("MovePlayerDown", speed);
	}

	private void StopGame()
	{
		CancelInvoke();

		gameRunning = false;
		startGamePanel.gameObject.SetActive(true);

		int x, y;

		for (y=0; y<height; y++)
		{
			for (x=0; x<width; x++)
			{
				if (grid[x, y] != null)
				{
					Destroy (grid[x, y]);
					grid[x, y] = null;
				}
			}

			Destroy (player);
		}
	}
	
	void Update () 
	{
		if (gameRunning)
		{
			if (Input.GetKeyDown("up"))
			{
				RotatePlayer(90);
			}
			
			if (Input.GetKeyDown("down"))
			{
				MovePlayer(new Vector3(0, -1, 0));
			}
			
			if (Input.GetKeyDown("left"))
			{
				MovePlayer(new Vector3(-1, 0, 0));
			}
			else if (Input.GetKeyDown("right"))
			{
				MovePlayer(new Vector3(1, 0, 0));
			}	
		}
		else
		{
			if (Input.GetKeyDown (KeyCode.Space))
			{
				StartGame ();
			}
		}
	}

	/**
	 * Update score and increase speed.
	 */
	private void UpdateScore(int updateScore)
	{
		score += updateScore;
		scoreText.text = score.ToString();

		speed -= 0.01f;
		if (speed < 0.05f)
		{
			speed = 0.05f;
		}
	}

	private bool PlayerPositionValid()
	{
		bool result = true;
		
		foreach (Transform child in player.transform)
		{
			int x = Mathf.RoundToInt(child.transform.position.x);
			int y = Mathf.RoundToInt(child.transform.position.y);

			if (x < 0 || x >=  width || y < 0 || grid[x,y] != null)
			{
				result = false;
			}
		}
		
		return result;
	}

	private bool RotatePlayer(int degrees)
	{
		bool result = true;

		player.transform.Rotate(0, 0, degrees);
		if (!PlayerPositionValid())
		{
			player.transform.Rotate(0, 0, -degrees);
			result = false;
		}

		return result;
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
			MovePlayerToGrid();
			DeleteRows ();
			SpawnPlayer();
		}

		Invoke("MovePlayerDown", speed);
	}

	private void MoveBlockDown(GameObject block)
	{
		block.transform.position += new Vector3(0, -1, 0);
	}

	/**
	 * Spawn new player tetromino. If the tetromino is spawned in an invalid
	 * position the game is over. 
	 */
	private void SpawnPlayer()
	{
		int i = UnityEngine.Random.Range(0, tetrominoes.Length);
		
		player = (GameObject)Instantiate(tetrominoes[i], new Vector3(width / 2, height - 2, 0), Quaternion.identity);
		
		OffsetScript tileScript = player.GetComponent<OffsetScript>();
		if (tileScript.offsetX != 0 || tileScript.offsetY != 0)
		{
			var move = new Vector3(tileScript.offsetX, tileScript.offsetY, 0);
			player.transform.position += move;
		}

		if (!PlayerPositionValid())
		{
			StopGame ();
		}
	}

	/**
	 * Add the player tetromino to the background grid. The grid will only hold 
	 * the individual blocks and the original tetromino will be destroyed.
	 */
	private void MovePlayerToGrid()
	{
		/* Can't unparent a child inside a foreach loop of the parent. */ 
		List<Transform> unparent = new List<Transform>(player.transform.childCount);
		
		foreach (Transform child in player.transform)
		{
			AddTileToGrid (child.gameObject, child.transform.position);
			unparent.Add(child);
		}
		
		foreach (Transform child in unparent)
		{
			child.parent = null;
		}

		Destroy (player);
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

	/**
	 * Delete full rows and drop all the blocks above.
	 */
	private void DeleteRows()
	{
		Func<bool> deleteRow = () => 
		{
			int x, y;
			bool rowFull;

			for (y=0; y<height; y++)
			{
				rowFull = true;
				for (x=0; x<width; x++)
				{
					if (grid[x, y] == null)
					{
						rowFull = false;
						break;
					}
				}

				if (rowFull)
				{
					for (x=0; x<width; x++)
					{
						Destroy (grid[x,y]);
						grid[x,y] = null;
					}

					return true;
				}
			}

			return false;
		};

		Func<bool> moveColumn = () => 
		{
			int x, y;

			for (x=0; x<width; x++)
			{
				for (y=0; y<(height-1); y++)
				{
					if (grid[x,y] == null)
					{
						grid[x,y] = grid[x,y+1];
						grid[x,y+1] = null;
						if (grid[x, y] != null)
						{
							MoveBlockDown(grid[x,y]);
						}
					}
				}
			}

			return true;
		};

		int updateScore = 1;

		while(deleteRow())
		{
			UpdateScore(updateScore++);
			moveColumn();
		}
	}

	private void CreatePlayingField()
	{
		int x, y;

		for (y=-1; y<height; y++)
		{
			Instantiate(block, new Vector3(-1, y, 0), Quaternion.identity);
			Instantiate(block, new Vector3(width, y, 0), Quaternion.identity);
		}
		
		for (x=-1; x<(width+1); x++)
		{
			Instantiate(block, new Vector3(x, -1, 0), Quaternion.identity);
		}
	}
}
