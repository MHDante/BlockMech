using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
public enum PieceType
{
    none,
    wall,
    door,
    block,
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
    public GameObject ColorizedSprite, ActivatedSprite, WhiteSprite;
    public float moveSpeed = 5f, teleportSpeed = 10f;
    private float tempSpeed = 5f;
    public float currentLerp = 0f, maxLerp = 100f;
    public Vector2 StartPos;
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

    public virtual void Awake() 
    {
        
    }
    public virtual void Start() 
    {
        FindRenderers();
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
                Cell temp = destination;
                destination = null;

                temp.Occupy(this);
            }
            else
            {
                transform.position = Vector2.Lerp(StartPos, destination.WorldPos(), currentLerp / 100f);
                currentLerp += tempSpeed;
            }
        }
        if (cell != null)
            transform.position = new Vector3(transform.position.x, transform.position.y, -getZPosition());
        }
    void OnValidate()
    {
        SetColorSlot(colorslot);
    }

    public void FindRenderers()
    {
        Transform temp = gameObject.transform.FindChild("Colorized");
        if (temp != null)
        {
            ColorizedSprite = temp.gameObject;
            ColorizedSprite.FillAndBorder(colorPreview);
        }
        temp = gameObject.transform.FindChild("Activated");
        if (temp != null)
        {
            ActivatedSprite = temp.gameObject;
        }
        temp = gameObject.transform.FindChild("White");
        if (temp != null)
        {
            WhiteSprite = temp.gameObject;
        } 
    }
    public void SetColorSlot(ColorSlot colorSlot)
    {
        this.colorslot = colorSlot;
        colorPreview = Author.GetColorSlot(colorSlot);
        var renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        if (ColorizedSprite != null)
        {
            ColorizedSprite.FillAndBorder(colorPreview);
        }
        if (ActivatedSprite != null && this is Triggerable)
        {
            Triggerable t = (Triggerable)this;
            ActivatedSprite.FillAndBorder(t.IsTriggered ? colorPreview : Color.white);
        }
        if (WhiteSprite != null)
        {
            WhiteSprite.FillAndBorder(Color.white);
        }
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            gameObject.GetComponent<SpriteRenderer>().color = colorPreview;
        }
    }
    [Flags]public enum Axis
    {
        Xaxis = 1,
        Yaxis = 2,
        Zaxis = 4,
    }
    public static Vector3 getAngleVector(float angle, Axis axis)
    {
        switch (axis)
        {
            case Axis.Xaxis: return new Vector3(angle, 0, 0);
            case Axis.Yaxis: return new Vector3(0, angle, 0);
            case Axis.Xaxis | Axis.Yaxis: return new Vector3(angle, angle, 0);
            case Axis.Zaxis: return new Vector3(0, 0, angle);
            case Axis.Xaxis | Axis.Zaxis: return new Vector3(angle, 0, angle);
            case Axis.Yaxis | Axis.Zaxis: return new Vector3(0, angle, angle);
            case Axis.Xaxis | Axis.Yaxis | Axis.Zaxis: return new Vector3(angle, angle, angle);
            default: return new Vector3(0, 0, 0);
        }
    }
    public virtual bool pushFrom(Side side, int strength = 1)
    {
        GamePiece nextpiece = this.nextPiece;
        if (!isSolid && nextpiece != null && nextpiece.pushFrom(side, strength-1))
			return true;
		if(!isPushable) 
			return false;
		Wall w = cell.getWall(Utils.opposite(side));
		if(w!=null && !w.isTraversible) 
			return false;
		if (strength < weight) 
			return false;
        Cell neighbour = cell.getNeighbour(Utils.opposite(side));
		GamePiece obstructor = neighbour.firstSolid();

        if (obstructor == null)
        {
            Side newside = Utils.opposite(side);
            return moveTo(newside);
        }
		if(obstructor.isSolid && !isPushable) 
			return false;
		if(!obstructor.isSolid && !obstructor.isPushable) 
			return moveTo(Utils.opposite(side));
		bool obsPushed = obstructor.pushFrom(side, strength - weight);
		if (obsPushed){
			obstructor.Detatch();
			bool succeed = moveTo(Utils.opposite(side));
			if (!succeed)
            {obstructor.moveTo(side);}
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
        //bool available = dest.Reserve();//Intentional Set.
		//if (available)
        //{
        //    isMoving = true;
        //    StartPos = cell.WorldPos();
        //    destination = dest;
        //}
        //return available;
        return StartLerp(cell, dest, moveSpeed);
	}
    public bool StartLerp(Cell source, Cell dest, float speed)
    {
        bool available = dest.Reserve() && dest != null;
        if (available)
        {
            isMoving = true;
            StartPos = source.WorldPos();
            destination = dest;
            tempSpeed = speed;
            currentLerp = 0f;
        }
        return available;
    }
    public bool JustTeleported = false;
	public virtual bool TeleportTo(Cell target)
    {
		Cell currentCell = cell;
		if (target.IsSolidlyOccupied())return false;
		Detatch();
        //f (!target.Occupy(this)) {
        //	currentCell.Occupy(this);
        //	return false;
        // //return true;
        return StartLerp(currentCell, target, teleportSpeed);
	}
    public void Detatch()
    {
        if (cell == null) return;
        //call onDeOccupy only for pieces Under you (possibly revisable design decision)
        List<GamePiece> piecesCopy = cell.pieces.ToList();
        cell.pieces.Remove(this);
        cell = null;
        foreach (GamePiece piece in piecesCopy)
        {
            if (piece == this) break;
            piece.onDeOccupy(this);
        }
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
    public virtual void OnDestroy(){
        if (cell != null) Detatch();
    }
}

public interface Triggerable
{
    bool IsTriggered { get; }
}

