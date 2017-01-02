using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class walkThroughInterface : MonoBehaviour {
    public Button[] interfaceBtn;
    public Sprite interfaceBtn_pressed;
    public Sprite interfaceBtn_unPressed;
    // Use this for initialization
    float startTime;
    float currentTime;
    public AudioSource tourSound; 
    void Start()
    {
        if (Tutorial.menuToWalkThrough)
        {
            startTime = Time.time;
            Time.timeScale = 1.2f;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    void unPressAll()
    {
        foreach (Button btn in interfaceBtn)
            btn.GetComponent<Image>().sprite = interfaceBtn_unPressed;
    }
    void Update() {
        if (Tutorial.menuToWalkThrough)
        {
            currentTime = (Time.time - startTime);
            if (currentTime >= 219f && currentTime < 222f)
            {
                gameObject.GetComponent<CanvasGroup>().DOFade(1, 2f);
            }
            else if (currentTime >= 222f && currentTime < 230f)
            {
                unPressAll();
                interfaceBtn[0].GetComponent<Image>().sprite = interfaceBtn_pressed;
            }
            else if (currentTime >= 230f && currentTime < 236f)
            {
                unPressAll();
            }
            else if (currentTime >= 236f && currentTime < 238f)
            {
                unPressAll();
                interfaceBtn[1].GetComponent<Image>().sprite = interfaceBtn_pressed;
            }
            else if (currentTime >= 238f && currentTime < 241f)
            {
                unPressAll();
                interfaceBtn[2].GetComponent<Image>().sprite = interfaceBtn_pressed;
            }
            else if (currentTime >= 241f && currentTime < 244f)
            {
                unPressAll();
                interfaceBtn[3].GetComponent<Image>().sprite = interfaceBtn_pressed;
            }
            else if (currentTime >= 244f && currentTime < 271)
            {
                unPressAll();
            }
            else if (currentTime >= 271f && currentTime < 275f)
            {
                unPressAll();
                interfaceBtn[4].GetComponent<Image>().sprite = interfaceBtn_pressed;
            }
            else if (currentTime >= 275f && currentTime < 280f)
            {
                unPressAll();
            }
            else if (currentTime >= 280f)
            {
                SceneManager.LoadScene("menu");
            }
        }
    }

}
