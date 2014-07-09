using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Wall : MonoBehaviour {

    public const int blockSize = 4;
    

    public enum Orientation { Horizontal, Vertical };
    public Orientation orientation = Orientation.Horizontal; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (orientation == Orientation.Horizontal){
            transform.position = new Vector3(
                ((int)Mathf.Round((transform.position.x - blockSize / 2) / blockSize)) * blockSize + blockSize / 2,
                ((int)Mathf.Round(transform.position.y / blockSize)) * blockSize,
                transform.position.z );
        }

        else if (orientation == Orientation.Vertical)
        {
            transform.position = new Vector3(
                ((int)Mathf.Round(transform.position.x / blockSize)) * blockSize,
                ((int)Mathf.Round((transform.position.y - blockSize / 2) / blockSize)) * blockSize + blockSize / 2,
                transform.position.z);
        }
	}
}
