using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToPlayer : MonoBehaviour {

    Vector3 lookToCameraDir;
    public bool floatsInFrontOfPlayer;
    Camera cam;

    // Use this for initialization
    void Start () {
		
        cam = FindObjectOfType(typeof(Camera)) as Camera;
	}
	
	// Update is called once per frame
	void Update () {

        lookToCameraDir = Camera.main.transform.position - this.transform.position;

        transform.rotation = Quaternion.LookRotation(lookToCameraDir, Vector3.up);

        if (floatsInFrontOfPlayer)
        {
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 1f);
        }
	}
}
