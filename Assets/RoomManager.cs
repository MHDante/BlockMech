using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {

    const int maxWidth = 16;
    const int maxHeight = 12;

    public Cell[][] Grid;


    void Awake() {

        Grid = new Cell[maxWidth][];
        for (int i = 0; i < maxWidth; i++)
        {
            Grid[i] = new Cell[maxHeight];
            for(int j = 0; j < maxHeight; j++){
                Grid[i][j] = new Cell();
            }
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Wall w = wall.GetComponent<Wall>();
            int PosY = (int)wall.transform.position.y / Wall.blockSize;
            int PosX = (int)wall.transform.position.x / Wall.blockSize;
            if (w.orientation == Wall.Orientation.Vertical) {
                if ((PosX >= 0 && PosX < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
                {
                    Grid[PosX][PosY].walls[Side.left] = w;
                }
                if ((PosX - 1 >= 0 && PosX - 1 < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
                {
                    Grid[PosX - 1][PosY].walls[Side.right] = w;
                }

            }
            else if (w.orientation == Wall.Orientation.Horizontal){
                if ((PosX >= 0 && PosX < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
                {
                    Grid[PosX][PosY].walls[Side.bottom] = w;
                }
                if ((PosX >= 0 && PosX < Grid.Length) && (PosY-1 >= 0 && PosY-1 < Grid[0].Length))
                {
                    Grid[PosX][PosY - 1].walls[Side.top] = w;
                }

            }
        }

    }

	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
