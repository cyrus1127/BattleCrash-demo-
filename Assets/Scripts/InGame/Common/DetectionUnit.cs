using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Class is doing help A.I. and KnockBackHelper to detection the possible way.
 * Need make A.I. seems smart if A.I. can detect no way to go .
 */

public class DetectionUnit : MonoBehaviour {

	AngleUnit rightAngle = new AngleUnit();
	float detect_radius = 0;
	List<BoxCollider> walls;

	// Use this for initialization
	void Start () {

		//init 
//		upAngle = new AngleUnit();
//		upAngle.myObject = gameObject;
		rightAngle = new AngleUnit();
		rightAngle.myObject = gameObject;
		walls = new List<BoxCollider>();

		//get wall
		GameObject board = GameObject.FindGameObjectWithTag("board");
		if(board != null)
		{
			BoxCollider[] getwalls = board.GetComponentsInChildren<BoxCollider>();
			if(getwalls.Length > 0)
			{
				foreach( BoxCollider coll in getwalls )
				{
					if(coll.gameObject.tag == "wall")
						walls.Add(coll);	
				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		float cur_angle = transform.rotation.eulerAngles.y;
//		upAngle.setCurrentAngle(cur_angle);
		rightAngle.setCurrentAngle(cur_angle);

//		getHitGameObject(rightAngle);
	}

	void OnDrawGizmosSelected() {
//		DrawDebugline();
	}

	//-------------- 

	public void setDetectionRadius( float n_radius )
	{
		if(n_radius > 0)
		{
			detect_radius = n_radius;
		}
	}

	void DrawDebugline()
	{
		if(true){

			float cur_angle = transform.rotation.eulerAngles.y;
//			upAngle.setCurrentAngle(cur_angle);
			rightAngle.setCurrentAngle(cur_angle+90);

//			Debug.DrawLine(transform.position, upAngle.GetPosition(detect_radius,false), Color.blue);	
//			Debug.DrawLine(transform.position, upAngle.GetPosition(detect_radius,true), Color.blue);	

			Debug.DrawLine(transform.position, rightAngle.GetPosition(detect_radius,false), Color.blue);	
			Debug.DrawLine(transform.position, rightAngle.GetPosition(detect_radius,true), Color.blue);	

			for(int i = 0; i < 6 ; i++)
			{
				Vector3 endPoint = transform.position;
				if( i < 2)
					endPoint.x += i < 1 ? detect_radius : detect_radius*-1;
				else if (i < 4)
					endPoint.z += i < 3 ? detect_radius : detect_radius*-1;
				else
					endPoint.y += i < 5 ? detect_radius : detect_radius*-1;
				Debug.DrawLine(transform.position, endPoint, Color.red);	
			}
		}
	}

	// pass an AngleUnit 
	public bool haveWaysToGoOut(AngleUnit in_angleUnit)
	{
		if(walls.Count > 0 && detect_radius > 0 && in_angleUnit != null)
		{
//			Debug.DrawLine(transform.position, in_angleUnit.GetPosition(detect_radius,true), Color.blue);

			Vector3 directon;
			directon = transform.forward;
//			Ray upray = new Ray(transform.position,transform.forward);
			Ray leftray = new Ray(transform.position,-transform.right);
			Ray rightray = new Ray(transform.position,transform.right);
			Ray downray = new Ray(transform.position,-transform.forward);

			RaycastHit hit;
			if(isHitTheWall(downray , out hit))
			{
				RaycastHit hit_l;
				RaycastHit hit_r;
				bool isLeftNoWay = isHitTheWall(leftray,out hit_l);
				bool isRightNoWay = isHitTheWall(rightray,out hit_r);
				if(isLeftNoWay && isRightNoWay)
				{
					//do nothing
					return false;
				}else{

					if(!isLeftNoWay && !isRightNoWay)
					{
						//choose randon

						//give an randon case to make a better choose ?

						{
							//testing get left
							setAngleToLeft(in_angleUnit);
						}
							
					}else if(isLeftNoWay)
					{
						//go right 
						setAngleToRight(in_angleUnit);
					}else if(isRightNoWay)
					{
						//go Left
						setAngleToLeft(in_angleUnit);
					}

					return true;
				}
			}else{
				setAngleToBackward(in_angleUnit);
				return true;
			}

		}

		return false;
	}

	bool isHitTheWall( Ray in_ray , out RaycastHit hit )
	{
		hit = new RaycastHit();
		bool onhit = false;
		if(walls.Count > 0)
		{
			foreach( BoxCollider coll in walls.ToArray() ) 
			{
				if(!onhit)
				{
					onhit = coll.Raycast(in_ray, out hit,detect_radius);	
				}else{
					break;
				}
			}	
		}
			
		return onhit;
	}

	void setAngleToBackward(AngleUnit in_angleUnit)
	{
		float new_angle = in_angleUnit.getCurrentAngleBase() - 180 ;
		if(new_angle > 360)
			new_angle %= 360;
		if(new_angle < 0)
			new_angle += 360;
		in_angleUnit.setCurrentAngle( new_angle );
	}

	void setAngleToLeft(AngleUnit in_angleUnit)
	{
		float new_angle = in_angleUnit.getCurrentAngleBase() - 90 ;
		if(new_angle > 360)
			new_angle %= 360;
		if(new_angle < 0)
			new_angle += 360;
		in_angleUnit.setCurrentAngle( new_angle );
	}

	void setAngleToRight(AngleUnit in_angleUnit)
	{
		float new_angle = in_angleUnit.getCurrentAngleBase() + 90 ;
		if(new_angle > 360)
			new_angle %= 360;
		in_angleUnit.setCurrentAngle( new_angle );
	}
}
