using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour {

    public Transform Tracker;

	// Use this for initialization
	void Start () {


		
	}
	
	// Update is called once per frame
	void Update () {

        

        if (MainGameManager.instance.thisInstanceIsServer)
        {
            transform.position = Tracker.position;
            transform.rotation = Tracker.rotation;
        }
		
	}
}
