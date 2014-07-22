using UnityEngine;
using System.Collections;

public class SpaceTravel : MonoBehaviour {
    public int offset;
    public float speed;
	// Use this for initialization
	void Start () {
	
	}
    
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x +speed, transform.position.y, transform.position.z);
        if (transform.position.x >= 152) transform.position = new Vector3(-88, transform.position.y, transform.position.z);
	}
}
