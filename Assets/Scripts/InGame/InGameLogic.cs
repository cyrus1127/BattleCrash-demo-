using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InGameLogic : MonoBehaviour {

	PlayerPrefHandler userDefault;

	public GameBoard myBoard;
	public GameObject singlePlayerGenerationGroup;
	List<Transform> cpuGenePoint = new List<Transform>();
	Transform playerGenePoint;

	//Canvas
	public SimpleTouchController loaclPlayer_controller;
	public GameObject panel_gameEnd;
	public GameObject panel_Loading;
	public Text debugLogBoard;
	int debugLogBoardLogCount = 0;

	//GameState
	enum GameState
	{
		ready = 0,
		readyDone = 1,
		playing = 2,
		ended = 3,
	};
	GameState curGameState;
	public float cpuGeneTimerDuration = 5;
	float cpuGeneTimeCount = 0;
	move_car_NPC cpuPreb;
	move_car userPreb;


	// Use this for initialization
	void Start () {

		{//Game status init


			{//read CSV files
				{//stage
					Common.getChessDetails();
				}
					
				{//prepare chesses
					Common.getSelectedTeam();
					Common.getCurrentStage();
				}
			}

			{
				if(userDefault == null)
				{
					userDefault = new PlayerPrefHandler();
				}

				if(userDefault.isPlayModelSingle())
				{
					if(singlePlayerGenerationGroup != null)
					{
						for(int idx = 0 ; idx < singlePlayerGenerationGroup.transform.childCount; idx++)
						{
							Transform child = singlePlayerGenerationGroup.transform.GetChild(idx);
							if(child.name.Contains("CPU"))
							{
								cpuGenePoint.Add(child);
							}else if(child.name.Contains("User")){
								playerGenePoint = child;
							}
						}
					}

					curGameState = GameState.ready;
				}else{
					//let NetWorkManager to handle
					//not support

					curGameState = GameState.ready;}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(curGameState == GameState.readyDone)
		{
			StartGame();

		}

		if(curGameState == GameState.playing )
		{
			cpuGeneTimeCount -= Time.deltaTime;

			if(cpuGeneTimeCount <= 0)
			{
				doGeneCPU();
				cpuGeneTimeCount = cpuGeneTimerDuration;
			}
		}

		if(curGameState == GameState.ended)
		{
			panel_gameEnd.SetActive(true);
		}
	}

	public void MoveTargetUnitToPoint(Vector3 n_position)
	{
		
	}

	public void resetUnitPos()
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Unit");
		Debug.Log(" resetUnitPos target count ? " + targets.Length);
	}

	public void showDebugMsg(string in_msg)
	{
		Debug.Log("in message will be show on scroll view : [MSG: "+ in_msg + "]");
		if(debugLogBoard != null)
		{
			float textheight = 16;
			float textViewHeight = debugLogBoardLogCount * textheight;
			string curText = debugLogBoard.text;
			curText = in_msg + "\n" + curText;
			debugLogBoard.text = curText;

			debugLogBoardLogCount ++;
		}
	}
		
	//Game cycle
	public void Reset()
	{
		//hide GameEndPanel
		curGameState = GameState.ready;
		panel_gameEnd.SetActive(false);
	}

	void StartGame(){
		curGameState = GameState.playing;
	}

	void doGeneCPU()
	{
		Transform myGenePos = cpuGenePoint[Random.Range(0,cpuGenePoint.Count)];
	}

	void EndGame(){
		panel_gameEnd.SetActive(true);
	}

	public void ExitToMenu(){
		panel_Loading.SetActive(true);

//		Scene tar_scene = SceneManager.GetSceneAt(0);
//		if(tar_scene == null)
		{
			SceneManager.LoadScene(0);	
		}

//		SceneManager.SetActiveScene(tar_scene);
	}

	public void UserOutSideTheBoard(){
		curGameState = GameState.ended;
	}

	public void isUserRegistered()
	{
		curGameState = GameState.readyDone;
	}
}
