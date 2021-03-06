using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

#pragma warning disable 0414 // TODO: Later do clean-up
#pragma warning disable 0436

public class CameraController : MonoBehaviour
{

    GameObject forwardPosition;
    public bool canUpdateDialog;
    public Text stringTitleText;

    public float eyeheight;
    public Texture crosshair;
    public bool drawCrosshair = true;
    public bool useElapsedTime = false;
    public bool printFPS = false;
    public bool showHelp = false;
    public bool ccDialogActive = false;

    RayObject raycast;
    public Tutorial tutorial;
    DialogViewer dialogViewer;
    CleanlinessLevel cleaner;
    public float sensitivityY = 2F;

    public float minimumY = -60F;
    public float maximumY = 60F;
    // Addition for locking screen
    public bool canUpdateCamera = true;

    float rotationY = 0F;
    float rotationX = 0F;
    int timeStart = 10, elapsed, minutesLeft, secondsLeft;
    string timeString = "";
    string fpsString = "";
    float cleanTime = 0f;
    bool cleanData = false;
    //public Text TimeString;

    public MouseLook_1 look;
    CharacterMotor motor;
    LockCursor cursor;
    Rect windowRect;
 

    public string currentRoom;
    public int currentRoomCount;
    public int currentRoomTotal;
    public int yardRoomCount;
    public int yardRoomTotal;
    public int livingRoomCount;
    public int livingRoomTotal;
    public int kitchenRoomCount;
    public int kitchenRoomTotal;
    public int hallwayRoomCount;
    public int hallwayRoomTotal;
    public int secondfloorRoomCount;
    public int secondfloorRoomTotal;
    public int sfbRoomCount;
    public int sfbRoomTotal;
    public int bathroomRoomCount;
    public int bathroomRoomTotal;
    public int bedroomRoomCount;
    public int bedroomRoomTotal;
    public int basementRoomCount;
    public int basementRoomTotal;

    public int totalRoomsCount;
    public int totalRoomsTotal;
    string moduleStr;

    GameObject fader;

    void Start()
    {
        dialogViewer = GameManager.DialogViewer;
        fader = dialogViewer.fader;

        if (SceneManager.GetActiveScene().name.Equals("accessment")) ObjectTracker.assessmentLock = true;

        if (GameManager.UsingOVR) {
            canUpdateDialog = true;
            forwardPosition = GameObject.Find("ForwardDirection");
        } else
            canUpdateDialog = false;
        if (!Tutorial.menuToTutorial && !Tutorial.menuToWalkThrough) { countTotal();/* wordSearch("Addtional"); */}

        fader.SetActive(true);
        fadeOut();
        
        if (GameObject.Find("Tutorial") != null)
            tutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();

        raycast = GameManager.MainCamera.GetComponent<RayObject>();

        look = GameManager.Player.GetComponent<MouseLook_1>();
        motor = GameManager.Player.GetComponent<CharacterMotor>();
        cursor = GameManager.Player.GetComponent<LockCursor>();

        windowRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300);
    }

    public void assessmentIntroFadeOut()
    {
        fader.SetActive(true);
        fader.GetComponent<CanvasGroup>().alpha = 0;
        dialogViewer.loading.text = "Teleporting...";
        fader.GetComponent<CanvasGroup>().DOFade(1, 0.7f).OnComplete(randomStartPosition);
    }

    void randomStartPosition()
    {
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            if (audioSource.gameObject.name != "overviewWindow" && audioSource.gameObject.name != "waterParticle" && audioSource.gameObject.name != "waterParticle_snd" && audioSource.gameObject.name != "bedroomtv"
                && audioSource.gameObject.name != "loudTV" && audioSource.gameObject.name != "tvupstairs")
                audioSource.enabled = true;
        }
        GameObject.Find("overviewWindow").SetActive(false);
        GameManager.DialogViewer.nextWindow.SetActive(true);
        Invoke("assessIntrofadeOut2", 2f);
        int room = UnityEngine.Random.Range(0, 3);
        GameObject teleportTarget = null;
        switch (room)
        {
            case 0:
                //to livingroom
                teleportTarget = GameObject.Find("locationCube_livingRoom");
                GameObject.Find("Living Room Trigger").GetComponent<RoomTrigger>().roomChange();
                break;
            case 1:
                //to kitchen
                teleportTarget = GameObject.Find("locationCube_kitchen");
                GameObject.Find("Kitchen Trigger").GetComponent<RoomTrigger>().roomChange();
                break;
            case 2:
                //to bedroom
                teleportTarget = GameObject.Find("locationCube_bedroom");
                GameObject.Find("Bedroom Trigger").GetComponent<RoomTrigger>().roomChange();
                break;
            case 3:
                //to Bathroom
                teleportTarget = GameObject.Find("locationCube_bathroom");
                GameObject.Find("First Floor Bathroom Trigger").GetComponent<RoomTrigger>().roomChange();
                break;
            default:
              //  Debug.Log("wrong case number");
                break;
        }
        GameManager.Player.transform.position = new Vector3(teleportTarget.transform.position.x, teleportTarget.GetComponent<locationCubeCollision>().iniY, teleportTarget.transform.position.z);
        detectRoomTrigger();
    }
    void fadeOut()
    {
        fader.GetComponent<CanvasGroup>().DOFade(0, 1f);
    }
    void assessIntrofadeOut2()
    {
        dialogViewer.loading.text = "";
        fader.GetComponent<CanvasGroup>().DOFade(0, 1f);
    }

    //wordSearch is a debug tool DO NOT DETELE
    /*
     void wordSearch(string word)
    {
        foreach (GameObject oneObj in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (oneObj.GetComponent("Dialog") as Dialog != null)
            {
                foreach (DialogFrame df in oneObj.GetComponent<Dialog>().trees[0].frames)
                {
                    if (df.title.Contains(word) && df.type != DialogFrame.Type.Image) Debug.Log(oneObj.name+",title" + df.title + LayerMask.LayerToName(oneObj.layer));
                    if (df.text.Contains(word) && df.type != DialogFrame.Type.Image) Debug.Log(oneObj.name +",text,"+ df.text + LayerMask.LayerToName(oneObj.layer));
                }
            }
        }
    }
    */
    void countTotal()
    {  // int count=0;
        foreach (GameObject dangerObj in GameObject.FindGameObjectsWithTag("SelectForDanger"))
        {
           // count++;
           if (dangerObj.GetComponent("Dialog") as Dialog != null)
            {
                if (dangerObj.layer == LayerMask.NameToLayer("outdoors")) { yardRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("livingRoom")) { livingRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("kitchen")) { kitchenRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("bedroom") ||
                    dangerObj.layer == LayerMask.NameToLayer("bedroomcloset")) { bedroomRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("bathroom")) { bathroomRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("secondfloor") ||
                    dangerObj.layer == LayerMask.NameToLayer("secondfloorBathroom") ||
                    dangerObj.layer == LayerMask.NameToLayer("secondfloorStorage")) { secondfloorRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("hallway") ||
                    dangerObj.layer == LayerMask.NameToLayer("hallwayCloset")) { hallwayRoomTotal++; }
                else if (dangerObj.layer == LayerMask.NameToLayer("basement")) { basementRoomTotal++;}

                //saving default answers
                  int correctAnswer = 1;
                if (SceneManager.GetActiveScene().name == "accessment") ObjectTracker.trackAllObjects_multiple(SceneManager.GetActiveScene().name, dangerObj.layer, dangerObj.name, correctAnswer);
                else
                    ObjectTracker.trackAllObjects(SceneManager.GetActiveScene().name, dangerObj.layer, dangerObj.name, correctAnswer, 999);
            }
            //else Debug.Log(dangerObj.name);


        }

        foreach (GameObject nonDangerObj in GameObject.FindGameObjectsWithTag("SelectForNotDanger"))
        {
            if (nonDangerObj.GetComponent("Dialog") as Dialog != null)
            {
                int correctAnswer = 0;
                if (SceneManager.GetActiveScene().name == "accessment") ObjectTracker.trackAllObjects_multiple(SceneManager.GetActiveScene().name, nonDangerObj.layer, nonDangerObj.name, correctAnswer);
                else
                ObjectTracker.trackAllObjects(SceneManager.GetActiveScene().name, nonDangerObj.layer, nonDangerObj.name, correctAnswer, 999);
            }//else  Debug.Log(nonDangerObj.name);
        }
        

        moduleStr = "";
     
        if (SceneManager.GetActiveScene().name.Contains("electrical")) moduleStr = "Electrical, Fire, & Burn Hazards";
        else if (SceneManager.GetActiveScene().name == "slipTripLift") moduleStr = "Slip, Trip, & Lift Hazards";
        else if (SceneManager.GetActiveScene().name == "environmental") moduleStr = "Environmental Hazards";
        else if (SceneManager.GetActiveScene().name == "accessment") moduleStr = "Assessment";

        totalRoomsTotal = yardRoomTotal + livingRoomTotal + kitchenRoomTotal + bedroomRoomTotal + bathroomRoomTotal + secondfloorRoomTotal + hallwayRoomTotal + basementRoomTotal;

    }
    void Update()
    {
        updateCamera();

        if (GameManager.UsingOVR && canUpdateDialog)
        {
            dialogViewer.dialogCanvas.transform.position = new Vector3(forwardPosition.transform.position.x, forwardPosition.transform.position.y, forwardPosition.transform.position.z);
            canUpdateDialog = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.DialogViewer.PauseGame();

        }
     /*   if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            GameManager.Player.GetComponent<CharacterMotor>().movement.maxForwardSpeed = 4;
        }else if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            GameManager.Player.GetComponent<CharacterMotor>().movement.maxForwardSpeed = 2;
        }*/
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            if (GameManager.UsingOVR) canUpdateDialog = true;
            if (!GameManager.DialogViewer.isDialogActive)
            {
                GameManager.DialogViewer.OpenEscapeMenu();
                LockCamera(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                if (GameManager.DialogViewer.escMenu.activeSelf)
                {
                    GameManager.DialogViewer.escMenu.SetActive(false);
                    GameManager.DialogViewer.isDialogActive = false;
                    LockCamera(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    GameManager.DialogViewer.escMenuTlt.text = "Menu";
                    GameManager.DialogViewer.escb1.text = "Toolbox";
                    GameManager.DialogViewer.escb2.text = "Return";
                    GameManager.DialogViewer.escb3.gameObject.transform.parent.gameObject.SetActive(true);

                }
                else if (GameManager.DialogViewer.toolWindow.activeSelf)
                {
                    GameManager.DialogViewer.toolWindow.SetActive(false);
                    GameManager.DialogViewer.isDialogActive = false;
                    LockCamera(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else if (GameManager.DialogViewer.soundWindow.activeSelf)
                {
                    GameManager.DialogViewer.soundWindow.SetActive(false);
                    GameManager.DialogViewer.isDialogActive = false;
                    LockCamera(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else if (GameManager.DialogViewer.teleportMenu.activeSelf)
                {
                    GameManager.DialogViewer.teleportMenu.SetActive(false);
                    GameManager.DialogViewer.isDialogActive = false;
                    LockCamera(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                }
                else if (GameManager.DialogViewer.helpWindow.activeSelf)
                {
                    GameManager.DialogViewer.helpWindow.SetActive(false);
                    GameManager.DialogViewer.isDialogActive = false;
                    LockCamera(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        
    }


    public void detectRoomTrigger()
    {
        if (!Tutorial.menuToWalkThrough)
        {
            Debug.Log("Room Changed");
#if UNITY_WEBGL
#else
            GameManager.performanceControl.checkAnimatedUV(SceneManager.GetActiveScene().name);
#endif

            if (currentRoom.Equals("Kitchen"))
            {
                currentRoomCount = kitchenRoomCount;
                currentRoomTotal = kitchenRoomTotal;
            }
            else if (currentRoom.Equals("Front Yard"))
            {
                currentRoomCount = yardRoomCount;
                currentRoomTotal = yardRoomTotal;
            }
            else if (currentRoom.Equals("Living Room"))
            {
                currentRoomCount = livingRoomCount;
                currentRoomTotal = livingRoomTotal;
            }
            else if (currentRoom.Equals("Hallway/Hall Closet"))
            {
                currentRoomCount = hallwayRoomCount;
                currentRoomTotal = hallwayRoomTotal;
            }
            else if (currentRoom.Equals("Second Floor"))
            {
                currentRoomCount = secondfloorRoomCount;
                currentRoomTotal = secondfloorRoomTotal;
            }
            else if (currentRoom.Equals("First Floor Bathroom"))
            {
                currentRoomCount = bathroomRoomCount;
                currentRoomTotal = bathroomRoomTotal;
            }
            else if (currentRoom.Equals("Bedroom"))
            {
                currentRoomCount = bedroomRoomCount;
                currentRoomTotal = bedroomRoomTotal;
            }
            else if (currentRoom.Equals("Basement"))
            {
                currentRoomCount = basementRoomCount;
                currentRoomTotal = basementRoomTotal;
            }

            totalRoomsCount = kitchenRoomCount + yardRoomCount + livingRoomCount + hallwayRoomCount + secondfloorRoomCount + bathroomRoomCount + bedroomRoomCount + basementRoomCount;

            GameObject scorePanel = dialogViewer.scorePanel;
            Text score_module = scorePanel.transform.FindChild("scoreText1_module").transform.GetComponent<Text>();
            Text score_found = scorePanel.transform.FindChild("scoreText2_found").transform.GetComponent<Text>();


            score_module.text = "Module: " + moduleStr + "\nCurrent Room:\nHazards found:\n\nPress ESC and Exit Module to finish the exam.";
            score_found.text = currentRoom + "\n" + currentRoomCount + " out of " + currentRoomTotal + " in this room" +
                "\n" + totalRoomsCount + " out of " + totalRoomsTotal + " in this module";


        }
    }
    void OnGUI()
    {
        if (!GameManager.UsingOVR)
        {
            float xMin = (Screen.width / 2) - (crosshair.width / 2);
            float yMin = (Screen.height / 2) - (crosshair.height / 2);

            if (drawCrosshair)
                GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
        }

        
    }

    public void LockCamera(bool val)
    {
       
        if (!GameManager.UsingOVR)
        {
            dialogViewer.blurEffect.enabled = val;
            drawCrosshair = !val;
            cursor.GUIActive = val;
        }

            look.canLook = !val;
            canUpdateCamera = !val;
            motor.canControl = !val;
    }

    void updateCamera()
    {
        if (!canUpdateCamera)
        {
            GameManager.Player.GetComponent<FPSInputController>().enabled = false;
            look.enabled = false;
            return;
        }else GameManager.Player.GetComponent<FPSInputController>().enabled = true;
       if(!GameManager.UsingOVR) look.enabled = true;
        Vector3 pos = GameManager.Player.transform.position;
        transform.position = new Vector3(pos.x, pos.y + eyeheight, pos.z);
        if (!GameManager.UsingOVR)
        {

            GameManager.Player.transform.eulerAngles = new Vector3(0, GameManager.Player.transform.eulerAngles.y + Input.GetAxis("Mouse X") * 2f, 0);
            transform.rotation = GameManager.Player.transform.rotation;
            if (GameManager.UsingIntersense)
            {
                // TODO: Revise if this doesn't work
                // It doesn't seem there is a need to preserve this
                // in the case of lost signal; it seems to preserve already
                if ((int)(GameManager.Intersense.trackingStatus2 / 2.55) != 0)
                {
                    rotationY = GameManager.Intersense.Yorien2;
                }
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            }

            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }
}