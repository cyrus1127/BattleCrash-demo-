using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_car_NPC : MonoBehaviour {

	Rigidbody myRig;

	bool isSideTheWall = false;
	bool isGameEnded = false;

	public float maxSpeed = 0;
	public float speedUpDurations_sec = 5;
	public float moveSpeed = 0;

	public float onHitDrag_wall = 5;
	public float onHit_speedRecoverDelay = 0;

	public GameObject myTarget;
	string targetTagName = "Player";

	// Use this for initialization

	void Awake(){
		GameObject new_target = GameObject.Find(targetTagName);
		if(new_target != null)
		{
			myTarget = new_target;
			Debug.Log("found GameObject in targetTagName("+ targetTagName +")");
		}else{
			Debug.Log("can't find GameObject in targetTagName("+ targetTagName +")");
		}
	}

	void Start () {
		if(myRig == null)
		{
			myRig = gameObject.GetComponent<Rigidbody>();
		}

		//NPC init speed, dont let it be still too long
		moveSpeed = maxSpeed / 100F * 25F;
	}
	
	// Update is called once per frame
	void Update () {
		if(myRig != null && !isGameEnded)
		{
			myRig.AddRelativeForce(Vector3.forward * moveSpeed);

//			speedUp(Time.deltaTime);

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

	public void moveBackBy( Transform targetTranf , float in_power){
		moveSpeed = 0;
		myRig.AddExplosionForce(in_power, targetTranf.localPosition , targetTranf.localScale.z * 2 , 0,ForceMode.Impulse);
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
				GameObject main = GameObject.Find("MainScript");
				InGameLogic gameLogic = main.GetComponent<InGameLogic>();
				gameLogic.NPCDestoried();

				//do Destory myself
				Destroy(gameObject);
			}	
		}
	}

	public void StopMove()
	{
		moveSpeed = 0;
		isGameEnded = true;
	}
}
