using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHandler : MonoBehaviour {

	public GameObject UnitStatesBar;
	public GameObject GameStatesBar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setGameReady()
	{
		GameStatesBar.SetActive(true);
	}

	public void setGameStart()
	{
		GameStatesBar.SetActive(false);
	}

	public void setGameEnd()
	{
		GameStatesBar.SetActive(true);
	}
}
