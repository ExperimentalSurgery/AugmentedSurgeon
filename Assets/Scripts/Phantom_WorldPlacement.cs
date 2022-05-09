using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.SpectatorView;
using Microsoft.MixedReality.QR;
public class Phantom_WorldPlacement : MonoBehaviour {


    AudioSource confirmationSoundSource;
    public Microsoft.MixedReality.SpectatorView.QRCodeMarkerDetector markerDetector;
    public Microsoft.MixedReality.SpectatorView.QRCodesManager qrCodesManager;
    public XboxTranslation controls;
    public GameObject CalibrationHint;
    public GameObject PatternHint;
    public string qrCodeMarkerText = "sv99";
    Guid qrSpatialNodeGraphID;

    bool trackingActive = false;
    bool subscribedToQRUpdates = false;

    // Use this for initialization
    void Start () {

        confirmationSoundSource = GetComponent<AudioSource>();

        StartTracking();
    }
	
	// Update is called once per frame
	void Update () {

        if (trackingActive)
        {
            if(qrCodesManager == null)
            {
                qrCodesManager = FindObjectOfType<QRCodesManager>();
                return;
            }

            else
            {
            }

            if (!qrCodesManager.IsWatcherRunning)
            {
                return;
            }

            if (!subscribedToQRUpdates)
            {
                qrCodesManager.QRCodeUpdated += QRCodesUpdated;
                subscribedToQRUpdates = true;
            }

            if(qrSpatialNodeGraphID != null)
            {
                Matrix4x4 qrLocation;

                if (qrCodesManager.TryGetLocationForQRCode(qrSpatialNodeGraphID, out qrLocation))
                {
                    Debug.Log("[Phantom QR Code] Successfully got location");

                    transform.position = new Vector3(qrLocation[0, 3], qrLocation[1, 3], qrLocation[2, 3]);
                    transform.rotation = qrLocation.rotation;
                }

                else
                {
                    Debug.Log("[Phantom QR Code] Not able to get location");

                }
            }
            
        }
	}


    public void StartTracking()
    {
        trackingActive = true;
        confirmationSoundSource.Play();
        markerDetector.StartDetecting();
        CalibrationHint.SetActive(true);
        PatternHint.SetActive(true);

        GameMananger.instance.isInCalibrationMode = true;

        controls.occlusionMesh.GetComponent<Renderer>().material = controls.standardMAT;

    }

    public void LockTracking()
    {
        trackingActive = false;
        CalibrationHint.SetActive(false);
        PatternHint.SetActive(false);
        markerDetector.StopDetecting();
        confirmationSoundSource.Play();

        GameMananger.instance.isInCalibrationMode = false;

        controls.occlusionMesh.GetComponent<Renderer>().material = controls.occlusionMAT;

        if (subscribedToQRUpdates)
        {
            qrCodesManager.QRCodeUpdated -= QRCodesUpdated;
            subscribedToQRUpdates = false;
        }
    }

    private void QRCodesUpdated(object sender, QRCode args)
    {
        //Debug.Log("[Phantom QRCode] Got an update for QR code with data: " + args.Data);

        if(args.Data == qrCodeMarkerText)
        {
            qrSpatialNodeGraphID = args.SpatialGraphNodeId;
        }
    }





}
