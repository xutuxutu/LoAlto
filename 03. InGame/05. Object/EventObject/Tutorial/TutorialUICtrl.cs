using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialUICtrl : MonoBehaviour
{
    private static int m_tutorialLevel = 0;
    public Image[] m_tutorial_Image;
    public Image m_guideText;
    public Image m_guideText_Glow;
    public float m_fadeOutTime;
    public float m_fadeInTime;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        setImageTransparents();
        fadeOutImage();
        StartCoroutine("userInput");
    }

    public void setImageTransparents()
    {
        for (int i = 0; i < m_tutorial_Image.Length; ++i)
            m_tutorial_Image[i].canvasRenderer.SetAlpha(0f);

        m_guideText.canvasRenderer.SetAlpha(0f);
        m_guideText_Glow.canvasRenderer.SetAlpha(0f);
    }

    public IEnumerator userInput()
    {
        bool printImage = true;
        yield return new WaitForSeconds(m_fadeOutTime + 1.0f);
        fadeOutText();
        StartCoroutine("blinkGuideText_Glow");
        while (printImage)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //나타난 이미지 사라짐
                fadeInImage();
                StopCoroutine("blinkGuideText_Glow");
                m_guideText_Glow.canvasRenderer.SetAlpha(1f);
                fadeInText();
                //사라지는 타임이 끝난 후 컨트롤 활성화, while문 탈출
                yield return new WaitForSeconds(0.5f);
                activeCharacterCtrl();
                printImage = false;
                if (m_tutorialLevel == 0)
                {
                    if (ProjectMgr.getInstance().getOwnCharacterType() == CHARACTER.TYPE.SAM)
                        m_tutorialLevel += 2;
                    else
                        m_tutorialLevel += 1;
                }
                else
                    activeSkiil();
            }
            yield return null;
        }
        Invoke("deActive", 0.7f);
    }

    public void fadeInImage()
    {
        m_tutorial_Image[m_tutorialLevel].CrossFadeAlpha(0.0f, m_fadeInTime, false);
    }

    public void fadeOutImage()
    {
        m_tutorial_Image[m_tutorialLevel].CrossFadeAlpha(1.0f, m_fadeOutTime, false);
    }

    public void fadeInText()
    {
        m_guideText.CrossFadeAlpha(0.0f, m_fadeInTime, false);
        m_guideText_Glow.CrossFadeAlpha(0.0f, m_fadeInTime, false);
    }

    public void fadeOutText()
    {
        m_guideText.CrossFadeAlpha(1.0f, 1f, false);
    }


    public void activeCharacterCtrl()
    {
        InGameMgr.getInstance().getOwnCharacterCtrl().setIsActive(true);
        InGameMgr.getInstance().getOwnCharacterCtrl().setCameraCtrlLock(false);
        InGameMgr.getInstance().getOwnCharacterCtrl().setDynamicCameraLock(false);
    }

    public void deActive()
    {
        QuestMgr.getInstance().activeMainQuestAlarm();
        gameObject.SetActive(false);
    }

    public void activeSkiil()
    {
        InGameMgr.getInstance().getOwnCharacterCtrl().setSkillActive(true, 0);
        InGameMgr.getInstance().getOwnCharacterCtrl().setSkillActive(true, 1);
    }

    public IEnumerator blinkGuideText_Glow()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            m_guideText_Glow.CrossFadeAlpha(1.0f, 0.5f, false);
            yield return new WaitForSeconds(0.5f);
            m_guideText_Glow.CrossFadeAlpha(0.0f, 0.5f, false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
