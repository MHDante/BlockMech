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
        if (orientation == Orientation.Vertical && transform.rotation.Equals(Quaternion.identity)) 
            orientation = Orientation.Vertical;
        else orientation = Orientation.Horizontal;
	}
	
	// Update is called once per frame
    protected virtual void Update()
    {
	    if (orientation == Orientation.Horizontal){
            transform.position = new Vector3(
                ((int)Mathf.Round((transform.position.x - halfBlock) / blockSize)) * blockSize + halfBlock,
                ((int)Mathf.Round(transform.position.y / blockSize)) * blockSize,
                transform.position.z );
            transform.eulerAngles = new Vector3(0, 0, 90);
        }

        else if (orientation == Orientation.Vertical)
        {
            transform.position = new Vector3(
                ((int)Mathf.Round(transform.position.x / blockSize)) * blockSize,
                ((int)Mathf.Round((transform.position.y - halfBlock) / blockSize)) * blockSize + halfBlock,
                transform.position.z);
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
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void Close()
    {
        isTraversible = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
