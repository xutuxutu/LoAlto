using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class GameEvent : MonoBehaviour
{
    public float m_fadeStartTime;
    public float m_fadeInTime;

    private Image m_blackScreen;
    private RawImage m_loadingImage;
    private AudioSource m_audioSource;

    public abstract void startEvent();
    public abstract void objectActive();

    public void init()
    {
        AudioListener.volume = 0f;
        m_blackScreen = GameObject.Find(OBJECT_NAME.BLACK_SCREEN).GetComponent<Image>();
        m_loadingImage = GameObject.Find(OBJECT_NAME.LOADING_IMAGE).GetComponent<RawImage>();
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.Play();
    }

    public void initAllEvent()
    {
        CreatureMgr.getInstance().AllCreatureFixedIdleState();
        Invoke("objectActive", 2f);
        Invoke("setActiveOther", 5f);
    }

    public void fadeInLoadingImage()
    {
        m_loadingImage.CrossFadeAlpha(0.0f, m_fadeStartTime, false); //fadein
    }

    public void fadeIn()
    {
        m_blackScreen.CrossFadeAlpha(0.0f, m_fadeInTime, false); //fadein
    }

    public void fadeOut()
    {
        m_loadingImage.CrossFadeAlpha(0.0f, 2f, false); //fadein
    }

    public void setLoadingImageDeActive()
    {
        m_loadingImage.gameObject.SetActive(false);
    }

    public void fadeInVolume()
    {
        StartCoroutine("fadeInVolumeRoutine");
    }
    
    public IEnumerator fadeInVolumeRoutine()
    {
        AudioListener.volume = 0f;
        float curVolume = 0f;
        float incValue = 0.01f;
        while (curVolume < 1f)
        {
            curVolume += incValue;
            incValue += 0.001f;
            if (curVolume >= 1f)
            {
                curVolume = 1f;
                AudioListener.volume = 1f;
                continue;
            }

            AudioListener.volume = curVolume;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void setActiveOther()
    {
        InGameMgr.getInstance().getOtherCharacterCtrl().setActive();
    }
}
