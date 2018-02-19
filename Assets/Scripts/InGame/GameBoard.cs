using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

	public GameObject markPrefab;
	public InGameLogic main;
	public Camera worldCam;
	public ItemProperty itemPrefab;

	public enum UnitPatten{ normal , attack , defence };
	public UnitPatten enemyPatten;
	public UnitPatten playerPatten;
	public int enemyUnit_onStart = 1; 


	List<GameObject> enemyUnits;
	List<GameObject> playerUnits;
	GameObject enemyPattenGeneratePoint;
	const string enemyPattenGeneratePointName = "enemyGenPoint";
	const string playerPattenGeneratePointName = "playerGenPoint";

	private List<GameObject> marks;

	Vector3 ray_start;

	// Use this for initialization
	void Start () {
		if(marks == null){
			marks = new List<GameObject>();
		}

		enemyUnits = new List<GameObject>();
		playerUnits = new List<GameObject>();

		{//for test, search unit itself.
			GameObject[] unitsFound = GameObject.FindGameObjectsWithTag("Unit");
			Debug.Log(" Unit object found ? " + unitsFound.Length);
			if(unitsFound.Length > 0)
			{
				foreach( GameObject unit in unitsFound )
				{
					if(unit.GetComponent<UnitProperty>() != null)
					{
						switch(unit.GetComponent<UnitProperty>().type_init)
						{
							case UnitProperty.UnitType.AI:
								enemyUnits.Add(unit);
							break;
							case UnitProperty.UnitType.Player:
							case UnitProperty.UnitType.PlayerSupport:
								playerUnits.Add(unit);
							break;
						}
					}
				}
			}
		}


		//default set the first one, if List no empty
		if(enemyUnits != null && playerUnits != null){
			if(enemyUnits.Count == 0)	
				Debug.Log("enemy units not found");
			if(playerUnits.Count == 0)	
				Debug.Log("player units not found");
		}else{
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void touchedUnit(GameObject unit , UnitProperty component)
	{
		if(unit != null && component != null)
		{
			UnitProperty.UnitType in_type = component.type_init;
			if(in_type == UnitProperty.UnitType.PlayerSupport)
			{	

				GameObject curPlayerUnit;

				//do selected Player unit change
				if(playerUnits.Count > 1)
				{
					foreach(GameObject listunit in playerUnits.ToArray()){
						if(listunit.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.Player)
						{
							curPlayerUnit = listunit;
						}
					}
				}else{
					//do nothing as no more unit able to swtich
				}
			}
			else if(in_type == UnitProperty.UnitType.AI)
			{
				
			}
		}
	}

	void OnMouseDrag() {
		
	}

	void OnMouseDown(){
		if (Input.GetMouseButton (0)) {
			ray_start = Input.mousePosition;
		}
	}

	void OnMouseUp(){
		
		if(ray_start != Vector3.zero)
		{
			Ray inputRay = worldCam.ScreenPointToRay(ray_start);
			BoxCollider boxCol = GetComponent<BoxCollider>();
			float rayLenght = 30F;

			//			Debug.Log ( "OnMouseDown Position :  " +  ray_start );
			//			Debug.Log(" world Ray" + inputRay);
			//			Debug.DrawRay(inputRay.origin, inputRay.direction * rayLenght, Color.yellow);
			if(boxCol != null)
			{
				RaycastHit hit;
				if(boxCol.Raycast(inputRay, out hit, rayLenght))
				{
					createPointMark(hit.point);
					DoUnitMoveToMarkPoint(hit.point);
//					Debug.Log("ray hit !! " + hit.point);
				}else{
//					Debug.Log("ray not hit the game board!!");
				}
			}else{
//				Debug.Log("Game board missing collider");
			}

			ray_start = Vector3.zero;
		}else{
			
		}
//		Debug.Log("Game board OnMouseUp");
	}
		

	public void DoUnitMoveToMarkPoint(Vector3 n_point)
	{
		if(playerUnits.Count > 0)
		{
			foreach(GameObject unit in playerUnits.ToArray())	
			{
				if( unit.GetComponent<UnitProperty>().type_init == UnitProperty.UnitType.Player)
				{
					unit.GetComponent<UnitProperty>().PlayerActionMoveTo(n_point);
				}
			}
		}else{
			Debug.Log( "no player units on List" );
		}
	}

	public void createPointMark(Vector3 n_point){
		if(markPrefab)
		{
			GameObject newMark = Instantiate(markPrefab);
			n_point.y = 1F;
			newMark.transform.position = n_point;
//			marks.Add(newMark);
		}

		//Debug
		for(int i = 0; i < marks.Count; i++)
		{
//			Debug.Log("");
		}
	}

	public void removePointMark()
	{
//		if(marks.Count > 0)
//		{
//			GameObject targetMark = marks[0];
//			marks.RemoveAt(0);
//			GameObject.Destroy(targetMark);
//		}
	}
}
