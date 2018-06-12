using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePageMainController : MonoBehaviour {

	PlayerPrefHandler userDefault;

	//canvas
	public GameObject panel_Loading;

	// Use this for initialization
	void Start () {
		if(userDefault == null)
		{
//			string instanceName = "PlayerPrefHandler";
//			GameObject obj_PlayerPrefHandler = GameObject.Find(instanceName);
//			if(obj_PlayerPrefHandler == null)
//			{
//				obj_PlayerPrefHandler = new GameObject("PlayerPrefHandler");
//				DontDestroyOnLoad(obj_PlayerPrefHandler);	
//				obj_PlayerPrefHandler.AddComponent<PlayerPrefHandler>();
//			}
//				
//			userDefault = obj_PlayerPrefHandler.GetComponent<PlayerPrefHandler>();

			userDefault = new PlayerPrefHandler();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayerAsSingle(){ 
		userDefault.setGamePlayModel(0); 
		panel_Loading.SetActive(true);
		SceneManager.LoadScene(1);
//		SceneManager.SetActiveScene(1);
	}

	public void PlayerAsLanMultiPlayer(){ userDefault.setGamePlayModel(1); }
}
