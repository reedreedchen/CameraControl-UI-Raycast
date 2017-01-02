using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class showMessage : MonoBehaviour {
    public GameObject _msg;
    public static GameObject msg;
    public static bool pausing;
	void Awake () {
        pausing = false;
        msg= _msg;
    }
	
	public static void showError(string error)
    {
        msg.GetComponent<Text>().text = error;
        if (error != "Game Pause")
           msg.GetComponent<Text>().DOFade(1, 0.5f);
	}
    public static void closeError()
    {
     //   msg.GetComponent<Text>().text = "";
        msg.GetComponent<Text>().DOFade(0, 0.5f);
    }
}
