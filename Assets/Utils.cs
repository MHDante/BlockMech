using System;
using UnityEngine;
public static class Utils
{
public static Side opposite(this Side s){
	switch(s){
	case Side.bottom:
		return Side.top;
	case Side.top:
		return Side.bottom;
	case Side.left:
		return Side.right;
	case Side.right:
		return Side.left;
	}throw new WTFException();
}
}

public class WTFException : Exception{
	public WTFException():base("What the Fuck?!?!"){
	}
	public WTFException(String s):base(s){
		Debug.Log("What the Fuck?!?!");
	}
	public WTFException(Exception e):base("What the Fuck?!?!", e){
	}
	public WTFException(String s, Exception e):base(s, e){
		Debug.Log("What the Fuck?!?!");
	}
}

