using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class controller_keyboard
{
	protected Hashtable keyMap;
	protected List<string> hashtable_keys;

	protected void addKeyMap(string keyName, string buttonName)
	{
		if(hashtable_keys.IndexOf(keyName) >= 0)
		{
			keyMap.Add(hashtable_keys[hashtable_keys.IndexOf(keyName)],buttonName);	
		}
	}

	protected void initKeys()
	{
		if(hashtable_keys == null)
		{
			hashtable_keys = new List<string>();
			hashtable_keys.Add("up");
			hashtable_keys.Add("down");
			hashtable_keys.Add("left");
			hashtable_keys.Add("right");
			hashtable_keys.Add("fire");
		}
	}

	public controller_keyboard()
	{
		//		initKeys();
		if(keyMap == null)
		{
			keyMap = new Hashtable();

			//init map
			keyMap.Add("w","up");
			keyMap.Add("s","down");
			keyMap.Add("a","left");
			keyMap.Add("d","right");
			keyMap.Add("f","fire");
		}
	}

	public string getMappedKeyPosition(string keyName)
	{
		foreach(object key in keyMap.Keys)
		{
			string key_str = (string)key;
			if(key_str == keyName)
				return (string)keyMap[key];
		}

		Debug.Log("inputed key not found");
		return string.Empty;
	}
}

class controller_keyboard_player2 : controller_keyboard
{
	public controller_keyboard_player2()
	{
		initKeys();
		if(keyMap == null)
		{
			keyMap = new Hashtable();

			//init map
			keyMap.Add("up","up");
			keyMap.Add("down","down");
			keyMap.Add("left","left");
			keyMap.Add("right","right");
			keyMap.Add("/","fire");
		}
	}
}


