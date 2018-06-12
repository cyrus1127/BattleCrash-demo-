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

	public void doRotateByAngle(float in_angle)
	{
		float out_angle;
		Vector3 ax;
		transf.rotation.ToAngleAxis(out out_angle,out ax);

		if(ax.y < 0)
			out_angle = 360 - out_angle;
		
		if( in_angle != out_angle )
		{
			bool isRight = (in_angle > out_angle);

			float dist_1 = Mathf.Max(in_angle,out_angle) - Mathf.Min(in_angle,out_angle);
			float dist_2 = (360F - Mathf.Max(in_angle,out_angle)) + Mathf.Min(in_angle,out_angle);
			if(isRight && dist_1 > 180)
			{
				isRight = false;
			}else if(!isRight && dist_1 > 180)
			{
				isRight = true;
			}

//			Debug.Log("in_angle" +in_angle+ " , out_angle" +out_angle+   ", dist_1 ? " + dist_1 + ", ax : " + ax );

			setIsRightKeyPressed(isRight);
			setIsPressedKeyDetected(true);

		}
	}

	public void setIsRightKeyPressed(bool isRight){
		isRightKeyPressed = isRight;
	}

	public void setIsPressedKeyDetected(bool isPressed){
		isPressedKeyDetected = isPressed;
	}
}


