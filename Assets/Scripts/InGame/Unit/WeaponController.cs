using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController :MonoBehaviour {

	public bool test = false;

	void Start() {
		
	}

	void Update(){

		if(test)
		{
			test = false;

			Action_attack_rotate((360F * 5) , 2.3F);
		}

	}

	public void Action_attack(float actionDuration)
	{
		
	}

	public void Action_attack_rotate(float total_angle, float actionDuration)
	{
		Hashtable tweenInfo = iTween.Hash("y",(total_angle/360F),"loopType", "none","time",actionDuration,"oncomplete","ActionEnd","oncompletetarget",gameObject);
		iTween.RotateBy(gameObject,tweenInfo);	

		showChild(true);
	}

	void ActionEnd(){
		showChild(false);
	}

	void showChild(bool isActive)
	{
		if(gameObject.transform.childCount > 0)
		{
			for(int i = 0; i < gameObject.transform.childCount ; i++)
			{
				gameObject.transform.GetChild(i).gameObject.SetActive(isActive);
			}
		}
	}

}
