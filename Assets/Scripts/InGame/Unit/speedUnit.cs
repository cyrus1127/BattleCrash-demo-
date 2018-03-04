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
	float distance = 0;

	public void setTarget( float in_speed , float in_distance , float speedUpDuration)
	{
		speed_base = 0.1F;
		speed_max = in_speed;
		distance = in_distance;
		//		speed_delta = in_speed/(speedUpDuration * 60);
		speedUp_duration = speedUpDuration * 60;
	}

	public float getCurSpeed()
	{
		return speed_base;
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
					float 
					speed_delta = (speed_max/speedUp_duration) * (time_past * 60);
					speed_base += speed_delta;
					//					Debug.Log("UpdateSpeed ? " + speed_base  + " + " + speed_delta + " , time ? " + (time_past * 60) + " , cur dur ? " + speedUp_duration_past);
					if(speed_base > speed_max)
						speed_base = speed_max;
				}
			}
		}else{
			//do speed losing 

		}
		return speed_base;
	}

	public float getTimeScaleBySpeed()
	{
		float scaledTime = 100F;
		if(distance > 0 && speed_base > 0)
		{
			scaledTime = (distance / speed_base);
		}else{
			Debug.Log("not set distance / MaxSpeed");
		}

		return scaledTime;
	}
}
