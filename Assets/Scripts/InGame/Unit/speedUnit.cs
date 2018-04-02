using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * To calculate the time with speed
 * handle speed up over time pass and time scaling
 */

public class SpeedUnit{
	float speed_base = 0.1F;
	float speed_max = 0;
	float speed_delta = 0;
	float speedUp_duration = 0;
	float speedUp_duration_past = 0;
	float speedDown_delta = 0.1F;
	float distance = 0;
	float movedDistance = 0;

	public void setTarget( float in_speed , float in_distance , float speedUpDuration)
	{
		//reset
		movedDistance = 0;
		speed_base = 0.1F;
		speedDown_delta = 0.1F;

		//init
		speed_max = in_speed;
		distance = in_distance;
		speedUp_duration = speedUpDuration * 60;
//		speed_delta = in_speed/(speedUpDuration * 60);

		Debug.Log("set distance ( "+ distance +" )");
	}

	public float getCurSpeed()
	{
		return speed_base;
	}

	public float UpdateSpeed(float time_past)
	{
		movedDistance += speed_base;
		float path_run_percent = movedDistance / distance;
		return UpdateSpeed(time_past , path_run_percent);
	}

	public float UpdateSpeed( float time_past , float path_run_percent)
	{
		if( path_run_percent < 0.8 )
		{
			//do speed handle
			if(speedUp_duration_past < speedUp_duration )
			{
				speedUp_duration_past += (time_past * 60);
				if(speed_base < speed_max)
				{
					float speed_delta = (speed_max/speedUp_duration) * (time_past * 60);
					speed_base += speed_delta;

//					Debug.Log("UpdateSpeed ? " + speed_base  + " + " + speed_delta + " , time ? " + (time_past * 60) + " , cur dur ? " + speedUp_duration_past);

					if(speed_base > speed_max)
						speed_base = speed_max;
				}
			}else{
//				Debug.Log("not UpdateSpeed ? speed_base("+ speed_base +")");
			}

		}else{
			//do speed losing 
			if(speed_base > 0)
			{
				speed_base -= speedDown_delta;
				speedDown_delta = speedDown_delta * 1.03F;
			}

			if(speed_base < 0)
				speed_base = 0;

//			Debug.Log("speed_base losing ("+speedDown_delta+") -> ("+ speed_base +")");
		}
		return speed_base;
	}

	public float getTimeScaleBySpeed()
	{
		float scaledTime = 100F;
		if(distance > 0)
		{
			if(speed_base > 0){
				scaledTime = (speed_base/speed_max) * scaledTime;
			}else{
				scaledTime = 0;
			}
		}else{

		}

//		Debug.Log("scaledTime(" + scaledTime + ") =  distance("+distance+") / MaxSpeed("+speed_base+")");
		return scaledTime;
	}
}
