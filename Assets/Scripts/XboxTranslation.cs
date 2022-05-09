using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.QR;

public class XboxTranslation : MonoBehaviour
{

    public delegate void FinishCalibration(string cmd);
    public static event FinishCalibration OnFinished;

    public Phantom_WorldPlacement phantomPlacement;
    public GameObject occlusionMesh;

    public float fastMovementSpeed;
    public float slowMovementSpeed;
    public float fastRotationSpeed;
    public float slowRotationSpeed;
    public float brightnessSpeed;

    public GameObject ToolTipIcon;
    public GameObject YHintIcon;
    public GameObject YHint;
    public GameObject floatingLiver;

    private Transform objectToMove;
    private Transform occlusionMeshTrans;

    public GameObject[] ListOfModels;

    public Material occlusionMAT;
    public Material standardMAT;

    bool editModeEnabled = false;
    bool rotationMode = false;
    bool occlusionMeshModeEnabled = false;
    int currentModelNo;

    Quaternion startRotation;

    List<Renderer> renderers = new List<Renderer>();
    List<float> startTransparencies = new List<float>();
    public float displayedTransparencyPercentage = 1f;


    // Use this for initialization
    void Start()
    {
        // Needed //////////////////////////////////////////////////

        startRotation = transform.localRotation;
        // First parameter is the number, starting at zero, of the controller you want to follow.
        // Second parameter is the default “dead” value; meaning all stick readings less than this value will be set to 0.0.
        ///////////////////////////////////////////////////////////

        ToolTipIcon.SetActive(false);

        objectToMove = this.transform;

        occlusionMeshTrans = transform.GetChild(0).transform;

        transform.GetChild(0).GetComponent<Renderer>().material = standardMAT;

        floatingLiver.SetActive(false);

        foreach (GameObject model in ListOfModels)
        {
            if (model.GetComponent<MeshRenderer>() != null)
            {
                renderers.Add(model.GetComponent<MeshRenderer>());
                startTransparencies.Add(renderers[renderers.Count - 1].material.color.a);
            }

            foreach (MeshRenderer childrenderer in model.transform.GetComponentsInChildren<MeshRenderer>())
            {
                renderers.Add(childrenderer);
                if (childrenderer.material.HasProperty("_Color"))
                    startTransparencies.Add(childrenderer.material.color.a);
                else
                    startTransparencies.Add(0);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                Color newTran = renderers[i].material.color;
                newTran.a = Mathf.Lerp(0, startTransparencies[i], displayedTransparencyPercentage);
                renderers[i].material.SetColor("_Color", newTran);
            }

        }

        SetBrightness();

        if (Input.GetButtonDown("BButton"))
        {
            if (GameMananger.instance.isInCalibrationMode)
                phantomPlacement.LockTracking();
            else
                phantomPlacement.StartTracking();
        }

        if (!GameMananger.instance.isInCalibrationMode)
        {
            ToogleEditMode();

            if (editModeEnabled)
            {
                SaveLoad();
                MoveRotateControl();
                ResetRotation();
            }

            if (Input.GetButtonDown("LButton"))
            {
                SwitchVisualisationDown();
            }

            if (Input.GetButtonDown("RButton"))
            {
                SwitchVisualisationUp();
            }

            if (Input.GetButtonDown("RightStickButton"))
            {
                bool floatingLiverActiveState = floatingLiver.activeInHierarchy;
                floatingLiver.SetActive(!floatingLiverActiveState);
            }
        }
    }

    public void SetBrightness()
    {

        displayedTransparencyPercentage += Input.GetAxis("StickRightY") * brightnessSpeed;

        if (displayedTransparencyPercentage > 1)
            displayedTransparencyPercentage = 1;

        if (displayedTransparencyPercentage < 0)
            displayedTransparencyPercentage = 0;
    }

    public void SwitchVisualisationUp()
    {
        if (!GameMananger.instance.isInCalibrationMode)
        {
            ListOfModels[currentModelNo].SetActive(false);

            currentModelNo++;
            if (currentModelNo == ListOfModels.Length)
            {
                currentModelNo = ListOfModels.Length - 1;
            }

            ListOfModels[currentModelNo].SetActive(true);
        }
    }

    public void SwitchVisualisationDown()
    {
        if (!GameMananger.instance.isInCalibrationMode)
        {
            ListOfModels[currentModelNo].SetActive(false);

            currentModelNo--;
            if (currentModelNo == -1)
            {
                currentModelNo = 0;
            }

            ListOfModels[currentModelNo].SetActive(true);
        }

    }

    public void SetEnvBrightness(float brightness)
    {
        UnityEngine.RenderSettings.ambientIntensity = brightness;
    }

    void MoveRotateControl()
    {
        if (editModeEnabled)
        {
            if (Input.GetButtonDown("AButton"))
            {
                rotationMode = !rotationMode;
                occlusionMeshModeEnabled = false;
            }

            if (!rotationMode && !occlusionMeshModeEnabled)
            {
                objectToMove = this.transform;

                if (Input.GetAxis("TriggerL") >= 0.8f)
                {
                    MoveZ();
                }

                if (Input.GetAxis("TriggerL") < 0.8f)
                {
                    MoveXZ();
                }

            }

            if (rotationMode && !occlusionMeshModeEnabled)
            {
                objectToMove = this.transform;

                if (Input.GetAxis("TriggerL") >= 0.8f)
                {
                    RotateZ();
                }

                if (Input.GetAxis("TriggerL") < 0.8f)
                {
                    RotateXZ();
                }

            }

            if (!rotationMode && occlusionMeshModeEnabled)
            {
                objectToMove = occlusionMeshTrans;

                if (Input.GetAxis("TriggerL") >= 0.8f)
                {
                    MoveZ();
                }

                if (Input.GetAxis("TriggerL") < 0.8f)
                {
                    MoveXZ();
                }
            }
        }
    }

    void MoveXZ()
    {
        objectToMove.localPosition += new Vector3(Input.GetAxis("StickLeftX") * fastMovementSpeed, Input.GetAxis("StickLeftY") * fastMovementSpeed * -1, 0);

        if (Input.GetAxis("DPADX") < -0.5f)
        {
            objectToMove.position -= new Vector3(slowMovementSpeed, 0, 0);
        }

        if (Input.GetAxis("DPADX") > 0.5f)
        {
            objectToMove.position += new Vector3(slowMovementSpeed, 0, 0);
        }


        if (Input.GetAxis("DPADY") > 0.5f)
        {
            objectToMove.position -= new Vector3(0, slowMovementSpeed, 0);
        }


        if (Input.GetAxis("DPADY") < -0.5f)
        {
            objectToMove.position += new Vector3(0, slowMovementSpeed, 0);
        }


    }

    void MoveZ()
    {
        objectToMove.localPosition += new Vector3(0, 0, Input.GetAxis("StickLeftY") * fastMovementSpeed);

        if (Input.GetAxis("DPADY") > 0.5f)
        {
            objectToMove.position += new Vector3(0, 0, slowMovementSpeed);
        }

        if (Input.GetAxis("DPADY") < -0.5f)
        {
            objectToMove.position -= new Vector3(0, 0, slowMovementSpeed);
        }
    }

    void RotateXZ()
    {
        transform.Rotate(new Vector3(Input.GetAxis("StickLeftY") * fastRotationSpeed, Input.GetAxis("StickLeftX") * fastRotationSpeed * -1, 0));


        if (Input.GetAxis("DPADY") > 0.5f)
        {
            transform.Rotate(new Vector3(slowRotationSpeed, 0, 0));
        }

        if (Input.GetAxis("DPADY") < -0.5f)
        {
            transform.Rotate(new Vector3(slowRotationSpeed * -1, 0, 0));
        }


        if (Input.GetAxis("DPADX") < -0.5f)
        {
            transform.Rotate(new Vector3(0, slowRotationSpeed, 0));
        }


        if (Input.GetAxis("DPADX") > 0.5f)
        {
            transform.Rotate(new Vector3(0, slowRotationSpeed * -1, 0));
        }
    }

    void RotateZ()
    {
        transform.Rotate(new Vector3(0, 0, Input.GetAxis("StickLeftX") * fastRotationSpeed * -1));

        if (Input.GetAxis("DPADX") < -0.5f)
        {
            transform.Rotate(new Vector3(0, 0, slowRotationSpeed));
        }

        if (Input.GetAxis("DPADX") > 0.5f)
        {
            transform.Rotate(new Vector3(0, 0, slowRotationSpeed * -1));
        }
    }

    void ResetRotation()
    {
        if (Input.GetButtonDown("XButton"))
        {
            transform.localRotation = startRotation;
        }
    }


    void ToogleEditMode()
    {
        if (Input.GetButtonDown("YButton"))
        {
            if (editModeEnabled)
            {
                editModeEnabled = false;
                ToolTipIcon.SetActive(false);
                YHint.SetActive(true);
                YHintIcon.SetActive(true);
                return;
            }

            if (!editModeEnabled)
            {
                editModeEnabled = true;
                ToolTipIcon.SetActive(true);
                YHint.SetActive(false);
                YHintIcon.SetActive(false);
                return;
            }
        }
    }

    void SaveLoad()
    {
        if (Input.GetButtonDown("Save"))
        {

            PlayerPrefs.SetFloat("XPos", transform.position.x);
            PlayerPrefs.SetFloat("YPos", transform.position.y);
            PlayerPrefs.SetFloat("ZPos", transform.position.z);

            PlayerPrefs.SetFloat("XRot", transform.rotation.x);
            PlayerPrefs.SetFloat("YRot", transform.rotation.y);
            PlayerPrefs.SetFloat("ZRot", transform.rotation.z);
            PlayerPrefs.SetFloat("WRot", transform.rotation.w);

            PlayerPrefs.SetFloat("XPosOcc", occlusionMeshTrans.position.x);
            PlayerPrefs.SetFloat("YPosOcc", occlusionMeshTrans.position.y);
            PlayerPrefs.SetFloat("ZPosOcc", occlusionMeshTrans.position.z);

            PlayerPrefs.Save();
        }

        if (Input.GetButtonDown("Load"))
        {
            if (PlayerPrefs.HasKey("XPos"))
            {
                transform.position = new Vector3(PlayerPrefs.GetFloat("XPos"), PlayerPrefs.GetFloat("YPos"), PlayerPrefs.GetFloat("ZPos"));

                occlusionMeshTrans.position = new Vector3(PlayerPrefs.GetFloat("XPosOcc"), PlayerPrefs.GetFloat("YPosOcc"), PlayerPrefs.GetFloat("ZPosOcc"));

                transform.rotation = new Quaternion(PlayerPrefs.GetFloat("XRot"), PlayerPrefs.GetFloat("YRot"), PlayerPrefs.GetFloat("ZRot"), PlayerPrefs.GetFloat("WRot"));
            }

        }

    }
}

