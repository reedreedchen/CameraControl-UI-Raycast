using UnityEngine;
using System.Collections;

public class flashlight : MonoBehaviour {

	public bool isOn = false;

	void Start () {}

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            isOn = !isOn;
            detectFlashlight();
        }

    }
	
    public void detectFlashlight()
    {
        if (isOn && !GameObject.Find("flashlight").GetComponent<Light>().isActiveAndEnabled)
            GameObject.Find("flashlight").GetComponent<Light>().enabled = true;
        else if (!isOn && GameObject.Find("flashlight").GetComponent<Light>().isActiveAndEnabled)
            GameObject.Find("flashlight").GetComponent<Light>().enabled = false;


    }

}
