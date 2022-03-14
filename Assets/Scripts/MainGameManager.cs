using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MainGameManager : MonoBehaviour {

    public static MainGameManager instance;
   

    public bool thisInstanceIsServer;
    public bool thisInstanceIsClient;
    

    void Awake()
    {
        instance = this; 
    }

    // Use this for initialization
    void Start () {



    }
	
	// Update is called once per frame
	void Update () {

       

    }
}
