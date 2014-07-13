using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Wall : MonoBehaviour {

    public const int blockSize = 4;
	public const int halfBlock = blockSize/2;
    //[ExposePropertyAttribute]
	public virtual bool isTraversible { get; set; }
    private bool _active = true;
    public bool active { get { return _active; } set { gameObject.SetActive(value); isTraversible = !value; _active = value; } }

    public enum Orientation { Horizontal, Vertical };
    public Orientation orientation = Orientation.Vertical;

	// Use this for initialization
    protected virtual void Start()
    {
        if (orientation== Orientation.Vertical && transform.rotation.Equals(Quaternion.identity)) 
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
}
