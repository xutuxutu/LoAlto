using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Final_GameEvent : MonoBehaviour
{
    public Text m_clearText;
    public RawImage m_endingCredit;
    public Text m_continueText;

	// Use this for initialization
	void Start ()
    {
        m_clearText.canvasRenderer.SetAlpha(0f);
        m_continueText.canvasRenderer.SetAlpha(0f);
        m_endingCredit.canvasRenderer.SetAlpha(0f);
        
        Invoke("fadeOutClear", 2);
        Invoke("fadeInClear", 4);

        Invoke("end", 6);
        /*
        Invoke("activeCredit", 6);
        Invoke("fadeOutCredit", 6);

        Invoke("fadeInCredit", 94);
        Invoke("deActiveCredit", 96);

        Invoke("fadeOutContinue", 96);
        Invoke("fadeInContinue", 99);

        Invoke("end", 101);*/
    }

    void Update()
    {
       if (Input.anyKeyDown)
            end();
    }

    public void fadeOutClear() //검어짐
    {
        m_clearText.CrossFadeAlpha(1.0f, 2, false);
    }

    public void fadeInClear() //밝아짐
    {
        m_clearText.CrossFadeAlpha(0.0f, 2, false);
    }

    public void activeCredit()
    {
        m_endingCredit.gameObject.SetActive(true);
    }

    public void fadeOutCredit()
    {
        m_endingCredit.CrossFadeAlpha(1f, 2, false);
    }

    public void fadeInCredit() //밝아짐
    {
        m_endingCredit.CrossFadeAlpha(0.0f, 2, false);
    }

    public void deActiveCredit()
    {
        m_endingCredit.gameObject.SetActive(false);
    }

    public void fadeOutContinue() //검어짐
    {
        m_continueText.CrossFadeAlpha(1.0f, 2, false);
    }

    public void fadeInContinue() //밝아짐
    {
        m_continueText.CrossFadeAlpha(0.0f, 2, false);
    }

    public void end()
    {
        if (ProjectMgr.getInstance() != null)
        {
            ProjectMgr.getInstance().setMouseLock(false);
        }
        if (InGameServerMgr.getInstance() != null)
            InGameServerMgr.getInstance().disConnectServer();

        Application.LoadLevel(0);
    }
}
