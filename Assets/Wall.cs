using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Wall : MonoBehaviour {

    public const int blockSize = 4;
	public const int halfBlock = blockSize/2;
    [ExposePropertyAttribute]
	public virtual bool isTraversible {get;set;}    

    public enum Orientation { Horizontal, Vertical };
    public Orientation orientation = Orientation.Horizontal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
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
