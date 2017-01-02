using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class menuOVRSwap : MonoBehaviour {

    public bool _UsingOVR;
    public Camera mainCamera;
    public GameObject OVRCameraRig;
    public GameObject menuCanvas;
    public GameObject OVRCenter;
    public static bool UsingOVR;
    public Image canvasBackground;
    public Image fader;
    public GameObject OriginalEventSystem;
    public GameObject OVRGUIEventSystem;
    public GameObject creditWindow;
    public GameObject creditWindowTxt;
    public GameObject MainCanvas;
   
    void Awake() {
        UsingOVR = _UsingOVR;
		if (_UsingOVR)
			Tutorial.menuToOvr = true;
    }

	// Use this for initialization
	void Start () {
        swapForOVR();

	}
	
	// Update is called once per frame
    void swapForOVR()
    {
        if (UsingOVR)
        {
            Cursor.lockState = CursorLockMode.Confined;
            mainCamera.enabled = false;
            OVRCameraRig.SetActive(true);
            MainCanvas.transform.SetParent(null);
            MainCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            menuCanvas.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //menuCanvas.transform.localPosition = new Vector3(0.24f, -3f, 320f);
            canvasBackground.transform.localScale = new Vector3(3.15f, 2.06f, 3.2f);
            creditWindow.transform.localScale = new Vector3(1.9f, 1.9f, 1.9f);
            creditWindowTxt.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            fader.transform.localScale = new Vector3(1.9f, 1.9f, 1.9f);
            OriginalEventSystem.SetActive(false);
            OVRGUIEventSystem.SetActive(true);
        }
        else {
            OVRCameraRig.SetActive(false);
            MainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }
	}

}
