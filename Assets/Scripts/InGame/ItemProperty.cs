using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperty : MonoBehaviour {

	public float value;
	public float value_duration;
	public ItemType type;
	public enum ItemType{
		heath,
		poison, //the value will be negative
		power,
	}
		
	// Use this for initialization
	void Start () {
		iTween.RotateBy(gameObject, iTween.Hash("y", 0.99F, "easeType", "linear", "loopType", "loop", "delay", 0F));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collision)
	{

		if(collision.gameObject.tag == "Unit" || collision.gameObject.tag == "PlayerUnit")
		{
			GameObject.Destroy(gameObject);

			UnitProperty coll_Unit = collision.gameObject.GetComponent<UnitProperty>();
			if(coll_Unit != null)
			{
				switch(type)
				{
				case ItemType.heath:
					coll_Unit.addHeathPoint(value);
					break;
				case ItemType.poison:
					coll_Unit.addHeathPoint(-value);
					break;
				case ItemType.power:
					coll_Unit.setPowerBuffWithDuration(value , value_duration);
					break;
				}
			}

			Debug.Log("Unit & PlayerUnit collied");
		}
	}


}
