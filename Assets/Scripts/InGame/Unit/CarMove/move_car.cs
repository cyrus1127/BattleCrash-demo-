﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_car : MonoBehaviour {

	Rigidbody myRig;

	bool isPlayerRegistered = false;
	bool isSideTheWall = false;

	public float maxSpeed = 0;
	public float speedUpDurations_sec = 5;
	public float moveSpeed = 0;

	public float onHitDrag_wall = 5;
	public float onHit_speedRecoverDelay = 0;

	RotateUpdateHelper myRotateUpdate;
	public SimpleTouchController leftController;
	AngleUnit myAngle;
	AngleUnit angleChecker;

	public AttachBarHandler barHander;
	public WeaponController[] weaponHolder;

	powerBarController powerBar;

	// Use this for initialization
	void Awake(){
		if(leftController == null)
		{
			leftController = GameObject.Find("SimpleTouch Joystick").GetComponent<SimpleTouchController>();
		}

		if(powerBar == null)
		{
			powerBar = GameObject.Find("PowerBar").GetComponent<powerBarController>();
		}
	}

	void Start () {
		if(myRig == null)
		{
			myRig = gameObject.GetComponent<Rigidbody>();

			myRotateUpdate = new RotateUpdateHelper();
			myRotateUpdate.transf = transform;

			myAngle = new AngleUnit();
			angleChecker = new AngleUnit();
			angleChecker.myObject = gameObject;

			if(gameObject.transform.GetChild(1).name == "Unit_body")
			{
				Transform body = gameObject.transform.GetChild(1);

				barHander = body.GetComponentInChildren<AttachBarHandler>();
				weaponHolder = body.GetComponentsInChildren<WeaponController>();	
			}else{
				Debug.Log("Unit_body not found");
			}

		}
	}
	
	// Update is called once per frame
	void Update () {
		if(myRig != null)
		{
			myRig.AddRelativeForce(Vector3.forward * moveSpeed);

			speedUp(Time.deltaTime);

			if(myRotateUpdate != null)
			{
				myRotateUpdate.Update();

				if(leftController != null){
					RotatePlayerByController();
				}
			}
		}
	}

	void speedUp(float deltaTime)
	{
		if(onHit_speedRecoverDelay > 0)
		{
			onHit_speedRecoverDelay -= deltaTime;
		}else{

			//check controller is in controlling
			powerBar

			if( maxSpeed > 0 && moveSpeed < maxSpeed)
			{
				float speed_to_add = (((deltaTime * 1000) * (maxSpeed/(speedUpDurations_sec*1000))) /1000);
				speed_to_add = 1;
				if(moveSpeed < maxSpeed/3)
					speed_to_add = 10;
				if(moveSpeed + speed_to_add > maxSpeed)
				{
					moveSpeed = maxSpeed;
				}else{
					moveSpeed += speed_to_add;	
				}
			}	
		}
	}



	void moveBack( Collision collisionInfo ){

//		angleChecker.setTarget(collisionInfo.transform);

		bool isFaceToTarget = false;
		{//check angle
//			float colliedObjectAngle =  angleChecker.getTranslatedCompleteAngle(angleChecker.getCurrentDirection(), angleChecker.getCurrentAngle());
//			float mixAngle_left = 0;
//			float mixAngle_right = 0;

			float out_angle;
			Vector3 ax;
			{//get current angle
				transform.rotation.ToAngleAxis(out out_angle,out ax);
				if(ax.y < 0)
					out_angle = 360 - out_angle;	
			}

			int layerMask = 1 << 8;
			layerMask = ~layerMask;
			RaycastHit hit;
			// Does the ray intersect any objects excluding the player layer
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
			{
				Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
				Debug.Log("Did Hit");
				isFaceToTarget = true;
			}
		}

		if(!isFaceToTarget){
			moveSpeed = 0;
			myRig.AddExplosionForce(250, collisionInfo.transform.localPosition , collisionInfo.transform.localScale.z * 2 , 0,ForceMode.Impulse);	
		}
	}


	//Control
	void RotatePlayerByController()
	{
		myAngle.SetOopAdj( Vector3.zero , new Vector3(leftController.GetTouchPosition.x,0,leftController.GetTouchPosition.y) );

		if(leftController.GetTouchPosition.x != 0 && myRotateUpdate != null)
		{
			float angle =  myAngle.getTranslatedCompleteAngle(myAngle.getCurrentDirection(), myAngle.getCurrentAngle());

			myRotateUpdate.doRotateByAngle(angle);
		}else{
			myRotateUpdate.setIsPressedKeyDetected(false);
		}

	}


	// Collision handling
	void OnCollisionStay(Collision collisionInfo)
	{
		if(!isPlayerRegistered)
		{
			if(collisionInfo.gameObject.tag == "board")
			{
				GameObject main = GameObject.Find("MainScript");
				InGameLogic gameLogic = main.GetComponent<InGameLogic>();
				gameLogic.isUserRegistered();	

				//off the 
			}	
		}else{

			if(collisionInfo.gameObject.tag == "wall")
			{
				if(moveSpeed > 0)
				{
					bool isFaceToTarget = false;
					int layerMask = 1 << 8;
					layerMask = ~layerMask;
					RaycastHit hit;
					// Does the ray intersect any objects excluding the player layer
					if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
					{
						Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
						Debug.Log("Did Hit");
						isFaceToTarget = true;
					}

					Debug.Log("on hit wall");
					if(isFaceToTarget)
					{
						moveSpeed = 0;	
						//					if(onHit_speedRecoverDelay <= 0)
						//					{
						//						onHit_speedRecoverDelay = 1;	
						//					}
					}
				}
			}

			if(collisionInfo.gameObject.tag == "Unit")
			{
				//			moveBack(collisionInfo);

				//Do weapon show

			}

		}
	}

	void OnTriggerStay(Collider collisionInfo)
	{
		if(!isSideTheWall)
		{
			if(collisionInfo.gameObject.tag == "wall")
			{
				isSideTheWall = true;
			}	
		}

	}

	void OnTriggerExit(Collider collisionInfo)
	{
		if(isSideTheWall)
		{
			if(collisionInfo.gameObject.tag == "wall")
			{
				GameObject main = GameObject.Find("MainScript");
				InGameLogic gameLogic = main.GetComponent<InGameLogic>();
				gameLogic.UserOutSideTheBoard();
			}	
		}
	}

}
