using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachBarHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateWithPercent(float value){
		//Mathf.InverseLerp(0, Screen.width, Input.mousePosition.x)
		GetComponent<Renderer>().material.SetFloat("_Cutoff", value); 
	}
}
