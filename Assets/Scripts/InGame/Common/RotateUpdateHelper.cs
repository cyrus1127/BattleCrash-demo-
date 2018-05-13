using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateUpdateHelper
{
	static string input_key_left = "left";
	static string input_key_right = "right";

	public Transform transf;
	public float rotateSpeed = 2.5f;

	bool isPressedKeyDetected = false;
	bool isRightKeyPressed = false;

	public void Update()
	{
		handleKeyboardButton();
		doRotate();
	}

	void handleKeyboardButton(){
		if(Input.GetKeyDown(input_key_right) || Input.GetKeyDown(input_key_left))
		{
			if(!isPressedKeyDetected)
			{
				isPressedKeyDetected = true;
				isRightKeyPressed= Input.GetKeyDown(input_key_right);
				Debug.Log("" + (isRightKeyPressed?"right":"Left") + " key was pressed");
			}
		}

		if(Input.GetKeyUp(input_key_right) || Input.GetKeyUp(input_key_left)){
			//check release
			if(isPressedKeyDetected)
			{
				if(isRightKeyPressed && Input.GetKeyUp(input_key_right)){
					isPressedKeyDetected = false;
					Debug.Log("right key was released");
				}

				if(!isRightKeyPressed && Input.GetKeyUp(input_key_left)){
					isPressedKeyDetected = false;
					Debug.Log("left key was released");
				}
			}
		}	
	}

	void doRotate()
	{
		if(isPressedKeyDetected)
		{
			float angle = 0;
			angle =(isRightKeyPressed)? rotateSpeed: -rotateSpeed;
			transf.Rotate(Vector3.up,angle);
		}
	}

	public void setIsRightKeyPressed(bool isRight){
		isRightKeyPressed = isRight;
	}

	public void setIsPressedKeyDetected(bool isPressed){
		isPressedKeyDetected = isPressed;
	}
}


