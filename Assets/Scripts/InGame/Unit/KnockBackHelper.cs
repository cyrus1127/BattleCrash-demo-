using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackHelper : MonoBehaviour {

	public float radius_explode = 5.0F;
	public bool doExplode = false;
	public bool testExplode = false;
	public float speedUpDuration = 0.2F;

	int playerindex = 0;
	controller_keyboard curMappedController;

	public float power = 0F;
	public float upWard = 1F;

	Vector3 startPos;
	iTween cur_tween;
	SpeedUnit myMove;

	MoveUpdateHelper myMoveUpdate;
	RotateUpdateHelper myRotateUpdate;
	WeaponController[] myWeapon;
	Rigidbody rb;
	public GameObject explosionCube_prefab;

	//turn handling parameter
	bool onRotate;

	// Use this for initialization
	void Start () {
		doExplode = false;
		startPos = transform.position;
//		myMove = new SpeedUnit();

		curMappedController = new controller_keyboard();
		rb = GetComponent<Rigidbody>();
		myWeapon = gameObject.GetComponentsInChildren<WeaponController>();

		if(myMoveUpdate == null && rb != null)
		{
			if(gameObject.GetComponent<UnitProperty>()!= null)
			{
				myMoveUpdate = new MoveUpdateHelper();
				myMoveUpdate.Set_Rigid(rb);
				myMoveUpdate.Set_ExplosionCube(explosionCube_prefab);
				myMoveUpdate.Set_ChargablePower( gameObject.GetComponent<UnitProperty>().power_init , gameObject.GetComponent<UnitProperty>().speedUpDuration_init );
				if(myWeapon != null)
					myMoveUpdate.Set_WeapanHelp(myWeapon);
				myRotateUpdate = new RotateUpdateHelper();
				myRotateUpdate.transf = transform;
			}
		}
	}

	public void doResetPostion()
	{
		GetComponent<Rigidbody>().isKinematic = true;

		Debug.Log(" doResetPostion to ? " + startPos);
		transform.position = startPos;
		transform.rotation = Quaternion.AngleAxis(0F , Vector3.up);

		GetComponent<Rigidbody>().isKinematic = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(doExplode)
		{
			
			doExplode = false;

			//Debug
			if(testExplode){
				foreach(GameObject ob in GameObject.FindGameObjectsWithTag("Unit"))
				{
					ob.GetComponent<Rigidbody>().AddExplosionForce(power,gameObject.transform.position,radius_explode,0F,ForceMode.Impulse);
				}
			}else{
				doExplodionKnockBackWithTween( GameObject.FindGameObjectWithTag("Item") , 300F );
			}
		}
			
		DrawDebugline();
	}

	void FixedUpdate() {
		if(rb != null)
		{
			if(myMoveUpdate != null)
				myMoveUpdate.Update();
			if(myRotateUpdate != null)
				myRotateUpdate.Update();
		}
	}

	void OnDrawGizmosSelected() {
		DrawDebugline();
	}

	void DrawDebugline()
	{
		if(true){
			for(int i = 0; i < 6 ; i++)
			{
				Vector3 endPoint = transform.position;
				if( i < 2)
					endPoint.x += i < 1 ? radius_explode : radius_explode*-1;
				else if (i < 4)
					endPoint.z += i < 3 ? radius_explode : radius_explode*-1;
				else
					endPoint.y += i < 5 ? radius_explode : radius_explode*-1;
				Debug.DrawLine(transform.position, endPoint, Color.red);	
			}
		}
	}

	public void setPlayerIndex(int setToIndex)
	{
		playerindex = setToIndex;
	}

	void doExplodionKnockBackWithPhysic(){
		Vector3 explosionPos = transform.position;
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Unit");
		Debug.Log(" target count ? " + targets.Length);
		foreach (GameObject target in targets) {
			if (target != null)
			{
				if (target.GetComponent<Rigidbody>())
					target.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius_explode, upWard);	
			}
		}
	}

	public void doExplodionKnockBackWithTween( GameObject by_target , float received_atk_power )
	{
		float min_knockBack_distance = 5F; // in pixle 
		//get angle : 
		// 1. use by_target to calcu with size
		// 2. use get by_target component<AngleProvidor> to handle direction;

		float knockDistance = min_knockBack_distance;// ReceivePowerToDistance(received_atk_power, gameObject.GetComponent<AngleProvidor>().GetCurrentDistance());
		if( knockDistance < min_knockBack_distance )
			knockDistance = min_knockBack_distance; //hard code minimue distance

		Vector3 deltaPosition = gameObject.GetComponent<AngleProvidor>().GetRefactPosition(by_target , knockDistance);

		if(deltaPosition != Vector3.zero)
		{
			Debug.Log("("+gameObject.name+") kockBack by ("+ by_target.name+ ") to " + deltaPosition );
			doMoveToPosition(deltaPosition);

		}else{
			Debug.Log("kockBack not effected !! position ? " + deltaPosition );
		}
	}

	public void doMoveToPosition(Vector3 n_point)
	{
		Debug.Log("("+gameObject.name+") MoveTo " + n_point );

		float moveSpeed = 10F;
		Hashtable moveHash = iTween.Hash("x", n_point.x, "z", n_point.z, "easeType", "linear", "loopType", "none", "speed", moveSpeed);
		moveHash.Add("onstarttarget",gameObject);
		moveHash.Add("onstart","moveStart");
		moveHash.Add("oncompletetarget",gameObject);
		moveHash.Add("oncomplete","moveEnd");
		iTween.MoveTo(gameObject, moveHash);
	}

	public void doMoveToPositionWithUpdate(SpeedUnit in_speedUnit , Vector3 n_point)
	{
		Debug.Log("("+gameObject.name+") MoveTo " + n_point +" With update" );

		if(in_speedUnit != null)
		{
//			moveUpdatePos = n_point;
			myMove = in_speedUnit;
			myMove.getTimeScaleBySpeed();
			Hashtable moveUpdateHash = iTween.Hash("x", n_point.x, "z", n_point.z, "easeType", "easeInCubic", "loopType", "none", "speed", gameObject.GetComponent<UnitProperty>().getMaxMoveSpeed());
			moveUpdateHash.Add("onstarttarget",gameObject);
			moveUpdateHash.Add("onstart","moveStart");
			moveUpdateHash.Add("oncompletetarget",gameObject);
			moveUpdateHash.Add("oncomplete","moveEnd");
			moveUpdateHash.Add("onupdatetarget",gameObject);
			moveUpdateHash.Add("onupdate","onMoveUpdate");
			iTween.MoveTo(gameObject, moveUpdateHash);	

			//use moveUpdate ....... 
//			myMoveUpdate = new MoveUpdateHelper();
		}else{
			Debug.Log(" SpeedUnit is required ! ");
		}
			
	}

	public float ReceivePowerToDistance( float n_power , float n_dist)
	{
		float dist_out = 0F;

		return dist_out;
	}
		
	public float GetCurrentMoveSpeed()
	{
		return myMove.getCurSpeed();
	}
}
