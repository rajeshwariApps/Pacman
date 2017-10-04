using UnityEngine;
using System.Collections;

public class EnemyInstantiation : MonoBehaviour {

	public GameObject enemy;
	public int currentRowNum;
	public int currentCoNum;
	public GameObject env;
	LevelDesign levelDesign;
	int inkyActiveCount;

	void Start () 
	{
		inkyActiveCount = 0;
		if (this.gameObject.name == "redEnemy_Anim_State")
		{
			currentRowNum = 4;
			currentCoNum = 10;
		}
		if (this.gameObject.name == "pinkEnemy_Left") 
	    {
			currentRowNum = 8;
			currentCoNum = 10;
		}
		if (this.gameObject.name == "Inky_Anim_State") 
		{
			//this.gameObject.SetActive (false);
			currentRowNum = 8;
			currentCoNum = 12;
		}
		if (this.gameObject.name == "Clyde_Anim_State") 
		{
			//this.gameObject.SetActive (false);
			currentRowNum = 10;
			currentCoNum = 11;
		}
		setPosition ();
	}

	void Update () 
	{
	}

	private void setPosition()
	{
		 levelDesign = env.GetComponent<LevelDesign> ();
		int enX = levelDesign.GetXOffset (currentRowNum, currentCoNum);
		int enY = levelDesign.GetYOffset (currentRowNum, currentCoNum);
		transform.position = new Vector3(enX, enY , -0.2f);
	}

	public int getEnemyRow()
	{
		return currentRowNum;
	}
	public int getEnemyCol()
	{
		return currentCoNum;
	}
	public void setEnemyRow(int row)
	{
		this.currentRowNum = row;
	}
	public void setEnemyCol(int col)
	{
		this.currentCoNum = col;
	}
}
