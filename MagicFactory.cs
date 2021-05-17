using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFactory : MonoBehaviour {
	public static MagicType getMagicType()
	{
		MagicType type = null;
		int caseSelect = 1;
		switch(caseSelect)
		{
		case 1:
			type = new Healing ();
			break;
		case 2:
			type = new Possession ();
			break;
		case 3:
			type = new Spirits ();
			break;
		}
		return type;		
	}
}

public interface MagicType
{
	int changeType ();
}

class Healing : MagicType
{
	public Healing()
	{
		Debug.Log ("Healing");
	}
	public int changeType()
	{
		
		return 1;
	}
}

class Possession : MagicType
{
	public int changeType()
	{
		Debug.Log ("Possession");
		return 1;
	}
}

class Spirits : MagicType
{
	public int changeType()
	{
		Debug.Log ("Spirits");
		return 1;
	}
}


