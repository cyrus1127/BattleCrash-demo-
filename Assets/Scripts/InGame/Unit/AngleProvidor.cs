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
	DetectionUnit myDetectionHelper;

	List<GameObject> seekingList;

	// Use this for initialization
	void Start () {
		myAngle = new AngleUnit();
		myAngle.myObject = gameObject;

		myDetectionHelper = GetComponent<DetectionUnit>();
		if(myDetectionHelper == null)
		{
			myDetectionHelper = gameObject.AddComponent<DetectionUnit>();
			myDetectionHelper.setDetectionRadius(gameObject.GetComponent<UnitProperty>().attack_radius);
		}

		if(gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.AI 
			|| gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport )
		{
			if(target == null)
			{
				GetClosestUnit();
			}

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
		UnitProperty comp = GetComponent<UnitProperty>();

		if(comp != null)
		{
			if(comp.type_init == UnitProperty.UnitType.AI 
				|| comp.type_init == UnitProperty.UnitType.PlayerSupport )
			{
				if(target != null)
				{
					//after found new target, lookat that
					if(isTargetInSearchRange(target))
					{

						//do check the target is out of my attack range
						bool isTargetInAttackRange = isTargetinRange( target , comp.attack_radius , false);

						if( isTargetInAttackRange )
						{
							if(isAttack)
							{
								//be still and make attack
								LookAtWithTarget(target);
								comp.moveEnd(); //call MoveEnd() to do checkAttack()
							}else{
								//move out of Attack Range
//								Vector3 n_pos = GetRefactPosition(target,comp.attack_radius/2);

								if(myDetectionHelper.haveWaysToGoOut(myAngle))
								{
									//check backward possibility
									comp.AIActionMoveTo( myAngle.GetPosition(comp.attack_radius/2,false) );
								}else{
									//try to Attack , as no way to go away
									comp.moveEnd();
								}
							}
						}else{
							if(isAttack)
							{
								//go and make attack
								LookAtWithTarget(target);
								comp.AIActionMoveTo( GetTendPosition(target,comp.attack_radius/2) );	
							}else{
								//be still to wait
								LookAtWithTarget(target);
								comp.moveEnd();
							}
						}
							
					}else{
						//do search
						GetClosestUnit();
					}

				}else{
					GetClosestUnit();
				}
			}else{
				//should UnitType.Player
			}
		}else{
			Debug.Log("A.I missing basic Component ( UnitProperty ) ");
		}
	}

	public float GetCurrentDistance()
	{
		return distance_record;
	}

	public Vector3 GetTendPosition( GameObject in_object, float n_distance )
	{
		//get_angle360
		myAngle.GetAngleBaseMyPositionWithObject(in_object.transform,false);

		//get distance from Angle Class
		Vector3 n_position = myAngle.GetPosition( n_distance, false );

		return n_position;
	}

	public Vector3 GetTendPositionWithOutNewTarget(float n_distance )
	{
		//get distance from Angle Class
		Vector3 n_position = myAngle.GetPosition( n_distance, false );

		return n_position;
	}

	public Vector3 GetRefactPosition( GameObject in_object, float n_distance )
	{
		//get_angle360
		myAngle.GetAngleBaseMyPositionWithObject(in_object.transform,false);

		//get distance from Angle Class
		Vector3 n_position = myAngle.GetPosition( n_distance, true );

		return n_position;
	}


	/*
	 * This Method should just provide to A.I. to have a ability to search enemy.
	 * If lose the target A.I. would not do move and attack. 
	 * Is an important part on console
	 */
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

	public bool isTargetinRange( GameObject n_target , float expectedRange , bool justForSearch)
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

			//DT ( distance ) : 
			float exact_DT = n_distance - Mathf.Max(n_target.transform.localScale.x , n_target.transform.localScale.z)/2;

			//SR ( search Range ): search Range should for much far as vision 
			float exact_SR = expectedRange - ( isSearchRangeIncludeSelfSize ? Mathf.Max(transform.localScale.x , transform.localScale.z) : 0F );
			if((exact_SR >= exact_DT && chooseable))
			{
				if(justForSearch)
				{
					if(seekingList.Count > 1)
					{
						return false;	
					}	
				}

				return true;
			}
		}

		return false;

	}

	// override the isTargetinRange(GameObject,Float) for just for searchRange
	bool isTargetInSearchRange( GameObject n_target )
	{
		return isTargetinRange(n_target, searchRange * 10 , true);
	}

	//will return distance
	public float LookAtWithTargetPosition(Vector3 position, bool is_refactAngle){
		do_refactAngle = is_refactAngle;
		float angle = myAngle.GetAngleBaseMyPositionWithObject(position , do_refactAngle);
		LookAtWithAngle( angle );

		return myAngle.getCurrentDistance();
	}

	//will return distance
	public float LookAtWithTarget(GameObject n_target, bool is_refactAngle){
		do_refactAngle = is_refactAngle;
		LookAtWithTarget(n_target);

		return myAngle.getCurrentDistance();
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
