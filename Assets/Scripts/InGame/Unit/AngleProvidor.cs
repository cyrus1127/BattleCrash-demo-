using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleProvidor : MonoBehaviour {

	public float turnungDuration = 45.0F;
	public float searchRange = 10F; //radius
	public bool isAI = false;
	public UnitProperty.UnitType targetType;
	public GameObject target;
	public bool isSearchRangeIncludeSelfSize = false;

	bool do_refactAngle = false;
	bool do_FindClosestTarget = false;
	float distance_record;
	AngleUnit myAngle;

	List<GameObject> seekingList;

	// Use this for initialization
	void Start () {
		myAngle = new AngleUnit();
		myAngle.myObject = gameObject;

		if(target == null)
		{
			GetClosestUnit();
		}

		if(gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.AI 
			|| gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport )
		{
			Debug.Log( "AI do select gameBoard object" );
			seekingList = new List<GameObject>();
			GameObject[] options = GameObject.FindGameObjectsWithTag("Unit");
			if(options.Length > 0 )
			{
				foreach( GameObject seekingUnit in options)	
				{
					//Type form
					UnitProperty.UnitType seekingType = seekingUnit.GetComponent<UnitProperty>().type_init;
					if(seekingUnit.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.Player 
						|| seekingUnit.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport)
					{
						seekingType = UnitProperty.UnitType.Player;
					}

					if(seekingType == targetType)
					{
						seekingList.Add(seekingUnit);
					}
				}
			}
		}
	}

	public void AIAction( bool isAttack )
	{
		if(gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.AI 
			|| gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport )
		{
			if(target != null)
			{
				//after found new target, lookat that
				if(isTargetinRange(target))
				{
					LookAtWithTarget(target);
					UnitProperty comp = GetComponent<UnitProperty>();
					if(comp != null)
					{
						if(isAttack)
						{
							comp.AIActionMoveTo( GetTendPosition(target,comp.attack_radius/2) );	
						}else{
							comp.AIActionMoveTo( GetRefactPosition(target,comp.attack_radius/2) );
						}
					}
				}

			}else{
				GetClosestUnit();
			}
		}else{
			//should UnitType.Player
		}
	}

	public float GetCurrentDistance()
	{
		return distance_record;
	}

	public Vector3 GetTendPosition( GameObject in_object, float n_distance )
	{
		float get_angle360 = myAngle.GetAngleBaseMyPositionWithObject(in_object.transform,false);

		//get distance from Angle Class
		Vector3 n_position = myAngle.GetPosition( n_distance, false );

		return n_position;
	}

	public Vector3 GetRefactPosition( GameObject in_object, float n_distance )
	{
		float get_angle360 = myAngle.GetAngleBaseMyPositionWithObject(in_object.transform,false);

		//get distance from Angle Class
		Vector3 n_position = myAngle.GetPosition( n_distance, true );

		return n_position;
	}

	public GameObject GetClosestUnit()
	{
		if( seekingList != null && seekingList.Count > 0)
		{
			float distance_mark = -1;
			foreach( GameObject n_target in seekingList.ToArray()  )
			{
				if( n_target.GetComponent<UnitProperty>() != null)
				{
					bool chooseable = false;
					if(n_target.GetComponent<UnitProperty>().getCurrentState() != UnitProperty.UnitState.Disable)
					{
						chooseable = true;
					}


					Vector3 oA = transform.position;
					Vector3 oB = n_target.transform.position;	

					float opp = Mathf.Max(oA.z, oB.z) - Mathf.Min(oA.z,oB.z);
					float adj = Mathf.Max(oA.x, oB.x) - Mathf.Min(oA.x,oB.x);

					float n_distance = Mathf.Sqrt( Mathf.Pow(opp , 2F) + Mathf.Pow(adj , 2F));

					//DT ( distance ) : 
					float exact_DT = n_distance - Mathf.Max(n_target.transform.localScale.x , n_target.transform.localScale.z)/2;

					//SR ( search Range ): search Range should for much far as vision 
					float exact_SR = searchRange * 10 + ( isSearchRangeIncludeSelfSize ? Mathf.Max(transform.localScale.x , transform.localScale.z) : 0F );

					if(exact_DT < distance_mark || distance_mark == -1)
					{
						if(exact_SR >= exact_DT && chooseable)
						{
							distance_mark = exact_DT;
							if(n_target != target)
								target = n_target;
//							Debug.Log("(" + gameObject.name +") found target(" + target.name + ") in " + distance_record);	
						}
					}
				}
			}

			//final record the mark
			if(target != null)
				distance_record = distance_mark;
		}

		return target;
	}

	bool isTargetinRange( GameObject n_target )
	{
		if(n_target != null && n_target.GetComponent<UnitProperty>() != null)
		{
			bool chooseable = false;
			if(n_target.GetComponent<UnitProperty>().getCurrentState() != UnitProperty.UnitState.Disable)
			{
				chooseable = true;
			}

			Vector3 oA = transform.position;
			Vector3 oB = n_target.transform.position;	

			float opp = Mathf.Max(oA.z, oB.z) - Mathf.Min(oA.z,oB.z);
			float adj = Mathf.Max(oA.x, oB.x) - Mathf.Min(oA.x,oB.x);

			float n_distance = Mathf.Sqrt( Mathf.Pow(opp , 2F) + Mathf.Pow(adj , 2F));

			float exact_DT = n_distance - Mathf.Max(n_target.transform.localScale.x , n_target.transform.localScale.z)/2;
			float exact_SR = searchRange + ( isSearchRangeIncludeSelfSize ? Mathf.Max(transform.localScale.x , transform.localScale.z) : 0F );
			if((exact_SR >= exact_DT && chooseable) || seekingList.Count == 1)
			{
				return true;
			}
		}

		return false;
	}

	public void LookAtWithTargetPosition(Vector3 position, bool is_refactAngle){
		do_refactAngle = is_refactAngle;
		float angle = myAngle.GetAngleBaseMyPositionWithObject(position , do_refactAngle);
		LookAtWithAngle( angle );
	}

	public void LookAtWithTarget(GameObject n_target, bool is_refactAngle){
		do_refactAngle = is_refactAngle;
		LookAtWithTarget(n_target);
	}

	void LookAtWithTarget(GameObject n_target){
		float angle = myAngle.GetAngleBaseMyPositionWithObject(n_target.transform , do_refactAngle);
		LookAtWithAngle( angle );
	}

	void LookAtWithAngle( float n_degree )
	{
		//this function is for rotation, can use lookat to handle instead.
		if(transform.eulerAngles.y != n_degree){
			float speedUp = ( Mathf.Max(transform.eulerAngles.y ,n_degree) - Mathf.Min(transform.eulerAngles.y ,n_degree) / ( turnungDuration * 1000 ))* 1000;
			float timeShift = Time.deltaTime * speedUp;

			float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, n_degree, timeShift);
			transform.eulerAngles = new Vector3(0, angle, 0);
		}
	}
		
}
