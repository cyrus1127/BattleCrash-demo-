using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchHelpper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//		Camera camera = GameObject.Find("Main Camera");
		//		if(camera)
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				touchBegan();
			}

			if (Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				touchMove();
			}

			//			Physics.Raycast();	
		}else{
			
		}
	}

	void touchBegan(){
		Debug.Log("touch Began");
	}

	void touchMove(){
		Debug.Log("touch Moved");
		//				// Get movement of the finger since last frame
		//				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
		//
		//				// Move object across XY plane
		//				transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
	}
}
