using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	public GameObject target;
	public Camera myCamera;

	void Start(){
		
		Hashtable moveHash;
		moveHash = iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1);
		if(myCamera != null)
		{
			Debug.Log("myCamera object outlet connected, do Tween move Update");
			moveHash.Add("looktarget",myCamera.transform);
			moveHash.Add("looktime",0.1);
			moveHash.Add("axis","y");
//			gameObject.transform.LookAt(myCamera.transform);
		}else{
			Debug.Log("myCamera outlet not connected");
		}

		iTween.MoveBy(gameObject,moveHash);
	}
}



