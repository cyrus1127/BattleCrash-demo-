using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGenerator : MonoBehaviour {


	public enum generatepattern{
		none, //should be random
		set_balance,
		set_defence,
		set_attack
	};
	public bool isLeftSide;
	public GameObject chessPrefab;

	float axis_y_angle = 0;
	Vector2 zone_size = Vector2.zero;
	generatepattern cur_pattern = generatepattern.none;

	// Use this for initialization
	void Start () {
		if(isLeftSide)
		{
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject[] setChesses( ArrayList chessesProperties, generatepattern n_pattern)
	{
		GameObject[] chesses; 
		if(chessesProperties != null && chessesProperties.Count > 0 )
		{
			chesses = new GameObject[chessesProperties.Count];	
			return chesses;
		}

		return null;
	}

	Vector2[] getChessesPosition( ArrayList chessProperties , generatepattern n_pattern)
	{
		Vector2[] locations = new Vector2[chessProperties.Count]; 

		switch(n_pattern){
		case generatepattern.set_balance:
			
			break;
		case generatepattern.set_defence:
			
			break;
		case generatepattern.set_attack:
			
			break;
		case generatepattern.none:
		default:
			break;
		}

		return locations;
	}
}
