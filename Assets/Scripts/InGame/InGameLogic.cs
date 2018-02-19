using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InGameLogic : MonoBehaviour {

	public GameBoard myBoard;

	// Use this for initialization
	void Start () {

		{//Game status init


			{
				//read CSV files
				{
					//stage
//					"/Resources/stage_stageDetails"
					Common.getChessDetails();
					//chess
//					"/Resources/stage_ChessDetails"

					//skill
//					"/Resources/stage_Skill"

				}

				if(true)
				{
					//prepare chesses
					Common.getSelectedTeam();
					Common.getCurrentStage();
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveTargetUnitToPoint(Vector3 n_position)
	{
		
	}

	public void resetUnitPos()
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Unit");
		Debug.Log(" resetUnitPos target count ? " + targets.Length);
		foreach (GameObject target in targets) {
			if (target != null)
			{
				if (target.GetComponent<KnockBackHelper>())
					target.GetComponent<KnockBackHelper>().doResetPostion();	
			}
		}
	}

}
