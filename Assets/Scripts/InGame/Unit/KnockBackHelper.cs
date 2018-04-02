using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class controller_keyboard
{
	protected Hashtable keyMap;
	protected List<string> hashtable_keys;

	protected void addKeyMap(string keyName, string buttonName)
	{
		if(hashtable_keys.IndexOf(keyName) >= 0)
		{
			keyMap.Add(hashtable_keys[hashtable_keys.IndexOf(keyName)],buttonName);	
		}
	}

	protected void initKeys()
	{
		if(hashtable_keys == null)
		{
			hashtable_keys = new List<string>();
			hashtable_keys.Add("up");
			hashtable_keys.Add("down");
			hashtable_keys.Add("left");
			hashtable_keys.Add("right");
			hashtable_keys.Add("fire");
		}
	}

	public controller_keyboard()
	{
//		initKeys();
		if(keyMap == null)
		{
			keyMap = new Hashtable();

			//init map
			keyMap.Add("w","up");
			keyMap.Add("s","down");
			keyMap.Add("a","left");
			keyMap.Add("d","right");
			keyMap.Add("f","fire");
		}
	}
		
	public string getMappedKeyPosition(string keyName)
	{
		foreach(object key in keyMap.Keys)
		{
			string key_str = (string)key;
			if(key_str == keyName)
				return (string)keyMap[key];
		}

		Debug.Log("inputed key not found");
		return string.Empty;
	}
}

class controller_keyboard_player2 : controller_keyboard
{
	public controller_keyboard_player2()
	{
		initKeys();
		if(keyMap == null)
		{
			keyMap = new Hashtable();

			//init map
			keyMap.Add("up","up");
			keyMap.Add("down","down");
			keyMap.Add("left","left");
			keyMap.Add("right","right");
			keyMap.Add("/","fire");
		}
	}
}

public class MoveUpdateHelper 
{
	static string input_key_fire = "space";

	List<Vector3> path; // reference to the path
	Rigidbody targetRigid;
	SpeedUnit myMove;
	bool isPressedKeyDetected = false;
	float targetMaxCharge = 10F;
	float chargedPower = 0;
	float releasePower = 0;

	public void Set_ChargablePower(float in_power)
	{
		targetMaxCharge = in_power;
	}

	public void Set_Rigid(Rigidbody rb)
	{
		if(targetRigid != rb)
			targetRigid = rb;

		if(myMove == null)
			myMove = new SpeedUnit();
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

		}else if(Input.GetKeyUp(input_key_fire)){
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
			chargedPower += targetMaxCharge * 0.1f;
			if(chargedPower > targetMaxCharge)
			{
				chargedPower = targetMaxCharge;
				Debug.Log("Power charged done");
			}else{
				Debug.Log("Power is charging");
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
			float total_distance = (releasePower * 60F * 0.8F);
			myMove.setTarget(releasePower,total_distance,2);	
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
				float moveSpeed = myMove.UpdateSpeed(Time.deltaTime); //get speed

				if(moveSpeed > 0){
					float timeScale = myMove.getTimeScaleBySpeed();

					Debug.Log("onMoveUpdate moveSpeed? " + moveSpeed);
					targetRigid.MovePosition(targetRigid.transform.position + targetRigid.transform.forward * (Time.deltaTime * timeScale));			
				}

				if(moveSpeed <= 0)
				{
					releasePower = 0;
				}
			}
		}
	}

	public bool isMoveEnd()
	{
		bool isEnd = (releasePower == 0);

		isEnd = (path.Count == 0);

		return isEnd;
	}
}

public class rotateUpdateHelper
{
	static string input_key_left = "left";
	static string input_key_right = "right";

	public Transform transf;
	public float rotateSpeed = 1f;

	bool isPressedKeyDetected = false;
	bool isRightKeyPressed = false;


	public void Update()
	{
		handleKeyboardButton();
		doRotate();
	}

	void handleKeyboardButton(){
		if(Input.GetKeyDown(input_key_right) || Input.GetKeyDown(input_key_left))
		{
			if(!isPressedKeyDetected)
			{
				isPressedKeyDetected = true;
				isRightKeyPressed= Input.GetKeyDown(input_key_right);
				Debug.Log("" + (isRightKeyPressed?"right":"Left") + " key was pressed");
			}
		}else{
			//check release
			if(isPressedKeyDetected)
			{
				if(isRightKeyPressed && Input.GetKeyUp(input_key_right)){
					isPressedKeyDetected = false;
					Debug.Log("right key was released");
				}else if(Input.GetKeyUp(input_key_left)){
					isPressedKeyDetected = false;
					Debug.Log("left key was released");
				}
			}
		}	
	}

	void doRotate()
	{
		if(isPressedKeyDetected)
		{
			float angle = 0;
			angle =(isRightKeyPressed)? rotateSpeed: -rotateSpeed;
			transf.Rotate(Vector3.up,angle);
		}
	}
}

public class KnockBackHelper : MonoBehaviour {

	public float radius_explode = 5.0F;
	public bool doExplode = false;
	public float speedUpDuration = 0.2F;

	int playerindex = 0;
	controller_keyboard curMappedController;

	float power = 0F;
	float upWard = 0F;

	Vector3 startPos;
	iTween cur_tween;
	SpeedUnit myMove;

	MoveUpdateHelper myMoveUpdate;
	rotateUpdateHelper myRotateUpdate;
	Rigidbody rb;

	//turn handling parameter
	bool onRotate;

	// Use this for initialization
	void Start () {
		doExplode = false;
		startPos = transform.position;
//		myMove = new SpeedUnit();

		curMappedController = new controller_keyboard();
		rb = GetComponent<Rigidbody>();

		if(myMoveUpdate == null && rb != null)
		{
			myMoveUpdate = new MoveUpdateHelper();
			myMoveUpdate.Set_Rigid(rb);
			myMoveUpdate.Set_ChargablePower( gameObject.GetComponent<UnitProperty>().power_init );
			myRotateUpdate = new rotateUpdateHelper();
			myRotateUpdate.transf = transform;
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
			doExplodionKnockBackWithTween( GameObject.FindGameObjectWithTag("Item") , 300F );
			doExplode = false;
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
