using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class forAssessmentIntro : MonoBehaviour {
    public AudioClip[] overviewVoiceOver;
    int maxcounter;
    int counter;
    float textSpeed;
    public Dialog overviewDialog;
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
    void Start() {
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            if(audioSource.gameObject.name != "overviewWindow" && audioSource.gameObject.name != "waterParticle" && audioSource.gameObject.name != "waterParticle_snd")
                audioSource.enabled = false;
        }
        counter = 1;
        maxcounter = overviewDialog.trees[0].frames.Count;
        overviewSource.Stop();
        Invoke("startVoice", 1f);
        Invoke("forOverviewReturn", 1f);
#if !UNITY_EDITOR
        returnButton.interactable = false; // slash this to skip introduction
#endif
        returnButton.GetComponent<Image>().color = Color.white;

        overviewTitle.text = "Assessment " + counter.ToString() + "/" + maxcounter.ToString();
        overviewTexts.text = overviewDialog.trees[0].frames[counter - 1].text;

    }

    public void startVoice() {
        overviewSource.Play();
    }
	void Update ()
    {

		if (counter > 1 && !previosButton.interactable) {
			previosButton.interactable = true;
			previosButton.GetComponent<Image> ().color = Color.green;
		} else if(counter <= 1 && previosButton.interactable)
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
            Time.timeScale = 0;
            overviewSource.Pause();
            overviewPlayImage.sprite = soundPlayBtn;
        }
        else
        {
            Time.timeScale = 1;
            overviewSource.UnPause();
            overviewPlayImage.sprite = soundPauseBtn;
        }
    }


    void addCounter()
    {
        if (counter <= maxcounter)
        { 
            counter+=1;
            forOverviewReturn();
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

    public void reveal()
    {
        if (counter == 1) //page 0-1
        {
            textSpeed = 3.3f;
            overviewTexts.text = overviewDialog.trees[1].frames[0].text;//0-1
            Invoke("reveal2", overviewVoiceOver[counter - 1].length / textSpeed);
        }
        else if (counter == 2)//page 1-1
        {
            overviewTexts.text = overviewDialog.trees[1].frames[2].text;//1-1
        }
        else if (counter == 3)//page 2-1
        {
            overviewTexts.text = overviewDialog.trees[1].frames[3].text;//2-1
        }
    }
    public void reveal2()
    {
        if (counter == 1)
        {
            overviewTexts.text = overviewDialog.trees[1].frames[1].text;//0-2
        }
    }

    public void allowBegin()
    {
        returnButton.interactable = true;
        returnButton.GetComponent<Image>().color = Color.green;
    }
    void forOverviewReturn()
    {
        if (counter <= maxcounter)
        {
           overviewTitle.text = "Assessment " + counter.ToString() + "/" + maxcounter.ToString();
           overviewTexts.text = overviewDialog.trees[0].frames[counter - 1].text;

           overviewSource.clip = overviewVoiceOver[counter - 1];
           overviewSource.Play();
            if(counter < maxcounter)
              Invoke("addCounter", overviewVoiceOver[counter - 1].length);
        }
        if (counter == 1) //0
        {
            textSpeed = 3.3f;
            Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
        }
        else if (counter == 2)//1
        {
            textSpeed = 2.5f;
            Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
        }
        else if (counter == 3)//2
        {
            textSpeed = 1.4f;
            Invoke("reveal", overviewVoiceOver[counter - 1].length / textSpeed);
            Invoke("allowBegin", overviewVoiceOver[counter - 1].length);
        }

    }
    public void forBegin()
    {
        GameManager.CameraController.GetComponent<CameraController>().assessmentIntroFadeOut();
        if (Time.timeScale == 0) switchSound();
        RoomTrigger.isIntroDone = true;
        GetComponent<AudioSource>().Stop();
        GameObject frontYardTrigger = GameObject.Find("Front Yard Trigger");
        frontYardTrigger.SetActive(false);
        frontYardTrigger.SetActive(true);
        

    }
}
