using UnityEngine;
using System.Collections;

public enum Orientation { Horizontal, Vertical };
public enum WallType { Wall, Door, Diode, Turnstile}

[ExecuteInEditMode]
 public class Wall : MonoBehaviour {

    public const int blockSize = 4;
	public const int halfBlock = blockSize/2;
    public ColorSlot colorslot;
    public Color colorPreview;
    public Orientation orientation = Orientation.Vertical;
    public bool isDoor { get { return colorslot != ColorSlot.None; } }

	public virtual bool isTraversible { get; set; }
    public bool isOpen { get { return isTraversible; } }

	// Use this for initialization
    protected virtual void Start()
    {
        if (transform.rotation.Equals(Quaternion.identity))
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }

        //if (orientation == Orientation.Vertical && transform.rotation.Equals(Quaternion.identity)) 
        //    orientation = Orientation.Vertical;
        //else orientation = Orientation.Horizontal;
	}
	
	// Update is called once per frame
    protected virtual void Update()
    {
	    if (orientation == Orientation.Horizontal){
            transform.position = new Vector3(
                ((int)Mathf.Round((transform.position.x - halfBlock) / blockSize)) * blockSize + halfBlock,
                ((int)Mathf.Round(transform.position.y / blockSize)) * blockSize,
                transform.position.z );

            if ((int)(transform.rotation.eulerAngles.z+90)%180 != 0)
                transform.eulerAngles = new Vector3(0, 0, 90);
        }

        else if (orientation == Orientation.Vertical)
        {
            transform.position = new Vector3(
                ((int)Mathf.Round(transform.position.x / blockSize)) * blockSize,
                ((int)Mathf.Round((transform.position.y - halfBlock) / blockSize)) * blockSize + halfBlock,
                transform.position.z);
            if ((int)transform.rotation.eulerAngles.z % 180 != 0)
                transform.eulerAngles = new Vector3(0, 0, 0);
        }
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
    public void Open()
    {
        isTraversible = true;
        GetComponent<Animator>().SetBool("Open", isTraversible);
        //gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void Close()
    {
        isTraversible = false;
        GetComponent<Animator>().SetBool("Open", isTraversible);
        //gameObject.GetComponent<SpriteRenderer>().enabled = true;
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
