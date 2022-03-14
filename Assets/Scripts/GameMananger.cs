using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMananger : MonoBehaviour {

    public static GameMananger instance;

    public GameObject objectToFocusOn;

    public bool isInCalibrationMode;



    // Use this for initialization
    void Awake () {

        instance = this;
		
	}
	
    public void SetCalibrationMode(bool enabled)
    {
        GameMananger.instance.isInCalibrationMode = enabled;
    }
}
