using UnityEngine;
using System.Collections;

public class ProjectMgr : MonoBehaviour
{
    private CHARACTER.TYPE m_ownCharacterType;
    private CHARACTER.TYPE m_otherCharacterType;

    private int m_ownID;
    private int m_otherID;
    private bool m_isHost;
    private int m_hostID;
    private string m_ownName;

    private bool[] m_skillUseble;

    //마우스 락
    private bool m_mouseLock = false;
    //소개 영상
    private GameObject m_introduceVideo;
    private float m_standByTime;
    private bool m_playIntroduceVideo;

    //저장할 정보
    private int m_mouseSensitive;
    private int m_partsNum;
    private static ProjectMgr m_instance;

    void Awake()
    {
        m_instance = this;
        m_isHost = false;
        m_skillUseble = new bool[4];

        for (int i = 0; i < m_skillUseble.Length; ++i)
            m_skillUseble[i] = false;

        m_standByTime = 0f;
        m_playIntroduceVideo = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_mouseSensitive = 10;
        m_partsNum = 0;
    }

    public static ProjectMgr getInstance()
    {
        if (m_instance == null)
            return null;
        return m_instance;
    }
    //setter
    public void setOwnCharacterType(CHARACTER.TYPE character) { m_ownCharacterType = character; }
    public void setOtherCharacterType(CHARACTER.TYPE character) { m_otherCharacterType = character; }

    public void setOwnID(int pID) { m_ownID = pID; }
    public void setOtherID(int pID) { m_otherID = pID; }

    public void setOwnName(string name)
    {
        m_ownName = name;
        InGameMgr.getInstance().getUserInfo(USER_INFO.TYPE.OWN).setUserName(m_ownName);
    }
    public void setHost()
    {
        if (m_ownID < m_otherID)
        {
            m_isHost = true;
            m_hostID = m_ownID;
        }
        else
        {
            m_isHost = false;
            m_hostID = m_otherID;
        }
    }
    public void setActiveSkill(bool active, int type) { m_skillUseble[type] = active; }
    public void setPartsNum(int num) { m_partsNum = num; }
    //getter
    public int getOwnID() { return m_ownID; }
    public int getOtherID() { return m_otherID; }
    public string getOwnName() { return m_ownName; }
    public bool[] skillUseble() { return m_skillUseble; }

    public bool isHost() { return m_isHost; }
    public int getHostID() { return m_hostID; }
    public CHARACTER.TYPE getOwnCharacterType() { return m_ownCharacterType; }
    public CHARACTER.TYPE getOtherCharacterType() { return m_otherCharacterType; }
    public int getPartsNum() { return m_partsNum; }

    public void Update()
    {
        checkUserInput();
    }
    public void mouseLock()
    {
        if (m_mouseLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            m_mouseLock = !m_mouseLock;
        }
    }

    public void setMouseLock(bool isLock) { m_mouseLock = isLock; }

    public void introduceViedo()
    {
        if (m_introduceVideo != null)
        {
            m_standByTime += Time.deltaTime;
            if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                m_standByTime = 0f;
                if (m_playIntroduceVideo == true)
                {
                    m_introduceVideo.GetComponent<PrintVideo>().m_movieTexture.Pause();
                    m_playIntroduceVideo = false;
                    m_introduceVideo.SetActive(false);
                    InGameMgr.getInstance().exitGame();
                }
            }
            if (m_standByTime > 300)
            {
                if (m_playIntroduceVideo == false)
                {
                    m_introduceVideo.SetActive(true);
                    m_introduceVideo.GetComponent<PrintVideo>().m_movieTexture.Play();
                    m_playIntroduceVideo = true;
                }
            }
        }
        else
        {
            m_introduceVideo = GameObject.Find(OBJECT_NAME.INTRODUCE_VIREO);
            if (m_introduceVideo != null)
            {
                m_introduceVideo.GetComponent<PrintVideo>().m_movieTexture.Pause();
                m_introduceVideo.SetActive(false);
            }
        }
    }

    public void checkUserInput()
    {
        //introduceViedo();
        mouseLock();
    }

    public int getMouseSensitive() { return m_mouseSensitive; }

    public void increaseMouseSensitive()
    {
        if (m_mouseSensitive < 20)
            m_mouseSensitive += 1;
    }
    public void decreaseMouseSensitive()
    {
        if(m_mouseSensitive > 0)
            m_mouseSensitive -= 1;
    }
}
