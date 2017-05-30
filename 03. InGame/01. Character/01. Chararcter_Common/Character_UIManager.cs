using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Character_UIManager
{
    private Animator[] m_partsGetEffect;
    private RectTransform[][] m_partsNumUI;
    private int m_curPartsNum_;
    private int m_nextPartNum_;
    private int m_increasePartsNum;
    private int[] m_partsNumUINextPos;
    private int[] m_curPartsNumScroll;

    private int[] m_curPartsNum;
    private int[] m_nextPartsNum;
    private int m_partsNumUI_Damp;

    private Image m_HP_Guage;
    private Image m_HP_Guage_Alpha;
    private RectTransform m_HP_GuagePiston;
    private RectTransform m_HP_GuagePistonBar;

    private Image m_EP_Guage;
    private Image m_EP_Guage_Alpha;
    private RectTransform m_EP_GuagePiston;
    private RectTransform m_EP_GuagePistonBar;

    private float m_HP_PistonDefaultXPos;
    private float m_HP_PistonBarDefualtWidth;
    private float m_HP_PistonMoveDist;

    private float m_EP_PistonDefaultXPos;
    private float m_EP_PistonBarDefualtWidth;
    private float m_EP_PistonMoveDist;

    public void initPartsUI()
    {
        m_partsGetEffect = new Animator[4];
        m_partsNumUI = new RectTransform[4][];
        m_curPartsNum = new int[4];
        m_nextPartsNum = new int[4];
        m_partsNumUINextPos = new int[4];
        m_curPartsNumScroll = new int[4];

        m_curPartsNum_ = 0;
        m_nextPartNum_ = 0;
        m_partsNumUI_Damp = 100;

        for (int i = 0; i < m_partsGetEffect.Length; ++i)
            m_partsGetEffect[i] = GameObject.Find(OBJECT_NAME.PARTS_GET_EFFECT + i).GetComponent<Animator>();

        for (int i = 0; i < m_partsNumUI.Length; ++i)
            m_partsNumUI[i] = new RectTransform[3];

        for (int i = 0; i < m_partsNumUI.Length; ++i)
            for (int k = 0; k < m_partsNumUI[i].Length; ++k)
                m_partsNumUI[i][k] = GameObject.Find(OBJECT_NAME.PARTS_NUMBER + i + k).GetComponent<RectTransform>();

        for (int i = 0; i < m_curPartsNum.Length; ++i)
        {
            m_curPartsNum[i] = 0;
            m_nextPartsNum[i] = 0;
            m_partsNumUINextPos[i] = 0;
            m_curPartsNumScroll[i] = 0;
        }
    }
    public void initHP_Guage()
    {
        m_HP_Guage = GameObject.Find(OBJECT_NAME.HP_GUAGE).GetComponent<Image>();
        m_HP_Guage_Alpha = GameObject.Find(OBJECT_NAME.HP_GUAGE_ALPHA).GetComponent<Image>();
        m_HP_GuagePiston = GameObject.Find(OBJECT_NAME.HP_GUAGE_PISTON).GetComponent<RectTransform>();
        m_HP_GuagePistonBar = GameObject.Find(OBJECT_NAME.HP_GUAGE_PISTON_BAR).GetComponent<RectTransform>();

        m_HP_PistonDefaultXPos = m_HP_GuagePiston.localPosition.x;
        m_HP_PistonBarDefualtWidth = m_HP_GuagePistonBar.rect.width;
        m_HP_PistonMoveDist = 0;
    }

    public void initEP_Guage()
    {
        m_EP_Guage = GameObject.Find(OBJECT_NAME.EP_GUAGE).GetComponent<Image>();
        m_EP_Guage_Alpha = GameObject.Find(OBJECT_NAME.EP_GUAGE_ALPHA).GetComponent<Image>();
        m_EP_GuagePiston = GameObject.Find(OBJECT_NAME.EP_GUAGE_PISTON).GetComponent<RectTransform>();
        m_EP_GuagePistonBar = GameObject.Find(OBJECT_NAME.EP_GUAGE_PISTON_BAR).GetComponent<RectTransform>();

        m_EP_PistonDefaultXPos = m_EP_GuagePiston.localPosition.x;
        m_EP_PistonBarDefualtWidth = m_EP_GuagePistonBar.rect.width;
        m_EP_PistonMoveDist = 0;
    }

    public void setHP_Guage(float percentage)
    {
        m_HP_Guage.fillAmount = percentage;
        float dist = (1 - percentage) * 200;

        m_HP_PistonMoveDist = dist;
    }

    public void setEP_Guage(float percentage)
    {
        m_EP_Guage.fillAmount = percentage;
        float dist = (1 - percentage) * 200;

        m_EP_PistonMoveDist = dist;
    }

    public IEnumerator setGuageAnimation()
    {
        while (true)
        {
            float xPos = Mathf.Lerp(m_HP_GuagePiston.localPosition.x, m_HP_PistonDefaultXPos - m_HP_PistonMoveDist, Time.deltaTime);
            float width = Mathf.Lerp(m_HP_GuagePistonBar.rect.width, m_HP_PistonBarDefualtWidth + m_HP_PistonMoveDist, Time.deltaTime);
            float percentage = Mathf.Lerp(m_HP_Guage_Alpha.fillAmount, m_HP_Guage.fillAmount, Time.deltaTime);

            m_HP_GuagePiston.localPosition = new Vector3(xPos, m_HP_GuagePiston.localPosition.y, 0);
            m_HP_GuagePistonBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            m_HP_Guage_Alpha.fillAmount = percentage;

            xPos = Mathf.Lerp(m_EP_GuagePiston.localPosition.x, m_EP_PistonDefaultXPos + m_EP_PistonMoveDist, Time.deltaTime);
            width = Mathf.Lerp(m_EP_GuagePistonBar.rect.width, m_EP_PistonBarDefualtWidth + m_EP_PistonMoveDist, Time.deltaTime);
            percentage = Mathf.Lerp(m_EP_Guage_Alpha.fillAmount, m_EP_Guage.fillAmount, Time.deltaTime);

            m_EP_GuagePiston.localPosition = new Vector3(xPos, m_EP_GuagePiston.localPosition.y, 0);
            m_EP_GuagePistonBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            m_EP_Guage_Alpha.fillAmount = percentage;
            yield return null;
        }
    }

    public void increasePartsNum(int number)
    {
        m_nextPartNum_ += number;
        m_increasePartsNum = m_nextPartNum_ - m_curPartsNum_;
    }

    public IEnumerator checkPartsNum()
    {
        int nextNum = 0;
        while (true)
        {
            if (m_increasePartsNum > 0)
            {
                if(m_increasePartsNum > 100)
                {
                    int increaseNum = m_increasePartsNum / 100;
                    m_increasePartsNum -= increaseNum * 100;

                    nextNum = ((m_curPartsNum_ % 1000) / 100) + increaseNum;
                    increasePartsNumUI(2, nextNum, increaseNum);
                    m_curPartsNum_ += increaseNum * 100;
                }

                if (m_increasePartsNum > 10)
                {
                    int increaseNum = m_increasePartsNum / 10;
                    m_increasePartsNum -= increaseNum * 10;

                    nextNum = ((m_curPartsNum_ % 100) / 10) + increaseNum;
                    increasePartsNumUI(1, nextNum, increaseNum);
                    m_curPartsNum_ += increaseNum * 10;
                }
                
                nextNum = (m_curPartsNum_ % 10) + m_increasePartsNum;
                increasePartsNumUI(0, nextNum, m_increasePartsNum);
                m_curPartsNum_ = m_nextPartNum_;
            }
            m_increasePartsNum = 0;
            yield return null;
        }
    }

    public void increasePartsNumUI(int index, int nextNum, int increasNum)
    {
        if(InGameMgr.getInstance().getMainUI().activeSelf == true)
            m_partsGetEffect[index].SetTrigger("active"); 
        m_partsNumUINextPos[index] += increasNum * m_partsNumUI_Damp;
        if (nextNum > 9) 
        {
            scrollNumberUI(index);

            int count = 1;
            for (int i = 0; i < index + 1; ++i)
                count *= 10;

            increasNum = nextNum / 10;
            nextNum = ((m_curPartsNum_ % (count * 10)) / count) + increasNum;

            increasePartsNumUI(index + 1, nextNum, increasNum);
        }
    }

    public void scrollNumberUI(int index)
    {
        if (m_curPartsNumScroll[index] == 0)
            m_curPartsNumScroll[index] = 1;
        else
            m_curPartsNumScroll[index] = 0;
        m_partsNumUI[index][m_curPartsNumScroll[index] + 1].localPosition -= Vector3.up * 2000;
    }
    
    public IEnumerator setPartNumUIAnimation()
    {
        while(true)
        {
            for (int i = 0; i < m_partsNumUI.Length; ++i)
            {
                m_partsNumUI[i][0].localPosition = Vector3.Lerp(m_partsNumUI[i][0].localPosition,
                  new Vector3(m_partsNumUI[i][0].localPosition.x, m_partsNumUINextPos[i], m_partsNumUI[i][0].localPosition.z), Time.deltaTime * 5f);
            }
            yield return null;
        }
    }
}
