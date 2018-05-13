using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_car : MonoBehaviour {

	Rigidbody myRig;

	public float maxSpeed = 0;
	public float speedUpDurations_sec = 5;
	public float moveSpeed = 0;

	public float onHitDrag_wall = 5;
	public float onHit_speedRecoverDelay = 0;

	RotateUpdateHelper myRotateUpdate;
	public SimpleTouchController leftController;

	// Use this for initialization
	void Start () {
		if(myRig == null)
		{
			myRig = gameObject.GetComponent<Rigidbody>();

			myRotateUpdate = new RotateUpdateHelper();
			myRotateUpdate.transf = transform;
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
			if( maxSpeed > 0 && moveSpeed < maxSpeed)
			{
				float speed_to_add = (((deltaTime * 1000) * (maxSpeed/(speedUpDurations_sec*1000))) /1000);
				speed_to_add = 1;
				if(moveSpeed + speed_to_add > maxSpeed)
				{
					moveSpeed = maxSpeed;
				}else{
					moveSpeed += speed_to_add;	
				}
			}	
		}
	}

	void OnCollisionStay(Collision collisionInfo)
	{
		if(collisionInfo.gameObject.tag == "wall")
		{

			if(moveSpeed == 0)
			{
				
			}else{
				Debug.Log("on hit wall");

//				moveSpeed -= onHitDrag_wall;
				{
					moveSpeed = maxSpeed * 0.25F;
				}

				if(onHit_speedRecoverDelay <= 0)
				{
					onHit_speedRecoverDelay = 1;	
				}
			}
		}

		if(collisionInfo.gameObject.tag == "Unit")
		{
			moveBack(collisionInfo);
		}
	}

	void moveBack( Collision collisionInfo ){
		moveSpeed = 0;
		myRig.AddExplosionForce(250, collisionInfo.transform.localPosition , collisionInfo.transform.localScale.z * 2 , 0,ForceMode.Impulse);
	}


	//Control
	void RotatePlayerByController()
	{
		if(leftController.GetTouchPosition.x != 0 && myRotateUpdate != null)
		{
			if(leftController.GetTouchPosition.x > 0) // right
			{
				myRotateUpdate.setIsRightKeyPressed(true);
			}else if(leftController.GetTouchPosition.x < 0)// left
			{
				myRotateUpdate.setIsRightKeyPressed(false);
			}	

			myRotateUpdate.setIsPressedKeyDetected(true);
		}else{
			myRotateUpdate.setIsPressedKeyDetected(false);
		}

	}
}
