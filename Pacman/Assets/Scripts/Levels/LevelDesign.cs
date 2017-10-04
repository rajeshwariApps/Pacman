using UnityEngine;
using System.Collections;

public class LevelDesign : MonoBehaviour {
	public const int RowNum = 22;
	public const int ColNum = 21;
	public GameObject[,] gameObjects;
	int[,] levels;
	public GameObject wall;
	public GameObject Food;
	public GameObject node;
	public GameObject PowerUp;
	public GameObject Empty;

	public const int EmptyItem = 0;
	public const int WallItem = 1;
	public const int FoodItem = 2;
	public const int PowerupItem = 3;

	public void Start ()
	{
		gameObjects = new GameObject[RowNum, ColNum];
		levels = new int[RowNum, ColNum] {
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
			{ 1, 3, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 3, 1 },
			{ 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1 },
			{ 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
			{ 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1 },
			{ 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1 },
			{ 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1 },
			{ 0, 0, 0, 0, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 0, 0, 0, 0 },
			{ 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 0, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1 },
			{ 2, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 0, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1 },
			{ 0, 0, 0, 0, 1, 2, 1, 2, 2, 2, 0, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1 },
			{ 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1 },
			{ 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
			{ 1, 3, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 3, 1 },
			{ 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1 },
			{ 1, 1, 1, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 1, 1, 1 },
			{ 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1 },
			{ 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1 },
			{ 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
		};
		GenerateGrid ();
	}

	public bool isWall(int row, int col) {
		return levels [row, col] == WallItem;
	}

	public int PeekRight(int row, int col) 
	{
		if (col + 1 == ColNum) { // last col
			if (row + 1 == RowNum) { // last row
				return -1;
			}
			return levels [row + 1, 0];
		}
		return levels [row, col + 1];
	}
	public int PeekLeft(int row, int col) 
	{
		if (col == 0) 
		{ 
			if (row == 0)
			{
				return -1;
			}
			return levels [row - 1, ColNum-1];
		}
		return levels [row, col - 1];
	}
	public int PeekUp(int row, int col) 
	{
		if (row == 0) 
		{ 
				return -1;
		}
		return levels [row-1, col];
	}
	public int PeekDown(int row, int col) 
	{
		if (row == RowNum-1
		) {
				return -1;
			}
		return levels [row +1, col];
	}

	public int GetXOffset(int row, int col) {
		return col;
	}

	public int GetYOffset(int row, int col) {
		return -row;
	}

	private void GenerateGrid()
	{
		for (int row = 0, yOffset = 0; row < RowNum; row++, yOffset--) {
			for (int col = 0, xOffset = 0; col < ColNum; col++, xOffset++) {
				if (levels[row, col] == 0)
				{
					gameObjects[row, col] = Instantiate (
						Empty, new Vector3 (xOffset, yOffset, 0), Empty.transform.rotation) as GameObject;
					gameObjects[row, col].name = "Empty" + row + col;
					gameObjects[row, col].tag = "Empty";
				}
				if (levels[row, col] == 2) 
				{
					gameObjects[row, col] = Instantiate (
						Food, new Vector3 (xOffset, yOffset, 0), Food.transform.rotation) as GameObject;
					gameObjects[row, col].name = "Food" + row + col;
					gameObjects[row, col].tag = "Food";
				}
				if (levels[row, col] == 3) {
					gameObjects[row, col] = Instantiate (
						PowerUp, new Vector3 (xOffset, yOffset, 0), PowerUp.transform.rotation) as GameObject;
					gameObjects[row, col].name = "PowerUp" + row + col;
					gameObjects[row, col].tag = "PowerUp";
				}
				if (levels[row, col] == 1)
				{
					gameObjects[row, col] = Instantiate (
						wall, new Vector3 (xOffset, yOffset, 0), wall.transform.rotation) as GameObject;
					gameObjects[row, col].name = "BlueWall" + row + col;
					gameObjects[row, col].tag = "BlueWall";
				}

			}
		}
	}

	public bool isBlueWall(int row, int col)
	{
		if (levels [row, col] == WallItem) 
		{
			return true;
		}
		return false;
	}

	public bool isFoodPill(int row, int col)
	{
		if (levels [row, col] == FoodItem) 
		{
			return true;
		}
		return false;
	}

	public int getTopLeftPowerUpRow()
	{
		return 2;
	}
	public int getTopLeftPowerUpColumn()
	{
		return 1;
	}
	public int getTopRightPowerUpRow()
	{
		return 2;
	}
	public int getTopRightPowerUpColumn()
	{
		return 19;
	}
	public int getBottomLeftPowerUpRow()
	{
		return 15;
	}
	public int getBottomLeftPowerUpColumn()
	{
		return 1;
	}
	public int getBottomRightPowerUpRow()
	{
		return 15;
	}
	public int getBottomRightPowerUpColumn()
	{
		return 19;
	}

	public bool allObjectInactive()
	{
		for (int i = 0; i < RowNum; i++) {
			for (int j = 0; j < ColNum; j++) {
				if (gameObjects [i, j].activeSelf && (levels[i, j] == PowerupItem || levels[i, j] == FoodItem)) {
					return false;
				}
			}
		}
		return true;
	}
}
