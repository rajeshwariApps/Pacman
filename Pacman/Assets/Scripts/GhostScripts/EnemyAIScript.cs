using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using System.Linq;
[System.Serializable]
public class EnemyAIScript : MonoBehaviour {
	public GameObject env;
	LevelDesign levelDesign ;
	public ArrayList nodeList;
	public bool init = false;
	public GameObject ghost;
	public GameObject pacmanObj;
	EnemyInstantiation enemyInit;
	PacmanMovementScript2 pacmanScript;
	public float ghostSpeed ;
	int moving ;
	int searching ;
	int randomTargetting;
	int runAway;
	public int state;
	public int randomPathTargetIndex=-1;
	int minRange;
	int maxRange;
	Node target ;
	int targetRow ;
	int targetCol; 
	int targetX; 
	int targetY;
	bool up ;
	bool down;
	bool left;
	bool right;
	public Animator animator;
	ArrayList randomNodearray;
	SearchResponse randomSearchResponse;
	float alarmTime;

	float gapTime ;
	public bool pinkUp ;
	public bool pinkDown ;
	public bool pinkRight ;
	public bool pinkLeft ;
	public int minDistanceLimitToGetAggressive;
	public int maxDistanceUntillAggression;

	public Text LifeText;

	public AudioSource powerPacmanAteGhostAudio;

	void Start () 
	{
		
		levelDesign = env.GetComponent<LevelDesign> ();
		enemyInit = ghost.GetComponent<EnemyInstantiation> ();
		pacmanScript = pacmanObj.GetComponent<PacmanMovementScript2> ();
		nodeList = new ArrayList ();
		moving = 1;
		searching = 0;
		randomTargetting = 2;
		runAway = 3;
		state = randomTargetting;
		//state = searching;
		randomSearchResponse = new SearchResponse();
		up = false;
		down = false;
		left = false;
		right = false;
		//ghostSpeed = 1.0f;

		randomNodearray = new ArrayList ();
		pinkUp = false;
		pinkDown = false;
		pinkRight = false;
		pinkLeft = false;
		alarmTime = 0;
		gapTime = 50.0f;
		if (this.gameObject.name == "pinkEnemy_Left") 
		{
			minDistanceLimitToGetAggressive = 20;
			maxDistanceUntillAggression = 37;
		} 
		else if (this.gameObject.name == "redEnemy_Anim_State")
		{
			minDistanceLimitToGetAggressive = 60;
			maxDistanceUntillAggression = 70;
		}
		else if (this.gameObject.name == "Inky_Anim_State")
		{
			minDistanceLimitToGetAggressive = 9;
			maxDistanceUntillAggression = 15;
		}
		else if (this.gameObject.name == "Clyde_Anim_State")
		{
			minDistanceLimitToGetAggressive = 4;
			maxDistanceUntillAggression = 10;
		}

	}

void Update () 
{
	if (!init)
	{
		init = true;
		MakingJunctionArray ();
	}

		if (CaughtPacman()) 
		{
			if (pacmanScript.powerMode) {
				this.gameObject.SetActive (false);
			} else {
				pacmanScript.pacmanDeathAudio.Play ();
				pacmanObj.SetActive (false);
				pacmanScript.lives --;
				if (pacmanScript.lives > 0) {
					pacmanScript.SetInitialPosition ();
					pacmanObj.gameObject.SetActive (true);
				}
				if (pacmanScript.lives < 0) {
					this.gameObject.SetActive (false);
					Time.timeScale = 0;
				}
				pacmanScript.LostSituation ();
			}
			return;
		}

		LifeText.text = pacmanScript.lives.ToString();
		
		switch (state) 
		{

		case 0:// searching
			{
				SearchResponse pacmanSearchResponse = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()), 
					                                     findNodeIndex (pacmanScript.getPacmanCurrentRow (), pacmanScript.getPacmanCurrentCol ()));
				if (pacmanSearchResponse.distFromDestination > maxDistanceUntillAggression) {
					state = randomTargetting;
					randomSearchResponse = new SearchResponse ();
					randomPathTargetIndex = -1;
					break;
				}
				if(pacmanSearchResponse.pathNodeIndexArray.Count ==0)
				{
					return;
				}
				target = ((Node)nodeList [((int)pacmanSearchResponse.pathNodeIndexArray [0])]);
				targetRow = target.getRow ();
				targetCol = target.getCol ();
				targetX = levelDesign.GetXOffset (targetRow, targetCol);
				targetY = levelDesign.GetYOffset (targetRow, targetCol);
				setDirections ();
				state = moving;
				break;
			}

		case 1: //moving
			moveGhost ();

			break;

		case 2: //random
			{
				//if pacman near me state turns to search
				{
					SearchResponse pacmanSearchResponse = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()), 
						                                     findNodeIndex (pacmanScript.getPacmanCurrentRow (), pacmanScript.getPacmanCurrentCol ()));
					if (pacmanSearchResponse.distFromDestination <= minDistanceLimitToGetAggressive) {
						state = searching;
						randomSearchResponse = new SearchResponse ();
						randomPathTargetIndex = -1;
						break;
					}
				}
				if (randomPathTargetIndex == randomSearchResponse.pathNodeIndexArray.Count - 1) {
					randomSearchResponse = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()), 
						getRandomTargetIndexInNodeList ());
					randomPathTargetIndex = -1;
				}
				if(randomSearchResponse.pathNodeIndexArray.Count == 0)
				{
					return;
				}
				target = ((Node)nodeList [((int)randomSearchResponse.pathNodeIndexArray [randomPathTargetIndex + 1])]);
				randomPathTargetIndex++;
				targetRow = target.getRow ();
				targetCol = target.getCol ();
				targetX = levelDesign.GetXOffset (targetRow, targetCol);
				targetY = levelDesign.GetYOffset (targetRow, targetCol);
				setDirections ();
				state = moving;


				//set random target
				break;
			}

		case 3://runAway
		
			if (pacmanScript.powerMode) {
				float distFromtopLeft = calculateHValue (enemyInit.getEnemyRow (), enemyInit.getEnemyCol (), 
					                        levelDesign.getTopLeftPowerUpRow (), levelDesign.getTopLeftPowerUpColumn ());

				float distFromTopRight = calculateHValue (enemyInit.getEnemyRow (), enemyInit.getEnemyCol (), 
					                         levelDesign.getTopRightPowerUpRow (), levelDesign.getTopRightPowerUpColumn ());

				float distFromBottomLeft = calculateHValue (enemyInit.getEnemyRow (), enemyInit.getEnemyCol (), 
					                           levelDesign.getBottomLeftPowerUpRow (), levelDesign.getBottomLeftPowerUpColumn ());

				float distFromBottomRight = calculateHValue (enemyInit.getEnemyRow (), enemyInit.getEnemyCol (), 
					                            levelDesign.getBottomRightPowerUpRow (), levelDesign.getBottomRightPowerUpColumn ());

				if (pacmanScript.ateTopLeft) {
					if (distFromTopRight <= distFromBottomLeft && distFromTopRight <= distFromBottomRight) {
						//go towards topRight
						SearchResponse runFromTopLeftSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                      findNodeIndex (levelDesign.getTopRightPowerUpRow (), levelDesign.getTopRightPowerUpColumn ()));

						if (runFromTopLeftSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromTopLeftSearch);
						state = moving;

					} else if (distFromBottomLeft < distFromTopRight && distFromBottomLeft < distFromBottomRight) {
						//go towards bottomLeft
						SearchResponse runFromTopLeftSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                      findNodeIndex (levelDesign.getBottomLeftPowerUpRow (), levelDesign.getBottomLeftPowerUpColumn ()));

						if (runFromTopLeftSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromTopLeftSearch);
						state = moving;
					} else if (distFromBottomRight < distFromBottomLeft && distFromBottomRight < distFromTopRight) {
						//go towards bottomRight
						SearchResponse runFromTopLeftSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                      findNodeIndex (levelDesign.getBottomRightPowerUpRow (), levelDesign.getBottomRightPowerUpColumn ()));

						if (runFromTopLeftSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromTopLeftSearch);
						state = moving;

					}
				
				
				}
				if (pacmanScript.ateTopRight) {
					if (distFromtopLeft <= distFromBottomLeft && distFromtopLeft <= distFromBottomRight) {
						//go towards topLeft
						SearchResponse runFromTopRightSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                       findNodeIndex (levelDesign.getTopLeftPowerUpRow (), levelDesign.getTopLeftPowerUpColumn ()));

						if (runFromTopRightSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromTopRightSearch);
						state = moving;

					} else if (distFromBottomLeft < distFromtopLeft && distFromBottomLeft < distFromBottomRight) {
						//go towards bottomLeft
						SearchResponse runFromTopRightSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                       findNodeIndex (levelDesign.getBottomLeftPowerUpRow (), levelDesign.getBottomLeftPowerUpColumn ()));

						if (runFromTopRightSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromTopRightSearch);
						state = moving;

					} else if (distFromBottomRight < distFromBottomLeft && distFromBottomRight < distFromtopLeft) {
						//go towards bottomRight
						SearchResponse runFromTopRightSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                       findNodeIndex (levelDesign.getBottomRightPowerUpRow (), levelDesign.getBottomRightPowerUpColumn ()));

						if (runFromTopRightSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromTopRightSearch);
						state = moving;
					}
				
				}
				if (pacmanScript.ateBottomLeft) {
					if (distFromtopLeft <= distFromTopRight && distFromtopLeft <= distFromBottomRight) {
						//go towards topLeft
						SearchResponse runFromBottomLeftSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                         findNodeIndex (levelDesign.getTopLeftPowerUpRow (), levelDesign.getTopLeftPowerUpColumn ()));

						if (runFromBottomLeftSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromBottomLeftSearch);
						state = moving;
					} else if (distFromBottomRight < distFromtopLeft && distFromBottomRight < distFromTopRight) {
						//go towards bottomright
						SearchResponse runFromBottomLeftSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                         findNodeIndex (levelDesign.getBottomRightPowerUpRow (), levelDesign.getBottomRightPowerUpColumn ()));

						if (runFromBottomLeftSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromBottomLeftSearch);
						state = moving;
					} else if (distFromTopRight < distFromBottomRight && distFromTopRight < distFromtopLeft) {
						//go towards topright
						SearchResponse runFromBottomLeftSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                         findNodeIndex (levelDesign.getTopRightPowerUpRow (), levelDesign.getTopRightPowerUpColumn ()));

						if (runFromBottomLeftSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromBottomLeftSearch);
						state = moving;
					}

				}
				if (pacmanScript.ateBottomRight) {
					if (distFromtopLeft <= distFromTopRight && distFromtopLeft <= distFromBottomLeft) {
						//go towards topLeft
						SearchResponse runFromBottomRightSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                          findNodeIndex (levelDesign.getTopLeftPowerUpRow (), levelDesign.getTopLeftPowerUpColumn ()));

						if (runFromBottomRightSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromBottomRightSearch);
						state = moving;
					} else if (distFromBottomLeft < distFromtopLeft && distFromBottomLeft < distFromTopRight) {
						//go towards bottomleft
						SearchResponse runFromBottomRightSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                          findNodeIndex (levelDesign.getBottomLeftPowerUpRow (), levelDesign.getBottomLeftPowerUpColumn ()));

						if (runFromBottomRightSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromBottomRightSearch);
						state = moving;
					} else if (distFromTopRight < distFromBottomLeft && distFromTopRight < distFromtopLeft) {
						//go towards topright
						SearchResponse runFromBottomRightSearch = Search (findNodeIndex (enemyInit.getEnemyRow (), enemyInit.getEnemyCol ()),
							                                          findNodeIndex (levelDesign.getTopRightPowerUpRow (), levelDesign.getTopRightPowerUpColumn ()));

						if (runFromBottomRightSearch.pathNodeIndexArray.Count == 0) {
							return;
						}
						setTheTarget (runFromBottomRightSearch);
						state = moving;
					}
				}
			} else {
				state = randomTargetting;
			}
				
		break;
	}


}
	void setTheTarget(SearchResponse searchOutput)
	{
		target = ((Node)nodeList [((int)searchOutput.pathNodeIndexArray [0])]);
		targetRow = target.getRow ();
		targetCol = target.getCol ();
		targetX = levelDesign.GetXOffset (targetRow, targetCol);
		targetY = levelDesign.GetYOffset (targetRow, targetCol);
		setDirections ();
	}

	void MakingJunctionArray()
	{
		for (int row = 0; row < LevelDesign.RowNum; row++) {
			for (int col = 0; col < LevelDesign.ColNum; col++) {
				if (levelDesign.isWall (row, col))
					continue;
				
				int resultForRight = levelDesign.PeekRight (row, col);
				int resultForLeft = levelDesign.PeekLeft (row, col);
				int resultForUp = levelDesign.PeekUp (row, col);
				int resultForDown = levelDesign.PeekDown (row, col);
				bool isOpenRight = false;
				bool isOpenLeft = false;
				bool isOpenUp = false;
				bool isOpenDown = false;
				if (resultForRight != LevelDesign.WallItem && resultForRight != -1) {
					isOpenRight = true;
				}
				if (resultForLeft != LevelDesign.WallItem && resultForLeft != -1) {
					isOpenLeft = true;
				}
				if (resultForUp != LevelDesign.WallItem && resultForUp != -1) {
					isOpenUp = true;
				}
				if (resultForDown != LevelDesign.WallItem && resultForDown != -1) {
					isOpenDown = true;
				}

				Node newNode = new Node (row, col);
				nodeList.Add (newNode);

			}
		}


		// adding edges
		for (int i = 0; i < nodeList.Count; i++) {
			for (int j = 0; j < nodeList.Count; j++) {
				Node node = (Node)nodeList [i];
				Node node2 = (Node)nodeList [j];
				Edge newEdge = findingConnection (node, node2);
				if (newEdge != null) {
					node.edge_List.Add (newEdge);

				}
			}
		}
	}
	
	Edge findingConnection(Node node1, Node node2)
	{
		int row1 = node1.getRow();
		int row2 = node2.getRow();
		int col1 = node1.getCol();
		int col2 = node2.getCol();
		bool horizontal = false;
		bool vertical = false;
		if (row1 == row2) {
			horizontal = true;
			if (absoluteVal (col1 - col2) != 1) {
				return null;
			}
		}
		if (col1 == col2) {
			vertical = true;
			if (absoluteVal (row1 - row2) != 1) {
				return null;
			}
		}
		if (vertical && horizontal) {
			return null;
		}
		if (vertical) 
		{
			int maxRow = row2 > row1 ? row2 : row1;
			int minRow = row2 < row1 ? row2 : row1;
			for (int row = minRow; row < maxRow; row++) 
			{
				if (levelDesign.isBlueWall ( row, col1))
				{
					return null;
				}
			}
			return new Edge (row1, row2, col1, col2, (maxRow - minRow));
		}
		if (horizontal) 
		{

			int maxCol = col2 > col1 ? col2 : col1;
			int minCol = col2 < col1 ? col2 : col1;
			for (int col = minCol; col < maxCol; col++) 
			{
				if (levelDesign.isBlueWall ( row1, col))
				{
					return null;
				}
			}
			return new Edge (row1, row2, col1, col2, (maxCol - minCol));
		}
		return null;
	}


	//BFS starts from here
	//return shortest PATH
	//returns array of  index of the nodes in nodeList which is in edgeList of the source node and through which the shortest path to pacman is discovered.
	SearchResponse Search(int sourceIndex, int destinationIndex)
	{
		if (sourceIndex == destinationIndex) {
			SearchResponse response = new SearchResponse ();
			response.distFromDestination = 0;
			return response;
		}
		HashSet<SetVisited> visitSet = new HashSet<SetVisited> ();
		ArrayList queue = new ArrayList ();
		Dictionary<int, int> cameFrom = new Dictionary<int, int> ();
		Dictionary<int, int> gValMap = new Dictionary<int, int> ();


		 
		Node destNode = (Node)nodeList [destinationIndex];
		QueueEntry source = new QueueEntry (sourceIndex,
			calculateHValue(enemyInit.getEnemyRow (), enemyInit.getEnemyCol (),destNode.getRow(), destNode.getCol() ), 
			                0); 
		queue.Add (source);
		SetVisited sourceVisited = new SetVisited (sourceIndex, 0);
		visitSet.Add (sourceVisited);

		//ArrayList reversedPath = new ArrayList ();
		while (queue.Count != 0) 
		{
			float minNum = 1000000;
			int minIndex = 0;
			for (int i = 0; i < queue.Count; i++)
			{
				QueueEntry entry = (QueueEntry)queue [i];
				//Debug.Log (entry.getFValue());
				if (entry.getFValue() < minNum) 
				{
					minNum = entry.getFValue ();
					minIndex = i;
				} 
			}
			//Debug.Log ("remove queue" + minIndex);
			QueueEntry lowestF = (QueueEntry) queue [minIndex];
			queue.RemoveAt (minIndex);

			Node lowestFNode = (Node)nodeList [lowestF.getNodeIndex()];
			if ((destNode.getRow () == lowestFNode.getRow ()) && (destNode.getCol () == lowestFNode.getCol ())) 
			{
				SearchResponse finalResponse = new SearchResponse ();
				int temp = destinationIndex;
				while (temp != sourceIndex)
				{
					//Debug.Log ("source" + sourceIndex);
					finalResponse.pathNodeIndexArray.Add (temp);
					for (int i = 0; i < finalResponse.pathNodeIndexArray.Count; i++) {
						//Debug.Log ("path values " + finalResponse.pathNodeIndexArray [i]);
					}
					bool isPresent = cameFrom.TryGetValue (temp, out temp);
				}

				// reverse the path
				for (int i = 0; i < finalResponse.pathNodeIndexArray.Count / 2; i++) {
					int tempIndex = (int)finalResponse.pathNodeIndexArray [i];
					finalResponse.pathNodeIndexArray [i] = finalResponse.pathNodeIndexArray [finalResponse.pathNodeIndexArray.Count - 1 - i];
					finalResponse.pathNodeIndexArray [finalResponse.pathNodeIndexArray.Count - 1 - i] = tempIndex;
				}
				gValMap.TryGetValue (destinationIndex, out finalResponse.distFromDestination);

				return finalResponse;
			}
			SetVisited nodeVisited = new SetVisited (lowestF.getNodeIndex (), lowestF.getGValue());
			//Debug.Log (nodeVisited.ToString());
			visitSet.Add (nodeVisited);

			for(int i = 0 ; i < lowestFNode.edge_List.Count; i++) 
			{
				Edge nodeEdge = (Edge)lowestFNode.edge_List [i];
				int nodeEdgeIndexInNodeList =  findNodeIndex(nodeEdge.getRow2 (), nodeEdge.getCol2 ());
				//nodeEdge present in visitSet or not
				//if visited continue
				SetVisited fakeNode = new SetVisited (nodeEdgeIndexInNodeList, 0);
				if (visitSet.Contains(fakeNode)) {
					//Debug.Log ("Contains" + fakeNode.ToString() + "nodeEdgeIndexInNodeList=" + nodeEdgeIndexInNodeList);
					continue;
				}
				// to check whose gval is lowest between lowestFnode and lowestFnode's child
				// tentative gval = gavel(lowestf) + distbetwn(lowestf, edges);
				//if tentative g val 

				int tentativeGVal = lowestF.getGValue() + nodeEdge.getWeight(); // gValue of the current node (edge of the Main node) + distbetween lowest and the next edge;
				//Debug.Log ("new val = " + tentativeGVal);
				int existingGValue;
				if (gValMap.TryGetValue (nodeEdgeIndexInNodeList, out existingGValue) && (tentativeGVal >= existingGValue)) 
				{
					continue;
				}
				if (gValMap.ContainsKey (nodeEdgeIndexInNodeList)) {
					gValMap [nodeEdgeIndexInNodeList] = tentativeGVal;
				} else 
				{
					gValMap.Add (nodeEdgeIndexInNodeList, tentativeGVal);
				}
				QueueEntry lowestFChild = new QueueEntry(
					findNodeIndex(nodeEdge.getRow2(), nodeEdge.getCol2()),
					calculateHValue(nodeEdge.getRow2(), nodeEdge.getCol2(),destNode.getRow(), destNode.getCol() ),
					tentativeGVal);
				
				queue.Add (lowestFChild);
				if (cameFrom.ContainsKey (lowestFChild.getNodeIndex ())) {
					cameFrom [lowestFChild.getNodeIndex ()] = lowestF.getNodeIndex ();
				} else 
				{
					cameFrom.Add (lowestFChild.getNodeIndex (), lowestF.getNodeIndex ());
				}
				//set the tentative gVal to the gval of the child
				lowestFChild.setGValue(tentativeGVal);
				Node lowestFChildNode = (Node) nodeList [lowestFChild.getNodeIndex ()];
				lowestFChild.setFValue (lowestFChild.getGValue () , 
					calculateHValue (lowestFChildNode.getRow(), lowestFChildNode.getCol(),destNode.getRow(), destNode.getCol()))  ;
			}
		}
		//Debug.Log ("Hey we found no path, how come its happening");
		return null;
	}

	private int findNodeIndex(int rowNum, int colNum)
	{
		int nodeIndex = 0;
		foreach (Node node in nodeList) 
		{
			if (node.getRow () == rowNum && node.getCol () == colNum) 
			{
				return nodeIndex;
			}
			nodeIndex++;
		}
		//Debug.Log ("Could not find the node");
		return -1;
	}


	private float calculateHValue(int row1, int col1, int row2, int col2)
	{
			return absoluteVal (row2 - row1) + absoluteVal (col2 - col1);
	}
	 
	int absoluteVal(int num)
	{
		return num > 0 ? num : -1 * num;
	}

	//movement of enemy

	// checking of row colum position 




	void setDirections()
	{
		//if (enemyAnimatorMap.TryGetValue (red, out ghostAnimator)) {
			animator.SetBool ("right", false);
			animator.SetBool ("left", false);
			animator.SetBool ("up", false);
			animator.SetBool ("down", false);
		

		if (enemyInit.getEnemyRow () > targetRow) 
		{

			animator.SetBool ("up", true);
		}
		if (enemyInit.getEnemyRow () < targetRow) 
		{

			animator.SetBool ("down", true);
		}

		if (enemyInit.getEnemyCol () < targetCol) 
		{

			animator.SetBool ("right", true);
		}
		if (enemyInit.getEnemyCol () > targetCol) 
		{
			animator.SetBool ("left", true);
		}
	}
	//}



	//moving
	void moveGhost ()
	{
		bool reachedTarget = false;
		if (animator.GetBool ("up")== true) 
		{
			transform.Translate (Vector3.up * Time.deltaTime * ghostSpeed,Space.World);
			if (targetY - transform.position.y < 0.2) 
			{
				reachedTarget = true;
			} else if (levelDesign.GetYOffset(enemyInit.getEnemyRow() -1, enemyInit.getEnemyCol()) - transform.position.y < 0.01)
			{
				enemyInit.setEnemyRow (enemyInit.getEnemyRow() -1);
				transform.position = new Vector3(
					levelDesign.GetXOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					levelDesign.GetYOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					-0.2f);
			}
		}
		else if (animator.GetBool ("down")== true) 
		{
			transform.Translate (Vector3.down * Time.deltaTime * ghostSpeed, Space.World);

			if (transform.position.y - targetY  < 0.2) 
			{
				reachedTarget = true;
			}
			else if( transform.position.y - levelDesign.GetYOffset(enemyInit.getEnemyRow() +1, enemyInit.getEnemyCol())  < 0.01)
			{
				enemyInit.setEnemyRow (enemyInit.getEnemyRow() +1);
				transform.position = new Vector3(
					levelDesign.GetXOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					levelDesign.GetYOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					-0.2f);
			}


		}
		else if (animator.GetBool ("right")== true) 
		{
			transform.Translate (Vector3.right * Time.deltaTime * ghostSpeed, Space.World);

			if ( targetX - transform.position.x  < 0.2) 
			{
				reachedTarget = true;
			}
			else if(levelDesign.GetXOffset(enemyInit.getEnemyRow(),enemyInit.getEnemyCol()+1)- transform.position.x < 0.01)
			{
				enemyInit.setEnemyCol (enemyInit.getEnemyCol() +1);
				transform.position = new Vector3(
					levelDesign.GetXOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					levelDesign.GetYOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					-0.2f);
			}
		}
		else if (animator.GetBool ("left")== true) 
		{
			transform.Translate (Vector3.left * Time.deltaTime * ghostSpeed, Space.World);

			if (  transform.position.x - targetX   < 0.2) 
			{
				reachedTarget = true;
			}
			else if( transform.position.x - levelDesign.GetXOffset(enemyInit.getEnemyRow(),enemyInit.getEnemyCol()-1) < 0.01)
			{
				enemyInit.setEnemyCol (enemyInit.getEnemyCol() -1);
				transform.position = new Vector3(
					levelDesign.GetXOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					levelDesign.GetYOffset(enemyInit.getEnemyRow(), enemyInit.getEnemyCol()),
					-0.2f);
			}
		}
		if (reachedTarget) {
			transform.position = new Vector3 (targetX, targetY, -0.2f);
			enemyInit.setEnemyCol (targetCol);
			enemyInit.setEnemyRow (targetRow);
			state = pacmanScript.powerMode ? runAway : randomTargetting;
		}

	}

	bool CaughtPacman()
	{
		if (enemyInit.getEnemyRow () == pacmanScript.getPacmanCurrentRow ()
		   && enemyInit.getEnemyCol () == pacmanScript.getPacmanCurrentCol ()) 
		{
			return true;
		}
		return false;
	}

	int getRandomTargetIndexInNodeList()
	{
		//find current row col of pink enemy
		//from that find the index in nodeList
		//find a random number in a range from that current index +-10
		int enRow = enemyInit.getEnemyRow();
		int enCol = enemyInit.getEnemyCol ();
		int indNode = findNodeIndex (enRow, enCol);
		int randomInd;
		do {
			randomInd = Random.Range (0,nodeList.Count-1);
		} while(randomInd == indNode);
		//Debug.Log("start " + indNode + "randomnum " + randomInd);
		return randomInd;
	}

	int clipMin(int x, int y)
	{
		return x > y ? x : y;
	}
	int clipMax(int x, int y)
	{
		return x < y ? x : y;
	}
}


public class Edge
{
	int row1, col1;
	int row2, col2;
	int weight;
	public Edge(int row1, int row2, int col1, int col2, int weight)
	{
		this.row1 = row1;
		this.row2 = row2;
		this.col1 = col1;
		this.col2 = col2;
		this.weight = weight;
	}
	public override string ToString ()
	{
		return string.Format ("Edge between ({0}, {1}) and ({2}, {3}) with weight {4}", row1, col1, row2, col2, weight );
	}
	public int getRow2()
	{
		return row2;
	}
	public int getCol2()
	{
		return col2;
	}

	public int getWeight()
	{
		return weight;
	}

}

public class Node
{
	private int row, col;
	public ArrayList edge_List  = new ArrayList();

	public Node(int row, int col)
	{
		this.row = row;
		this.col = col;
	}

	public int getCol() {
		return this.col;
	}

	public int getRow() {
		return this.row;
	}
	public override string ToString ()
	{
		string str = string.Format ("[Node] rowNumber {0} ,  column number {1}, edges: ", row, col);
		foreach (Edge edge in edge_List) 
		{
			str += edge.ToString ();
		}
		return str;
	}
}

public class QueueEntry
{
	int nodeIndex;
	int gValue;
	float hValue;
	public QueueEntry(int indx, float hVal, int gVal)
	{
		this.nodeIndex = indx;
		this.gValue = gVal;
		this.hValue = hVal;
	}
	public float getFValue()
	{
		return (gValue + hValue);
	}
	public int getNodeIndex()
	{
		return this.nodeIndex;
	}
	public int getGValue()
	{
		return gValue;
	}
	public void setGValue(int gScore)
	{
		this.gValue = gScore;

	}
	public float setFValue(int gScore, float hScore)
	{
		return (gScore + hScore);
	}

	public override string ToString ()
	{
		return string.Format ("node index={0}, g={1}, h={2}", nodeIndex, gValue, hValue );
	}
}

public class SetVisited
{
	int nodeIndex;
	int distFromSource;

	public SetVisited(int index, int dist)
	{
		this.nodeIndex = index;
		this.distFromSource = dist;
	}

	public override bool Equals(object obj)
	{
		SetVisited x = obj as SetVisited;
		if (x == null)
			return false;
		return nodeIndex == x.nodeIndex;
	}

	public override int GetHashCode()
	{
		return (nodeIndex * 3) + 53;
	}

	public override string ToString ()
	{
		return string.Format ("node index={0}, dist={1}", nodeIndex, distFromSource );
	}
	
}
public class SearchResponse
{
	public ArrayList pathNodeIndexArray;
	public int distFromDestination;

	public SearchResponse()
	{
		this.pathNodeIndexArray = new ArrayList ();
		this.distFromDestination = -1;
	}
	public override string ToString ()
	{
		return string.Format ("[SearchResponse]");
	}
}
