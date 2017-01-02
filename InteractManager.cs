using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class InteractManager : MonoBehaviour
{
    /**
     * Using this class to not only handle doors but other objects
     * that can be interacted with by RayObject.cs based on their
     * tags. todo; change name of class.
     */

    /**
     * Note: If objects are not moving exactly the way they should,
     * chances are the name of the object in-scene has been changed,
     * and needs to be changed in here.
     * (TODO: Perhaps give each object a public variable to change
     *        in-scene and take that in here)
     */
    bool doorOpen;
    private static List<GameObject> listRot = new List<GameObject>();
    private static List<GameObject> listPos = new List<GameObject>();
    private static List<GameObject> listScl = new List<GameObject>();

    private static List<Quaternion> initialRot = new List<Quaternion>();
    private static List<Quaternion> targetsRot = new List<Quaternion>();
    private static List<Vector3> initialPos = new List<Vector3>();
    private static List<Vector3> targetsPos = new List<Vector3>();
    private static List<float> initialScl = new List<float>();
    private static List<float> targetsScl = new List<float>();

    private static List<float> deltaRot = new List<float>();
    private static List<float> deltaPos = new List<float>();
    private static List<float> deltaScl = new List<float>();

    //private bool curtainCondition1 = false, curtainCondition2 = false;
    private static bool oneWindowOpen = false, twoWindowsOpen = false;
    public List<ParticleSystem> snowParticle;

    public static float speed = 1.5f;

    void Start()
    {


        if (SceneManager.GetActiveScene().name.Contains("slipTripLift") || SceneManager.GetActiveScene().name.Contains("environmental"))
            if (!GameManager.MainCamera.GetComponent<CameraController>().currentRoom.Equals("Front Yard"))
                foreach (ParticleSystem a in snowParticle) a.Play();
    }
    public static void closeWarning()
    {
        GameObject.Find("closerWarning").GetComponent<Text>().DOFade(0, 1f);
    }
    public static void AddDoorLeft(GameObject door)
    {
        InteractManager.speed = 1.5f;
        listRot.Add(door);
        if (door.name == "backDoor_dr")
        {
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 22, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        else if (door.name == "dogCrateDoor_dr")
        {
            InteractManager.speed = 2f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 90, 0));
            playSound.playTheSoundForCamera(playSound.Crate, playSound.soundVol);
            GameObject.Find("closerWarning").GetComponent<Text>().text = "The dog bit you.";
            GameObject.Find("closerWarning").GetComponent<Text>().DOFade(1, 1f).OnComplete(InteractManager.closeWarning);
            GameObject.Find("dog").GetComponent<BoxCollider>().enabled = false;
            InteractManager.AddPull(GameObject.Find("dog"));
        }
        else if (door.name == "microDoor_dr")
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 65, 0));
        else if (door.name.Equals("stove_knob1"))
            GameObject.Find("stove_coil01").SetActive(false);
        else if (door.name.Equals("stove_knob2"))
            GameObject.Find("stove_coil02").SetActive(false);
        else if (door.name.Equals("stove_knob3"))
            GameObject.Find("stove_coil03").SetActive(false);
        else if (door.name.Equals("stove_knob4"))
            GameObject.Find("stove_coil04").SetActive(false);
        else if (door.name.Contains("fridgeDoor") || door.name.Contains("freezerDoor"))
        {
            speed = 2f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 90, 0));
            playSound.playTheSoundForCamera(playSound.FridgeDoor, playSound.soundVol);
            if (door.name.Contains("fridgeDoor"))
            {
                GameObject.Find("fridgeLight").GetComponent<Light>().enabled = false;
            }
        }
        else if (door.name.Contains("Closet") || door.name.Contains("bathSinkDoor"))
        {
            speed = 3.5f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 90, 0));
            playSound.playTheSoundForCamera(playSound.ClosetDoor, playSound.soundVol);
        }
        else if (door.name.Contains("bathSinkFaucetPart"))
        {
            speed = 3.5f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 90, 0));
        }
        else if (door.name.Contains("bathdoor01a_dr"))
        {
            speed = 2.7f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 85, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        else
        {
            speed = 2.8f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, 90, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        initialRot.Add(door.transform.rotation);
        deltaRot.Add(0);
        door.tag = "lockedLeft";
        door.GetComponent<BoxCollider>().enabled = false;

    }

    public static void AddDoorRight(GameObject door)
    {
        speed = 1.5f;
        listRot.Add(door);
        if (door.name == "backDoor_dr")
        {
            speed = 0.7f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -22, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        else if (door.name == "dogCrateDoor_dr")
        {
            speed = 2f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -90, 0));
            playSound.playTheSoundForCamera(playSound.Crate, playSound.soundVol);
            GameObject.Find("dog").GetComponent<BoxCollider>().enabled = true;
            AddPush(GameObject.Find("dog"));
        }
        else if (door.name == "microDoor_dr")
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -65, 0));
        else if (door.name.Equals("stove_knob1"))
            GameObject.Find("stove_coil01").SetActive(true);
        else if (door.name.Equals("stove_knob2"))
            GameObject.Find("stove_coil02").SetActive(true);
        else if (door.name.Equals("stove_knob3"))
            GameObject.Find("stove_coil03").SetActive(true);
        else if (door.name.Equals("stove_knob4"))
            GameObject.Find("stove_coil04").SetActive(true);
        else if (door.name.Contains("fridgeDoor"))
        {
            InteractManager.speed = 2f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -90, 0));
            playSound.playTheSoundForCamera(playSound.FridgeDoor, playSound.soundVol);
            if (door.name.Contains("fridgeDoor"))
            {
                GameObject.Find("fridgeLight").GetComponent<Light>().enabled = true;
            }

        }
        else if (door.name.Contains("Closet") || door.name.Contains("bathSinkDoor"))
        {
            speed = 3.5f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -90, 0));
            playSound.playTheSoundForCamera(playSound.ClosetDoor, playSound.soundVol);
        }
        else if (door.name.Contains("bathSinkFaucetPart"))
        {
            speed = 3.5f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -90, 0));
        }
        else if (door.name.Contains("bathdoor01a_dr"))
        {
            speed = 2.7f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -85, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        else
        {
            speed = 2.8f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(0, -90, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }

        initialRot.Add(door.transform.rotation);
        deltaRot.Add(0);
        door.tag = "lockedRight";
        door.GetComponent<BoxCollider>().enabled = false;
    }

    public static void AddDoorUp(GameObject door)
    {
        listRot.Add(door);

        speed = 1.5f;

        if (door.name.Contains("stoveflat_open01_reversed_dr") || door.name.Contains("dishwasherDoor01_reversed_dr"))
        {
            speed = 1.8f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(90, 0, 0));
            playSound.playTheSoundForCamera(playSound.FridgeDoor, playSound.soundVol);
        }
        else
        {
            speed = 2.8f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(90, 0, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        initialRot.Add(door.transform.rotation);
        deltaRot.Add(0);
        door.tag = "lockedUp";
    }

    public static void AddDoorDown(GameObject door)
    {

        listRot.Add(door);
        speed = 1.5f;
        if (door.name.Contains("stoveflat_open01_reversed_dr") || door.name.Contains("dishwasherDoor01_reversed_dr"))
        {
            speed = 1.8f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(-90, 0, 0));
            playSound.playTheSoundForCamera(playSound.FridgeDoor, playSound.soundVol);
        }
        else
        {
            speed = 2.8f;
            targetsRot.Add(door.transform.rotation * Quaternion.Euler(-90, 0, 0));
            playSound.playTheSoundForCamera(playSound.Door, playSound.soundVol);
        }
        initialRot.Add(door.transform.rotation);
        deltaRot.Add(0);
        door.tag = "lockedDown";
    }

    public static void AddPull(GameObject drawer)
    {
        listPos.Add(drawer);

        // todo; naming conventions! and a better plan here
        if (drawer.name.Equals("stove_low01"))
            targetsPos.Add(drawer.transform.position + new Vector3(-0.3f, 0, 0));
        else if (drawer.name.Equals("dog"))
            targetsPos.Add(drawer.transform.position + new Vector3(-0.5f, 0, 0));
        else if (drawer.name.Equals("endTableDrawer01_TV"))
            targetsPos.Add(drawer.transform.position + new Vector3(-0.2f, 0, .2f));
        else if (drawer.name.Equals("endTableDrawer01_bedRoom"))
            targetsPos.Add(drawer.transform.position + new Vector3(-0.2f, 0, 0));
        else if (drawer.name.Equals("smallDrawer01")
            || drawer.name.Equals("smallDrawer02"))
            targetsPos.Add(drawer.transform.position + new Vector3(0, 0, 0.3f));
        else
            targetsPos.Add(drawer.transform.position + new Vector3(0, 0, -.2f));

        initialPos.Add(drawer.transform.position);
        deltaPos.Add(0);
        drawer.tag = "lockedPull";
    }

    public static void AddPush(GameObject drawer)
    {
        listPos.Add(drawer);

        if (drawer.name.Equals("stove_low01"))
            targetsPos.Add(drawer.transform.position + new Vector3(0.3f, 0, 0));
        else if (drawer.name.Equals("dog"))
            targetsPos.Add(drawer.transform.position + new Vector3(0.5f, 0, 0));
        else if (drawer.name.Equals("endTableDrawer01_TV"))
            targetsPos.Add(drawer.transform.position + new Vector3(0.2f, 0, -.2f));
        else if (drawer.name.Equals("endTableDrawer01_bedRoom"))
            targetsPos.Add(drawer.transform.position + new Vector3(0.2f, 0, 0));
        else if (drawer.name.Equals("smallDrawer01")
            || drawer.name.Equals("smallDrawer02"))
            targetsPos.Add(drawer.transform.position + new Vector3(0, 0, -.3f));
        else
            targetsPos.Add(drawer.transform.position + new Vector3(0, 0, .2f));

        initialPos.Add(drawer.transform.position);
        deltaPos.Add(0);
        drawer.tag = "lockedPush";
    }

    public static void AddOpenWindow(GameObject window)
    {
        // 7 and 4 are in living Room
        listPos.Add(window);

        if (window.name.Equals("windows08_open") || window.name.Equals("windows06_open"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0, 0.23f, 0));
        }
        else if (window.name.Contains("slideDoor02"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(-0.7f, 0, 0));
        }
        else if (window.name.Contains("slideDoor01"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0.7f, 0, 0));
        }
        else if (window.name.Contains("bathtubGlassGlass_r01"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0, 0, -0.7f));
        }
        else
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0, 0.4f, 0));
        }
        if (window.name.Contains("07") || window.name.Contains("04"))
        {
            if (oneWindowOpen)
            {
                twoWindowsOpen = true;
                oneWindowOpen = false;
            }
            else
                oneWindowOpen = true;
        }
        playSound.playTheSoundForCamera(playSound.SlidingDoor, playSound.soundVol);
        initialPos.Add(window.transform.position);
        deltaPos.Add(0);
        window.tag = "lockedOpenWindow";
    }

    public static void AddCloseWindow(GameObject window)
    {
        listPos.Add(window);

        if (window.name.Equals("windows08_open") || window.name.Equals("windows06_open"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0, -0.23f, 0));

        }
        else if (window.name.Contains("slideDoor02"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0.7f, 0, 0));
        }
        else if (window.name.Contains("slideDoor01"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(-0.7f, 0, 0));

        }
        else if (window.name.Contains("bathtubGlassGlass_r01"))
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0, 0, 0.7f));
        }
        else
        {
            speed = 0.7f;
            targetsPos.Add(window.transform.position + new Vector3(0, -0.4f, 0));

        }
        if (window.name.Contains("07") || window.name.Contains("04"))
        {
            if (twoWindowsOpen)
            {
                oneWindowOpen = true;
                twoWindowsOpen = false;
            }
            else
                oneWindowOpen = false;
        }
        playSound.playTheSoundForCamera(playSound.SlidingDoor, playSound.soundVol);
        initialPos.Add(window.transform.position);
        deltaPos.Add(0);
        window.tag = "lockedCloseWindow";
    }

    public static void AddCloseCurtain(GameObject curtain)
    {
        speed = 1.5f;
        listScl.Add(curtain);

        targetsScl.Add(0f);

        playSound.playTheSoundForCamera(playSound.Curtains, playSound.soundVol);

        if (curtain.layer != LayerMask.NameToLayer("bathroom"))
        {
            curtain.GetComponent<BoxCollider>().center = curtain.GetComponent<BoxCollider>().center + new Vector3(-25.8f, 0f, 0f);
            curtain.GetComponent<BoxCollider>().size = new Vector3(80f, 120f, 2);
        }
        else if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2") || SceneManager.GetActiveScene().name.Equals("environmental") || SceneManager.GetActiveScene().name.Equals("accessment")) {
            curtain.GetComponent<BoxCollider>().center = curtain.GetComponent<BoxCollider>().center + new Vector3(-35f, 0f, 0f);
            curtain.GetComponent<BoxCollider>().size = new Vector3(120, 120f, 2f);
        }
        initialScl.Add(100f);
        deltaScl.Add(0);
        curtain.tag = "lockedCloseCurtain";
    }

    public static void AddOpenCurtain(GameObject curtain)
    {
        listScl.Add(curtain);

        targetsScl.Add(100f);


        playSound.playTheSoundForCamera(playSound.Curtains, playSound.soundVol);
        if (curtain.layer != LayerMask.NameToLayer("bathroom"))
        {
            curtain.GetComponent<BoxCollider>().size = new Vector3(20f, 127f, 2f);
            curtain.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0f);
        }
        else if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2") || SceneManager.GetActiveScene().name.Equals("environmental") || SceneManager.GetActiveScene().name.Equals("accessment"))
        {
            curtain.GetComponent<BoxCollider>().size = new Vector3(20f, 120f, 2f);
            curtain.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0f);
        }

        initialScl.Add(0f);
        deltaScl.Add(0);
        curtain.tag = "lockedOpenCurtain";
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("slipTripLift") || SceneManager.GetActiveScene().name.Contains("environmental"))
        {
            if (GameManager.MainCamera.GetComponent<CameraController>().currentRoom.Equals("Front Yard"))
                foreach (ParticleSystem a in snowParticle) a.Play();
            else
                foreach (ParticleSystem a in snowParticle) a.Stop();
        }

        for (int i = listRot.Count - 1; i >= 0; i--)
        {
            deltaRot[i] += Time.deltaTime * speed;
            listRot[i].transform.rotation = Quaternion.Slerp(initialRot[i], targetsRot[i], Mathf.Min(deltaRot[i], 1));

            if (Mathf.Min(deltaRot[i], 1) == 1)
            {
                if (listRot[i].tag == "lockedLeft")
                    listRot[i].tag = "SelectToOpenRight";
                if (listRot[i].tag == "lockedRight")
                    listRot[i].tag = "SelectToOpenLeft";
                if (listRot[i].tag == "lockedUp")
                    listRot[i].tag = "SelectToOpenDown";
                if (listRot[i].tag == "lockedDown")
                    listRot[i].tag = "SelectToOpenUp";
                bool allowBox = true;

                if (SceneManager.GetActiveScene().name.Equals("electricalFireHazards2") && listRot[i].name.Equals("dishwasherDoor01_reversed_dr"))
                {
                    allowBox = false;
                }
                else if (SceneManager.GetActiveScene().name.Equals("slipTripLift") || SceneManager.GetActiveScene().name.Equals("electricalFireHazards2"))
                {
                    if (listRot[i].name.Equals("freezerDoor01_dr_reversed") && listRot[i].tag.Equals("SelectToOpenLeft"))
                    {
                        allowBox = false;
                    }
                    else if (listRot[i].name.Equals("fridgeDoor01_dr_reversed") && listRot[i].tag.Equals("SelectToOpenLeft"))
                    {
                        allowBox = false;
                    }

                }

                if (allowBox)
                       listRot[i].GetComponent<BoxCollider>().enabled = true;



                listRot.RemoveAt(i);
                initialRot.RemoveAt(i);
                targetsRot.RemoveAt(i);

                deltaRot.RemoveAt(i);
            }
        }

        
        for (int i = listScl.Count - 1; i >= 0; i--)
        {
            deltaScl[i] += Time.deltaTime * speed;

            float f = Mathf.Lerp(initialScl[i], targetsScl[i], Mathf.Min(deltaScl[i], 1));
            listScl[i].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, f);


            if (Mathf.Min(deltaScl[i], 1) == 1)
            {
                if (listScl[i].tag == "lockedCloseCurtain")
                    listScl[i].tag = "SelectToOpenCurtain";
                if (listScl[i].tag == "lockedOpenCurtain")
                    listScl[i].tag = "SelectToCloseCurtain";

                listScl.RemoveAt(i);
                initialScl.RemoveAt(i);
                targetsScl.RemoveAt(i);

                deltaScl.RemoveAt(i);
            }
        }
    
        for (int i = listPos.Count - 1; i >= 0; i--)
        {
            deltaPos[i] += Time.deltaTime * speed;
            listPos[i].transform.position = Vector3.Lerp(initialPos[i], targetsPos[i], Mathf.Min(deltaPos[i], 1));

            if (Mathf.Min(deltaPos[i], 1) == 1)
            {
                if (listPos[i].tag == "lockedPull")
                    listPos[i].tag = "SelectToPush";
                if (listPos[i].tag == "lockedPush")
                    listPos[i].tag = "SelectToPull";
                if (listPos[i].tag == "lockedOpenWindow")
                    listPos[i].tag = "SelectToCloseWindow";
                if (listPos[i].tag == "lockedCloseWindow")
                    listPos[i].tag = "SelectToOpenWindow";

                if (listPos[i].name == "dog")
                    listPos[i].tag = "SelectForNotDanger";

                listPos.RemoveAt(i);
                initialPos.RemoveAt(i);
                targetsPos.RemoveAt(i);

                deltaPos.RemoveAt(i);
            }
        }

    }
}
