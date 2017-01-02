using UnityEngine;
using System.Collections;
using UnityEngine.VR;

#pragma warning disable 0436 // TODO: Clean up later

[RequireComponent(typeof(DialogViewer))]
[RequireComponent(typeof(Intersense))]
public class GameManager : MonoBehaviour {
    public GameObject player, mainCamera, cameraController;
    public GameObject OVRCameraRig;
    public bool usingOVR, gameOver, practiceMode;
    public bool useIntersense, walkThrough;
    public GameObject trackspace;
    public GameObject _OriginalEventSystem;
    public GameObject _OVRGUIEventSystem;
	public GameObject tourcam;
    public performanceControl _performanceControl;

    public static GameObject Player;
    public static bool UsingOVR, GameOver, PracticeMode, UsingIntersense;
    public static GameObject MainCamera;
    public static GameObject CameraController;
    public static Intersense Intersense;
    public static DialogViewer DialogViewer;
    public static GameObject Trackspace;
    public static GameObject OriginalEventSystem;
    public static GameObject OVRGUIEventSystem;
    public static performanceControl performanceControl;

    public GameObject walkThroughCanvas;
    void Awake()
    {

        performanceControl = _performanceControl;
        OVRGUIEventSystem = _OVRGUIEventSystem;
        OriginalEventSystem = _OriginalEventSystem;
        Player = player;
        UsingIntersense = useIntersense;
       	UsingOVR = usingOVR;
        GameOver = gameOver;
        PracticeMode = practiceMode;
        MainCamera = mainCamera;
        Trackspace = trackspace;
        CameraController = cameraController;
        Intersense = this.GetComponent<Intersense>();
        DialogViewer = this.GetComponent<DialogViewer>();
        GameObject.Find("dryerLid01_dr").transform.localEulerAngles = Vector3.zero;
		switchOVRSettings();

    }


    void switchOVRSettings() 
	{
        if (usingOVR)
        {
            if (Tutorial.menuToWalkThrough)
            {
                if (!OVRCameraRig.activeSelf) OVRCameraRig.SetActive(true);
                OVRCameraRig.transform.SetParent(tourcam.transform);
                OVRCameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
                GameManager.DialogViewer.finger.SetActive(false);

                if (walkThroughCanvas.GetComponent<Canvas>().renderMode != RenderMode.WorldSpace)
                {
                    walkThroughCanvas.transform.SetParent(tourcam.transform);
                    walkThroughCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                }
                MainCamera.GetComponent<Camera>().enabled = false;
                player.GetComponent<CharacterMotor>().enabled = false;
            }
            else
            {   
                MainCamera.GetComponent<Camera>().enabled = false;
                OVRCameraRig.SetActive(true);
                DialogViewer.dialogCanvas.transform.SetParent(null); 
                DialogViewer.dialogCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                player.GetComponent<CharacterMotor>().enabled = false;
                player.GetComponent<MouseLook_1>().enabled = false;
                player.GetComponent<OVRPlayerController>().enabled = true;

                GameManager.OriginalEventSystem.SetActive(false);
                GameManager.OVRGUIEventSystem.SetActive(true);


            }

        }
        else {

            if (OVRCameraRig.activeSelf) OVRCameraRig.SetActive(false);

            if (Tutorial.menuToWalkThrough)
            {
                walkThroughCanvas.transform.SetParent(null);
                DialogViewer.dialogCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                walkThroughCanvas.gameObject.SetActive(true);
               
                DialogViewer.dialogCanvas.SetActive(false);
                
            }
            else
            {
                DialogViewer.dialogCanvas.transform.SetParent(null);
                DialogViewer.dialogCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                MainCamera.GetComponent<Camera>().enabled = true;
                player.GetComponent<CharacterMotor>().enabled = true;
            }


            
            GameManager.OVRGUIEventSystem.SetActive( false);
            GameManager.OriginalEventSystem.SetActive(true);


        }

        
	}
}


