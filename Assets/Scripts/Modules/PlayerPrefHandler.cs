using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefHandler{

	string key_GameModel = "game_model";

	public void setGamePlayModel(int model)
	{
		PlayerPrefs.SetInt(key_GameModel,model);
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
}
