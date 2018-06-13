using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class powerBarController : MonoBehaviour {

//	public float testBarSize = 50F;
	float myWidth = 800F;
	public RectTransform contentBarTransf;

	void Update()
	{
//		UpdateBarProcess(testBarSize);
	}

	void UpdateBarProcess(float precetage)
	{
		if(contentBarTransf != null)
		{
			float final_ptg = precetage;
			if(final_ptg > 100F)
				final_ptg = 100F;
			else if(final_ptg < 0)
				final_ptg = 0;

			final_ptg = (100F - final_ptg);

			float finalWidth = (myWidth * final_ptg) / 100f;
			//set contentBar Transf
			contentBarTransf.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right,0, finalWidth / 2);
			contentBarTransf.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,0, myWidth - finalWidth);

			//set color
			Image img = contentBarTransf.gameObject.GetComponent<Image>();
			if(precetage >= 50){
				img.color = Color.yellow;	
			}else if(precetage >= 80){
				img.color = Color.red;
				Debug.Log("is red now");
			}else 
				img.color = Color.white;				
		}
	}
}
