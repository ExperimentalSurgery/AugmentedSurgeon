using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            Gamepad gamepad = Gamepad.current;

            if (gamepad.bButton.wasPressedThisFrame)
            {
                if (GameMananger.instance.isInCalibrationMode)
                    phantomPlacement.LockTracking();
                else
                    phantomPlacement.StartTracking();
            }

            if (!GameMananger.instance.isInCalibrationMode)
            {
                occlusionMesh.GetComponent<Renderer>().material = occlusionMAT;

                ToogleEditMode(gamepad);

                if (editModeEnabled)
                {
                    SaveLoad(gamepad);
                    MoveRotateControl(gamepad);
                    ResetRotation(gamepad);
                }

                if (gamepad.leftShoulder.wasPressedThisFrame)
                {
                    ListOfModels[currentModelNo].SetActive(false);

                    currentModelNo--;
                    if (currentModelNo == -1)
                    {
                        currentModelNo = 0;
                    }

                    ListOfModels[currentModelNo].SetActive(true);
                }

                if (gamepad.rightShoulder.wasPressedThisFrame)
                {
                    ListOfModels[currentModelNo].SetActive(false);


                    currentModelNo++;
                    if (currentModelNo == ListOfModels.Length)
                    {
                        currentModelNo = ListOfModels.Length - 1;
                    }

                    ListOfModels[currentModelNo].SetActive(true);
                }

                if (gamepad.rightStickButton.wasPressedThisFrame)
                {
                    bool floatingLiverActiveState = floatingLiver.activeInHierarchy;
                    floatingLiver.SetActive(!floatingLiverActiveState);
                }
            }

            else
            {
                occlusionMesh.GetComponent<Renderer>().material = standardMAT;
            }
        }
    }

    void MoveRotateControl(Gamepad gamepad)
    {
        if (editModeEnabled && Gamepad.current != null)
        {
            if (gamepad.aButton.wasPressedThisFrame)
            {
                rotationMode = !rotationMode;
                occlusionMeshModeEnabled = false;
            }

            if (!rotationMode && !occlusionMeshModeEnabled)
            {
                objectToMove = this.transform;

                if (gamepad.leftTrigger.ReadValue() >= 0.8f)
                {
                    MoveZ(gamepad);
                }

                if (gamepad.leftTrigger.ReadValue() < 0.8f)
                {
                    MoveXZ(gamepad);
                }

            }

            if (rotationMode && !occlusionMeshModeEnabled)
            {
                objectToMove = this.transform;

                if (gamepad.leftTrigger.ReadValue() >= 0.8f)
                {
                    RotateZ(gamepad);
                }

                if (gamepad.leftTrigger.ReadValue() < 0.8f)
                {
                    RotateXZ(gamepad);
                }

            }

            if(!rotationMode && occlusionMeshModeEnabled)
            {
                objectToMove = occlusionMeshTrans;

                if (gamepad.leftTrigger.ReadValue() >= 0.8f)
                {
                    MoveZ(gamepad);
                }

                if (gamepad.leftTrigger.ReadValue() < 0.8f)
                {
                    MoveXZ(gamepad);
                }
            }
        }
    }

    void MoveXZ(Gamepad gamepad)
    {
        objectToMove.localPosition += new Vector3(gamepad.leftStick.x.ReadValue() * fastMovementSpeed, gamepad.leftStick.y.ReadValue() * fastMovementSpeed * -1, 0);

        if (gamepad.dpad.left.wasPressedThisFrame)
        {
            objectToMove.position -= new Vector3(slowMovementSpeed, 0, 0);
        }

        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            objectToMove.position += new Vector3(slowMovementSpeed, 0, 0);
        }


        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            objectToMove.position -= new Vector3(0, slowMovementSpeed, 0);
        }


        if (gamepad.dpad.down.wasPressedThisFrame)
        {
            objectToMove.position += new Vector3(0, slowMovementSpeed, 0);
        }


    }

    void MoveZ(Gamepad gamepad)
    {
        objectToMove.localPosition += new Vector3(0, 0, gamepad.leftStick.y.ReadValue() * fastMovementSpeed);

        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            objectToMove.position += new Vector3(0, 0, slowMovementSpeed);
        }

        if (gamepad.dpad.down.wasPressedThisFrame)
        {
            objectToMove.position -= new Vector3(0, 0, slowMovementSpeed);
        }
    }

    void RotateXZ(Gamepad gamepad)
    {
        transform.Rotate(new Vector3(gamepad.leftStick.y.ReadValue() * fastRotationSpeed, gamepad.leftStick.x.ReadValue() * fastRotationSpeed * -1, 0));


        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            transform.Rotate(new Vector3(slowRotationSpeed, 0, 0));
        }

        if (gamepad.dpad.down.wasPressedThisFrame)
        {
            transform.Rotate(new Vector3(slowRotationSpeed * -1, 0, 0));
        }


        if (gamepad.dpad.left.wasPressedThisFrame)
        {
            transform.Rotate(new Vector3(0, slowRotationSpeed, 0));
        }


        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            transform.Rotate(new Vector3(0, slowRotationSpeed * -1, 0));
        }
    }

    void RotateZ(Gamepad gamepad)
    {
        transform.Rotate(new Vector3(0, 0, gamepad.leftStick.x.ReadValue() * fastRotationSpeed * -1));

        if (gamepad.dpad.left.wasPressedThisFrame)
        {
            transform.Rotate(new Vector3(0, 0, slowRotationSpeed));
        }

        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            transform.Rotate(new Vector3(0, 0, slowRotationSpeed * -1));
        }
    }

    void ResetRotation(Gamepad gamepad)
    {
        if (gamepad.xButton.wasPressedThisFrame)
        {
            transform.localRotation = startRotation;
        }
    }


    void ToogleEditMode(Gamepad gamepad)
    {
        if (gamepad.yButton.wasPressedThisFrame)
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

    void SaveLoad(Gamepad gamepad)
    {
        if (gamepad.selectButton.wasPressedThisFrame)
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

        if (gamepad.startButton.wasPressedThisFrame)
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

