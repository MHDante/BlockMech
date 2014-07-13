using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
public enum PieceType
{
    none,
    wall,
    door,
    player,
    end,
    button,
    switcH,
    key,
    keyhole,
    teleport,
    tile,
    trap,
    antitrap,
}
public enum ColorSlot
{
    none,
    A,
    B,
    C,
    D,
    E,
}
[ExecuteInEditMode]
public abstract class GamePiece : MonoBehaviour
{
    public static Dictionary<Type, int> spawnNumbers = new Dictionary<Type, int>();
    public Cell _cell ;
    public Cell cell { get { return _cell; } set { _cell = value; if (value!=null)transform.position = value.WorldPos(); } }
    public PieceType piecetype;
	private const int defaultWeight  = 1;
	Cell destination;
	public bool isMoving = false;
    public ColorSlot colorslot = ColorSlot.A;
    public Color colorPreview;
    public GamePiece previousPiece
    {
        get
        {
            if (cell == null) return null;
            if (!cell.pieces.Contains(this)) throw new WTFException("Piece was expected in cell pieces list.");
            int index = cell.pieces.IndexOf(this);
            if (index == 0) return null;
            return cell.pieces[index - 1];
        }
    }

    public int getZPosition()
    {
        return cell.pieces.IndexOf(this);
    }

    public GamePiece nextPiece
    {
        get
        {
            if (cell == null) return null;
            if (!cell.pieces.Contains(this)) throw new WTFException("Piece was expected in cell pieces list.");
            int index = cell.pieces.IndexOf(this);
            if (index == cell.pieces.Count - 1) return null;
            return cell.pieces[index + 1];
        }
    }
    public abstract bool isSolid { get; set; }

	private int _weight = defaultWeight;
	public virtual int weight { get{return _weight;} set{_weight=value;}}

    public abstract bool isPushable { get; set; }

    public bool IsOccupied { get { return nextPiece != null; } }
    public GamePiece()
    {
        //Debug.Log("CONSTRUCTOR");
    }

    public virtual void Awake() {

        
    }
    private int DebugCounter = 0;
    public virtual void Start() 
    {
        Cell.GetFromWorldPos(transform.position).QueuedOccupy((int)transform.position.z, this) ;
        SetColorSlot(colorslot);
    }

    public virtual void Update()
    {

        if (isMoving)
        {
            if (currentLerp >= maxLerp)
            {
                currentLerp = 0f;
                isMoving = false;
                Detatch();
                destination.Unreserve();
                destination.Occupy(this);
                destination = null;
            }
            else
            {
                transform.position = Vector2.Lerp(StartPos, destination.WorldPos(), currentLerp / 100f);
                currentLerp += speed;
            }
        }
        if (cell != null)
            transform.position = new Vector3(transform.position.x, transform.position.y, getZPosition());
        }
    void OnValidate()
        {
        SetColorSlot(colorslot);
        }
    public void SetColorSlot(ColorSlot colorSlot)
    {
        this.colorslot = colorSlot;
        colorPreview = Author.GetColorSlot(colorSlot);
        gameObject.GetComponent<SpriteRenderer>().color = colorPreview;
    }
    public virtual bool pushFrom(Side side, int strength = 1)
    {
        GamePiece nextpiece = this.nextPiece;
        if (!isSolid && nextpiece != null && nextpiece.pushFrom(side, strength))
			return true;
		if(!isPushable) 
			return false;
		Wall w = cell.getWall(Utils.opposite(side));
		if(w!=null && !w.isTraversible) 
			return false;
		if (strength < weight) 
			return false;
		GamePiece obstructor = cell.getNeighbour(side).firstSolid();

		if(obstructor==null)return moveTo(Utils.opposite(side));
		if(obstructor.isSolid && !isPushable) 
			return false;
		if(!obstructor.isSolid && !obstructor.isPushable) 
			return moveTo(Utils.opposite(side));
		bool obsPushed = obstructor.pushFrom(side, strength - weight);
		if (obsPushed){
			obstructor.Detatch();
			bool succeed = moveTo(Utils.opposite(side));
			if (!succeed){obstructor.moveTo(side);}
			return succeed;
		} 
		return false;
    }
    public virtual bool onOccupy(GamePiece piece)
    {
        return true;
    }

    public virtual bool canBeOccupiedBy(GamePiece piece)
    {
        return true;
    }
    public virtual void onDeOccupy(GamePiece piece) 
    {
    }
    public virtual bool moveTo(Side side) {
		if (isMoving) return false;
        if (cell == null)
            return false;
		Cell dest = cell.getNeighbour(side);
        Wall w = cell.getWall(side);
        if(w!=null && !w.isTraversible) return false;
        if (dest == null) return false;
        bool available = dest.Reserve();//Intentional Set.
		if (available)
        {
            isMoving = true;
            StartPos = cell.WorldPos();
            destination = dest;
        }
        return available;
	}
	public virtual bool TeleportTo(Cell target){
		Cell currentCell = cell;
		if (target.IsSolidlyOccupied())return false;
		Detatch();
		if (!target.Occupy(this)) {
			currentCell.Occupy(this);
			return false;
		} return true;

	}
    public void Detatch()
    {
        if (cell == null) return;
        //call onDeOccupy only for pieces Under you (possibly revisable design decision)
        foreach(GamePiece piece in cell.pieces)
        {
            if (piece == this) break;
            piece.onDeOccupy(this);
        }
        cell.pieces.Remove(this);
        cell = null;

    }
    //detatches with all children, calls onDeOccupy for all pieces underneath, and returns full list of detatched pieces
    public List<GamePiece> DetatchWithChilren()
    {
        if (cell == null) return null;
        var detatchList = new List<GamePiece>();
        var currentPieces = cell.pieces.ToList();
        int thisPieceIndex = currentPieces.IndexOf(this);
        for (int i = thisPieceIndex; i < currentPieces.Count; i++)
        {
            GamePiece piece = currentPieces[i];
            piece.Detatch();
            detatchList.Add(piece);
        }
        return detatchList;

    }
    public void Destroy()
    {
        Detatch();
        if (this.gameObject) DestroyImmediate(this.gameObject);
    }
    //----------------
    public void DestroyWithChildren()
    {
        List<GamePiece> detatched = DetatchWithChilren();
        foreach(var piece in detatched)
        {
            if (piece.gameObject) DestroyImmediate(piece.gameObject);
        }
    }
    public float speed = 5f;
    public float currentLerp = 0f, maxLerp = 100f;
    public Vector2 StartPos;
	
    public virtual void OnDestroy(){
        if (cell != null) Detatch();
    }
}

