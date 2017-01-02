using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class forOverview : MonoBehaviour {
    public AudioClip[] overviewVoiceOver;
    float textSpeed;
    int maxcounter;
    int counter;
    public Dialog overviewDialog;
    public GameObject fader;
    public Slider soundSliderControl;
    public AudioSource overviewSource;
    public Image overviewPlayImage;
    public GameObject overviewPlayButton;
    public Sprite soundPlayBtn;
    public Sprite soundPauseBtn;
    public Text returnText;
    public Text overviewTitle;
    public Button returnButton;
    public Text overviewTexts;
    public Button previosButton;
	// Use this for initialization
	void Start () {
	    fader.SetActive(true);
        fader.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        textSpeed = 2.3f;
        counter = 1;
        maxcounter = overviewDialog.trees[0].frames.Count;
        forOverviewReturn();
	}
	
	// Update is called once per frame
	void Update ()
    {

		if (counter > 1) {
			previosButton.interactable = true;
			previosButton.GetComponent<Image> ().color = Color.green;
		} else
		{
			previosButton.interactable = false;
			previosButton.GetComponent<Image>().color=Color.white;
		}
        if (Input.GetKey(KeyCode.Period))
        {
            soundSliderControl.value += 0.001f;
            AudioListener.volume = soundSliderControl.value;
        }
        else if (Input.GetKey(KeyCode.Comma))
        {
            soundSliderControl.value -= 0.001f;
            AudioListener.volume = soundSliderControl.value;
        }
	}

   //overview starts
    public void soundSlider()
    {
        AudioListener.volume = soundSliderControl.value;
    }
    public void switchSound()
    {
        if (Time.timeScale == 1)
        {
            Debug.Log(Time.timeScale);
            Time.timeScale = 0;
            overviewSource.Pause();
            overviewPlayImage.sprite = soundPlayBtn;
        }
        else
        {
            Debug.Log(Time.timeScale);
            Time.timeScale = 1;
            overviewSource.UnPause();
            overviewPlayImage.sprite = soundPauseBtn;
        }
    }

    public void endOverview()
    {
        Time.timeScale = 1;
        GameObject.FindWithTag("MainCamera").GetComponent<AudioListener>().enabled = false;
        fader.GetComponent<CanvasGroup>().DOFade(1, .5f).OnComplete(loadMenu);

    }

    void loadMenu() { SceneManager.LoadScene("menu");}

    void addCounter()
    {
        counter += 1;
        if (counter <= maxcounter)
            forOverviewReturn();
        else
        {
            overviewPlayButton.SetActive(false);
            returnButton.gameObject.SetActive(true);
            returnText.text = "Return";
        }
    }
    public void forOverviewPrevious()
    {
        if (counter > 1)
        {
            counter -= 1;
            CancelInvoke();
            forOverviewReturn();
        }
    
    }
    public void forOverviewReturn()
    {
        if (counter <= maxcounter)
        {
           overviewTitle.text = "Overview " + counter.ToString() + "/" + maxcounter.ToString();
           overviewTexts.text = overviewDialog.trees[0].frames[counter - 1].text;

           overviewSource.clip = overviewVoiceOver[counter - 1];
           overviewSource.Play();

            Invoke("addCounter", overviewVoiceOver[counter - 1].length);

            if (counter == 1)
            {
                textSpeed = 2.1f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 2)
            {
                textSpeed = 2.5f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 5)
            {
                textSpeed = 1.9f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 7)
            {
                textSpeed = 2f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 8)
            {
                textSpeed = 5.5f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 9)
            {
                textSpeed = 2f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 11)
            {
                textSpeed = 1.7f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
            else if (counter == 12)
            {
                textSpeed = 4f;
                Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            }
          
        }else
        {
            SceneManager.LoadScene("menu");
        }
    }

    public void reveal()
    {
        if (counter == 8)
        {
            textSpeed = 2.2f;
            overviewTexts.text = overviewDialog.trees[1].frames[counter - 1].text;
            Invoke("reveal2", overviewVoiceOver[counter - 1].length / textSpeed);
        }
        else if (counter == 11)
        {
            textSpeed = 6f;
            overviewTexts.text = overviewDialog.trees[1].frames[counter - 1].text;
            Invoke("reveal2", overviewVoiceOver[counter - 1].length / textSpeed);

        }
        else if (counter == 12)
        {
            textSpeed = 7f;
            overviewTexts.text = overviewDialog.trees[1].frames[counter - 1].text;
            Invoke("reveal2", overviewVoiceOver[counter - 1].length / textSpeed);
        }
        else
            overviewTexts.text = overviewDialog.trees[1].frames[counter - 1].text;
    }
    public void reveal2()
    {
        if (counter == 8)
        {
            overviewTexts.text = overviewDialog.trees[2].frames[0].text;
        }
        else if (counter == 11)
        {
            overviewTexts.text = overviewDialog.trees[2].frames[1].text;
            textSpeed = 7f;
            Invoke("reveal3", overviewVoiceOver[counter - 1].length / textSpeed);
        }
        else if (counter == 12)
        {
            overviewTexts.text = overviewDialog.trees[2].frames[3].text;
            textSpeed = 8f;
            Invoke("reveal3", overviewVoiceOver[counter - 1].length / textSpeed);
        }
    }
    public void reveal3()
    {
        if (counter == 11)
        {
            overviewTexts.text = overviewDialog.trees[2].frames[2].text;
        }
        if (counter == 12)
        {
            textSpeed = 9f;
            overviewTexts.text = overviewDialog.trees[2].frames[4].text;

            Invoke("reveal4", overviewVoiceOver[counter - 1].length / textSpeed);
        }
    }
    public void reveal4()
    {
        if (counter == 12)
        {
            textSpeed = 8f;
            overviewTexts.text = overviewDialog.trees[2].frames[5].text;
            Invoke("reveal5", overviewVoiceOver[counter - 1].length / textSpeed);
        }
    }
    public void reveal5()
    {
        if (counter == 12)
        {
            overviewTexts.text = overviewDialog.trees[2].frames[6].text;
        }
    }
//overview ends

}
