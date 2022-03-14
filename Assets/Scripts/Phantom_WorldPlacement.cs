using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom_WorldPlacement : MonoBehaviour {

    Transform QRCode;
    AudioSource confirmationSoundSource; 

    public GameObject CalibrationHint;
    public GameObject PatternHint;

    bool trackingActive = false;

    // Use this for initialization
    void Start () {

        confirmationSoundSource = GetComponent<AudioSource>();

        LockTracking();
    }
	
	// Update is called once per frame
	void Update () {

        if (trackingActive)
        {
            if(QRCode == null)
            {
                GameObject[] qrcodes = GameObject.FindGameObjectsWithTag("QR");

                if (qrcodes.Length > 0)
                {
                    QRCode = qrcodes[0].transform;
                }
            }

            else
            {
                transform.position = QRCode.position;
                transform.rotation = QRCode.rotation;
            }

            
        }
	}


    public void StartTracking()
    {
        trackingActive = true;
        QRCode = null;
        confirmationSoundSource.Play();

        CalibrationHint.SetActive(true);
        PatternHint.SetActive(true);

        GameMananger.instance.isInCalibrationMode = true;
    }

    public void LockTracking()
    {
        trackingActive = false;

        CalibrationHint.SetActive(false);
        PatternHint.SetActive(false);

        confirmationSoundSource.Play();

        GameMananger.instance.isInCalibrationMode = false;
    }





}
