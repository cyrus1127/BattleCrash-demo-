﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProperty : MonoBehaviour {

	public UnitType type_init;
	public UnitState state_init;
	public float healthPoint_Init = 10;
	public float weight_init = 1;
	public float power_init = 1;
	public float chargebonus_init = 1;
	public float speedUpDuration_init = 0.2F;
	public float attack_radius = 5F;
	public float friction = 0F;
	public float coolDownTime_normalAttack = 0.2F;
	public float AIWarninghealthPercentage = 30; //For AI

	AttachBarHandler[] barHanders;
	float coolDownTimeCount;
	bool isReadyToAttack;
	float healthPoint;
	float healthPoint_buff;
	float weight;
	float weight_buff;
	float power;
	float power_buff;
	float chargebonus;
	float chargebonus_buff;
	float speedUpDuration;
	float speedUpDuration_buff;
	float buff_duration;
	AngleProvidor myAngleProvidor;
	BuffType curBuffType;
	UnitState state;

	GameObject body;
	GameObject particle;
	GameObject weaponHolder;
	Rigidbody rb;


	GameBoard delegate_board;

	bool onMove_AI;
	public bool debug_onMove_AI = false;

	enum ChildIndex
	{
		body = 0,
		particleEffect = 1,
		totalChildCount
	};

	enum inBodyChildIndex
	{
		point = 0,
		coolDownBar = 1,
		weaponHolder = 2,
		totalChildCount
	}

	public enum UnitState
	{
		Disable,
		Normal,
		Effected,
	};
	public enum BuffType
	{
		None,
		Power,
		Weight,
		HealthPoint
	};
	public enum UnitType
	{
		AI,
		PlayerSupport,
		Player,
	};

	// Use this for initialization
	void Start () {

		if(transform.childCount == (int)UnitProperty.ChildIndex.totalChildCount)
		{
			Debug.Log(@"is using version 2 unit perfab!");
			body = transform.GetChild( (int)ChildIndex.body ).gameObject;

		}else{
			Debug.Log(@"is using version 1 unit.");
			body = gameObject;
			rb = GetComponent<Rigidbody>();
		}
		particle = transform.GetChild( (int)ChildIndex.particleEffect ).gameObject;
		if(body != null)
		{
			barHanders = body.GetComponentsInChildren<AttachBarHandler>();	
			weaponHolder = body.transform.GetChild( (int)inBodyChildIndex.weaponHolder ).gameObject;
		}


		isReadyToAttack = true;
		coolDownTimeCount = coolDownTime_normalAttack;

		healthPoint = healthPoint_Init;
		power = power_init;
		weight = weight_init;
		speedUpDuration = speedUpDuration_init;
		chargebonus = chargebonus_init;

		power_buff = 0;
		weight_buff = 0;
		healthPoint_buff = 0;
		chargebonus_buff = 0;
		speedUpDuration_buff = 0;
		state = state_init;
		onMove_AI = false;


	}

	void initAngleProvidor()
	{
		myAngleProvidor = GetComponent<AngleProvidor>();
		if(myAngleProvidor == null)
		{
			myAngleProvidor = gameObject.AddComponent<AngleProvidor>();
		}

		myAngleProvidor.searchRange = attack_radius * 2;	

	}

	// Update is called once per frame
	void Update () {
		UpdateAttackCDTimeBar();
		UpdateBuffTime();

		//AI
		UpdateAIAction();

		if(type_init == UnitType.Player)
		{
			//do we need to do attack ??
		}
			


	}

	/// <summary>
	/// Updates the AI action.
	/// </summary>

	public void UpdateAIAction()
	{
		if(type_init == UnitType.AI || type_init == UnitType.PlayerSupport)
		{
			if(!onMove_AI && state == UnitState.Normal){ 
//			if(!onMove_AI && debug_onMove_AI){ // for debuging 
				bool isTimeToAttack = false;
				if( getIsReadyToAttack() )
				{
					isTimeToAttack = true;

					//A.I do check current Heath Point is eqaul or under Warninghealth
					if(AIWarninghealthPercentage > 0 && healthPoint <= (healthPoint_Init * AIWarninghealthPercentage)/100 )
					{// is in danger 
						
//						if(Random.Range(0,10) < 8)
//						{
//							isTimeToAttack = true;
//						}else{
							isTimeToAttack = false;
//						}
					}else{
						//do attack
					}
				}
					
				GetComponent<AngleProvidor>().AIAction(isTimeToAttack);	
				debug_onMove_AI = false;
			}
		}
	}

	public void AIActionMoveTo( Vector3 n_point )
	{
		KnockBackHelper comp_kb = GetComponent<KnockBackHelper>();
		if(comp_kb != null){

			SpeedUnit n_speedUnit = new SpeedUnit();
			n_speedUnit.setTarget( getMaxMoveSpeed() , attack_radius/2 , getSpeedUpDuration() );

			comp_kb.doMoveToPositionWithUpdate(n_speedUnit , n_point);
			Debug.Log("Do move to " + n_point);
		}
	}

	/// <summary>
	/// Players the action move to.
	/// </summary>
	/// <param name="n_point">N point.</param>

	public void PlayerActionMoveTo( Vector3 n_point )
	{
		//tend to target position
		AngleProvidor comp_tend = GetComponent<AngleProvidor>();
		float lookAtPos_distance = comp_tend.LookAtWithTargetPosition(n_point,false);

		//do transform the n_point with maximu move distance
		if(lookAtPos_distance > attack_radius/2)
		{
			Debug.Log("lookat Position "+ n_point+" is too far. ");
			n_point = comp_tend.GetTendPositionWithOutNewTarget(attack_radius/2);	
		}

		KnockBackHelper comp_kb = GetComponent<KnockBackHelper>();
		if(comp_kb != null){

			SpeedUnit n_speedUnit = new SpeedUnit();
			n_speedUnit.setTarget( getMaxMoveSpeed() , attack_radius/2 , getSpeedUpDuration() );

			comp_kb.doMoveToPositionWithUpdate(n_speedUnit,n_point);
			Debug.Log("Do move to " + n_point);
		}
	}

	public void setGameBoardDelegate( GameBoard _delegate )
	{
		if(_delegate != null)
			delegate_board = _delegate;
	}

	void UpdateAttackCDTimeBar()
	{
		if(barHanders != null && barHanders.Length > 0)
		{
			if(!isReadyToAttack)
			{
				coolDownTimeCount -= Time.deltaTime;
				float percent = Mathf.InverseLerp(0, coolDownTime_normalAttack, coolDownTimeCount);
				if(percent < 0.001F){
					percent = 0.001F;
				}

				//update the cutout
				foreach( AttachBarHandler barHandler in barHanders ){
					barHandler.UpdateWithPercent(percent);
				}

				if(coolDownTimeCount <= 0F){
					coolDownTimeCount = coolDownTime_normalAttack;
					isReadyToAttack = true;
					Debug.Log("coolDown time End");
				}
			}
		}
	}

	void UpdateBuffTime(){
		if(buff_duration > 0)
		{
			buff_duration -= Time.deltaTime;

			if(buff_duration <= 0)
			{
				buff_duration = 0;

				switch(curBuffType)
				{
				case BuffType.Power:
					power_buff = 0;	
					break;
				case BuffType.Weight:
					weight_buff = 0;
					break;
				case BuffType.HealthPoint:
					healthPoint_buff = 0;	
					break;
				}

			}
		}
	}

	public void enableRigidKinematic(){ setRigidKinematic(true); }
	public void disableRigidKinematic(){ setRigidKinematic(false); }
	public void enableColliTrigger(){ setisTrigger(true); }
	public void disableColliTrigger(){ setisTrigger(false); }

	public void moveStart(){
		enableRigidKinematic();
//		Debug.Log("("+gameObject.name+") moveStart" );
		if(type_init == UnitType.AI || type_init == UnitType.PlayerSupport)
		{
			onMove_AI = true;
		}
	}

	public void moveEnd(){
//		disableRigidKinematic();
//		Debug.Log("("+gameObject.name+") moveEnd" );
		GameObject target = gameObject.GetComponent<AngleProvidor>().GetClosestUnit();
		if(target != null)
		{
			UnitProperty coll_Unit = target.GetComponent<UnitProperty>();
			if(coll_Unit != null){
				CheckAttack(coll_Unit);	
			}	
		}else{
			Debug.Log("get no target in range");
		}

		if(type_init == UnitType.AI || type_init == UnitType.PlayerSupport)
		{
			onMove_AI = false;
		}
	}

	void removeiTweenComponent(){
		if(GetComponent<iTween>() != null)
		{
			Destroy(GetComponent<iTween>());
			Debug.Log("remove iTween Component");
		}
	}

	void setRigidKinematic(bool enable)
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if(rb != null)
		{
			rb.isKinematic = enable;
		}else{
			Debug.Log("setRigidKinematic :: fount no rigidbody");
		}
	}
	void setisTrigger(bool enable){
		if(GetComponent<MeshCollider>() != null)
		{
			GetComponent<MeshCollider>().isTrigger = enable;
		}
	}

	//setter
	public void addHealthPoint(float n_in)
	{
		healthPoint += n_in;

		if(healthPoint < 0)
		{
			healthPoint = 0;
			state = UnitState.Disable;
		}

		Debug.Log("("+gameObject.name+") health point left : " + healthPoint );
	}
	public void setPowerBuffWithDuration(float n_value, float n_duration){
		power_buff = n_value;
		buff_duration = n_duration;
	}

	//getter
	public UnitState getCurrentState(){ return state; }
	public bool getIsReadyToAttack(){ return isReadyToAttack; }
	public float getCurrentWeight(){ return weight + weight_buff; }
	public float getCurrentPower(){ return power + power_buff; }
	public float getChargeBonus(){ return chargebonus + chargebonus_buff; }
	public float getSpeedUpDuration(){ return speedUpDuration + speedUpDuration_buff; }
	public float getKnockPower()
	{
		float knockPower_out = 0;
		if(GetComponent<KnockBackHelper>() != null)
		{
			knockPower_out = GetComponent<KnockBackHelper>().GetCurrentMoveSpeed() * getChargeBonus();	
		}
		return knockPower_out;
	}
		
	public float getMaxMoveSpeed()
	{
		float speed_out = Mathf.Round(getCurrentPower()/ getCurrentWeight());
		return speed_out;
	}

	float getKnockDistance(float targetWeight){
		if(targetWeight <= 0 )
			targetWeight = 1f;
		
		float distance_out = getKnockPower() / targetWeight;

		return distance_out;
	}

	//monoBavior 
	void OnMouseDown(){
		Debug.Log ( "OnMouseDown Unit selected");
		if(state != UnitState.Disable)
		{
			delegate_board.touchedUnit( gameObject , this);	
		}
	}

	void OnTriggerEnter(Collider collision)
	{
		if(collision.gameObject.tag == "Unit" 
			|| collision.gameObject.tag == "UnitPointer" 
			|| collision.gameObject.tag == "PlayerUnit")
		{
			removeiTweenComponent();

			UnitProperty coll_Unit = collision.GetComponent<UnitProperty>();
			if(coll_Unit != null){
				CheckAttack(coll_Unit);	
			}
		}else if(collision.gameObject.tag == "wall")
		{
//			removeiTweenComponent();
//			moveEnd();
		}
	}
		

	//for opporane
	void CheckAttack( UnitProperty coll_Unit )
	{
		if(coll_Unit.gameObject.tag == "Unit"){
			//hit the units are face to face 
			enableRigidKinematic();
//			Debug.Log(" get object " + coll_Unit.gameObject.name +  "is  trigger entered ");
			if(coll_Unit != null)
			{
				float distance = GetComponent<AngleProvidor>().GetCurrentDistance();
				//				Debug.Log(" check attack distance - how far between target ? " + distance + " / " + (attack_radius/2) );
				if(getIsReadyToAttack() && distance < attack_radius/2)
				{
					//set my healthPoint
					doNormalAttack( coll_Unit.GetComponent<KnockBackHelper>() , coll_Unit);
				}

				if(coll_Unit.getIsReadyToAttack()){
//					Debug.Log("wiil receive unit("+ coll_Unit.gameObject.name+ ") attack");
				}
			}

		}else if(coll_Unit.gameObject.tag == "Item"){ 
			//hit the unit not face to this Unit
			disableRigidKinematic();

		}else{

		}
	}

	void doNormalAttack( KnockBackHelper target_knockBackHelper , UnitProperty coll_Unit){
		//set explosion
		if(target_knockBackHelper != null){
			power = getCurrentPower();
			Debug.Log("("+ gameObject.name +") hit Unit ("+coll_Unit.gameObject.name+") knockBack with power " + power);

			//knock back
			target_knockBackHelper.doExplodionKnockBackWithTween(gameObject, power);

			//lost health point
			coll_Unit.addHealthPoint( -getCurrentPower());

			//set Attack
			isReadyToAttack = false;
		}else{
			Debug.Log("knockback not found");
		}
			
	}
}
