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
    color1,
    color2,
    color3,
    color4,
    color5,
}
[ExecuteInEditMode]
public abstract class GamePiece : MonoBehaviour
{
    public Cell _cell ;
    public Cell cell { get { return _cell; } set { _cell = value; if (value!=null)transform.position = value.WorldPos(); } }
    public PieceType piecetype;
	private const int defaultWeight  = 1;
	Cell destination;
	public bool isMoving = false;
    public ColorSlot colorslot = ColorSlot.color1;
    private ColorSlot oldColorSlot = ColorSlot.color1;
    public Color colorPreview;
    private Color oldColorPreview;
        
	//public GamePiece container
    //{
    //    get 
    //    {
    //        if (cell == null)
    //            return null;
	//		GamePiece ret = cell.gamePiece;
    //        if (ret == this || ret == null) return null;
    //
    //        while (ret != null && ret.containedPiece != this)
    //        {
	//			ret = ret.containedPiece;
    //        } 
    //        return ret;
	//	}
    //}
    //--------
    //formerly called container
    public GamePiece previousPiece
    {
        get
        {
            if (cell == null) return null;
            if (!cell.pieces.Contains(this)) throw new WTFException("Piece was expected in cell pieces list.");
            if (cell.pieces.Count < 2) return null;
            int index = cell.pieces.IndexOf(this);
            if (index == 0) return null;
            return cell.pieces[index - 1];
        }
    }
    //formerly called containedPiece
    public GamePiece nextPiece
    {
        get
        {
            if (cell == null) return null;
            if (!cell.pieces.Contains(this)) throw new WTFException("Piece was expected in cell pieces list.");
            int listSize = cell.pieces.Count;
            if (listSize < 2) return null;
            int index = cell.pieces.IndexOf(this);
            if (index == listSize - 1) return null;
            return cell.pieces[index + 1];
        }
    }
    public abstract bool isSolid { get; set; }

	private int _weight = defaultWeight;
	public virtual int weight { get{return _weight;} set{_weight=value;}}

    public abstract bool isPushable { get; set; }

    //public GamePiece containedPiece { get ;set; }
    //public bool IsOccupied { get { return containedPiece != null; } }
    public bool IsOccupied { get { return nextPiece != null; } }

    public virtual void Awake() {
        
    }
    public virtual void Start() 
    {
        if (!Cell.GetFromWorldPos(transform.position).Occupy(this)) 
            throw new WTFException(this.GetType().ToString());
        UpdateColor();
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
        //prevent user from changing the color in inspector (it's only a preview)
        if (colorPreview != oldColorPreview)
        {
            colorPreview = oldColorPreview;
        }
        //change the actual color based on the colorslot enum inspector change
        if (colorslot != oldColorSlot)
        {
            UpdateColor();
        }
    }
    private void UpdateColor()
    {
        oldColorSlot = colorslot;
        colorPreview = Author.GetColorSlot(colorslot);
        oldColorPreview = colorPreview;
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
    //public virtual bool onOccupy(GamePiece piece)
    //{
    //    if (containedPiece == null)
    //    {//not intentional set
    //        containedPiece = piece;
    //        return true;
    //    } return containedPiece.onOccupy(piece);
    //}
    public virtual bool onOccupy(GamePiece piece)
    {
        return true;
    }

    public virtual void onDeOccupy(GamePiece piece) 
    {
    }
    //public GamePiece GetNeighbour(Side s){
    //	Cell neighbour = cell.getNeighbour(s); 
    //	if(neighbour == null) return null;
    //    return neighbour.gamePiece;
    //}
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
    //public void Detatch(bool bringChildren = true)
    //{
    //    if (bringChildren)
    //    {
    //        if (cell == null) return;
    //        if (container == null)
    //        {
    //            var gg = cell.gamePiece;
    //            while (gg != this)
    //            {
    //                if (gg == null) return;
    //                gg = gg.containedPiece;
    //            }
    //            var g = cell.Empty(); 
    //        }
    //        else
    //        {
    //            container.onDeOccupy();
    //        }
    //    }
    //    else
    //    {
    //        if (containedPiece == null)
    //        {
    //            this.Detatch(true);
    //            return;
    //        }
    //        else if (cell == null) 
    //        {
    //            return;
    //        }
    //        else if (container == null)
    //        {
    //            cell.gamePiece = containedPiece;
    //        }
    //        else
    //        {
    //            container.containedPiece = containedPiece;
    //            if (containedPiece == null) container.onDeOccupy();
    //            this.containedPiece = null;
    //        }
    //    }
    //}
    //----------------
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
    //----------------
    //detatches with all children, calls onDeOccupy for all pieces underneath, and returns full list of detatched pieces
    //public List<GamePiece> DetatchWithChildren()
    //{
    //    if (cell == null) return null;
    //    //if you don't have children, call normal detatch
    //    if (nextPiece == null)
    //    {
    //        Detatch();
    //        return new List<GamePiece>(){this};
    //    }
    //    //pieces to be detatched and returned
    //    List<GamePiece> detatched = new List<GamePiece>();
    //    //pieces list as it was at the beginning
    //    List<GamePiece> currentPieces = cell.pieces.ToList();
    //    //index of piece being originally detatched
    //    int thisPieceIndex = currentPieces.IndexOf(this);
    //    for (int i = 0; i < currentPieces.Count; i++)
    //    {
    //        GamePiece current = currentPieces[i];
    //        //if the piece will not be detatched
    //        if (i < thisPieceIndex)
    //        {
    //            //call onDeOccupy on this piece with ALL pieces will be detatched
    //            for (int j = thisPieceIndex; j < currentPieces.Count; j++)
    //            {
    //                current.onDeOccupy(currentPieces[j]);
    //            }
    //        }
    //        //otherwise, if the piece is being detatched
    //        else
    //        {
    //            //remove and add the return list
    //            current.cell.pieces.Remove(current);
    //            current.cell = null;
    //            detatched.Add(current);
    //        }
    //    }
    //    return detatched;
    //}
    //----------------
    public List<GamePiece> DetatchWithChilren()
    {
        if (cell == null) return null;
        var detatchList = new List<GamePiece>();
        var currentPieces = cell.pieces.ToList();
        int thisPieceIndex = currentPieces.IndexOf(this);
        for (int i = 0; i < currentPieces.Count; i++)
        {
            if (i >= thisPieceIndex)
            {
                GamePiece piece = currentPieces[i];
                piece.Detatch();
                detatchList.Add(piece);
            }
        }
        return detatchList;

    }
    //public void Destroy(bool destroyChildren = true)
    //{
    //    Detatch(destroyChildren);
    //    GamePiece g = this;
    //    while (g!=null){
    //        if (this.gameObject)DestroyImmediate(this.gameObject);
    //        g = g.containedPiece; 
    //    }
    //    
    //}
    //----------------
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

