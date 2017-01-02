using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.IO;

public class manuBtns : MonoBehaviour {
    public GameObject fader;
    public GameObject loading;
    public GameObject menuWindow;
    public Button assessmentButton;
    public GameObject creditWindow;
    public GameObject overviewWindow;
    public GameObject userIDWindow;
    public GameObject warning;
    public Text userID;
    public Text creditText;
    public GameObject scoreWindow;
    bool exit = false;
    bool readyToQuit = false;
    int fakeNumber=0;
    AsyncOperation async;

    bool passTest=true;

    public void closeUserID() {
      
            char[] letters = new char[3];
            char[] numbers = new char[3];
            StringReader sr = new StringReader(userID.text);
            sr.Read(letters, 0, 3);
            sr.Read(numbers, 0, 3);
            
            foreach (char i in letters)
                if (!char.IsLetter(i)) passTest = false;
            foreach (char i in numbers)
                if (!char.IsNumber(i)) passTest = false; 
            if (userID.text.Length != 6) passTest = false;

        if(!passTest) { warning.SetActive(true); passTest = true; }
        else
        {
            ObjectTracker.userID = userID.text;
            userIDWindow.SetActive(false);

            gameLoader.loadData();

            if (ObjectTracker.crashTrack.Equals(2)) {
                ObjectTracker.trackModuleTime("electricalFireHazards2", ObjectTracker.moduleTime_temp);
                forEFModule();
            }
            else if (ObjectTracker.crashTrack.Equals(3))
            {
                ObjectTracker.trackModuleTime("slipTripLift", ObjectTracker.moduleTime_temp);
                forSTFModule();
            } //stl
            else if (ObjectTracker.crashTrack.Equals(4)) {
                ObjectTracker.trackModuleTime("environmental", ObjectTracker.moduleTime_temp);
                forBHModule(); 
            }//env
            else if (ObjectTracker.crashTrack.Equals(5)) {
                ObjectTracker.trackModuleTime("accessment", ObjectTracker.moduleTime_temp);
                forAssessment(); 
            }//ass 
            
            if (ObjectTracker.assessmentLock)
            {
                GameObject.Find("AssessmentTxt").GetComponent<Text>().text = "Assessment Module Score Check";
            }

        }

    }

    void fadeIn()
    {
        fader.SetActive(true);
        fader.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        fader.GetComponent<CanvasGroup>().blocksRaycasts = false;
       // loading.GetComponent<Text>().text = "Loading... 0%";
    }

    void fadeOutTo(TweenCallback tweener)
    {
        fader.GetComponent<CanvasGroup>().blocksRaycasts = true;
        fader.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(tweener);
    }


    void OnApplicationQuit()
    {
        ObjectTracker.printScore();
        gameLoader.saveData();
    }

    private void SyncLoadLevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        async.allowSceneActivation = false;
        Load();
    }
    void checkScore()
    {
        fadeIn();
        if (ObjectTracker.assessmentLock)
        {
            ObjectTracker.scoreVisited = true;
            menuWindow.SetActive(false);
            scoreWindow.SetActive(true);
            float WhyPercentage, WTDPercentage;

            if (ObjectTracker.WhyTotal == 0) WhyPercentage = 0;
            else WhyPercentage = (ObjectTracker.WhyCorrect / ObjectTracker.WhyTotal) * 100;

            if (ObjectTracker.WTDTotal == 0) { WTDPercentage = 0; }
            else WTDPercentage = (ObjectTracker.WTDCorrect / ObjectTracker.WTDTotal) * 100;

            float TotalPercentage;
            if (ObjectTracker.moduleHazardTotal_ass == 0)
                TotalPercentage = 0;
            else
                TotalPercentage = (ObjectTracker.moduleHazardFound_ass / ObjectTracker.moduleHazardTotal_ass) * 100;

            string comment = "";

            float forComment = (ObjectTracker.WhyCorrect + ObjectTracker.WTDCorrect) / (ObjectTracker.WhyTotal + ObjectTracker.WTDTotal) * 100;
            if (TotalPercentage >= 90 && forComment >= 90) comment = "Excellent!";
            else if (TotalPercentage >=80 && forComment >= 80) comment = "Good job!";
            else comment = "Good try – please consider reviewing the training modules to increase your comfort with this safety information.";


            GameObject.Find("scoreText").GetComponent<Text>().text =
                "You correctly found " + string.Format("{0:0.#}", TotalPercentage) + "% of the hazards.\n"
                + "You correctly identified " + string.Format("{0:0.#}", WhyPercentage) + "% of reasons why it is a hazard.\n"
                + "You correctly identified " + string.Format("{0:0.#}", WTDPercentage) + "% of what actions to take for the hazards.\n" + comment;
        }
        /*
        else if(ObjectTracker.assessmentLock)
        {
            menuWindow.SetActive(false);
            scoreWindow.SetActive(true);
            GameObject.Find("scoreText").GetComponent<Text>().text = "You didn't really play the game. Please contact the research agency for help.";
            ObjectTracker.scoreVisited = false;
            ObjectTracker.assessmentLock = false;
        }
        */
    }
    void Start() {

        if (ObjectTracker.userID != "")
        {
            ObjectTracker.crashTrack = SceneManager.GetActiveScene().rootCount;
            userIDWindow.SetActive(false);
        }

        if (ObjectTracker.assessmentLock && !ObjectTracker.scoreVisited) checkScore();
        else fadeIn(); 
        if (SceneManager.GetActiveScene().name == "overview") {
        }

        if (ObjectTracker.assessmentLock)
        {
            menuWindow.transform.FindChild("AssessmentBtn").transform.FindChild("AssessmentTxt").GetComponent<Text>().text = "Assessment Module Score Check";
        }

        ObjectTracker.crashTrack = 0;
        creditWindow.SetActive(false);
        creditWindow.GetComponent<CanvasGroup>().alpha = 0;

    }
    IEnumerator wait(float seconds)
    { 
        yield return new WaitForSeconds(seconds);
    }
    void Update()
    {
        if(readyToQuit) quitApp();
        if (async != null)
        {
            if (!async.isDone)
            {
                if (((int)(100 * async.progress)) < 89)
                {
                    loading.GetComponent<Text>().text = "Loading... " + ((int)(100 * async.progress)).ToString() + "%";
                }
                else if (fakeNumber < 100)
                {
                    fakeNumber = (int)(100 * async.progress + Time.time * 3);
                    loading.GetComponent<Text>().text = "Loading... " + fakeNumber.ToString() + "%";
                }
                else if (fakeNumber >= 100)
                {
                    loading.GetComponent<Text>().text = "Loading... " + "100%";
                    async.allowSceneActivation = true;
                }
            }
            else
            {
                async.allowSceneActivation = true;
            }
        }

      

        if (exit){
            if (creditText.rectTransform.position.y > -200f) exit = false;
            readyToQuit = true;
        }
    }

    void LateUpdate() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    IEnumerator Load()
    {
       // if ((int)(100 * async.progress) != 90)
            yield return async;
       // else
         //   yield return null;
    }


    void quitApp()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            //#elif UNITY_WEBPLAYER
            //Application.OpenURL(webplayerQuitURL);
        #else
                    Application.Quit();
        #endif
        }
    }

    //backtoMenu
    public void backtoMenu()
    {
        fadeOutTo(toMenu);
    }

    void toMenu() {
        scoreWindow.SetActive(false);
        menuWindow.SetActive(true);
        fadeIn();
    }

    //Exit
    public void forExit()
    {
        if (userIDWindow.activeSelf) userIDWindow.SetActive(false);
        if (scoreWindow.activeSelf) scoreWindow.SetActive(false);
        fadeOutTo(LoadExit);
        creditWindow.SetActive(true);
        menuWindow.SetActive(false);
        GameObject.Find("bg").SetActive(false);
        exit = true;
    }
    void LoadExit()
    {
        creditWindow.GetComponent<CanvasGroup>().alpha = 1;
        fadeIn();
    }

    public void forOverview()
    {
        loading.GetComponent<Text>().text = "Loading... 0%";
        Debug.Log("Overview");
        fadeOutTo(LoadOverview);
    }

    void LoadOverview()
    {
        SyncLoadLevel("overview");
    }

    //Tutorial
    public void forTutorial()
    {
        loading.GetComponent<Text>().text = "Loading... 0%";
        Debug.Log("Tutorial");
        Tutorial.menuToTutorial = true;
        Tutorial.menuToWalkThrough = false;
        fadeOutTo(LoadEF);
    }

    //Walk Through
    public void forWalkThrough()
    {
        loading.GetComponent<Text>().text = "Loading... 0%";
        Tutorial.menuToTutorial = false;
        Tutorial.menuToWalkThrough = true;
        fadeOutTo(LoadEF);
    }
 

    //Module 1
    public void forEFModule()
    {
        loading.GetComponent<Text>().text = "Loading... 0%";
        Debug.Log("EF");
        Tutorial.menuToTutorial = false;
        Tutorial.menuToWalkThrough = false;
        fadeOutTo(LoadEF);     
    }
    void LoadEF()
    {
        SyncLoadLevel("electricalFireHazards2");
    }

    //Module 2
    public void forSTFModule()
    {
        loading.GetComponent<Text>().text = "Loading... 0%";
        Tutorial.menuToTutorial = false;
        Tutorial.menuToWalkThrough = false;
        fadeOutTo(LoadSTF);
    }
    void LoadSTF()
    {
        SyncLoadLevel("slipTripLift");
    }

    //Module 3
    public void forBHModule()
    {
        loading.GetComponent<Text>().text = "Loading... 0%";
        Tutorial.menuToTutorial = false;
        Tutorial.menuToWalkThrough = false;
        fadeOutTo(LoadBH);
    }
    void LoadBH()
    {
        SyncLoadLevel("environmental");
    }

    //Module 4
    public void forAssessment()
    {
        if (ObjectTracker.assessmentLock)
            fadeOutTo(checkScore);

        else if (!ObjectTracker.assessmentLock)
        {
            loading.GetComponent<Text>().text = "Loading... 0%";
            Debug.Log("AS");
            Tutorial.menuToTutorial = false;
            Tutorial.menuToWalkThrough = false;
            fadeOutTo(LoadAssessment);
        }
    }

    void LoadAssessment()
    {
        SyncLoadLevel("accessment");
    }
}
