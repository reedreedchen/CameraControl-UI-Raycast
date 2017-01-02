using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class zoom : MonoBehaviour {
	public int value = 20;
	public int normal = 60;
	public bool isZoomed = false;
	void Start () {

	}

    void Update()
    {
        // TODO: Make things transparent
        if (Input.GetKeyDown(KeyCode.Z))
        {
            detectZoom();
        }
    }

    public void detectZoom()
    {
        if (GetComponent<Camera>().fieldOfView==normal)
        {
            GetComponent<Camera>().fieldOfView = value;
            GameObject.Find("mag").GetComponent<Image>().enabled = true;
        }
        else if (GetComponent<Camera>().fieldOfView == value)
        {
            GetComponent<Camera>().fieldOfView = normal;
            GameObject.Find("mag").GetComponent<Image>().enabled = false;
        }
    }
}
