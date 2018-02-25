using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProperty : MonoBehaviour {

	public float healthPoint_Init;
	public float weight_init;
	public float power_init;
	public float attack_radius = 5F;
	public float friction = 0.5F;
	public float coolDownTime_normalAttack = 0.2F;
	public UnitState state_init;
	public UnitType type_init;
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
	float buff_duration;
	BuffType curBuffType;
	UnitState state;
	GameBoard delegate_board;

	bool onMove_AI;
	public bool debug_onMove_AI = false;

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
		barHanders = gameObject.GetComponentsInChildren<AttachBarHandler>();

		isReadyToAttack = true;
		coolDownTimeCount = coolDownTime_normalAttack;

		healthPoint = healthPoint_Init;
		power = power_init;
		weight = weight_init;

		power_buff = 0;
		weight_buff = 0;
		healthPoint_buff = 0;
		state = state_init;
		onMove_AI = false;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateAttackCDTimeBar();
		UpdateBuffTime();

		//AI
		UpdateAIAction();
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
					if(AIWarninghealthPercentage > 0 && healthPoint <= (healthPoint_Init * AIWarninghealthPercentage)/100 )
					{
//						if(Random.Range(0,10) < 8)
//						{
//							isTimeToAttack = true;
//						}else{
//							isTimeToAttack = false;
//						}
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
			comp_kb.doMoveToPositionWithUpdate(n_point);
			Debug.Log("Do move to " + n_point);
		}
	}

	/// <summary>
	/// Players the action move to.
	/// </summary>
	/// <param name="n_point">N point.</param>

	public void PlayerActionMoveTo( Vector3 n_point )
	{
		KnockBackHelper comp_kb = GetComponent<KnockBackHelper>();
		if(comp_kb != null){
			comp_kb.doMoveToPositionWithUpdate(n_point);
			Debug.Log("Do move to " + n_point);
		}

		//tend to target position
		AngleProvidor comp_tend = GetComponent<AngleProvidor>();
		comp_tend.LookAtWithTargetPosition(n_point,false);
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
		Debug.Log("("+gameObject.name+") moveStart" );
		if(type_init == UnitType.AI || type_init == UnitType.PlayerSupport)
		{
			onMove_AI = true;
		}
	}

	public void moveEnd(){
		disableRigidKinematic();
		Debug.Log("("+gameObject.name+") moveEnd" );
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
	public float getCurrentWeight(){ return weight_init + weight_buff; }
	public float getCurrentPower(){ return power_init + power_buff; }
	public float getChargeBonus(){ return 3F; }
	public float getKnockPower(){
		float knockPower_out = getCurrentMoveSpeed() * getChargeBonus();
		return knockPower_out;
	}
		
	float getMaxMoveSpeed()
	{
		float speed_out = Mathf.Round(getCurrentPower()/ getCurrentWeight());
		return speed_out;
	}

	float getCurrentMoveSpeed()
	{
		return 0F;
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
		}
	}
		

	//for opporane
	void CheckAttack( UnitProperty coll_Unit )
	{
		if(coll_Unit.gameObject.tag == "Unit"){
			//hit the units are face to face 
			enableRigidKinematic();
			Debug.Log(" get object " + coll_Unit.gameObject.name +  "is  trigger entered ");
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
					Debug.Log("wiil receive unit("+ coll_Unit.gameObject.name+ ") attack");
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
