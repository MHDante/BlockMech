using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    int distance = 4;
    float timer;
    float moveDelayTime = 0.2f;

	// Use this for initialization
	void Start () {
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        if (start != null) {
            transform.position = start.transform.position;
        }
        else
        {
            Debug.Log("Player requries a start prefab to exist.");
        }
        timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float x, y;
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        float deltaTick = Time.time - timer;

        if (deltaTick > moveDelayTime)
        {
            timer += deltaTick;
        }

        if (x != 0 || y != 0)
        {
            transform.position = new Vector3(
                transform.position.x + x * distance, 
                transform.position.y + y * distance, 
                transform.position.z
             );
        }
    }
}
