using UnityEngine;
using System.Collections;

public enum Orientation { Horizontal, Vertical };
public enum WallType { Wall, Door, Diode, Turnstile}

[ExecuteInEditMode]
 public class Wall : MonoBehaviour, Activatable {


    public ColorSlot colorslot;
    public Color colorPreview;
    public Orientation orientation = Orientation.Vertical;
    public WallType wallType = WallType.Door;
    public bool isDoor { get { return wallType == WallType.Door; } }
    public bool StartsOpen = false;

    private bool _IsTraversible = false;
    public virtual bool IsTraversible { get { return _IsTraversible; } set { _IsTraversible = value; if (wallType == WallType.Door)GetComponent<Animator>().SetBool("Open", value); } }

	// Use this for initialization
    protected virtual void Start()
    {
        if (transform.rotation.Equals(Quaternion.identity))
        {
            orientation = Orientation.Vertical;
            transform.position = new Vector3(
                ((int)Mathf.Round(transform.position.x / Values.blockSize)) * Values.blockSize,
                ((int)Mathf.Round((transform.position.y - Values.halfBlock) / Values.blockSize)) * Values.blockSize + Values.halfBlock,
                transform.position.z);

            if ((int)transform.rotation.eulerAngles.z % 180 != 0)
                transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            orientation = Orientation.Horizontal;
            transform.position = new Vector3(
                ((int)Mathf.Round((transform.position.x - Values.halfBlock) / Values.blockSize)) * Values.blockSize + Values.halfBlock,
                ((int)Mathf.Round(transform.position.y / Values.blockSize)) * Values.blockSize,
                transform.position.z);

            if ((int)(transform.rotation.eulerAngles.z + 90) % 180 != 0)
                transform.eulerAngles = new Vector3(0, 0, 90);
        }
	}
   
    protected virtual void OnValidate()
    {

        if (colorslot == ColorSlot.None /*&& wallType == WallType.Door*/) wallType = WallType.Wall;
        else wallType = WallType.Door;
        SetColorSlot(colorslot);
        //IsTraversible = wallType == WallType.Wall ? false : _IsTraversible;
        if (wallType == WallType.Wall)
        {
            StartsOpen = false;
        }
        IsTraversible = StartsOpen;

    }
    public void SetColorSlot(ColorSlot colorSlot)
    {
        this.colorslot = colorSlot;
        colorPreview = MetaData.GetColorSlot(colorSlot);
        gameObject.GetComponent<SpriteRenderer>().color = colorPreview;
    }
    public void Activate()
    {
        if (wallType == WallType.Door) //IsTraversible = !_IsTraversible;
        {
            IsTraversible = !StartsOpen;
        }
    }
    public void Deactivate()
    {
        if (wallType == WallType.Door)
        {
            IsTraversible = StartsOpen;
        }
    }
    internal void BendFrom(Side s)
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("bellied")) return;
        transform.eulerAngles =
            s == Side.right ? new Vector3(0, 0, 0) :
            s == Side.top ? new Vector3(0, 0, 90) :
            s == Side.left ? new Vector3(0, 0, 180) :
            s == Side.bottom ? new Vector3(0, 0, 270) :
            new Vector3(float.NaN, float.NaN, float.NaN);
        GetComponent<Animator>().SetTrigger("PushedThrough");
    }
    
 }
