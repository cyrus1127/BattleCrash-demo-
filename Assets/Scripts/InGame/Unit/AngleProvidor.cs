using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Angle
{
	public Transform myTransfrom;
	public enum TendDirection { undefine , up , down, left , right , upleft, upright , downleft , downright};

	TendDirection direction = TendDirection.undefine;

	float angleInTan = 0F;
	TendDirection directionTended = TendDirection.undefine;

	public TendDirection getCurrentDirection(){ return directionTended;}
	public float getCurrentAngle(){ return angleInTan;}

	public float GetAngleBaseMyPositionWithObject( Transform b_target , bool do_refact)
	{
		return GetAngleBaseMyPositionWithObject( b_target.position , do_refact);
	}

	public float GetAngleBaseMyPositionWithObject( Vector3 oB , bool do_refact)
	{
		float angle_base = 0F;
		if( myTransfrom != null )
		{
			Vector3 oA = myTransfrom.position;

			angleInTan = 0F;

			if(!oA.Equals(oB))
			{
				float opp = Mathf.Max(oA.z, oB.z) - Mathf.Min(oA.z,oB.z);
				float adj = Mathf.Max(oA.x, oB.x) - Mathf.Min(oA.x,oB.x);

				if(opp > 0.1 && adj > 0.1)
				{
					angleInTan = (Mathf.Atan(opp/adj)* Mathf.Rad2Deg);	
				}

				if(oA.z != oB.z || oA.x != oB.x)
				{
					if(opp < 0.1 || adj < 0.1){
						if(adj < 0.1){
							if(oA.z < oB.z){
								angle_base = do_refact ? 180F : 0F ;
								directionTended = do_refact ? TendDirection.down : TendDirection.up ;
							}else if(oA.z > oB.z){
								angle_base = do_refact ? 0F : 180F ;
								directionTended = do_refact ? TendDirection.up : TendDirection.down ;
							}
						}else if(opp < 0.1){
							if(oA.x < oB.x){
								angle_base = do_refact ? 270F :90F;
								directionTended = do_refact ? TendDirection.left : TendDirection.right ;
							}else if(oA.x > oB.x){
								angle_base = do_refact ? 90F :270F;
								directionTended = do_refact ? TendDirection.right : TendDirection.left ;
							}
						}
					}else if(oA.x < oB.x){
						float _base = do_refact ? 270F :90F;
						if(oA.z < oB.z){
							angle_base = _base - angleInTan;
							directionTended = do_refact ? TendDirection.upleft : TendDirection.upright ;
						}else if(oA.z > oB.z){
							angle_base = _base + angleInTan;
							directionTended = do_refact ? TendDirection.downleft : TendDirection.downright ;
						}
					}else if(oA.x > oB.x){
						float _base = do_refact ? 90F :270F;
						if(oA.z < oB.z){
							angle_base = _base + angleInTan;
							directionTended = do_refact ? TendDirection.upright : TendDirection.upleft ;
						}else if(oA.z > oB.z){
							angle_base = _base - angleInTan;
							directionTended = do_refact ? TendDirection.downright : TendDirection.downleft ;
						}
					}
				}
			}
		}

		return angle_base;
	}
		
}

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
	Angle myAngle;


	// Use this for initialization
	void Start () {
		myAngle = new Angle();
		myAngle.myTransfrom = gameObject.transform;

		if(target == null)
		{
			GetClosestUnit();
		}
	}

	public void AIAction()
	{
		if(gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.AI 
			|| gameObject.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport )
		{
			if(do_FindClosestTarget)
			{
				do_FindClosestTarget = false;
				target = null;
			}

			if(target != null)
			{
				//after found new target, lookat that
				if(isTargetinRange(target))
				{
					LookAtWithTarget(target);
					UnitProperty comp = GetComponent<UnitProperty>();
					if(comp != null)
					{
						comp.AIActionMoveTo( GetTendPosition(target,comp.attack_radius/2) );
					}
				}else{
					target = null;
					LookAtWithAngle(0);	
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
		Vector3 n_position = Vector3.zero;

		float get_angle360 = myAngle.GetAngleBaseMyPositionWithObject(in_object.transform,false);
		float get_angle = myAngle.getCurrentAngle();

		float new_x = Mathf.Cos(get_angle)* n_distance;// * Mathf.Rad2Deg * n_distance;
		float new_z = Mathf.Sin(get_angle)* n_distance;// * Mathf.Rad2Deg * n_distance;

		Debug.Log( "(" +gameObject.name+ ") Get ("+in_object.name+") Position ? " + new_x + "," + new_z  + "\n Direction ?" + myAngle.getCurrentDirection() +" \n angle ? " + get_angle);

		switch(myAngle.getCurrentDirection())
		{
		case Angle.TendDirection.up:
			n_position.z = -n_distance;
			break;
		case Angle.TendDirection.right:
			n_position.x = n_distance;
			break;
		case Angle.TendDirection.left:
			n_position.x = -n_distance;
			break;
		case Angle.TendDirection.down:
			n_position.z = n_distance;
			break;
		case Angle.TendDirection.upright:
			n_position.x = -new_x;
			n_position.z = new_z;
			break;
		case Angle.TendDirection.upleft:
			n_position.x = new_x;
			n_position.z = new_z;
			break;
		case Angle.TendDirection.downright:
			n_position.x = -new_x;
			n_position.z = -new_z;
			break;
		case Angle.TendDirection.downleft:
			n_position.x = new_x;
			n_position.z = -new_z;
			break;
		}

		return n_position;
	}

	public Vector3 GetRefactPosition( GameObject in_object, float n_distance )
	{
		Vector3 n_position = Vector3.zero;

		float get_angle360 = myAngle.GetAngleBaseMyPositionWithObject(in_object.transform,true);
		float get_angle = myAngle.getCurrentAngle();

		float new_x = Mathf.Cos(get_angle)* n_distance;// * Mathf.Rad2Deg * n_distance;
		float new_z = Mathf.Sin(get_angle)* n_distance;// * Mathf.Rad2Deg * n_distance;

		Debug.Log( "(" +gameObject.name+ ") Get ("+in_object.name+") Position ? " + new_x + "," + new_z  + "\n Direction ?" + myAngle.getCurrentDirection() +" \n angle ? " + get_angle);

		switch(myAngle.getCurrentDirection())
		{
			case Angle.TendDirection.up:
				n_position.z = n_distance;
			break;
			case Angle.TendDirection.right:
				n_position.x = -n_distance;
			break;
			case Angle.TendDirection.left:
				n_position.x = n_distance;
			break;
			case Angle.TendDirection.down:
				n_position.z = -n_distance;
			break;
			case Angle.TendDirection.upright:
				n_position.x = new_x;
				n_position.z = new_z;
			break;
			case Angle.TendDirection.upleft:
				n_position.x = new_x;
				n_position.z = new_z;
			break;
			case Angle.TendDirection.downright:
				n_position.x = new_x;
				n_position.z = new_z;
			break;
			case Angle.TendDirection.downleft:
				n_position.x = new_x;
				n_position.z = new_z;
			break;
		}

		return n_position;
	}

	public GameObject GetClosestUnit()
	{
		GameObject[] options = GameObject.FindGameObjectsWithTag("Unit");
		if(options.Length > 0)
		{
			float distance_mark = -1;
			foreach( GameObject n_target in options  )
			{
				if( n_target.GetComponent<UnitProperty>() != null)
				{
					bool chooseable = false; 
					if(n_target.GetComponent<UnitProperty>().getCurrentState() != UnitProperty.UnitState.Disable)
					{
						UnitProperty.UnitType seekingType = n_target.GetComponent<UnitProperty>().type_init;
						if(n_target.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.Player 
							|| n_target.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport)
						{
							seekingType = UnitProperty.UnitType.Player;
						}

						if(seekingType == targetType)
						{
							chooseable = true;
						}
					}

					Vector3 oA = transform.position;
					Vector3 oB = n_target.transform.position;	

					float opp = Mathf.Max(oA.z, oB.z) - Mathf.Min(oA.z,oB.z);
					float adj = Mathf.Max(oA.x, oB.x) - Mathf.Min(oA.x,oB.x);

					float n_distance = Mathf.Sqrt( Mathf.Pow(opp , 2F) + Mathf.Pow(adj , 2F));

					//DT ( distance ) : 
					float exact_DT = n_distance - Mathf.Max(n_target.transform.localScale.x , n_target.transform.localScale.z)/2;

					//SR ( search Range ): search Range should for much far as vision 
					float exact_SR = searchRange * 3 + ( isSearchRangeIncludeSelfSize ? Mathf.Max(transform.localScale.x , transform.localScale.z) : 0F );

					if((target == null) || exact_DT < distance_mark || distance_mark == -1)
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
				UnitProperty.UnitType seekingType = n_target.GetComponent<UnitProperty>().type_init;
				if(n_target.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.Player 
					|| n_target.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.PlayerSupport)
				{
					seekingType = UnitProperty.UnitType.Player;
				}

				if(seekingType == targetType)
				{
					chooseable = true;
				}
			}
			Vector3 oA = transform.position;
			Vector3 oB = n_target.transform.position;	

			float opp = Mathf.Max(oA.z, oB.z) - Mathf.Min(oA.z,oB.z);
			float adj = Mathf.Max(oA.x, oB.x) - Mathf.Min(oA.x,oB.x);

			float n_distance = Mathf.Sqrt( Mathf.Pow(opp , 2F) + Mathf.Pow(adj , 2F));

			float exact_DT = n_distance - Mathf.Max(n_target.transform.localScale.x , n_target.transform.localScale.z)/2;
			float exact_SR = searchRange + ( isSearchRangeIncludeSelfSize ? Mathf.Max(transform.localScale.x , transform.localScale.z) : 0F );
			if(exact_SR >= exact_DT && chooseable)
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
