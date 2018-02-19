using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Consts{
	public const string default_key_ = "";
	public const string default_key_selectedTeam = "SELECTED_TEAM";  //int
	public const string default_key_currentStage = "SELECTED_STAGE"; //String

}

public class Common
{
	private static Module_Chess chess;

	// ---- user default get-setter handling ---- //
	public static int getSelectedTeam(){ return PlayerPrefs.GetInt( Consts.default_key_selectedTeam );}
	public static void setSelectedTeam(int selected_index){PlayerPrefs.SetInt( Consts.default_key_selectedTeam, selected_index);}

	public static string getCurrentStage(){ return PlayerPrefs.GetString( Consts.default_key_currentStage );}
	public static void setSelectedStage(string selected_stage){PlayerPrefs.SetString( Consts.default_key_currentStage , selected_stage);}

	public static Module_Chess getChessDetails(){
		string filePath = Application.dataPath;
		if(filePath.Length > 0)
		{
			filePath = string.Concat(filePath , "/Resources/stage_ChessDetails.csv");
			Debug.Log(string.Format("target path ? ", filePath));

			string dataString = System.IO.File.ReadAllText(filePath);
			if(dataString.Length > 0)
			{
				Debug.Log(string.Format("readed CSV data : ",dataString));
				chess = new Module_Chess(dataString);
			}
		}

		return chess;
	}

//	public static void csvToJson()
//	{
//		return;
//	}
}