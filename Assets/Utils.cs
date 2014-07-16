<<<<<<< HEAD
using System;
using UnityEngine;
using System.Collections.Generic;
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

    //convert dictionary<primitive, List<primitive>> to 2 arrays for serialization to be accessible in Unity inspector
    public class DictionaryToArraysMagic 
    {
        public List<string> zNames, zLevels;
        public List<int> zIndexes;
        public List<string> getNames { get { return zNames; } }
        public List<string> getLevels { get { return zLevels; } }
        public List<int> getIndexes { get { return zIndexes; } }
        public DictionaryToArraysMagic(Dictionary<string, List<string>> masterScenes)
        {
            zNames = new List<string>();
            zLevels = new List<string>();
            zIndexes = new List<int>();

            int count = 0;
            foreach (string name in masterScenes.Keys)
            {
                List<string> levelsAtName = masterScenes[name];
                if (levelsAtName.Count == 0) continue;

                zNames.Add(name);
                zIndexes.Add(levelsAtName.Count);
                foreach (string level in levelsAtName)
                {
                    zLevels.Add(level);
                    count++;
                }
            }
        }
        
        //private string[] names;
        //private string[][] scenes;
        //public DictionaryToArraysMagic(Dictionary<string, List<string>> masterScenes) 
        //{
        //    //simply initilize this array
        //    names = new string[masterScenes.Count];
        //
        //    
        //    int countMax = 0;
        //    int i = 0;
        //    foreach (KeyValuePair<string, List<string>> sceneSet in masterScenes) 
        //    {
        //        //determine other array max size
        //        if (sceneSet.Value.Count > countMax) countMax = sceneSet.Value.Count; 
        //
        //        //populate first array
        //        names [i++] = sceneSet.Key;
        //    }
        //
        //    //allocate (init?) other array
        //    scenes = new string[masterScenes.Count][];
        //    for (int j = 0; j < scenes.Length; j++) 
        //    {
        //        scenes[j] = new string[countMax];
        //    }
        //
        //    //populate second array
        //    int x = 0;
        //    foreach (KeyValuePair<string, List<string>> sceneSet in masterScenes)
        //    {
        //        int y = 0;
        //        foreach (string scene in sceneSet.Value) 
        //        {
        //            scenes[x][y] = scene;
        //            y++;
        //        }
        //        x++;
        //    }
        //}
        //public string[] get1d() { return names; }
        //public string[][] get2d() { return scenes; }
    }

    //convert dictionary<primitive, List<primitive>> to 2 arrays for serialization to be accessible in Unity inspector
    public class ArraysToDictionaryMagic
    {
        private Dictionary<string, List<string>> masterScenes;
        public Dictionary<string, List<string>> getScenes { get { return masterScenes; } }
        public ArraysToDictionaryMagic(ReadSceneNames readSceneNames)
        {
            masterScenes = new Dictionary<string, List<string>>();
            int count = 0;
            for (int i = 0; i < readSceneNames.zNames.Count; i++)
            {
                string name = readSceneNames.zNames[i];
                List<string> levels = new List<string>();
                int levelCount = readSceneNames.zIndexes[i];
                for (int j = 0; j < levelCount; j++)
                {
                    levels.Add(readSceneNames.zLevels[count++]);
                }
                masterScenes[name] = levels;
            }
        }


        //private Dictionary<string, List<string>> masterScenes;
        //public Dictionary<string, List<string>> get() { return masterScenes; }
        //public ArraysToDictionaryMagic(string[] names, string[][] scenes)
        //{
        //
        //    masterScenes = new Dictionary<string, List<string>>();
        //    for (int x = 0; x < scenes.Length; x++)
        //    {
        //        List<string> temp = new List<string>();
        //        for (int y = 0; y < scenes[scenes.Length].Length; y++)
        //        {
        //            temp.Add(scenes[x][y]);
        //        }
        //        masterScenes.Add(names[x], temp);
        //    }
        //}
    }
=======
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
	
	
>>>>>>> origin/master
