using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PacmanMovementScript2 : MonoBehaviour 
{
	MonoBehaviour script;

	public int currentRow, currentCol;
	public GameObject pacman_Object;

	public GameObject env;
	public Animator pacmanAnim;
	LevelDesign levelDesign;

	public bool right;
	public bool left;
	public bool up;
	public bool down;

	public float speed ;
    Transform startMarker;
    Transform endMarker;

	int gameStarCounter = 0;
	public bool pressedRight ;
	public bool pressedLeft ;
	public bool pressedUp ;
	public bool pressedDown ;
    public GameObject node;
	public bool powerMode;
	int timeGap = 4;
	float alarmTiming ;
	int timeIntervalForPU = 15;
	public bool ateTopLeft;
	public bool ateTopRight;
	public bool ateBottomLeft;
	public bool ateBottomRight;
	float alarmTimeOne;
	float alarmTimeTwo;
	float alarmTimeThree;
    float alarmTimeFour;
	public int lives;

	public Text scoreText;
	private int score;

	public AudioSource StartAudio;
	public AudioSource eatAudio;
	public AudioSource powerUpAudio ;
	public AudioSource pacmanDeathAudio;
	int counter;
	private int initialRow;
	private int initialCol;

	private int foodScore;

	public GameObject winPanel;
	private float startTime;
	public GameObject lostPanel;
	public Text showScoreText;
	public Text showTimeText;

	void Start ()
	{
		foodScore = 10;
		levelDesign = env.GetComponent<LevelDesign> ();
		initialRow = 12;
		initialCol = 10;
		currentRow = initialRow;
		currentCol = initialCol;
		lives = 3;
		right = pacmanAnim.GetBool ("right");
		left = pacmanAnim.GetBool ("left");
		up = pacmanAnim.GetBool ("up");
		down = pacmanAnim.GetBool ("down");
        startMarker = transform;
		pressedRight = true;
		pressedLeft = false;
		pressedUp = false;
		pressedDown = false;
		speed = 4.0f;
		powerMode = false;
		ateTopLeft = false;
		ateTopRight = false;
		ateBottomLeft = false;
		ateBottomRight = false;
		SetInitialPosition ();
		winPanel.gameObject.SetActive (false);
		lostPanel.gameObject.SetActive (false);
		startTime = Time.time;
	}

	void Update () 
	{
		WinSituation ();
		counter++;
		if (counter == 1) 
		{
			StartAudio.Play ();
		}
		DetectKeyPressed ();
		MovePacman ();
		scoreText.text = "  : " + score.ToString();
	}

	public void SetInitialPosition() {
		SetPosition (initialRow, initialCol);
	}

	public void SetPosition() {
		SetPosition (currentRow, currentCol);
	}

	private void SetPosition(int row, int col) {
		LevelDesign levelDesign = env.GetComponent<LevelDesign> ();
		int x = levelDesign.GetXOffset (row, col);
		int y = levelDesign.GetYOffset (row, col);
		transform.position = new Vector3(x, y, -0.1f);
		currentRow = row;
		currentCol = col;
	}

	private float abs(float input) {
		return input;
	}

	void MovePacman()
	{
		int resultForRight = levelDesign.PeekRight (currentRow, currentCol);
		int resultForLeft = levelDesign.PeekLeft (currentRow, currentCol);
		int resultForUp = levelDesign.PeekUp (currentRow, currentCol);
		int resultForDown = levelDesign.PeekDown (currentRow, currentCol);

		if (pacmanAnim.GetBool ("right")
			&& (resultForRight == LevelDesign.FoodItem
				|| resultForRight == LevelDesign.PowerupItem
				|| resultForRight == LevelDesign.EmptyItem))
		{
			transform.Translate(Vector3.right * Time.deltaTime * speed, Space.World);
			float actualDist = (transform.position.y) -  levelDesign.GetXOffset(currentRow, currentCol);
			float bottomDist  = (transform.position.y) - levelDesign.GetXOffset(currentRow +1, currentCol) ;
			float toptDist = levelDesign.GetXOffset(currentRow-1, currentCol) - (transform.position.y) ;

			if (actualDist < toptDist && actualDist < bottomDist) 
			{
				transform.position = new Vector3(transform.position.x,levelDesign.GetYOffset (currentRow, currentCol ) ,
					transform.position.z);
				currentRow = currentRow ;
			} 
			if (bottomDist < toptDist && bottomDist < actualDist) 
			{
				transform.position = new Vector3(transform.position.x,levelDesign.GetYOffset (currentRow +1, currentCol),
					transform.position.z);
				currentRow = currentRow + 1;
			} 
			else if (toptDist < bottomDist  && toptDist < actualDist) 
			{
				transform.position = new Vector3(transform.position.x, levelDesign.GetYOffset (currentRow-1, currentCol),
					transform.position.z);
				currentRow = currentRow - 1;
			}
			if (abs(levelDesign.GetXOffset (currentRow, currentCol + 1) - transform.position.x) <= 0.05)
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol + 1),
					transform.position.y, transform.position.z);
				currentCol = currentCol + 1;
			}

		}

		
        if (pacmanAnim.GetBool ("left")
			&& (resultForLeft == LevelDesign.FoodItem
				|| resultForLeft == LevelDesign.PowerupItem
				|| resultForLeft == LevelDesign.EmptyItem))
		{
			transform.Translate(Vector3.left * Time.deltaTime * speed, Space.World);

			float actualDist = (transform.position.y) -  levelDesign.GetXOffset(currentRow, currentCol);
			float bottomDist  = (transform.position.y) - levelDesign.GetXOffset(currentRow +1, currentCol) ;
			float toptDist = levelDesign.GetXOffset(currentRow-1, currentCol) - (transform.position.y) ;
			if (actualDist < toptDist && actualDist < bottomDist) 
			{
				transform.position = new Vector3(transform.position.x,levelDesign.GetYOffset (currentRow, currentCol ) ,
					                           transform.position.z);
				currentRow = currentRow ;
			} 
			if (bottomDist < toptDist && bottomDist < actualDist) 
			{
				transform.position = new Vector3(transform.position.x,levelDesign.GetYOffset (currentRow +1, currentCol),
												transform.position.z);
				currentRow = currentRow + 1;
			} 
			else if (toptDist < bottomDist  && toptDist < actualDist) 
			{
				transform.position = new Vector3(transform.position.x, levelDesign.GetYOffset (currentRow-1, currentCol),
					                            transform.position.z);
				currentRow = currentRow - 1;
			}
			if (abs(transform.position.x -levelDesign.GetXOffset (currentRow, currentCol - 1))<= 0.05)
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol - 1),
					transform.position.y, transform.position.z);
				currentCol = currentCol - 1;
			}
		}

		if (pacmanAnim.GetBool ("up")
			&& (resultForUp == LevelDesign.FoodItem
				|| resultForUp == LevelDesign.PowerupItem
				|| resultForUp == LevelDesign.EmptyItem))
		{
			transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
			float actualDist = (transform.position.x) -  levelDesign.GetXOffset(currentRow, currentCol);
			float leftDist = (transform.position.x) - levelDesign.GetXOffset(currentRow, currentCol-1) ;
			float rightDist = levelDesign.GetXOffset(currentRow, currentCol+1) - (transform.position.x) ;
			if (actualDist < rightDist && actualDist < leftDist) 
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol ),
					transform.position.y, transform.position.z);
				currentCol = currentCol ;
			} 
			if (leftDist < rightDist && leftDist < actualDist) 
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol - 1),
					transform.position.y, transform.position.z);
				currentCol = currentCol - 1;
			} 
			else if (rightDist < leftDist  && rightDist < actualDist) 
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol + 1),
					transform.position.y, transform.position.z);
				currentCol = currentCol + 1;
			}
			if (abs(levelDesign.GetYOffset (currentRow-1, currentCol)-transform.position.y) <= 0.05)
			{
				transform.position = new Vector3(transform.position.x,
					levelDesign.GetYOffset (currentRow-1, currentCol) ,transform.position.z);
				currentRow = currentRow - 1;
			}
		}
		
        if (pacmanAnim.GetBool ("down")
			&& (resultForDown == LevelDesign.FoodItem
				|| resultForDown == LevelDesign.PowerupItem
				|| resultForDown == LevelDesign.EmptyItem))
		{
			transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);

			float actualDist = (transform.position.x) -  levelDesign.GetXOffset(currentRow, currentCol);
			float leftDist = (transform.position.x) - levelDesign.GetXOffset(currentRow, currentCol-1) ;
			float rightDist = levelDesign.GetXOffset(currentRow, currentCol+1) - (transform.position.x) ;
			if (actualDist < rightDist && actualDist < leftDist) 
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol ),
					transform.position.y, transform.position.z);
				currentCol = currentCol ;
			} 
			if (leftDist < rightDist && leftDist < actualDist) 
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol - 1),
					transform.position.y, transform.position.z);
				currentCol = currentCol - 1;
			} 
			else if (rightDist < leftDist  && rightDist < actualDist) 
			{
				transform.position = new Vector3(levelDesign.GetXOffset (currentRow, currentCol + 1),
					transform.position.y, transform.position.z);
				currentCol = currentCol + 1;
			}
			if (abs(transform.position.y - levelDesign.GetYOffset (currentRow + 1, currentCol)) <= 0.05)
			{
				transform.position = new Vector3(transform.position.x,
					levelDesign.GetYOffset (currentRow + 1, currentCol) ,transform.position.z);
				currentRow = currentRow + 1;
			}
		}

		// if foodpill and active, set inacative and +10
		if (levelDesign.isFoodPill (currentRow, currentCol) && levelDesign.gameObjects [currentRow, currentCol].activeSelf) 
		{
			levelDesign.gameObjects [currentRow, currentCol].SetActive (false);
			eatAudio.Play ();
			score += foodScore;
		}

		if (eatenPowerUp (levelDesign.getTopLeftPowerUpRow(), levelDesign.getTopLeftPowerUpColumn())
			|| eatenPowerUp (levelDesign.getTopRightPowerUpRow(), levelDesign.getTopRightPowerUpColumn())
			|| eatenPowerUp (levelDesign.getBottomLeftPowerUpRow(), levelDesign.getBottomLeftPowerUpColumn())
			|| eatenPowerUp (levelDesign.getBottomRightPowerUpRow(), levelDesign.getBottomRightPowerUpColumn()) ) 
		{
			powerMode = true;
			alarmTiming = Time.time + timeGap;

			if(eatenPowerUp (levelDesign.getTopLeftPowerUpRow(), levelDesign.getTopLeftPowerUpColumn()))
			{
				ateTopLeft = true;
				alarmTimeOne = Time.time + timeIntervalForPU;
			}
			if (Time.time >= alarmTimeOne) 
			{
				ateTopLeft = false;
			}

			if(eatenPowerUp (levelDesign.getTopRightPowerUpRow(), levelDesign.getTopRightPowerUpColumn()))
			{
				ateTopRight = true;
				alarmTimeTwo = Time.time + timeIntervalForPU;
			}
			if (Time.time >= alarmTimeTwo) 
			{
				ateTopRight = false;
			}

			if(eatenPowerUp (levelDesign.getBottomLeftPowerUpRow(), levelDesign.getBottomLeftPowerUpColumn()))
			{
				ateBottomLeft = true;
				alarmTimeThree = Time.time + timeIntervalForPU;
			}
			if (Time.time >= alarmTimeThree) 
			{
				ateBottomLeft = false;
			}

			if(eatenPowerUp (levelDesign.getBottomRightPowerUpRow(), levelDesign.getBottomRightPowerUpColumn()))
			{
				ateBottomRight = true;
				alarmTimeFour = Time.time + timeIntervalForPU;
			}
			if (Time.time >= alarmTimeFour) 
			{
				ateBottomRight = false;
			}

			// set power pill inactive and +40
			levelDesign.gameObjects [currentRow, currentCol].SetActive (false);
			score += 40;
			powerUpAudio.Play ();
		}

		if (Time.time >= alarmTiming && powerMode) 
		{
			Debug.Log ("power mode OFF");
			powerMode = false;
			ateTopRight = false;
			ateTopLeft = false;
			ateBottomLeft = false;
			ateBottomRight = false;
		}
	}

	void DetectKeyPressed()
	{
		bool anyNewKeyPressed =
			Input.GetKey ("up") || Input.GetKey ("down") || Input.GetKey ("left") || Input.GetKey ("right");

		// reset
		if (anyNewKeyPressed
			|| (pressedUp && (levelDesign.PeekUp (currentRow, currentCol) == LevelDesign.WallItem))
			|| (pressedRight && (levelDesign.PeekRight (currentRow, currentCol) == LevelDesign.WallItem))
			|| (pressedDown && (levelDesign.PeekDown (currentRow, currentCol) == LevelDesign.WallItem))
			|| (pressedLeft && (levelDesign.PeekLeft (currentRow, currentCol) == LevelDesign.WallItem))) {
			pressedUp = false;
			pressedDown = false;
			pressedLeft = false;
			pressedRight = false;
		}
	
		if(Input.GetKey ("up")) {
			pressedUp = true;
		} else if (Input.GetKey ("down")) {
			pressedDown = true;
		} else if (Input.GetKey ("left")) {
			pressedLeft = true;
		} else if (Input.GetKey ("right")) {
			pressedRight = true;
		}

		pacmanAnim.SetBool ("left", pressedLeft);
		pacmanAnim.SetBool ("up", pressedUp);
		pacmanAnim.SetBool ("down", pressedDown);
		pacmanAnim.SetBool ("right", pressedRight);
	 }

    
	public int getPacmanCurrentRow()
	{
		return currentRow;
	}

	public int getPacmanCurrentCol()
	{
		return currentCol;
	}

	private bool eatenPowerUp(int powerUpRow, int powerUpCol)
	{
		if (levelDesign.gameObjects [powerUpRow, powerUpCol].activeSelf) {
			if (this.currentRow == powerUpRow && this.currentCol == powerUpCol) {
				return true;
			}
		}
		return false;
	}

	void WinSituation()
	{
		if (levelDesign.allObjectInactive()) 
		{
			Debug.Log ("won");
			winPanel.gameObject.SetActive (true);
			Time.timeScale = 0;
		}
	}

	public void LostSituation ()
	{
		
		if (lives < 0) 
		{
			//lostscreen score and time and pause replay button
			lostPanel.gameObject.SetActive (true);
			showScoreText.text = "Your highest score : " + score.ToString ();
			double totalTime = ((int)((Time.time - startTime) * 100)) / 100.0;
			showTimeText.text = "Total time taken : " + totalTime.ToString () + "s";
			Time.timeScale = 0;
		}
	}

}
