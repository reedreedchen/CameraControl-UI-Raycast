using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class Tutorial : MonoBehaviour
{
    public static bool menuToTutorial;
    public static bool menuToWalkThrough;
	public static bool menuToOvr;

    DialogViewer dv;
    RayObject ro;

    public GameObject overviewWindow;
    public int counter;
    int clickCount;
    bool locked, pause;
    bool stopUpdates = false;
    float delay = 0f;
    bool firstJump;

    bool answeredYes = false;
    bool answeredQuit = false;
    public GameObject NWindow;
    public GameObject YNWindow;
    public GameObject IWindow;
    public GameObject MC4Window;
    public Text ITitle;
    public Text NTitle;
    public Text YNTitle;
    public Text MC4Title;
    public RawImage IImage;
    public Text NText;
    public Text YNText;
    public Text MC4Text;
    public Text NButtonText;
    public Text YN1ButtonText;
    public Text YN2ButtonText;
    public Button IButton1;
    public Button IButton2;
    public Button IButton3;
    public Button IButton4;
    public Button MC4Button1;
    public Button MC4Button2;
    public Button MC4Button3;
    public Button MC4Button4;
    public GameObject escWindow;
    public GameObject toolWindow;
	public GameObject ScorePanel;
    public GameObject dialogbox;
    public GameObject sound;
    public GameObject reminder;
    public GameObject taskWindow;
    public GameObject taskText;
    public GameObject helpWindow;
    public GameObject teleportWindow;
    MouseLook_1 look;
    CharacterMotor motor;
    CameraController tutorialCamera;
    GameObject ball_shadow;
    GameObject wagon_shadow;
    GameObject ball;
    GameObject wagon;
    GameObject mainCamera;
  //  FPSInputController fpsController;
    GameObject porchDoor;
    public GameObject soundWindow;
    float oringinalSoundLvl;
    bool toolactive = false;
    bool soundAdjusted = false;
    int flashlightUsed = 0;
    int magUsed = 0;
 
    public GameObject tourCamera;


    bool wPressed, aPressed, sPressed, dPressed;

    void Start()
    {

        GameObject.Find("forShadows").SetActive(false);
        wPressed = aPressed = sPressed = dPressed = false;
        clickCount = 0;

        ball = GameObject.Find("Ball");
        wagon = GameObject.Find("wagon");
        ball_shadow = GameObject.Find("ball_fakeShadow");
        wagon_shadow = GameObject.Find("wagon_fakeShadow");
        if (ball != null) ball.tag = "Untagged";
        if (wagon != null) wagon.tag = "Untagged";


        mainCamera = GameObject.Find("Main Camera");
    //    fpsController = GameManager.Player.GetComponent<FPSInputController>();
		porchDoor = GameObject.Find("porchdoor_dr_reversed");
        look = GameManager.Player.GetComponent<MouseLook_1>();
        motor = GameManager.Player.GetComponent<CharacterMotor>();
        tutorialCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        dv = GameObject.Find("GameController").GetComponent<DialogViewer>();
        ro = GameObject.Find("Main Camera").GetComponent<RayObject>();
        GameManager.Player.GetComponent<LockCursor>().enabled = false;
        // look.canLook = true;



        if (Tutorial.menuToTutorial) //tutorial
        {
            backgroundControl(true);
            foreach (GameObject allDanger in GameObject.FindGameObjectsWithTag("SelectForDanger"))
			{
				allDanger.tag = "Untagged";
			/*	if(allDanger.GetComponent<BoxCollider>()!=null)
				{
					allDanger.GetComponent<BoxCollider>().enabled=false;
				}else if(allDanger.GetComponent<MeshCollider>()!=null)
				{
					allDanger.GetComponent<MeshCollider>().enabled=false;
				}*/
			}
			foreach (GameObject allNotDanger in GameObject.FindGameObjectsWithTag("SelectForNotDanger"))
			{
				allNotDanger.tag="Untagged";
				/*if(allNotDanger.GetComponent<BoxCollider>()!=null)
				{
					allNotDanger.GetComponent<BoxCollider>().enabled=false;
				}else if(allNotDanger.GetComponent<MeshCollider>()!=null)
				{
					allNotDanger.GetComponent<MeshCollider>().enabled=false;
				}*/
			}


            ScorePanel.SetActive(false);
			ScorePanel.SetActive(false);

            ball.SetActive(true);
            ball_shadow.SetActive(true);
            wagon.SetActive(true);
            wagon_shadow.SetActive(true);
        }
        else if (!Tutorial.menuToTutorial && !Tutorial.menuToWalkThrough)
        {
            ScorePanel.SetActive(false);
            if(ball != null)
                ball.SetActive(false);
            if (ball_shadow != null)
                ball_shadow.SetActive(false);
            if(wagon !=null)
                wagon.SetActive(false);
            if (wagon_shadow != null)
                wagon_shadow.SetActive(false);
            //scenario page1
            if (!SceneManager.GetActiveScene().name.Equals("accessment"))
                NWindow.SetActive(true);
            else {
                overviewWindow.SetActive(true);
            }
        }
        else if (Tutorial.menuToWalkThrough)
        { //walk through
            GameManager.Player.gameObject.GetComponent<AudioListener>().enabled = false;
            mainCamera.SetActive(false);
            ball.SetActive(false);
            wagon.SetActive(false);
            ball_shadow.SetActive(false);
            wagon_shadow.SetActive(false);
            tourCamera.SetActive(true);
            dialogbox.SetActive(false);
            GameObject.Find("outside").GetComponent<AudioSource>().mute = true;
            GameObject.Find("loudTV").GetComponent<AudioSource>().volume = 0.1f;
            taskWindow.SetActive(true);
            taskText.GetComponent<Text>().text = "Please turn on your speakers.";
            Invoke("turnOffTask", 5f);
        }
    }

    public void turnOffTask() {
        taskWindow.SetActive(false);
    }
    public void turnOffTask2()
    {
        taskWindow.SetActive(false);
        ScorePanel.SetActive(true);
    }
    public void backtomenu()
    {

        SceneManager.LoadScene("menu");
    }

    void backgroundControl(bool value)
    {
        GameManager.DialogViewer.blurEffect.enabled = !value;
        if (value) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
        Cursor.visible =!value;
        tutorialCamera.drawCrosshair = value;
        dv.isDialogActive = !value;
        if (!GameManager.UsingOVR) look.canLook = value;
        tutorialCamera.canUpdateCamera = value;
        motor.canControl = value;
        tutorialCamera.ccDialogActive = value;
    }

    void LateUpdate()
    {

        if (Tutorial.menuToWalkThrough)
        {         
            if (Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                GameObject.Find("tourCam").GetComponent<AudioListener>().enabled = false;
                GameObject.Find("fader").GetComponent<CanvasGroup>().DOFade(1, 1f).OnComplete(backtomenu);
            }
        }
        
    /*    if(!GameManager.UsingOVR)
        {
            if (Tutorial.menuToTutorial)
            {
                if (teleportWindow.activeSelf || escWindow.activeSelf || toolWindow.activeSelf || NWindow.activeSelf || YNWindow.activeSelf || MC4Window.activeSelf || IWindow.activeSelf || helpWindow.activeSelf || soundWindow.activeSelf)
                {

                }
                else
                {
                    if (!tutorialCamera.drawCrosshair)
                        tutorialCamera.drawCrosshair = true;
                }
            }
		}*/

        if (!Tutorial.menuToTutorial && !Tutorial.menuToWalkThrough && counter == 0) 
		{  
            backgroundControl(false);
            NText.text = gameObject.GetComponent<Dialog>().trees[1].frames[0].text;
            NTitle.text = gameObject.GetComponent<Dialog>().trees[1].frames[0].title;
            NButtonText.text = gameObject.GetComponent<Dialog>().trees[1].frames[0].buttons[0].text;
            pause = true;

        }
    }
    void closeError()
    {
        showMessage.closeError();
    }
    void Update()
    {
        if (menuToTutorial) tutorialUpdates();
    }
    void tutorialUpdates()
    {

        if (!dv.isDialogActive)
            GameManager.Player.transform.eulerAngles = new Vector3(0, GameManager.Player.transform.eulerAngles.y + Input.GetAxis("Mouse X") * 2f, 0);
        //task starts~renee

        if (counter == 3 && ro.rayInfo.rigidbody != null && ro.hovered.name.Equals("Ball"))
        {
            taskText.GetComponent<Text>().text = "";
            pause = false;
        }
        else if (counter == 4)
        {
            if (!NWindow.activeSelf && !escWindow.activeSelf)
            {
                string collectText = "";
                /*
                string w = "-Press \"W\" or the upwards arrow and let go.";
                string a = "-Press \"A\" or the leftwards arrow and let go.";
                string s = "-Press \"S\" or the downwards arrow and let go.";
                string d = "-Press \"D\" or the rightwards arrow and let go.";
               */
                string w = "-Press \"W\" or \"↑\" and let go.";
                string a = "-Press \"A\" or \"←\" and let go.";
                string s = "-Press \"S\" or \"↓\" and let go.";
                string d = "-Press \"D\" or \"→\" and let go.";


                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                    wPressed = true;
                else if (wPressed == false) collectText += w + "\n";
                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
                    aPressed = true;
                else if (aPressed == false) collectText += a + "\n";
                if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
                    sPressed = true;
                else if (sPressed == false) collectText += s + "\n";
                if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
                    dPressed = true;
                else if (dPressed == false) collectText += d;

                if (wPressed && aPressed && sPressed && dPressed)
                {
                    taskText.GetComponent<Text>().text = "";
                    pause = false;
                }
                else taskText.GetComponent<Text>().text = collectText;
            }

        }
        else if (counter == 5)
        {
            if (!NWindow.activeSelf && ball.tag != "SelectForDanger")
                ball.tag = "SelectForDanger";

            if (Input.GetMouseButtonUp(0) && !IWindow.activeSelf)
            {
                clickCount += 1;
                if (!NWindow.activeSelf && clickCount % 3 == 0)
                {
                    showMessage.showError("Move closer to the ball.");
                    Invoke("closeError", 2f);
                }
            }

            if (ro.rayInfo.rigidbody != null && ro.rayInfo.rigidbody.gameObject.name == "Ball" && Input.GetButtonDown("Fire1") && ball.tag == ("SelectForDanger"))
            {
                GameObject.Find("closerWarning").GetComponent<Text>().text = "";
                taskText.GetComponent<Text>().text = "";
                ball.GetComponent<SphereCollider>().enabled = false;
                pause = false;
            }

        }
        else if (counter == 8)
        {
            if (ro.rayInfo.rigidbody != null && ro.rayInfo.rigidbody.gameObject.name == "wagon" && Input.GetButtonDown("Fire1"))
            {
                taskText.GetComponent<Text>().text = "";
                wagon.tag = "Untagged";
                pause = false;
            }
            oringinalSoundLvl = soundWindow.transform.FindChild("Slider").GetComponent<Slider>().value;
        }
        else if (counter == 11)//tools
        {
            if (!NWindow.activeSelf && !toolactive)
            {
                toolactive = true;
            }
            else if (!NWindow.activeSelf && (toolWindow.activeSelf || soundWindow.activeSelf))
            {

                float currentSoundLvl = soundWindow.transform.FindChild("Slider").GetComponent<Slider>().value;

                string collectText = "";
                string flashLightText = "-Click on the \"Flashlight\" to turn on and off the flashlight.\n";
                string magText = "-Click on the \"Magnifier\" to turn on and off the magnifier.\n";
                string soundText = "-Click on the \"Sound\" to access the sound box and adjust the volume.";

                if (currentSoundLvl > oringinalSoundLvl + 0.05f || currentSoundLvl < oringinalSoundLvl - 0.05f) soundAdjusted = true;

                //0=no action, 1=turned on, 2= has been turned on and off
                if (flashlightUsed == 0 && GameManager.MainCamera.GetComponent<flashlight>().isOn) flashlightUsed = 1;
                else if (flashlightUsed == 1 && !GameManager.MainCamera.GetComponent<flashlight>().isOn) flashlightUsed = 2;
                if (magUsed == 0 && GameObject.Find("mag").GetComponent<Image>().isActiveAndEnabled) magUsed = 1;
                else if (magUsed == 1 && !GameObject.Find("mag").GetComponent<Image>().isActiveAndEnabled) magUsed = 2;

                if (flashlightUsed != 2) collectText += flashLightText;
                if (magUsed != 2) collectText += magText;
                if (!soundAdjusted) collectText += soundText;
                
                if (taskText.GetComponent<Text>().text != collectText)
                    taskText.GetComponent<Text>().text = collectText;

                

                if (magUsed == 2 && flashlightUsed == 2 && soundAdjusted)
                {
                    soundWindow.SetActive(false);
                    if (toolWindow.activeSelf) toolWindow.SetActive(false);
                    if (escWindow.activeSelf) escWindow.SetActive(false);
                    pause = false;
                }

            }
        }
        else if (counter == 12)
        {
            if (ro.rayInfo.rigidbody != null && ro.rayInfo.rigidbody.gameObject == porchDoor && ro.rayInfo.rigidbody.gameObject.tag == "SelectToOpenRight" && Input.GetButtonDown("Fire1"))
            {
                InteractManager.AddDoorRight(porchDoor);
                taskText.GetComponent<Text>().text = "";
                pause = false;
            }
        }
        else if (counter == 13)
        {
            if (ro.rayInfo.rigidbody != null && ro.rayInfo.rigidbody.gameObject == porchDoor && ro.rayInfo.rigidbody.gameObject.tag == "SelectToOpenLeft" && Input.GetButtonDown("Fire1"))
            {
                InteractManager.AddDoorLeft(porchDoor);
                taskText.GetComponent<Text>().text = "";
                pause = false;
            }
        }
        else if (counter == 14)
        {
            if (ro.rayInfo.rigidbody != null && Input.GetButtonDown("Fire1"))
            {
                if (ro.rayInfo.rigidbody.gameObject.name == "cuteTowel_upstairs" || ro.rayInfo.rigidbody.gameObject.name == "dotTowel_upstairs")
                {
                    GameObject.Find("cuteTowel_upstairs").tag = "Untagged";
                    GameObject.Find("dotTowel_upstairs").tag = "Untagged";
                    taskText.GetComponent<Text>().text = "";
                    pause = false;
                }
            }
        }
        else if (counter == 15)
        {
            if (ro.rayInfo.rigidbody != null && Input.GetButtonDown("Fire1"))
            {
                if (ro.rayInfo.rigidbody.gameObject.name == "lamp2" || ro.rayInfo.rigidbody.gameObject.name == "lamp3")
                {
                    taskText.GetComponent<Text>().text = "";
                    GameObject.Find("lamp2").tag = "Untagged";
                    GameObject.Find("lamp3").tag="Untagged";
                    pause = false;
                }
            }
        }
        else if (counter == 16)
        {
            if (ro.rayInfo.rigidbody != null && (ro.rayInfo.rigidbody.gameObject.name.Contains("toothBrush") || ro.rayInfo.rigidbody.gameObject.name.Contains("toothCeaning")) && Input.GetButtonDown("Fire1"))
            {
                taskText.GetComponent<Text>().text = "";
                GameObject.Find("toothCeaning").tag = "Untagged";
                GameObject.Find("toothBrush").tag = "Untagged";
                pause = false;
            }
        }
        else if (counter == 17)
        {
            if (ro.rayInfo.rigidbody != null && ro.rayInfo.rigidbody.gameObject.name == "washer" && Input.GetButtonDown("Fire1"))
            {
                taskText.GetComponent<Text>().text = "";
                GameObject.Find("washer").tag = "Untagged";
                pause = false;
            }
        }
        //task ends~ renee~

        if (stopUpdates)
        {
            if (Time.time - delay >= 2f)
            {
                if (Tutorial.menuToTutorial)
                {
                    tutorialCamera.drawCrosshair = false;
                    GameObject.Find("fader").GetComponent<CanvasGroup>().DOFade(1, 1f).OnComplete(backtomenu);
                }
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.X) || answeredQuit)
        {
            counter = 14;
            tutorialCamera.drawCrosshair = false;
            GameObject.Find("fader").GetComponent<CanvasGroup>().DOFade(1, 1f).OnComplete(backtomenu);

        }

        if (Tutorial.menuToTutorial && !pause)
        {
            if (firstJump)
                counter = 10;
            else
            {
                counter++;
            }

            firstJump = false;
            locked = false;
        }
        else if (pause) locked = true;
        //update tutorial
        if (counter > 0 && counter < 19 && !locked)
        {
            if (counter == 1)
            {
                // Text: Welcome
                YNWindow.SetActive(true);

                backgroundControl(false);

                YNText.text = gameObject.GetComponent<Dialog>().trees[0].frames[0].text;
                YNTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[0].title;
                YN1ButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[0].buttons[0].text;
                YN2ButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[0].buttons[1].text;
                pause = true;
            }
            else if (counter == 2)
            {
                YNWindow.SetActive(false);
                NWindow.SetActive(true);
                taskWindow.SetActive(true);
                taskText.GetComponent<Text>().text = "This is the task window.";
                // Text: Task Window
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[1].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[1].title;
                NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[1].buttons[0].text;
                //          if (GameManager.UsingOVR) dv.taskWindow.SetActive(true);
                pause = true;
            }
            else if (counter == 3)
            {
                backgroundControl(false);
                taskText.GetComponent<Text>().text = "-Move your mouse to the ball.";
                ro.rayCastLength = 5f;
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[2].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[2].title;
                NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[2].buttons[0].text;
                pause = true;
            }
            else if (counter == 4)
            {
                //taskText.GetComponent<Text>().text = "-Press \"W\" or the upwards arrow and let go. \n-Press \"A\" or the leftwards arrow and let go.\n-Press \"S\" or the downwards arrow and let go. \n-Press \"D\" or the rightwards arrow and let go.";
                taskText.GetComponent<Text>().text = "-Press \"W\" or \"↑\" and let go. \n-Press \"A\" or \"←\" and let go.\n-Press \"S\" or \"↓\" and let go. \n-Press \"D\" or \"→\" and let go.";

                backgroundControl(false);
                NWindow.SetActive(true);

                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[3].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[3].title;
                NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[3].buttons[0].text;
                ro.rayCastLength = 2f;
                pause = true;
            }
            else if (counter == 5)
            {
                // Select ball
                backgroundControl(false);
                taskText.GetComponent<Text>().text = "-Click on the ball.";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[5].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[5].title;
                NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[5].buttons[0].text;
                NWindow.SetActive(true);
                pause = true;
            }
            else if (counter == 6)
            {
                //select ball

                backgroundControl(false);
                taskText.GetComponent<Text>().text = "-Click on Yes or No to answer the question.";
                ITitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[6].title;
                IImage.texture = gameObject.GetComponent<Dialog>().trees[0].frames[6].texture;
                IButton1.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.green;
                IButton2.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.green;
                IButton3.GetComponent<Button>().interactable = false;
                IButton4.GetComponent<Button>().interactable = false;
                IWindow.SetActive(true);

                pause = true;
            }
            else if (counter == 7)
            {
                taskText.GetComponent<Text>().text = "-Click on Continue to go on.";
                // ball: correct/incorrect
                MC4Text.text = "This is not a hazard.";
                if (answeredYes)
                    MC4Title.text = "Incorrect";
                else
                    MC4Title.text = "Correct";

                IWindow.SetActive(false);
                MC4Button1.GetComponent<Button>().interactable = false;
                MC4Button2.GetComponent<Button>().interactable = false;
                MC4Button3.GetComponent<Button>().interactable = false;
                MC4Button4.GetComponent<Button>().interactable = true;
                MC4Button4.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.green;
                MC4Window.SetActive(true);
                pause = true;
            }
            else if (counter == 8)
            {
                // Select wagon
                wagon.tag = "SelectForDanger";
                taskText.GetComponent<Text>().text = "-Find and click on the wagon.";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[8].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[8].title;
                NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[8].buttons[0].text;
                NWindow.SetActive(true);
                pause = true;
            }
            else if (counter == 9)
            {
                taskText.GetComponent<Text>().text = "-Click on Yes or No to answer the question.";
                backgroundControl(false);
                IWindow.SetActive(true);
                IImage.texture = gameObject.GetComponent<Dialog>().trees[0].frames[9].texture;
                IButton1.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.green;
                IButton2.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.green;
                IButton3.GetComponent<Button>().interactable = false;
                IButton4.GetComponent<Button>().interactable = false;
                pause = true;
            }
            else if (counter == 10)
            {
                taskText.GetComponent<Text>().text = "-Click on Continue to go on.";
                if (answeredYes)
                    ITitle.text = "Correct";
                else
                    ITitle.text = "Incorrect";

                IButton1.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                IButton2.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                IButton4.gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.green;
                IImage.texture = gameObject.GetComponent<Dialog>().trees[0].frames[10].texture;
                IButton1.GetComponent<Button>().interactable = false;
                IButton2.GetComponent<Button>().interactable = false;
                IButton3.GetComponent<Button>().interactable = false;
                IButton4.GetComponent<Button>().interactable = true;

                pause = true;
            }
            else if (counter == 11)
            {
                //press esc
                taskText.GetComponent<Text>().text = "-Press \"ESC\" to access the Toolbox Menu.";
                backgroundControl(false);
                IWindow.SetActive(false);
                NWindow.SetActive(true);
                // Moved forward
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[4].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[4].title;
                NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[4].buttons[0].text;
                pause = true;
            }
            else if (counter == 12)
            {
                // open the door
                IWindow.SetActive(false);
                porchDoor.tag = "SelectToOpenRight";
                taskText.GetComponent<Text>().text = "-Click on the door.";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[12].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[12].title;
                //NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[11].buttons[11].text;
                NWindow.SetActive(true);
                backgroundControl(false);
                pause = true;

            }
            else if (counter == 13)
            {
                // open the door
                taskText.GetComponent<Text>().text = "-Go into the house and close the door.";
                NWindow.SetActive(true);
                backgroundControl(false);
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[13].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[13].title;
                //NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[11].buttons[11].text;

                pause = true;
            }
            else if (counter == 14)
            {
                // open the door
                taskText.GetComponent<Text>().text = "-Go upstairs and find a towel. You may walk upstairs or press ESC to use the Teleport function.";
                NWindow.SetActive(true);
                backgroundControl(false);
                GameObject.Find("cuteTowel_upstairs").tag = "SelectForNotDanger";
                GameObject.Find("dotTowel_upstairs").tag = "SelectForNotDanger";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[14].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[14].title;
                //NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[11].buttons[11].text;

                pause = true;
            }
            else if (counter == 15)
            {
                // open the door
                NWindow.SetActive(true);
                backgroundControl(false);
                taskText.GetComponent<Text>().text = "-Go to the bedroom on this floor and find a lamp.";
                GameObject.Find("lamp2").tag = "SelectForNotDanger";
                GameObject.Find("lamp3").tag = "SelectForNotDanger";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[15].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[15].title;
                //NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[11].buttons[11].text;

                pause = true;
            }
            else if (counter == 16)
            {   // Good job
                NWindow.SetActive(true);
                backgroundControl(false);
                taskText.GetComponent<Text>().text = "-Go to the bathroom and find a toothbrush.";
                GameObject.Find("toothCeaning").tag = "SelectForNotDanger";
                GameObject.Find("toothBrush").tag = "SelectForNotDanger";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[16].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[16].title;
                //NButtonText.text = gameObject.GetComponent<Dialog>().trees[0].frames[17].buttons[0].text;
                pause = true;
            }
            else if (counter == 17)
            {
                // open the door
                NWindow.SetActive(true);
                backgroundControl(false);
                taskText.GetComponent<Text>().text = "-Go to the basement and find a washing machine.";
                GameObject.Find("washer").tag = "SelectForNotDanger";
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[17].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[17].title;
                pause = true;
            }
            else if (counter == 18)
            {   // Good job
                taskWindow.SetActive(false);
                NWindow.SetActive(true);
                backgroundControl(false);
                NText.text = gameObject.GetComponent<Dialog>().trees[0].frames[18].text;
                NTitle.text = gameObject.GetComponent<Dialog>().trees[0].frames[18].title;
                pause = true;
            }
            locked = true;
        }

    }
    public void ButtonInput(int id)
    {
        if (Tutorial.menuToTutorial && Tutorial.menuToTutorial)
		{
			if (id == 0)
            {
                if (counter == 3 || counter == 4 || counter == 5 || counter == 8 || counter == 12 || counter == 13 || counter == 14 || counter == 15 || counter == 16 || counter == 17)
                {
                    NWindow.SetActive(false);
                    backgroundControl(true);
  
                }
                else if (counter == 11) {
                    NWindow.SetActive(false);
                    backgroundControl(true);
                    if (toolactive && (magUsed !=2 || flashlightUsed !=2))
                    {
                        toolWindow.SetActive(true); 
                        backgroundControl(false);
                    }

                }
                else if (counter == 7)
                {
                    MC4Window.SetActive(false);
                    pause = false;
                }
                else if (counter == 18)
                {
                    tutorialCamera.drawCrosshair = false;
                    GameObject.Find("fader").GetComponent<CanvasGroup>().DOFade(1, 1f).OnComplete(backtomenu);
                }
                else
                    pause = false;
                

				if (counter == 6 || counter == 9) {
					pause = false;
					answeredYes = true;
				}
			}
			if (id == 1) {
				if (counter == 1) {
					pause = false;
				}
				if (counter == 6 || counter == 9) {

					pause = false;
					answeredYes = false;
				}
			}
			if (id == 2) {
                if (counter == 1)
                {
                    answeredQuit = true;
                }

			}
			if(id == 3)
			{
				if(counter == 10)
				{pause = false;}

			}
		}

        if (!Tutorial.menuToTutorial)
        {
            if (id == 0)
            {
                if (counter == 0)
                {
                    if (SceneManager.GetActiveScene().name.Contains("environmental") || SceneManager.GetActiveScene().name.Contains("accessment"))//has only 1 page
                    {
                        endIntro();
                    }
                    else
                    {
                        NText.text = gameObject.GetComponent<Dialog>().trees[1].frames[1].text;
                        NTitle.text = gameObject.GetComponent<Dialog>().trees[1].frames[1].title;
                        NButtonText.text = gameObject.GetComponent<Dialog>().trees[1].frames[1].buttons[0].text;
                        counter = 18;
                    }
                }
                else if (counter == 18)
                {
                    if (SceneManager.GetActiveScene().name.Contains("electricalFireHazards2") ||                      
                        SceneManager.GetActiveScene().name.Contains("slipTripLift"))
                            endIntro();//has only 2 pages
                }
                else if (counter == 19)
                {
                    endIntro();
                }
            }
        }
	} 

    void pageThree()
    {
        NText.text = gameObject.GetComponent<Dialog>().trees[1].frames[2].text;
        NTitle.text = gameObject.GetComponent<Dialog>().trees[1].frames[2].title;
        NButtonText.text = gameObject.GetComponent<Dialog>().trees[1].frames[2].buttons[0].text;
        counter = 19;
    }

    void endIntro()
    {
        if(!SceneManager.GetActiveScene().name.Equals("accessment"))
        { 
            RoomTrigger.isIntroDone = true;
            GameObject frontYardTrigger = GameObject.Find("Front Yard Trigger");
            frontYardTrigger.SetActive(false);
            frontYardTrigger.SetActive(true);//to trigger roomtrigger script
        }
        NWindow.SetActive(false);

        backgroundControl(true);

        pause = false;
        gameObject.SetActive(false);
        counter = 20;

        taskText.GetComponent<Text>().text = "-Movement is controlled by the W, A, S, and D keys or the ↑, ←, ↓, and → keys.\n-Click on the items you think are the hazards. \n-Press ESC for help.";

//      taskText.GetComponent<Text>().text = "-Movement is controlled by the W, A, S, and D keys or the arrow keys.\n-Click on the items you think are the hazards. \n-Press ESC for help.";
        taskWindow.SetActive(true);
        Invoke("turnOffTask2", 5f);
        gameObject.SetActive(false);
        enabled = false;
        
    }

}