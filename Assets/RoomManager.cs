using UnityEngine;
using System.Collections;
using System.Linq;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;

    const int maxWidth = 16;
    const int maxHeight = 12;

    public Cell[][] Grid;


    void Awake() {
        roomManager = this;
        Grid = new Cell[maxWidth][];
        for (int i = 0; i < maxWidth; i++)
        {
            Grid[i] = new Cell[maxHeight];
            for(int j = 0; j < maxHeight; j++){
                Grid[i][j] = new Cell(this, i, j);
            }
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            AddWall(wall);
        }
    }
    public void AddPiece(GameObject piece, PieceType piecetype)
    {
        GamePiece gamePiece = piece.GetComponent<Player>();
        if (gamePiece == null)
        {
            gamePiece = piece.AddComponent<Player>();
            gamePiece.piecetype = piecetype;
        }
        int PosX = (int)piece.transform.position.x / Wall.blockSize;
        int PosY = (int)piece.transform.position.y / Wall.blockSize;
        Cell cell = Grid[PosX][PosY];
        if (cell != null)
        {
            var list = cell.getPiecesOnCell();
            if (list.Select(gpiece => gpiece.piecetype).Contains(piecetype))
            {
                return;
            }
            bool success = cell.Occupy(gamePiece);
            //do something if the cell was successfully placed.
        }

    }
    public void AddWall(GameObject wall)
    {
        Wall w = wall.GetComponent<Wall>();
        int PosX = (int)wall.transform.position.x / Wall.blockSize;
        int PosY = (int)wall.transform.position.y / Wall.blockSize;
        if (w.orientation == Wall.Orientation.Vertical)
        {
            if ((PosX >= 0 && PosX < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
            {
                Grid[PosX][PosY].setWall(Side.left, w);
            }
            if ((PosX - 1 >= 0 && PosX - 1 < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
            {
                Grid[PosX - 1][PosY].setWall(Side.right, w);
            }
        }
        else if (w.orientation == Wall.Orientation.Horizontal)
        {
            if ((PosX >= 0 && PosX < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
            {
                Grid[PosX][PosY].setWall(Side.bottom, w);
            }
            if ((PosX >= 0 && PosX < Grid.Length) && (PosY - 1 >= 0 && PosY - 1 < Grid[0].Length))
            {
                Grid[PosX][PosY - 1].setWall(Side.top, w);
            }
        }
    }
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
        if(!Application.isPlaying && roomManager == null)
        { roomManager = this; Awake(); }
	}
    public void RemovePiece(Vector2 target, bool destroyChildren = false)//, PieceType piece)
    {
        int PosY = (int)target.y / Wall.blockSize;
        int PosX = (int)target.x / Wall.blockSize;
        Cell c = Grid[PosX][PosY];
        if (c != null)
        {
            var gamepieces = c.getPiecesOnCell();
            if (c.gamePiece == null || gamepieces.Count == 0) return;
            GamePiece g = gamepieces.Last();

            g.Destroy(destroyChildren);
        }
    }
    public void RemoveWall(Vector2 target, Wall.Orientation orientation)
    {
        int PosY = (int)target.y / Wall.blockSize;
        int PosX = (int)target.x / Wall.blockSize;

        if (orientation == Wall.Orientation.Vertical)
        {
            if ((PosX >= 0 && PosX < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
            {
                RemoveWall(PosX, PosY, Side.left);
            }
            if ((PosX - 1 >= 0 && PosX - 1 < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
            {
                RemoveWall(PosX - 1, PosY, Side.right);
            }
        }
        else if (orientation == Wall.Orientation.Horizontal)
        {
            if ((PosX >= 0 && PosX < Grid.Length) && (PosY >= 0 && PosY < Grid[0].Length))
            {
                RemoveWall(PosX, PosY, Side.bottom);
            }
            if ((PosX >= 0 && PosX < Grid.Length) && (PosY - 1 >= 0 && PosY - 1 < Grid[0].Length))
            {
                RemoveWall(PosX, PosY-1, Side.top);
            }
        }
    }
    private void RemoveWall(int x, int y, Side side)
    {
        if (Grid[x][y].getWall(side) != null)
        {
            DestroyImmediate(Grid[x][y].getWall(side).gameObject);
        }
        Grid[x][y].setWall(side, null);
    }
}
