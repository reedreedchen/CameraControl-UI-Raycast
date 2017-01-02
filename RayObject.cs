using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

#pragma warning disable 0414 // unused variables
#pragma warning disable 0219 // unused variables


public class RayObject : MonoBehaviour
{   public List<GameObject> subObj = new List<GameObject>();

    GameObject focusHolder;
    bool teleporting;
    List<GameObject> locationCubes = new List<GameObject>();
    List<Vector3> locationCubes_iniPos = new List<Vector3>();
    public GameObject fallingObjects;
    public RaycastHit rayInfo;
    public GameObject hovered;
    public float rayCastLength;

 
    public GameObject oldObject;

    public List<GameObject> correct;
    public List<GameObject> selected;

    public AudioClip lightFlick;
    public AudioClip doorOpen;
    public AudioClip doorClose;

    public GameObject OVRcenter;

    //cached objects
    private CameraController cameraController;

    //raycasting variables
    // public Vector3 ovrOffset = new Vector3(0.59f, 0.49f, 0);
    public float ovrRayLength = 600f;

    private bool filled = false;
    private bool fire1 = false;
    
    public Renderer lampCover, curtain;
    public Material lampCoverOnFire, curtainOnFire;
    public GameObject lampCoverObject, curtainObject;


    public string stringTitle = "";

    private SpeechInput speech;
    private ItemPickup itemPickup;
    private ISenseCursor isenseCursor;
    private bool selectDoor, selectLight, selectObject, selectDrawer, selectWindow, selectCurtain;
    ParticleSystem waterParticle;
    ParticleSystem waterParticle_snd;
    public Text mc4Text;
    public GameObject lampfire, lampSmoke, lampBigFire, curtainBigFire;
    public GameObject curtainfire, curtainSmoke;
    public bool curtainFireOn, lampFireOn;
    ParticleSystem.EmissionModule waterParticleEM;
    ParticleSystem.EmissionModule waterParticle_snd_EM;
    public Material curtainMoldy;

    GameObject fader;
    GameObject sunFlare;
    GameObject freezerDoor;
    GameObject fridgeDoor;
    GameObject newHover;
    GameObject highlightingObj = null;

    bool fridgeDialog;
    void Awake() {
        selected = new List<GameObject>();
        correct = new List<GameObject>();
    }

    void Start()
    {
        fridgeDialog = false;
         freezerDoor = GameObject.Find("freezerDoor01_dr_reversed");
         fridgeDoor = GameObject.Find("fridgeDoor01_dr_reversed");
        focusHolder = GameObject.Find("focusPosition");
        
        GameObject locationCubes_grp = GameObject.Find("locationCubes");

        for (int i = 0; i <= locationCubes_grp.transform.childCount-1; i++)
        {
            locationCubes.Add(locationCubes_grp.transform.GetChild(i).gameObject);
            locationCubes_iniPos.Add(locationCubes_grp.transform.GetChild(i).gameObject.transform.position);
        }

        fader = GameManager.DialogViewer.fader;
        if (fallingObjects != null) {
            for (int i=0; i < fallingObjects.transform.childCount; i++)
                fallingObjects.transform.GetChild(i).gameObject.SetActive(false);
        }
        //use hashset for hashed searches O(1) compared to O(n) list/array search 



        cameraController = GameManager.MainCamera.GetComponent<CameraController>();
        itemPickup = GameManager.MainCamera.GetComponent<ItemPickup>();
        speech = GameManager.MainCamera.GetComponent<SpeechInput>();
        isenseCursor = GameManager.MainCamera.GetComponent<ISenseCursor>();
        

        waterParticleEM = GameObject.Find("waterParticle").GetComponent<ParticleSystem>().emission;
        waterParticle_snd_EM = GameObject.Find("waterParticle_snd").GetComponent<ParticleSystem>().emission;

        waterParticleEM.enabled = false;
        waterParticle_snd_EM.enabled = false;

        sunFlare = GameObject.Find("sunFlare");
    }

    void Update()
    {

            shootRay();

        if (SceneManager.GetActiveScene().name == "electricalFireHazards2") {
            if(GameManager.MainCamera.GetComponent<CameraController>().currentRoom == "Front Yard") sunFlare.SetActive(true);
            else sunFlare.SetActive(false);
        }
        
    }     
    float speed = 0.005f;

    void shootRay()
    {     
        //Raycast for each camera type
        if (GameManager.UsingOVR)
        {
            Debug.DrawRay(OVRcenter.transform.position, OVRcenter.transform.forward, Color.red);
            Physics.Raycast(OVRcenter.transform.position, OVRcenter.transform.forward, out rayInfo, rayCastLength);

            if (GameManager.DialogViewer.isDialogActive) {
                if (rayInfo.rigidbody != null && rayInfo.rigidbody.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                   Physics.Raycast(OVRcenter.transform.position, OVRcenter.transform.forward, out rayInfo, 0f);
                }
            }
        }
        else if (GameManager.UsingIntersense)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(isenseCursor.xPos, isenseCursor.yPos, 0));
            Physics.Raycast(ray, out rayInfo);
        }
        else
        {
            Physics.Raycast(transform.position, transform.forward, out rayInfo, rayCastLength);
            
            if (GameManager.DialogViewer.isDialogActive)
            {
                if (rayInfo.rigidbody != null && rayInfo.rigidbody.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                    Physics.Raycast(transform.position, transform.forward, out rayInfo, 0);
                }

            }
        }

        if (rayInfo.rigidbody == null) { newHover = null;  }
        else newHover = rayInfo.rigidbody.gameObject;
        stringTitle = "";

        if (GameManager.MainCamera.GetComponent<CameraController>().stringTitleText.text!=stringTitle)
            GameManager.MainCamera.GetComponent<CameraController>().stringTitleText.text = stringTitle;

        if ((newHover == null || newHover != highlightingObj) && filled && !oldObject.name.Contains("locationCube") && !oldObject.name.Contains("_dr") && !oldObject.name.Contains("indow") && !oldObject.name.Contains("lightSwitch") && !oldObject.name.Contains("curtain"))
        {
            highlightingObj = null;
            stringTitle = "";

            for (int i = 0; i < subObj.Count; i++)
            {
                for (int j = 0; j < subObj[i].GetComponent<Renderer>().materials.Length; j++)
                {
                    Material subObjMaterial = subObj[i].GetComponent<Renderer>().materials[j];
                 //this part recovers non-what and transparent RGBA 
                    if (subObjMaterial.name.Contains("puddle")) subObjMaterial.color = new Color(1f, 1f, 1f, 105 / 255f);
                    else if (subObjMaterial.name.Contains("grease")) subObjMaterial.color = new Color(1f, 245 / 255f, 107 / 255f, 0.49f);
                    else if (subObjMaterial.name.Contains("thermostateGlass")) subObjMaterial.color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 0f);
                    else if (subObjMaterial.name.Contains("waterHeaterGuageGlass")) subObjMaterial.color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 115/255f);
                    else if (subObjMaterial.name.Contains("bathTubGlass")) subObjMaterial.color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 174 / 255f);
                    else subObjMaterial.color = Color.white;
                //this part recovers non-zero emission
                    if (subObjMaterial.name.Contains("lightGlass")) noEmissionColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    else if (subObjMaterial.name.Contains("lampColor1")) noEmissionColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    else if (subObjMaterial.name.Contains("bulbGlass")) noEmissionColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                    subObjMaterial.SetColor("_EmissionColor", noEmissionColor);

                    noEmissionColor = new Color(0f, 0f, 0f, 1f);

                }
            }
        }
        
        //check for ray collision
        if (rayInfo.rigidbody != null && newHover != highlightingObj)
        {            
            oldObject = rayInfo.rigidbody.gameObject;
            hovered = oldObject; // TODO: Gauge the necessity of this line

            filled = true;
            fire1 = Input.GetButtonDown("Fire1") || GameManager.Intersense.blackPressedDown;

            if (hovered.tag.Equals("SelectForDanger"))
            {
                    placeInObjectArray();
                    GameManager.DialogViewer.dialogSwitch = true;
            }
            else if (hovered.tag.Equals("SelectForNotDanger"))
            {
                    placeInObjectArray();
                    GameManager.DialogViewer.dialogSwitch = true;
            }
            else if (hovered.tag.Equals("SelectToSwitchLight"))
            {
                askToSwitchLight();
            }
            else if (hovered.tag.Equals("SelectToOpenLeft"))
            {
                if (hovered.name.Contains("bathSinkFaucetPart"))
                {
                    stringTitle = "Turn On Water";
                }
                else if (hovered.name.Contains("reversed"))
                {
                    stringTitle = "Close";
                }
                else
                {
                    stringTitle = "Open";
                }
                askToOpenLeft();
            }
            else if (hovered.tag.Equals("SelectToOpenRight"))
            {
                if (hovered.name.Contains("bathSinkFaucetPart"))
                {
                    stringTitle = "Turn Off Water";
                }
                else if (hovered.name.Contains("reversed"))
                {
                    stringTitle = "Open";
                }
                else
                {
                    stringTitle = "Close";
                };
                askToOpenRight();
            }
            else if (hovered.tag.Equals("SelectToOpenDown"))
            {
                if (hovered.name.Contains("reversed"))
                {
                    stringTitle = "Close";
                }
                else
                {
                    stringTitle = "Open";
                }
                askToOpenDown();
            }
            else if (hovered.tag.Equals("SelectToOpenUp"))
            {

                if (hovered.name.Contains("reversed"))
                {
                    stringTitle = "Open";
                }
                else
                {
                    stringTitle = "Close";
                }
                askToOpenUp();
            }
            else if (hovered.tag.Equals("SelectToPull"))
            {
                askToPull();
            }
            else if (hovered.tag.Equals("SelectToPush"))
            {
                askToPush();
            }
            else if (hovered.tag.Equals("SelectToOpenWindow"))
            {
                stringTitle = "Open";
                askToOpenWindow();
            }
            else if (hovered.tag.Equals("SelectToCloseWindow"))
            {
                stringTitle = "Close";
                askToCloseWindow();
            }
            else if (hovered.tag.Equals("SelectToCloseCurtain"))
            {
                stringTitle = "Close";
                askToCloseCurtain();
            }
            else if (hovered.tag.Equals("SelectToOpenCurtain"))
            {
                stringTitle = "Open";
                askToOpenCurtain();
            }
            else
            {
                filled = false;
            }
            if (GameManager.MainCamera.GetComponent<CameraController>().stringTitleText.text != stringTitle)
                GameManager.MainCamera.GetComponent<CameraController>().stringTitleText.text = stringTitle;
        }
    }

    public void setFire()
    {
        if (oldObject.name.Contains("CandleSet") && !lampFireOn)
        {
            ParticleSystem.EmissionModule lampBigFireEM = lampBigFire.GetComponent<ParticleSystem>().emission;
            lampBigFireEM.enabled = true;
            showMessage.showError("The lamp cover is on fire.");
            playSound.playTheSoundForCamera(playSound.bomb, playSound.soundVol);
            playSound.playTheSoundForCamera(playSound.drop, playSound.soundVol);
            Invoke("closeError", 2f);
            Invoke("triggerLampSmoke", 0.5f);
        }
        else if (oldObject.name.Equals("spaceHeater_forfire") && !curtainFireOn)
        {
            ParticleSystem.EmissionModule curtainBigFireEM = curtainBigFire.GetComponent<ParticleSystem>().emission;
            curtainBigFireEM.enabled = true;
            showMessage.showError("The curtain is on fire.");
            playSound.playTheSoundForCamera(playSound.bomb, playSound.soundVol);
            Invoke("closeError", 2f);
            Invoke("triggerCurtainSmoke", 0.5f);
            curtainObject.tag = "Untagged";
        }
    }

    void triggerLampSmoke()
    {
        ParticleSystem.EmissionModule lampBigFireEM = lampBigFire.GetComponent<ParticleSystem>().emission;
        lampBigFireEM.enabled = false;

        for (int i = 0; i < lampfire.transform.childCount; i++)
        {
            ParticleSystem.EmissionModule lampFireEM = lampfire.transform.GetChild(i).GetComponent<ParticleSystem>().emission;
            lampFireEM.enabled = true;
        }

        ParticleSystem.EmissionModule lampSmokeEM = lampSmoke.GetComponent<ParticleSystem>().emission;
        lampSmokeEM.enabled = true;

        lampCover.material = lampCoverOnFire;
        lampCoverObject.GetComponent<Rigidbody>().useGravity = true;
        lampFireOn = true;
    }

    void triggerCurtainSmoke()
    {
        ParticleSystem.EmissionModule curtainBigFireEM = curtainBigFire.GetComponent<ParticleSystem>().emission;
        curtainBigFireEM.enabled = false;
        for (int i = 0; i < curtainfire.transform.childCount; i++)
        {
            ParticleSystem.EmissionModule curtainfireEM = curtainfire.transform.GetChild(i).GetComponent<ParticleSystem>().emission;
            curtainfireEM.enabled = true;
        }
        ParticleSystem.EmissionModule curtainSmokeEM=curtainSmoke.GetComponent<ParticleSystem>().emission;
        curtainSmokeEM.enabled = true;
        curtain.material = curtainOnFire;
        curtainFireOn=true;
    }

    void closeError()
    {
        showMessage.closeError();
    }
    /* 
     * Highlight the object with the given material.
     * An object must have a renderer, or one of
     * its child objects must have a renderer.
     */

    public Color highlightEmissionColor = new Color(0.3f,0.3f, 0.3f, 0.3f);
    public Color noEmissionColor = new Color(0f, 0f, 0f, 1f);
    public void Highlight(GameObject x)
    {
        if (highlightingObj != x)
        {
            highlightingObj = x;
            subObj = new List<GameObject>(childGameObjects(x));
            //objList.oldMatsColorList = new List<List<Color>>();

            foreach (GameObject o in subObj)
            {
                List<Color> materialColorList = new List<Color>();

                int numMaterials = o.GetComponent<Renderer>().materials.Length;
                for (int i = 0; i < numMaterials; i++)
                {
                    materialColorList.Add(o.GetComponent<Renderer>().materials[i].color);
                }

                foreach (Material m in o.GetComponent<Renderer>().materials)
                {
                    m.color = Color.yellow;
                    m.SetColor("_EmissionColor", highlightEmissionColor);
                }

                //objList.oldMatsColorList.Add(materialColorList);
            }
        }
    }

    /**
     * For this to work - all of the child objects
     * must have textures (obviously).
     */
    public List<GameObject> childGameObjects(GameObject x)
    {
        List<GameObject> array = new List<GameObject>();

        Transform[] tr = x.GetComponentsInChildren<Transform>();

        foreach (Transform child in tr)
            if (child.gameObject.GetComponent<Renderer>() != null
            && child.gameObject.GetComponent<Renderer>().material.mainTexture != null)
                array.Add(child.gameObject);

        return array;
    }

    /* 
     * Place the object into the selected array, since it has been selected
     */
    public void placeInObjectArray()
    {

            if (!hovered.name.Contains("_dr") && !hovered.name.Contains("locationCube") 
                && SceneManager.GetActiveScene().name.Equals("accessment") && !selected.Contains(oldObject))
            {
                Highlight(oldObject);
            }
            else if (!hovered.name.Contains("_dr") && !hovered.name.Contains("locationCube") && !SceneManager.GetActiveScene().name.Equals("accessment"))
            {
                Highlight(oldObject);
            }


            if (fire1 || selectObject)
            {
                selectObject = false;
            }
        
     
    }

    /* 
     * Switch the light on/off
     */
    void askToSwitchLight()
    {
        if (rayInfo.rigidbody.gameObject.GetComponent<LightSwitch>().lightOn)
        {
            stringTitle = "Turn Off Light";
        }
        if (!rayInfo.rigidbody.gameObject.GetComponent<LightSwitch>().lightOn)
            stringTitle = "Turn On Light";
        // Highlight(oldObject, hoverToon);

        if (fire1 || selectLight)
        {
            AudioSource.PlayClipAtPoint(lightFlick, this.gameObject.transform.position);

            if (oldObject == null)
                Debug.Log("oldObject is null");
            else if (oldObject.GetComponent<LightSwitch>() == null)
                Debug.Log("LS is null");
            else if (hovered == null)
                Debug.Log("Hovered is null");
            else
                oldObject.GetComponent<LightSwitch>().switchLight(hovered.name);
            selectLight = false;
        }
    }

    void bathRoomDoorDialog()
    {
        if (SceneManager.GetActiveScene().name.Equals("slipTripLift"))
        {
            GameObject.Find("tubGlass").GetComponent<BoxCollider>().enabled = false;
            GameObject.Find("bathtubGlassGlass_r01").tag = "SelectToOpenWindow";
        }
    }
    void shutCurtainDialog()
    {
        if (SceneManager.GetActiveScene().name.Equals("environmental") || SceneManager.GetActiveScene().name.Equals("accessment"))
        {
            GameObject.Find("moldingShowerCurtain").GetComponent<BoxCollider>().enabled = false;
            GameObject.Find("curtainDownstairs").tag = "SelectToOpenCurtain";
            GameObject.Find("curtainDownstairs").GetComponent<BoxCollider>().size = new Vector3(120f, 120f, 2f);
        }

    }

    void shutSinkDialog()
    {
        if (SceneManager.GetActiveScene().name.Equals("accessment"))
        {
            GameObject.Find("bathroomSink_upstairs").GetComponent<BoxCollider>().enabled = false;
        }

    }

    void shutWaterDialog()
    {
        if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
        {
            GameObject.Find("bathSinkFaucet01_hazard").GetComponent<BoxCollider>().enabled = false;
        }

    }

    void shutOvenDialog()
    {
        if (SceneManager.GetActiveScene().name.Equals("slipTripLift"))
            GameObject.Find("stoveBox").GetComponent<BoxCollider>().enabled = false;
            
    }
    void shutFridgeDialog() {
        if (SceneManager.GetActiveScene().name.Equals("slipTripLift")|| SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
        {
            fridgeDialog = false; 
            GameObject.Find("fridge").GetComponent<BoxCollider>().enabled = false;
            fridgeDoor.GetComponent<BoxCollider>().enabled = true;
            freezerDoor.GetComponent<BoxCollider>().enabled = true;
        }
    }
    void shutDishwasherDialog()
    {
        if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
        {
            GameObject.Find("dishwasherDoor01_reversed_dr").GetComponent<BoxCollider>().enabled = true;
            GameObject.Find("dishwasher").GetComponent<BoxCollider>().enabled = false;
        } else
            GameObject.Find("dishwasher").GetComponent<BoxCollider>().enabled = false;
    }

    /* 
     * Open an object (usually a door) to the left.
     */
    void askToOpenLeft()
    {
        bool canOpen = true;
        //stringTitle = "Open/Close";

        //  Highlight(oldObject, hoverToon);
        if (hovered.name.Equals("bathdoor01a_dr_reversed"))
        {
            if (GameObject.Find("bathSinkDoorDownstairs_l_01_reversed_dr").tag.Equals("SelectToOpenLeft"))
            {
                canOpen = false;
                stringTitle = "Blocked";
                if (fire1 || selectDoor)
                {
                    showMessage.showError("The closet door is in the way.");
                    Invoke("closeError", 2f);
                }
            }
        }
        else if (hovered.name.Equals("kitchenCloset_r_door03_dr"))
        {
            if (GameObject.Find("kitchenCloset_l_door02_dr_reversed").tag.Equals("SelectToOpenLeft") || GameObject.Find("kitchenCloset_l_door02_dr_reversed").tag.Equals("lockedRight"))
            {
                canOpen = false;
                stringTitle = "Blocked";
                if (fire1 || selectDoor)
                {
                    showMessage.showError("This door is blocked.");
                    Invoke("closeError", 2f);
                }
            }
        }
        else if (hovered.name.Equals("kichenCloset_r_door03_dr01"))
        {
            if (GameObject.Find("kichenCloset_l_door03_dr_reversed01").tag.Equals("SelectToOpenLeft") || GameObject.Find("kichenCloset_l_door03_dr_reversed01").tag.Equals("lockedRight"))
            {
                canOpen = false;
                stringTitle = "Blocked";
                if (fire1 || selectDoor)
                {
                    showMessage.showError("This door is blocked.");
                    Invoke("closeError", 2f);
                }
            }
        }
        else if (hovered.name == "fenceDoor" || (hovered.layer == LayerMask.NameToLayer("basement") && hovered.name == ("door01")) ||
            (hovered.layer == LayerMask.NameToLayer("kitchen") && hovered.name == ("backDoor_dr")))
        {
            canOpen = false;
            stringTitle = "Locked";
            if (fire1 || selectDoor)
            {
                showMessage.showError("This door is locked.");
                Invoke("closeError", 2f);
            }
        }
        if (canOpen)
        {
            if (fire1 || selectDoor)
            {
                if (SceneManager.GetActiveScene().name.Equals("environmental") 
                    && hovered.name.Equals("kitchenCloset_r_door03_dr") && !fallingObjects.transform.GetChild(0).gameObject.activeSelf
                    && GameObject.Find("kitchenCloset_r_door03_dr").tag.Equals("SelectToOpenLeft"))
                {
                    if (fallingObjects != null)
                    {
                        for (int i = 0; i < fallingObjects.transform.childCount; i++)
                            fallingObjects.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    fallingObjects.GetComponent<SphereCollider>().enabled = true;
                }

                if (oldObject.name.Equals("bathSinkFaucetPart_dr"))
                {
                    waterParticleEM.enabled = true;

                    GameObject.Find("waterParticle").GetComponent<AudioSource>().Play();
                    if (oldObject.transform.parent.name == "bathSinkFaucet01_hazard") oldObject.transform.parent.GetComponent<BoxCollider>().enabled = true;

                }
                else if (oldObject.name.Equals("bathSinkFaucetPart_dr_snd"))
	    	    {
                    waterParticle_snd_EM.enabled = true;

                    GameObject.Find("waterParticle_snd").GetComponent<AudioSource>().Play();
		        }
                else if (oldObject.name.Equals("porchDoor_dr_reversed"))
                {
                    AudioSource.PlayClipAtPoint(doorOpen, gameObject.transform.position);
                }
                InteractManager.AddDoorLeft(rayInfo.rigidbody.gameObject);
                selectDoor = false;
            }
        }
    }

    /* 
     * Open an object (usually a door) to the right.
     */


    void askToOpenRight()
    {
        bool canOpen = true;


        if (hovered.name.Equals("bathdoor01a_dr_reversed"))
        {
            if (GameObject.Find("bathSinkDoorDownstairs_l_01_reversed_dr").tag.Equals("SelectToOpenLeft"))
            {
                canOpen = false;
                stringTitle = "Blocked";
                if (fire1 || selectDoor)
                {
                    showMessage.showError("The closet door is in the way.");
                    Invoke("closeError", 2f);
                }
            }
        }
        else if (hovered.name.Equals("kitchenCloset_l_door02_dr_reversed"))
        {
            if (GameObject.Find("kitchenCloset_r_door03_dr").tag.Equals("SelectToOpenRight")
                || GameObject.Find("kitchenCloset_r_door03_dr").tag.Equals("lockedLeft"))
            {
                canOpen = false;
                stringTitle = "Blocked";
                if (fire1 || selectDoor)
                {
                    showMessage.showError("This door is blocked.");
                    Invoke("closeError", 2f);
                }
            }
        }
        else if (hovered.name.Equals("kichenCloset_l_door03_dr_reversed01"))
        {
            if (GameObject.Find("kichenCloset_r_door03_dr01").tag.Equals("SelectToOpenRight")
                || GameObject.Find("kichenCloset_r_door03_dr01").tag.Equals("lockedLeft"))
            {
                canOpen = false;
                stringTitle = "Blocked";
                if (fire1 || selectDoor)
                {
                    showMessage.showError("This door is blocked.");
                    Invoke("closeError", 2f);
                }
            }
        }
        else if (SceneManager.GetActiveScene().name.Equals("slipTripLift") && hovered.layer.Equals(LayerMask.NameToLayer("secondfloorBathroom")) && hovered.name.Equals("bathSinkDoor_l_02_dr_reversed"))
        {
            canOpen = false;
            stringTitle = "Blocked";
            if (fire1 || selectDoor)
            {
                showMessage.showError("This door is blocked.");
                Invoke("closeError", 2f);
            }
        }
        else if (Tutorial.menuToTutorial && hovered.name == "porchdoor_dr_reversed" && GameObject.Find("Tutorial").GetComponent<Tutorial>().counter<12)
        {
            canOpen = false;
            stringTitle = "Locked";
            if (fire1 || selectDoor)
            {
                showMessage.showError("This door is locked.");
                Invoke("closeError", 2f);
            }
        }

        if (canOpen)
        {

            if (fire1 || selectDoor) {
                if (SceneManager.GetActiveScene().name.Equals("slipTripLift") || SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
                {
                    if (hovered.name.Equals(fridgeDoor.name))//openFridge
                    {
                        if (freezerDoor.tag.Equals("SelectToOpenLeft") || freezerDoor.tag.Equals("lockedRight")) //freezer has been opend
                        {
                            fridgeDialog = true;
                        }
                    }
                    else if (hovered.name.Equals(freezerDoor.name))//opend freezer
                    {
                        if (fridgeDoor.tag.Equals("SelectToOpenLeft") || fridgeDoor.tag.Equals("lockedRight")) //fridge has been opend
                        {
                            fridgeDialog = true;
                        }
                    }
                }

                if (oldObject.name.Equals("bathSinkFaucetPart_dr"))
                {
                    
                    waterParticleEM.enabled = false;
                    GameObject.Find("waterParticle").GetComponent<AudioSource>().Stop();

			    }else  if (oldObject.name.Equals("bathSinkFaucetPart_dr_snd"))
			    {
                    
                    waterParticle_snd_EM.enabled = false;
                    GameObject.Find("waterParticle_snd").GetComponent<AudioSource>().Stop();
			    }
                   InteractManager.AddDoorRight(rayInfo.rigidbody.gameObject);
                   selectDoor = false;
                if (fridgeDialog)
                {
                    freezerDoor.GetComponent<BoxCollider>().enabled = false;
                    fridgeDoor.GetComponent<BoxCollider>().enabled = false;
                    GameObject.Find("fridge").GetComponent<BoxCollider>().enabled = true;
                }
            }
        }
    }

    // stringTitle = "Open/Close";

    // Highlight(oldObject, hoverToon);




    /* 
     * Open an object (usually a door) in an upward direction.
     */
    void askToOpenUp()
    {
        //  stringTitle = "Open/Close";

        //   Highlight(oldObject, hoverToon);

        if (fire1 || selectDoor)
        {
            InteractManager.AddDoorUp(rayInfo.rigidbody.gameObject);

            selectDoor = false;
            if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
            {
                GameObject.Find("dishwasher").GetComponent<BoxCollider>().enabled = false;
                GameObject.Find("dishwasherDoor01_reversed_dr").GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    /* 
     * Open an object (usually a door) in a downward direction.
     */
    void askToOpenDown()
    {
        // stringTitle = "Open/Close";

        //  Highlight(oldObject, hoverToon);

        if (fire1 || selectDoor)
        {
            InteractManager.AddDoorDown(rayInfo.rigidbody.gameObject);

            selectDoor = false;
            if (SceneManager.GetActiveScene().name.Equals("slipTripLift"))
            {
                if (hovered.name.Equals("stoveflat_open01_reversed_dr_Downstairs")) GameObject.Find("stoveBox").GetComponent<BoxCollider>().enabled = true;
                else if (hovered.name.Equals("dishwasherDoor01_reversed_dr_Downstairs")) GameObject.Find("dishwasher").GetComponent<BoxCollider>().enabled = true;
            }
            else if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
            {
                GameObject.Find("dishwasher").GetComponent<BoxCollider>().enabled = true;
                GameObject.Find("dishwasherDoor01_reversed_dr").GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    /* 
     * Pull an object (usually a drawer) in an outward direction.
     */
    void askToPull()
    {
        // stringTitle = "Push/Pull";

        Highlight(oldObject);

        if (fire1 || selectDrawer)
        {
            InteractManager.AddPull(rayInfo.rigidbody.gameObject);
            selectDrawer = false;
        }
    }

    /* 
     * Push an object (usually a drawer) in an inward direction.
     */
    void askToPush()
    {
        // stringTitle = "Push/Pull";

        Highlight(oldObject);

        if (fire1 || selectDrawer)
        {
            InteractManager.AddPush(rayInfo.rigidbody.gameObject);
            selectDrawer = false;
        }
    }

    /* 
     * Opens a window in an upward direction.
     */
    void askToOpenWindow()
    {
        // stringTitle = "Open/Close";

        //    Highlight(oldObject, hoverToon);

        if (fire1 || selectWindow)
        {
            if (hovered.name.Equals("bathtubGlassGlass_r01"))
            {
                GameObject.Find("bathtubGlassGlass_r01").tag = "Untagged";
            }

            InteractManager.AddOpenWindow(rayInfo.rigidbody.gameObject);
            selectWindow = false;
        }
    }


    /* 
     * Closes a window in a downward direction.
     */
    void askToCloseWindow()
    {
        // stringTitle = "Open/Close";

       // Highlight(oldObject, hoverToon);

        if (fire1 || selectWindow)
        {
            if (hovered.name.Equals("bathtubGlassGlass_r01"))
            {
                GameObject.Find("tubGlass").GetComponent<BoxCollider>().enabled = true;
                GameObject.Find("bathtubGlassGlass_r01").tag = "Untagged";
            }
            InteractManager.AddCloseWindow(rayInfo.rigidbody.gameObject);
            selectWindow = false;
        }
    }


    /* 
     * Open an object (usually a door) in an upward direction.
     */
    void askToCloseCurtain()
    {
        //  stringTitle = "Open/Close";

        //   Highlight(oldObject, hoverToon);

        if (fire1 || selectCurtain)
        {
            if (SceneManager.GetActiveScene().name.Equals("environmental"))
            {
                if (hovered.name.Equals("curtainDownstairs"))
                {
                    GameObject.Find("moldingShowerCurtain").GetComponent<BoxCollider>().enabled = true;
                    GameObject.Find("curtainDownstairs").tag = "Untagged";
                }
            }
            InteractManager.AddCloseCurtain(rayInfo.rigidbody.gameObject);
            selectCurtain = false;
        }
    }

    /* 
     * Open an object (usually a door) in a downward direction.
     */
    void askToOpenCurtain()
    {
        // stringTitle = "Open/Close";

       //  Highlight(oldObject, hoverToon);

        if (fire1 || selectCurtain)
        {
            InteractManager.AddOpenCurtain(rayInfo.rigidbody.gameObject);
            selectCurtain = false;
        }
    }

/*    void askToTeleport()
    {
        //stringTitle = "Open/Close";

        //  Highlight(oldObject, hoverToon);

        if (fire1 || selectCube)
        {
            fader.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(fadeOut);
        }
        
    }*/
    void fadeOut()
    {
        GameManager.Player.transform.position = new Vector3(hovered.transform.position.x, hovered.GetComponent<locationCubeCollision>().iniY + 0.094f, hovered.transform.position.z);
        Invoke("fadeIn", 0.5f);
    }
    void fadeIn() {
        fader.GetComponent<CanvasGroup>().DOFade(0, 0.7f);
    }

}