using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_car_NPC : MonoBehaviour {

	Rigidbody myRig;

	bool isSideTheWall = false;

	public float maxSpeed = 0;
	public float speedUpDurations_sec = 5;
	public float moveSpeed = 0;

	public float onHitDrag_wall = 5;
	public float onHit_speedRecoverDelay = 0;

	public GameObject myTarget;
	string targetTagName = "Player";

	// Use this for initialization
	void Start () {
		if(myRig == null)
		{
			myRig = gameObject.GetComponent<Rigidbody>();
		}

		GameObject new_target = GameObject.FindGameObjectWithTag(targetTagName);
		if(new_target != null)
		{
			myTarget = new_target;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(myRig != null)
		{
			myRig.AddRelativeForce(Vector3.forward * moveSpeed);

			speedUp(Time.deltaTime);

			if(myTarget != null)
			{
				transform.LookAt(myTarget.transform);
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

	public void moveBackBy( Collision collisionInfo ){
		moveSpeed = 0;
		myRig.AddExplosionForce(1000, collisionInfo.transform.localPosition , collisionInfo.transform.localScale.z * 2 , 0,ForceMode.Impulse);
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

				Destroy(gameObject);
			}	
		}
	}
}
