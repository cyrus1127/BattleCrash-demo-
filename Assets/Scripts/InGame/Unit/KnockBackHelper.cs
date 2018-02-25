using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpeedUnit{
	public float speed_base = 0;
	public float speed_delta = 0;

	public void UpdateSpeed( float time_past)
	{
		
	}
}

public class KnockBackHelper : MonoBehaviour {

	public float radius_explode = 5.0F;
	public bool doExplode = false;

	float power = 0F;
	float upWard = 0F;

	Vector3 startPos;

	// Use this for initialization
	void Start () {
		doExplode = false;
		startPos = transform.position;
	}

	public void doResetPostion()
	{
		{
			GetComponent<Rigidbody>().isKinematic = true;

			Debug.Log(" doResetPostion to ? " + startPos);
			transform.position = startPos;
			transform.rotation = Quaternion.AngleAxis(0F , Vector3.up);

			GetComponent<Rigidbody>().isKinematic = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(doExplode)
		{
			doExplodionKnockBackWithTween( GameObject.FindGameObjectWithTag("Item") , 300F );
			doExplode = false;
		}


		DrawDebugline();
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
			deltaPosition.x += gameObject.transform.position.x;
			deltaPosition.z += gameObject.transform.position.z;

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

	public void doMoveToPositionWithUpdate(Vector3 n_point)
	{
		Debug.Log("("+gameObject.name+") MoveTo " + n_point +" With update" );

		float moveSpeed = 10F;
		Hashtable moveHash = iTween.Hash("x", n_point.x, "z", n_point.z, "easeType", "linear", "loopType", "none", "speed", moveSpeed);
		moveHash.Add("onstarttarget",gameObject);
		moveHash.Add("onstart","moveStart");
		moveHash.Add("oncompletetarget",gameObject);
		moveHash.Add("oncomplete","moveEnd");
		moveHash.Add("onupdatetarget",gameObject);
		moveHash.Add("onupdate","onMoveUpdate");
		iTween.MoveTo(gameObject, moveHash);
	}

	public float ReceivePowerToDistance( float n_power , float n_dist)
	{
		float dist_out = 0F;

		return dist_out;
	}

	public void onMoveUpdate()
	{
		Debug.Log("onMoveUpdate");
	}
}
