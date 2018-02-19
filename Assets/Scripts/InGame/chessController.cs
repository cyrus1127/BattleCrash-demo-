using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class chessController : MonoBehaviour{
	public enum chessType{
		Attack,
		Defence,
		Support
	};
	public Vector2 Pos;
	public bool doMove;
	public float thrust;
	Rigidbody rb;
	Collider cl;
	GameObject pointer;

	// Use this for initialization
	void Start () {
		doMove = false;
		rb = GetComponent<Rigidbody>();
		cl = (Collider)GetComponent<CapsuleCollider>();
	}

	// Update is called once per frame
	void Update () {


	}

	public void setData(Hashtable properties)
	{
		if(properties != null)
		{
			if(properties.Contains(""))
			{
				if(properties[""].GetType() == typeof(chessType))
				{

				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log(string.Concat("is hit ? " , other.name));
	}
		

}
