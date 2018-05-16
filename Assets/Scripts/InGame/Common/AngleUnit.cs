using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleUnit
{
	public GameObject myObject;
	Transform myTransfrom;
	public enum TendDirection { undefine , up , down, left , right , upleft, upright , downleft , downright};

	TendDirection directionRefact = TendDirection.undefine;
	TendDirection directionTended = TendDirection.undefine;
	float angleInTan = 0F;
	float angle_base = 0F;
	float angle_refact = 0F;
	float distance = 0F;


	public void setCurrentDirect(TendDirection n_direct){ directionTended = n_direct; }
	public void setCurrentAngle(float n_angle360){ 
		
		if(angle_base != n_angle360)
		{
			angle_base = n_angle360; 
			//auto define direction
			directionTended = angleToDirection(angle_base);
		}
	}

	TendDirection angleToDirection(float in_angle360)
	{
		float angle_form = in_angle360 % 360;
		TendDirection out_dir = TendDirection.undefine;

		if(angle_form > 360-1 && angle_form < 1)//is up
		{
			out_dir = TendDirection.up;
		}
		if(angle_form > 90-1 && angle_form < 90+1)//is left
		{
			out_dir = TendDirection.left;
		}
		if(angle_form > 90-1 && angle_form < 90+1)//is right
		{
			out_dir = TendDirection.right;
		}
		if(angle_form > 270-1 && angle_form < 270+1)//is left
		{
			out_dir = TendDirection.left;
		}

		if(angle_form >= 1 && angle_form <= 90-1)
		{
			out_dir = TendDirection.upright;
		}

		if(angle_form >= 270+1 && angle_form <= 360-1)
		{
			out_dir = TendDirection.upleft;
		}

		if(angle_form >= 180+1 && angle_form <= 270-1)
		{
			out_dir = TendDirection.downleft;
		}

		if(angle_form >= 90+1 && angle_form <= 180-1)
		{
			out_dir = TendDirection.downright;
		}
		
		return out_dir;
	}

	public void setTarget( Vector3 p_target )
	{
		DoCalculate(p_target);
	}

	public float getCurrentDistance(){ return distance; }
	public TendDirection getCurrentDirection(){ return directionTended;}
	public TendDirection getCurrentRefactDirection(){ return directionRefact;}
	public float getCurrentAngle(){ return angleInTan;}
	public float getCurrentAngleBase(){ return angle_base;}
	public float getTranslatedCompleteAngle(AngleUnit.TendDirection inDirection, float inAngle)
	{
		float angle =  0;
		{
			if(inDirection == AngleUnit.TendDirection.downleft ||
				inDirection == AngleUnit.TendDirection.upleft ||
				inDirection == AngleUnit.TendDirection.left)
			{
				if(inDirection == AngleUnit.TendDirection.left)
					angle = 270;
				else{
					if(inDirection == AngleUnit.TendDirection.upleft)
					{
						angle = inAngle + 270;
					}else{
						angle = (inAngle - 270) * -1;		
					}
				}
			}else if(inDirection == AngleUnit.TendDirection.downright ||
				inDirection == AngleUnit.TendDirection.upright ||
				inDirection == AngleUnit.TendDirection.right)
			{
				if(inDirection == AngleUnit.TendDirection.right)
					angle = 90;
				else{
					if(inDirection == AngleUnit.TendDirection.upright)
					{
						angle = (inAngle - 90) * -1;
					}else{
						angle = inAngle + 90;		
					}
				}
			}else{
				if(inDirection == AngleUnit.TendDirection.up)
				{
					angle = 0;
				}else{
					angle = 180;		
				}
			}

			if(angle > 360)
				angle %= 360;
			//				Debug.Log("angle ? " + angle);
		}
		return angle;
	}

	public float GetAngleBaseMyPositionWithObject( Transform b_target , bool do_refact)
	{
		return GetAngleBaseMyPositionWithObject( b_target.position , do_refact);
	}

	public float GetAngleBaseMyPositionWithObject( Vector3 oB , bool do_refact)
	{
		setTarget(oB);
		return do_refact? angle_refact : angle_base;
	}

	void DoCalculate(Vector3 oB){
		if( myObject != null )
		{
			myTransfrom = myObject.transform;
			Vector3 oA = myTransfrom.position;
			SetOopAdj(oA, oB);
		}
	}

	public void SetOopAdj(Vector3 oA , Vector3 oB)
	{
		//reset
		angle_base = 0F;
		angleInTan = 0F;

		if(!oA.Equals(oB))
		{
			float opp = Mathf.Max(oA.z, oB.z) - Mathf.Min(oA.z,oB.z);
			float adj = Mathf.Max(oA.x, oB.x) - Mathf.Min(oA.x,oB.x);

			distance = Mathf.Sqrt( Mathf.Pow(opp , 2F) + Mathf.Pow(adj , 2F));

			if(opp > 0.1 && adj > 0.1)
			{
				angleInTan = (Mathf.Atan(opp/adj)* Mathf.Rad2Deg);	
			}
			if(oA.z != oB.z || oA.x != oB.x)
			{
				if(opp < 0.1 || adj < 0.1){
					if(adj < 0.1){
						if(oA.z < oB.z){
							angle_base = 0F ;
							angle_refact = 180F;
							directionTended = TendDirection.up ;
							directionRefact = TendDirection.down;
						}else if(oA.z > oB.z){
							angle_base = 180F ;
							angle_refact = 0F;
							directionTended = TendDirection.down ;
							directionRefact = TendDirection.up;
						}
					}else if(opp < 0.1){
						if(oA.x < oB.x){
							angle_base = 90F;
							angle_refact = 270F;
							directionTended =  TendDirection.right ;
							directionRefact = TendDirection.left;
						}else if(oA.x > oB.x){
							angle_base = 270F;
							angle_refact = 90F;
							directionTended = TendDirection.left ;
							directionRefact = TendDirection.right;
						}
					}
				}else if(oA.x < oB.x){ //target on right
					float _base = 90F;
					float _refact = 270F;
					if(oA.z < oB.z){ //target on top
						angle_base = _base - angleInTan;
						angle_refact = _refact - angleInTan;
						directionTended = TendDirection.upright ;
						directionRefact = TendDirection.downleft;
					}else if(oA.z > oB.z){ //target on down
						angle_base = _base + angleInTan;
						angle_refact = _refact + angleInTan;
						directionTended = TendDirection.downright ;
						directionRefact = TendDirection.upleft;
					}
				}else if(oA.x > oB.x){ // target on left
					float _base = 270F;
					float _refact = 90F;
					if(oA.z < oB.z){ //target on top
						angle_base = _base + angleInTan;
						angle_refact = _refact + angleInTan;
						directionTended = TendDirection.upleft ;
						directionRefact = TendDirection.downright;
					}else if(oA.z > oB.z){ //target on down
						angle_base = _base - angleInTan;
						angle_refact = _refact - angleInTan;
						directionTended = TendDirection.downleft ;
						directionRefact = TendDirection.upright;
					}
				}
			}
			//				Debug.Log("A("+oA.x+","+oA.z+")  B("+oB.x+","+oB.z+")");
//							Debug.Log(" target in "+ directionTended+" , refact"+ directionRefact );
		}
	}

	public Vector3 GetPosition( float n_distance , bool do_refact )
	{
		myTransfrom = myObject.transform;
		Vector3 n_position = myTransfrom.position;

		float new_x = 0;
		float new_z = 0;

		new_x = Mathf.Sin( (Mathf.PI/180) * angle_base) * n_distance;  //Mathf.Abs(Mathf.Cos(angleInTan)* n_distance);
		new_z = Mathf.Cos( (Mathf.PI/180) * angle_base) * n_distance; //Mathf.Abs(Mathf.Sin(angleInTan)* n_distance);

		if(do_refact){
			new_x *= -1;
			new_z *= -1;
		}

		switch( directionTended )
		{
		case AngleUnit.TendDirection.up:
			n_position.z += n_distance;
			break;
		case AngleUnit.TendDirection.right:
			n_position.x += n_distance;
			break;
		case AngleUnit.TendDirection.left:
			n_position.x -= n_distance;
			break;
		case AngleUnit.TendDirection.down:
			n_position.z -= n_distance;
			break;
		case AngleUnit.TendDirection.upright:
			n_position.x += new_x;
			n_position.z += new_z;
			break;
		case AngleUnit.TendDirection.upleft:
			n_position.x += new_x;
			n_position.z += new_z;
			break;
		case AngleUnit.TendDirection.downright:
			n_position.x += new_x;
			n_position.z += new_z;
			break;
		case AngleUnit.TendDirection.downleft:
			n_position.x += new_x;
			n_position.z += new_z;
			break;
		}

		//		Debug.DrawLine(myTransfrom.position, n_position);
//		Debug.Log(" angle("+angleInTan+") to ( " +new_x + "," + new_z+") \n is refact ? "+ do_refact + " direction " + ( do_refact ? directionRefact : directionTended ) + "\n pos "  + myTransfrom.position +" to " + n_position);

		return n_position;
	}



}
