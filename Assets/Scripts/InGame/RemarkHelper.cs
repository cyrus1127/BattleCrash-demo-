using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemarkHelper : MonoBehaviour {

	float lifeTime = 10F;

	void Update()
	{
		lifeTime -= Time.deltaTime;
		if(lifeTime <= 0)
		{
			//do self destory
			GameObject.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider collision){
		DestorySelfWith(collision.gameObject.tag);
	}

	void DestorySelfWith(string tag)
	{
		//		Debug.Log("remark OnTriggerEnter colli with tag ? " + collision.gameObject.tag);
		if(tag == "PlayerUnit")
		{
			GameObject.Destroy(gameObject);
		}
	}
}
