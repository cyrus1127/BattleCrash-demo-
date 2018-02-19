using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Chess{

	public Module_Chess(string data)
	{
		if(data.Length > 0)
		{
//			if(data.IndexOf("\n") > 0 )
			{
				string[] dataLines = data.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
				if(dataLines.Length > 0 )
				{
					Debug.Log(string.Concat("total data line readed ? ", dataLines.Length));
					//get keys
					string[] fields = dataLines[0].Split(","[0]);

					//by pass first data line, as its data fields
					//make json string to JsonUtility
					for(int data_idx = 1 ; data_idx < dataLines.Length ; data_idx++)
					{
						string[] values = dataLines[data_idx].Split(","[0]);

						if(values.Length == fields.Length)
						{
							string[] jsonParas = new string[fields.Length];
							for(int val_idx = 0 ; val_idx < fields.Length ; val_idx++)
							{
								jsonParas[val_idx] = string.Format("{0}:{1}",fields[val_idx],values[val_idx]);
							}
						}


					}	
				}
			}
		}
	}

}
