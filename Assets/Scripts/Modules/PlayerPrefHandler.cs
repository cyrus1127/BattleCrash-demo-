using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefHandler{

	string key_GameModel = "game_model";
	string key_HighScore = "game_highScore";

	public void setGamePlayModel(int model)
	{
		PlayerPrefs.SetInt(key_GameModel,model);
	}

	public void SetHighestScore(int score)
	{
		PlayerPrefs.SetInt(key_HighScore,score);
	}

	public bool isPlayModelSingle()
	{
		int cur_model_index = PlayerPrefs.GetInt(key_GameModel);

		if(cur_model_index == 0)
		{
			return true;
		}

		return false;
	}

	public int GetHighestScore()
	{
		return PlayerPrefs.GetInt(key_HighScore);
	}
}
