using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_car : MonoBehaviour {

	Rigidbody myRig;

	bool isPlayerRegistered = false;
	bool isInsideTheBoard = false;
	bool isSideTheWall = false;
	bool isGameEnded = false;

	public float maxSpeed = 0;
	public float speedUpDurations_sec = 5;
	public float moveSpeed = 0;

	public float onHitDrag_wall = 5;
	public float onHit_speedRecoverDelay = 0;

	RotateUpdateHelper myRotateUpdate;
	public SimpleTouchController leftController;
	bool isleftControllerNotStill = false;
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
		if(myRig != null && !isGameEnded)
		{
			if(myRotateUpdate != null)
			{
				myRotateUpdate.Update();

				if(leftController != null){
					RotatePlayerByController();
				}
			}

			speedUp(Time.deltaTime);
			//update visual
			powerBar.UpdateBarProcess( (moveSpeed/maxSpeed) * 100F);
			//update my speed
			myRig.AddRelativeForce(Vector3.forward * moveSpeed);

		}
	}

	void speedUp(float deltaTime)
	{
		if(onHit_speedRecoverDelay > 0)
		{
			onHit_speedRecoverDelay -= deltaTime;
		}else{
			
			if(isleftControllerNotStill)
			{
				if( maxSpeed > 0 && moveSpeed < maxSpeed)
				{
//					float speed_to_add;// = (((deltaTime * 1000) * (maxSpeed/(speedUpDurations_sec*1000))) /1000);
//					speed_to_add = (maxSpeed /100F) * 5F;
					if(moveSpeed < maxSpeed/3F){
						if(moveSpeed == 0){
							moveSpeed = (maxSpeed /100F) * 5F;
						}else{
							moveSpeed = moveSpeed * 1.05F;
						}
					}else{
						moveSpeed = moveSpeed * 2.2F;
					}

					if(moveSpeed > maxSpeed)
					{
						moveSpeed = maxSpeed;
					}
				}	
			}else{
				if( moveSpeed > 0 )
				{
					moveSpeed = moveSpeed * 0.99F;
					if(moveSpeed <= 0)
						moveSpeed = (maxSpeed /100F) * 5F;
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
//			moveSpeed = 0;
			myRig.AddExplosionForce(250F, collisionInfo.transform.localPosition , collisionInfo.transform.localScale.z * 2 , 0,ForceMode.Impulse);	
		}else{
			move_car_NPC npc_target = collisionInfo.transform.GetComponent<move_car_NPC>();
			if(npc_target != null)
			{
				moveSpeed = moveSpeed * 0.5F;
				npc_target.moveBackBy(transform, maxSpeed/100 * powerBar.CurrentSpeed());
			}
		}
	}


	//Control
	void RotatePlayerByController()
	{
		myAngle.SetOopAdj( Vector3.zero , new Vector3(leftController.GetTouchPosition.x,0,leftController.GetTouchPosition.y) );

		//check user one controll and want to let it move
		float dete = 0F;
		if((leftController.GetTouchPosition.x > dete || leftController.GetTouchPosition.x < -dete) || (leftController.GetTouchPosition.y > dete || leftController.GetTouchPosition.y < -dete))
		{
			isleftControllerNotStill = true;
		}else{
			isleftControllerNotStill = false;
		}

		//check the angle and update it
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
				isPlayerRegistered = true;
				isInsideTheBoard = true;
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
				if(powerBar.CurrentSpeed() >= 50F)
				{
					//Do weapon show
					foreach(WeaponController wc in weaponHolder)
					{
						wc.isLoop = false;
						wc.Action_attack_rotate((360F * 5), 1.3F);
					}
						
					moveBack(collisionInfo);

//					//stop the movement
//					moveSpeed = 0F;
				}
			}

		}
	}

	void OnCollisionExit(Collision collisionInfo) {
		if(collisionInfo.gameObject.tag == "board")
		{
			isInsideTheBoard = false;
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
		if(isSideTheWall && !isInsideTheBoard)
		{
			if(collisionInfo.gameObject.tag == "wall")
			{
				GameObject main = GameObject.Find("MainScript");
				InGameLogic gameLogic = main.GetComponent<InGameLogic>();
				gameLogic.UserOutSideTheBoard();
			}	
		}
	}

	public void StopMove()
	{
		moveSpeed = 0;
		isGameEnded = true;
	}

}
