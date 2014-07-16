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
    public static GameObject GetParent(this GameObject child, string name = null)
    {
        if (child.transform.parent == null) return null;
        if (name == null || child.transform.parent.gameObject.name == name)return child.transform.parent.gameObject;
        return child.transform.parent.gameObject.GetParent(name);
    }
    public static bool HasParent(this GameObject child, string name)
    {
        return (child.GetParent(name) != null);
    }
    public static void FillAndBorder(this GameObject obj, Color fillColor)
    {
        try
        {
            SpriteRenderer fill = obj.transform.FindChild("Fill").GetComponent<SpriteRenderer>();
            SpriteRenderer border = obj.transform.FindChild("Border").GetComponent<SpriteRenderer>();

            fill.color = fillColor;
            border.color = fillColor.Invert()*.8f;
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Tried to call Fill and Border on a NonCompliant Item.");
            throw e;
        }
    }
    
    public static Color Invert(this Color color){
        return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
    }
	public static bool isWithinGrid(this Vector2 worldPos){
		
		if (worldPos.x > RoomManager.gridWidth * Wall.blockSize || worldPos.x < 0) return false;
		if (worldPos.y > RoomManager.gridHeight * Wall.blockSize || worldPos.y  < 0)return false;
		return true;
	}
	public static Vector2 WorldToWallPos(Vector2 worldPos, out Side s, out Wall.Orientation orientation)
	{


		int blockSize = Wall.blockSize;
		int originX = ((int)Mathf.Floor(worldPos.x / blockSize)) * blockSize;
		int originY = ((int)Mathf.Floor(worldPos.y / blockSize)) * blockSize;

		if (!new Vector2(originX, originY).isWithinGrid()) throw new IndexOutOfRangeException("Don't Use a try Catch, check using isWithinGrid");



		float x = worldPos.x - originX;
		float y = worldPos.y - originY;

		Vector3 vect = Vector3.zero;
		if (x > y)
		{
			if (x < Wall.blockSize - y)
			{
				s = Side.bottom;
				vect.x += Wall.halfBlock;
				orientation = Wall.Orientation.Horizontal;
			}
			else
			{
				s = Side.right;
				vect.x += Wall.blockSize;
				vect.y += Wall.halfBlock;
				orientation = Wall.Orientation.Vertical;
			}
		}
		else
		{
			if (x < Wall.blockSize - y)
			{
				s = Side.left;
				vect.y += Wall.halfBlock;
				orientation = Wall.Orientation.Vertical;
			}
			else
			{
				s = Side.top;
				vect.x += Wall.halfBlock;
				vect.y += Wall.blockSize;
				orientation = Wall.Orientation.Horizontal;
			}
		}

		vect.x += originX;
		vect.y += originY;
		return vect;
	}
}
	
	public class WTFException : Exception{
		public const string WTF= "What the Fjord?!?!";
		public WTFException():base(WTF){
		}
		public WTFException(String s):base(s){
			Debug.Log(WTF);
		}
		public WTFException(Exception e):base(WTF, e){
		}
		public WTFException(String s, Exception e):base(s, e){
			Debug.Log(WTF);
		}
	}
	
	