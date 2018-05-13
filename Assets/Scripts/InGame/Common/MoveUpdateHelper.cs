using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveUpdateHelper 
{
	static string input_key_fire = "space";

	List<Vector3> path; // reference to the path
	Rigidbody targetRigid;
	GameObject explosionCube;
	GameObject explosionCube_prefab;
	WeaponController[] weapon;
	SpeedUnit myMove;
	bool isPressedKeyDetected = false;
	float targetMaxCharge = 10F;
	float chargedPower = 0;
	float releasePower = 0;
	float chargeDuration = 1F;

	public void Set_ChargablePower(float in_power , float in_duration)
	{
		targetMaxCharge = in_power;
		chargeDuration = in_duration;
	}

	public void Set_Rigid(Rigidbody rb)
	{
		if(targetRigid != rb)
			targetRigid = rb;

		if(myMove == null)
			myMove = new SpeedUnit();
	}

	public void Set_ExplosionCube(GameObject ec)
	{
		explosionCube_prefab = ec;

		if(explosionCube == null && explosionCube_prefab != null)
		{
			explosionCube = GameObject.Instantiate(explosionCube_prefab);
		}
	}

	public void Set_WeapanHelp(WeaponController[] wc)
	{
		weapon = wc;
	}

	public void AddPath(Vector3 n_path)
	{
		if(path == null)
		{
			path = new List<Vector3>();
		}

		if(path.Count > 1)
		{
			path.Insert(0,n_path); //keep to add path on first index;
		}else{
			path.Add(n_path);
		}
	}

	public void Update()
	{
		handleKeyboardButton();
		holdAndCharge();

		doCheckPathAndMove();
	}

	void handleKeyboardButton(){

		if (Input.GetKeyDown(input_key_fire))
		{
			if(!isPressedKeyDetected)
			{
				isPressedKeyDetected = true;
				chargedPower = 0; // do reset;
				releasePower = 0;
				Debug.Log("fire key was pressed");
			}

		}
		if(Input.GetKeyUp(input_key_fire)){
			if(isPressedKeyDetected)
			{
				isPressedKeyDetected = false;
				//				if(chargingPower > 0.5f)
				{
					//do :fire 
					doEmit();
				}
				Debug.Log("fire key was released");
			}
		}
	}

	void holdAndCharge()
	{
		if(isPressedKeyDetected)
		{
			chargedPower += targetMaxCharge * (0.1f * Time.deltaTime );
			if(chargedPower > targetMaxCharge)
			{
				chargedPower = targetMaxCharge;
				Debug.Log("Power charged done");
			}else{
				float pctage = ((chargedPower/targetMaxCharge)*100);
				Debug.Log("Power is charging ("+ pctage +")");

				//update explosion cube position
				if(explosionCube != null)
				{
					float max_distance = 10f;
					float z_distance = targetRigid.transform.localScale.z/2 + (max_distance - (max_distance/100 * pctage));
					z_distance = targetRigid.transform.localScale.z/2; // test
					Vector3 n_localPostion = targetRigid.transform.localPosition;
					n_localPostion += (targetRigid.transform.forward * -(z_distance + explosionCube.transform.localScale.z));

					//set to cube
					explosionCube.transform.localPosition = n_localPostion;	
					explosionCube.transform.localRotation = targetRigid.transform.localRotation;
				}
			}
		}
	}

	void doEmit()
	{
		releasePower = chargedPower;
		chargedPower = 0;

		if(releasePower > 0)
		{
			//setUp speedUtil
			if(explosionCube == null)
			{
				float total_distance = (releasePower * 60F * 0.8F);
				myMove.setTarget(releasePower,total_distance,chargeDuration);	
			}else{
				//if explosionCube is here, the speedUnit will no need
				if(weapon != null)
				{
					foreach(WeaponController wp in weapon)
					{
						wp.Action_attack_rotate( 360F*3, 1.5F );	
					}
				}
			}

		}
	}

	void doCheckPathAndMove()
	{
		if(path != null)
		{
			if(targetRigid.transform.position.Equals( path[path.Count - 1]))
			{
				//				targetRigid.MovePosition(targetRigid.transform.position + targetRigid.transform.forward * Time.deltaTime * moveSpeed);	
			}
		}else{
			if(releasePower > 0)
			{
				float moveSpeed = 0;
				if(explosionCube == null)
				{
					moveSpeed = myMove.UpdateSpeed(Time.deltaTime); //get speed
					if(moveSpeed > 0){
						float timeScale = myMove.getTimeScaleBySpeed();

						Debug.Log("onMoveUpdate moveSpeed? " + moveSpeed + " , releasePower ? " + releasePower);
						targetRigid.MovePosition(targetRigid.transform.position + targetRigid.transform.forward * (Time.deltaTime * timeScale));
					}

					if(moveSpeed <= 0)
					{
						releasePower = 0;
					}
				}else{
					float z_distance = 10; //releasePower effect

					if(targetRigid != null)
					{
						targetRigid.AddExplosionForce(targetMaxCharge,explosionCube.transform.localPosition, z_distance, -1F , ForceMode.Impulse  );
						releasePower = 0;
					}
				}
			}
		}
	}

	public bool isMoveEnd()
	{
		if(explosionCube == null)
		{
			bool isEnd = (releasePower == 0);

			isEnd = (path.Count == 0);

			return isEnd;
		}else{
			if(targetRigid != null)
			{

			}
		}

		return true;
	}
}

