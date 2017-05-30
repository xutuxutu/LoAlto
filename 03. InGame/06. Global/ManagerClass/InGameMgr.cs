#define SAM_SPARKY
//#define SAM_SAM
//#define SPARKY_SAM
//#define SPARKY_SPARKY
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InGameMgr : MonoBehaviour
{
    //최대 유저 수
    private static int MAX_USER_NUMBER;
    //두 캐릭터의 프리팹 정보
    private GameObject[] m_userInfo;
    //캐릭터 컨트롤 스크립트
    private CharacterCtrl_Own m_ownCharacterCtrl;
    private CharacterCtrl_Other m_otherCharacterCtrl;
    //캐릭터 카메라 관련
    private GameObject m_characterCamera;
    private GameObject m_characterViewTarget;
    //죽음 관련
    private bool[] m_retry;
    //죽음 연출 카메라 관련
    private Image m_gameOverUI;
    private Image m_gameOverWaitOtherUI;
    private UnityEngine.UI.Button[] m_gameOverButtom;
    private GameObject m_exitUI;
    private GameObject m_deadViewCameraCtrl;
    private GameObject m_deadViewCameraTarget;
    private GameObject m_deadViewCamera;
    //UI관련
    private bool m_isActiveUI;
    private GameObject m_dialogueScreen;
    private Image m_dialogueLayOut;
    private GameObject m_mainUI;
    private GameObject m_questUI;
    private Text m_errorMessage;
    //크리쳐 웨이포인트
    private GameObject[] m_wayPoints;
    //캐릭터 시작 위치 관련
    public Transform startPos_Sam;
    public Transform startPos_Sparky;
    //게임 시작 관련
    private bool m_isStart = false;
    //싱글톤 관련
    private static InGameMgr m_instance;
    public static InGameMgr getInstance()
    {
        if (m_instance == null)
            return null;

        return m_instance;
    }

    void Awake()
    {
        //인스턴스 설정
        m_instance = this;
        //변수 초기화
        init();
        //매니저 클래스 세팅
        setManagerObject();
        //캐릭터 및 유저 정보 초기화
        initCharacterInfo();
        //죽음 연출 카메라 설정.
        setDeadCamera();

        ProjectMgr.getInstance().setMouseLock(true);
    }
    public void init()
    {
        //UI 세팅
        m_isActiveUI = true;
        m_dialogueScreen = GameObject.Find(OBJECT_NAME.DIALOGUE_SCREEN);
        m_dialogueLayOut = GameObject.Find(OBJECT_NAME.DIALOGUE_LAYOUT).GetComponent<Image>();
        m_gameOverUI = GameObject.Find(OBJECT_NAME.GAME_OVER_UI).GetComponent<Image>();
        m_gameOverWaitOtherUI = GameObject.Find(OBJECT_NAME.GAME_OVER_WAIT_OTHER_UI).GetComponent<Image>();
        m_gameOverButtom = m_gameOverUI.GetComponentsInChildren<UnityEngine.UI.Button>();
        m_exitUI = GameObject.Find(OBJECT_NAME.EXIT_UI);
        m_questUI = GameObject.Find(OBJECT_NAME.QUEST_UI);
        m_errorMessage = GameObject.Find(OBJECT_NAME.ERROR_MESSGE).GetComponent<Text>();
        m_gameOverUI.gameObject.SetActive(false);
        m_exitUI.SetActive(false);
        m_errorMessage.canvasRenderer.SetAlpha(0f);
        //최대 유저 수 설정
        MAX_USER_NUMBER = 2;
        m_userInfo = new GameObject[MAX_USER_NUMBER];
        m_retry = new bool[MAX_USER_NUMBER];
        for(int i = 0; i < MAX_USER_NUMBER; ++i)
        {
            m_retry[i] = false;
        }

        // Waypint 정보 얻기
        GameObject[] wayArray = GameObject.FindGameObjectsWithTag(TAG.WAYPOINT);
        m_wayPoints = new GameObject[wayArray.Length];
        for (int i = 0; i < wayArray.Length; ++i)
        {
            String name = wayArray[i].name;
            int token = name.IndexOf("_");
            String name2 = name.Substring(0, token + 1);
            name = name.Replace(name2, "");
            int index = Int32.Parse((name));
            m_wayPoints[index] = wayArray[i];
        }
    }
    // Use this for initialization
    void Start()
    {
#if SERVER_OFF
        setCharacterStartPosition();
        m_isStart = true;
#endif
        StartCoroutine("startGameEvent");
    }

    public void setManagerObject()
    {
        if (ProjectMgr.getInstance() == null)
        {
            //프로젝트 매니저 생성.
            GameObject projectMgr = Resources.Load(PREFAB_PATH.PROJECT_MANAGER, typeof(GameObject)) as GameObject;
            projectMgr = GameObject.Instantiate(projectMgr, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
            Debug.Log("createProjectManager");
#if SERVER_ON && TEST
            ProjectMgr.getInstance().setOwnID(-1);
            ProjectMgr.getInstance().setOtherID(-1);
#elif SERVER_OFF
        ProjectMgr.getInstance().setOwnID(0);
        ProjectMgr.getInstance().setOtherID(1);
#endif
        }
        ProjectMgr.getInstance().transform.parent = transform.parent;

        if (InGameServerMgr.getInstance() == null)
        {
            GameObject networkMgr = Resources.Load(PREFAB_PATH.INGAME_NETWORK_MANAGER, typeof(GameObject)) as GameObject;
            networkMgr = GameObject.Instantiate(networkMgr, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
            networkMgr.transform.parent = transform.parent;
            Debug.Log("createNerworkManager");
        }
        else
        {
            InGameServerMgr.getInstance().transform.parent = transform.parent;
            InGameServerMgr.getInstance().resetPacketQueue();
        }
    }

    private void initCharacterInfo()
    {
        CHARACTER.TYPE ownCharacterType = CHARACTER.TYPE.NONE;
        CHARACTER.TYPE otherCharacterType = CHARACTER.TYPE.NONE;
        string ownCharacterPrefab = "";
        string otherCharacterPrefab = "";
        string mainUI = "";
#if TEST
#if SAM_SPARKY
        ownCharacterType = CHARACTER.TYPE.SAM;
        otherCharacterType = CHARACTER.TYPE.SPARKY;
#elif SAM_SAM
        ownCharacterType = CHARACTER.TYPE.SAM;
        otherCharacterType = CHARACTER.TYPE.SAM;
#elif SPARKY_SAM
        ownCharacterType = CHARACTER.TYPE.SPARKY;
        otherCharacterType = CHARACTER.TYPE.SAM;
#elif SPARKY_SPARKY
        ownCharacterType = CHARACTER.TYPE.SPARKY;
        otherCharacterType = CHARACTER.TYPE.SPARKY;
#endif
        ProjectMgr.getInstance().setOwnCharacterType(ownCharacterType);
        ProjectMgr.getInstance().setOtherCharacterType(otherCharacterType);
#else
        ownCharacterType = ProjectMgr.getInstance().getOwnCharacterType();
        otherCharacterType = ProjectMgr.getInstance().getOtherCharacterType();
#endif

        switch (ownCharacterType)
        {
            case CHARACTER.TYPE.SAM :
                ownCharacterPrefab = PREFAB_PATH.CHARACTER_OWN_SAM;
                mainUI = PREFAB_PATH.CHARACTER_SAM_MAIN_UI;
                break;
            case CHARACTER.TYPE.SPARKY :
                ownCharacterPrefab = PREFAB_PATH.CHARACTER_OWN_SPARKY;
                mainUI = PREFAB_PATH.CHARACTER_SPARKY_MAIN_UI;
                break;
        }

        switch (otherCharacterType)
        {
            case CHARACTER.TYPE.SAM:
                otherCharacterPrefab = PREFAB_PATH.CHARACTER_OTHER_SAM;
                break;
            case CHARACTER.TYPE.SPARKY:
                otherCharacterPrefab = PREFAB_PATH.CHARACTER_OTHER_SPARKY;
                break;
        }
        //UI생성
        m_mainUI = Resources.Load(mainUI, typeof(GameObject)) as GameObject;
        m_mainUI = GameObject.Instantiate(m_mainUI) as GameObject;
        //캐릭터 생성
        m_userInfo[(int)USER_INFO.TYPE.OWN] = Resources.Load(ownCharacterPrefab) as GameObject;
        m_userInfo[(int)USER_INFO.TYPE.OWN] = GameObject.Instantiate(m_userInfo[(int)USER_INFO.TYPE.OWN]) as GameObject;

        m_userInfo[(int)USER_INFO.TYPE.OTHER] = Resources.Load(otherCharacterPrefab) as GameObject;
        m_userInfo[(int)USER_INFO.TYPE.OTHER] = GameObject.Instantiate(m_userInfo[(int)USER_INFO.TYPE.OTHER]) as GameObject;

        m_characterCamera = Resources.Load(PREFAB_PATH.CHARACTER_CAMERA, typeof(GameObject)) as GameObject;
        m_characterCamera = GameObject.Instantiate(m_characterCamera);
        m_characterCamera.transform.parent = m_userInfo[(int)USER_INFO.TYPE.OWN].transform;
        m_characterCamera.name = OBJECT_NAME.CAMERA;
        if (ownCharacterType == CHARACTER.TYPE.SAM)
            m_characterCamera.GetComponent<Camera>().fieldOfView = 50;

        m_characterViewTarget = GameObject.Find(OBJECT_NAME.VIEW_TARGET);
        m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<UserInput>().setControlCamera(m_userInfo[(int)USER_INFO.TYPE.OWN].transform, m_characterCamera.transform, m_characterViewTarget.transform, 3f, 20);

        //유저 정보 설정
        m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<UserInfo>().setUserType(USER_INFO.TYPE.OWN);
        m_userInfo[(int)USER_INFO.TYPE.OTHER].GetComponentInChildren<UserInfo>().setUserType(USER_INFO.TYPE.OTHER);

        //캐릭터 컨트롤러 적용.
        m_ownCharacterCtrl = m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<CharacterCtrl_Own>();
        m_otherCharacterCtrl = m_userInfo[(int)USER_INFO.TYPE.OTHER].GetComponentInChildren<CharacterCtrl_Other>();

        m_ownCharacterCtrl.init();
        m_otherCharacterCtrl.init();

        if(Application.loadedLevel == (int)SCENE.INDEX.Ar1St4)
        {
            m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponent<UnityEngine.AI.NavMeshObstacle>().carving = false;
            //m_userInfo[(int)USER_INFO.TYPE.OTHER].GetComponent<NavMeshObstacle>().carving = false;
        }
#if SERVER_OFF
        ProjectMgr.getInstance().setHost();
        m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<UserInfo>().setUserID(0);
        m_userInfo[(int)USER_INFO.TYPE.OTHER].GetComponentInChildren<UserInfo>().setUserID(1);
#endif
#if DB_ON && SERVER_OFF
        StartCoroutine("analysePacket_HTTP");
        HTTPManager.getInstance().SEND_HTTP(NET_HTTP.URL_NAME.CHARACTER_DATA);
#elif SERVER_OFF
        testBuild();
#endif

#if SERVER_ON
        //StartCoroutine("analysePacket_HTTP");
        //DB에서 캐릭터 값 요청.
        //HTTPManager.getInstance().SEND_HTTP(NET_HTTP.URL_NAME.CHARACTER_DATA);
        testBuild();
#endif
    }

    public void testBuild()
    {
        //ProjectMgr.getInstance().setActiveSkill(true, 0);
        //ProjectMgr.getInstance().setActiveSkill(true, 1);
        bool[] skillUseble = ProjectMgr.getInstance().skillUseble();
        Debug.Log("스킬 사용 : " + skillUseble[0]);
        Debug.Log("스킬 사용 : " + skillUseble[1]);
        Debug.Log("set Character State");

        switch (ProjectMgr.getInstance().getOwnCharacterType())
        {
            case CHARACTER.TYPE.SAM:
//initState(float maxHP, float maxEP, float atkPoint_1, float atkPoint_2, float atkPoint_3, float defPoint, float RUT, float runSpd, float jSpd, float gravity, float dgSPD, int partsNum)
                m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<Character_Own_Sam>().initState(650, 200, 25, 28, 33, 10, 7, 50, 6, 5, 10, 9);
                m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<Character_Own_Sam>().initSkillInfo(5, 8, 5, 5, skillUseble[0], skillUseble[1], skillUseble[2], skillUseble[3]);
                break;
            case CHARACTER.TYPE.SPARKY:
//initState(float maxHP, float maxEP, float atkPoint, float defPoint, float RUT, float runSpd, float walkSpd, float jSpd, float gravity, float dgSPD, int partsNum)
                m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<Character_Own_Sparky>().initState(450, 200, 8, 5, 7, 10, 6, 2, 5, 10, 8);
                m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<Character_Own_Sparky>().initSkillInfo(6, 6, 5, 5, skillUseble[0], skillUseble[1], skillUseble[2], skillUseble[3]);
                break;
        }
        switch(ProjectMgr.getInstance().getOtherCharacterType())
        {
            case CHARACTER.TYPE.SAM:
//initState(float maxHP, float maxEP, float atkPoint, float defPoint, float RUT, float runSpd, float jSpd, float gravity, float dgSPD)
                m_userInfo[(int)USER_INFO.TYPE.OTHER].GetComponentInChildren<Character_Other_Sam>().initState(400, 50, -1, 0, 5, 5, 5, 5, 10, 10);
                break;
            case CHARACTER.TYPE.SPARKY:
//initState(float maxHP, float maxEP, float atkPoint, float defPoint, float RUT, float runSpd, float walkSpd, float jSpd, float gravity, float dgSPD)
                m_userInfo[(int)USER_INFO.TYPE.OTHER].GetComponentInChildren<Character_Other_Sparky>().initState(200, 100, -1, 0, 5, 5, 5, 2, 5, 10, 10);
                break;
        }

        int partsNum = ProjectMgr.getInstance().getPartsNum();
        m_ownCharacterCtrl.setPartsNum(partsNum);

#if SERVER_ON
        checkServerConnect();
#endif
    }

    public void receiveGameData(List<NET_HTTP.RECV.CHARACTER_INFO> charData, List<NET_HTTP.RECV.CHARACTER_NORMAL_ATTACK_INFO> attackInfo, List<NET_HTTP.RECV.CHARACTER_SKILL_DATA> skillData)
    {
        Debug.Log("Recv DataBase Value");
        DB_DATA.CHARACTER_INFO[] charStatus = new DB_DATA.CHARACTER_INFO[charData.Count];
        DB_DATA.CHARACTER_NORMAL_ATTACK_DATA[] charAtkInfo = new DB_DATA.CHARACTER_NORMAL_ATTACK_DATA[attackInfo.Count];
        DB_DATA.CHARACTER_SKILL_DATA[] charSkillData = new DB_DATA.CHARACTER_SKILL_DATA[skillData.Count];

        //기본 스테이터스 초기화
        for (int i = 0; i < charStatus.Length; ++i)
        {
            charStatus[i] = HTTPManager.getInstance().parsingPacket().PARS_CHARACTER_INFO_FC(charData[i]);

            if (charStatus[i].CHAR_TYPE == ProjectMgr.getInstance().getOwnCharacterType())
                m_ownCharacterCtrl.initState(charStatus[i]);
            
            else if (charStatus[i].CHAR_TYPE == ProjectMgr.getInstance().getOtherCharacterType())
                m_otherCharacterCtrl.initState(charStatus[i]);
            
        }
        //공격력 초기화
        for (int i = 0; i < charAtkInfo.Length; ++i)
        {
            charAtkInfo[i] = HTTPManager.getInstance().parsingPacket().PARS_CHARACTER_NORMAL_ATTACK_DATA_FC(attackInfo[i]);

            if (charAtkInfo[i].CHAR_TYPE == ProjectMgr.getInstance().getOwnCharacterType())
                m_ownCharacterCtrl.setNormalAttackPoint(charAtkInfo[i].ATCK_TYPE, (int)charAtkInfo[i].ATCK_PONT);
        }
        //스킬 정보 초기화
        bool[] skillUseble = ProjectMgr.getInstance().skillUseble();
        for (int i = 0; i < charSkillData.Length; ++i)
        {
            charSkillData[i] = HTTPManager.getInstance().parsingPacket().PARS_CHARACTER_SKILL_DATA_FC(skillData[i]);

            if (charSkillData[i].CHAR_TYPE == ProjectMgr.getInstance().getOwnCharacterType()) {
                m_ownCharacterCtrl.initSkillStatus(charSkillData[i], skillUseble[charSkillData[i].SKIL_TYPE]);
                //m_ownCharacterCtrl.initSkillStatus(charSkillData[i], skillUseble[charSkillData[i].SKIL_TYPE]);
            }
            else if (charSkillData[i].CHAR_TYPE == ProjectMgr.getInstance().getOtherCharacterType())
                m_otherCharacterCtrl.initSkillStatus(charSkillData[i], true);
        }

        //파츠 갯수 설정
        int partsNum = ProjectMgr.getInstance().getPartsNum();
        m_ownCharacterCtrl.setPartsNum(partsNum);

#if SERVER_ON
        checkServerConnect();
#endif
    }

    public void checkServerConnect()
    {
        //---------------------------------------------------------------------------
        Debug.Log("recvGameData / Try To Connect");
        if (InGameServerMgr.getInstance().isConnected() == false)
            InGameServerMgr.getInstance().connectToServer();
        else
            InGameServerMgr.getInstance().sendStartPacket();

        StartCoroutine("analysePacket_Ingame");
        StartCoroutine("sendTransformInfo");
    }

    public void resetSceneData()
    {
        StopAllCoroutines();
        m_isStart = false;
        InGameServerMgr.getInstance().resetPacketQueue();
    }

    public IEnumerator analysePacket_HTTP()
    {
        Debug.Log("analysePacket : HTTP");
        while (true)
        {
            HTTPManager.getInstance().analysePacket();
            yield return null;
        }
    }

    public IEnumerator analysePacket_Ingame()
    {
        Debug.Log("analysePacket : InGame");
        while (true)
        {
            InGameServerMgr.getInstance().analysePacket();
            yield return null;
        }
    }

    public void setStartGame(bool isStart) { m_isStart = isStart; }
    public bool isStart() { return m_isStart; }

    public GameObject getCharacter(USER_INFO.TYPE type) { return m_userInfo[(int)type]; }
    public UserInfo getUserInfo(USER_INFO.TYPE userType) { return m_userInfo[(int)userType].GetComponentInChildren<UserInfo>(); }
    public GameObject getWayPoint(int _index)
    {
        if (m_wayPoints.Length > _index)
        { return m_wayPoints[_index]; }
        return null;
    }

    public void setCharacterStartPosition()
    {
        if (ProjectMgr.getInstance().getOwnCharacterType() == CHARACTER.TYPE.SAM)
        {
            m_ownCharacterCtrl.setStartTransform(startPos_Sam);
            m_otherCharacterCtrl.setStartTransform(startPos_Sparky);
        }
        else
        {
            m_ownCharacterCtrl.setStartTransform(startPos_Sparky);
            m_otherCharacterCtrl.setStartTransform(startPos_Sam);
        }
#if SERVER_OFF
        m_userInfo[(int)USER_INFO.TYPE.OTHER].SetActive(false);
#endif
    }

    //게임 시작후 오브젝트 및 몬스터 초기화
    //-------------------------------------------------------------
    public IEnumerator startGameEvent()
    {
        ObjectMgr.getInstance().initObject();

        while (m_isStart == false)
            yield return null;

        Debug.Log("start All Event");
        CreatureMgr.getInstance().CreaturesActiveAIUpdate();
        GameObject.Find(OBJECT_NAME.GAME_EVENT).GetComponent<GameEvent>().startEvent();
    }
    //-------------------------------------------------------------
    //캐릭터 관련 함수 호출용 getter
    //-------------------------------------------------------------
    public CharacterCtrl_Own getOwnCharacterCtrl() { return m_ownCharacterCtrl; }
    public CharacterCtrl_Other getOtherCharacterCtrl()
    {
        return m_otherCharacterCtrl;
    }
    //-------------------------------------------------------------
    //네트워크 패킷 송신용 함수
    //-------------------------------------------------------------
    public Vector3 getOwnCharacterPosition() { return m_userInfo[(int)USER_INFO.TYPE.OWN].transform.position; }
    public Quaternion getOwnCharacterRotation() { return m_userInfo[(int)USER_INFO.TYPE.OWN].transform.rotation; }
    //-------------------------------------------------------------
    //캐릭터 죽음 & 부활 관련
    public bool checkAllCharacterDead()
    {
#if SERVER_ON
        if (getOwnCharacterCtrl().isDie() == true && getOtherCharacterCtrl().isDie() == true)
            return true;
        return false;
#else
        return getOwnCharacterCtrl().isDie();
#endif
    }
    public void printGameOverUI(bool isPrint)
    {
        m_gameOverWaitOtherUI.canvasRenderer.SetAlpha(0f);
        ProjectMgr.getInstance().setMouseLock(false);
        m_gameOverUI.gameObject.SetActive(isPrint);
    }
    public void setDeadCamera()
    {
        m_deadViewCameraCtrl = GameObject.Find(OBJECT_NAME.DEAD_VIEW_CAMERA_CTRL);
        m_deadViewCameraTarget = GameObject.Find(OBJECT_NAME.DEAD_VIEW_CAMERA_TARGET);
        m_deadViewCamera = m_deadViewCameraCtrl.GetComponentInChildren<Camera>().gameObject;
        m_deadViewCameraCtrl.SetActive(false);
    }
    public void setDeadCameraTransform()
    {
        m_mainUI.SetActive(false);
        m_questUI.SetActive(false);

        m_deadViewCameraCtrl.transform.position = m_userInfo[(int)USER_INFO.TYPE.OWN].transform.position;
        m_deadViewCameraCtrl.transform.rotation = m_userInfo[(int)USER_INFO.TYPE.OWN].transform.rotation;

        m_deadViewCamera.transform.localPosition = m_characterCamera.transform.localPosition;
        m_deadViewCamera.transform.rotation = m_characterCamera.transform.rotation;
        m_deadViewCameraTarget.transform.rotation = m_characterViewTarget.transform.rotation;

        Invoke("printDeadDirection", 1f);
    }
    public void printDeadDirection()
    {
        m_characterCamera.gameObject.SetActive(false);
        m_deadViewCameraCtrl.gameObject.SetActive(true);
        m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<UserInput>().setControlCamera(m_deadViewCameraCtrl.transform, m_deadViewCamera.transform, m_deadViewCameraTarget.transform, 6f, 1);
        m_deadViewCamera.GetComponent<Animator>().SetTrigger("active");
    }

    public void printRevivalDirection()
    {
        setActiveMainUI(true);

        m_questUI.SetActive(true);
        m_deadViewCamera.GetComponent<Animator>().SetTrigger("active");
        m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<UserInput>().setControlCamera(m_deadViewCameraCtrl.transform, m_deadViewCamera.transform, m_deadViewCameraTarget.transform, 3f, 3);
        Invoke("setCharacterCameraTransform", 3f);
    }

    private void setCharacterCameraTransform()
    {
        m_characterCamera.transform.localPosition = m_deadViewCamera.transform.localPosition;
        m_characterCamera.transform.localRotation = m_deadViewCamera.transform.localRotation;

        m_characterViewTarget.transform.localRotation = m_deadViewCameraTarget.transform.localRotation;
        m_deadViewCameraCtrl.SetActive(false);
        m_characterCamera.SetActive(true);
        m_userInfo[(int)USER_INFO.TYPE.OWN].GetComponentInChildren<UserInput>().setControlCamera(m_userInfo[(int)USER_INFO.TYPE.OWN].transform, m_characterCamera.transform, m_characterViewTarget.transform, 3f, 20);
    }

    public bool isActiveExitUI() { return m_exitUI.activeSelf; }
    public void activeExitUI()
    {
        ProjectMgr.getInstance().setMouseLock(false);
        m_ownCharacterCtrl.setIsActive(false);
        m_ownCharacterCtrl.setCameraCtrlLock(true);
        m_exitUI.SetActive(true);
    }

    public void deActiveExitUI()
    {
        ProjectMgr.getInstance().setMouseLock(true);
        m_ownCharacterCtrl.setIsActive(true);
        m_ownCharacterCtrl.setCameraCtrlLock(false);
        m_exitUI.SetActive(false);
    }

    public void pressRetryButton(bool retry)
    {
        for (int i = 0; i < m_gameOverButtom.Length; ++i)
            m_gameOverButtom[i].interactable = false;

#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_GAME_RETRY_EM, retry);
#endif
        if (retry == true)
        {
            m_retry[0] = retry;
#if SERVER_OFF
        if (m_retry[0] == true)
            retryGame();
#elif SERVER_ON
            if (m_retry[0] == true && m_retry[1] == true)
                retryGame();
            else
                m_gameOverWaitOtherUI.CrossFadeAlpha(1, 0.5f, false);
#endif
        }
        else
        {
#if SERVER_OFF
            exitGame();
            return;
#endif
        }
    }

    public void recvRetryInfo(int pID, bool retry)
    {
        int type = 1;
        if (ProjectMgr.getInstance().getOwnID() == pID)
            type = 0;

        if (retry == true)
            m_retry[type] = true;
        else
        {
            exitGame();
            return;
        }

        if (m_retry[0] == true && m_retry[1] == true)
            retryGame();
    }

    public void exitGame()
    {
#if SERVER_ON
        InGameServerMgr.getInstance().disConnectServer();
        Application.LoadLevel(0);
#elif SERVER_OFF
        Application.Quit();
#endif
    }
    public void retryGame()
    {
        StopAllCoroutines();
        InGameServerMgr.getInstance().resetPacketQueue();
        ProjectMgr.getInstance().transform.parent = null;
        InGameServerMgr.getInstance().transform.parent = null;

        DontDestroyOnLoad(ProjectMgr.getInstance().gameObject);
        DontDestroyOnLoad(InGameServerMgr.getInstance().gameObject);
        Application.LoadLevel(Application.loadedLevel);
    }
    //------------------------------------------------------------------
    //UI 관련
    public void setActiveWholeUI()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_isActiveUI = !m_isActiveUI;
            m_dialogueScreen.SetActive(m_isActiveUI);
            m_questUI.SetActive(m_isActiveUI);
            m_mainUI.SetActive(m_isActiveUI);
            m_dialogueLayOut.canvasRenderer.SetAlpha(0f);
        }
    }
    public void setActiveMainUI(bool isActive)
    {
        m_mainUI.SetActive(isActive);
        if (isActive == true)
            InGameMgr.getInstance().getOwnCharacterCtrl().resetSkillAnimation();
    }
    public void printErrorMessage(string errorMessage, bool overrap)
    {
        if (overrap == false)
        {
            if (m_errorMessage.canvasRenderer.GetAlpha() != 0)
                return;
        }

        m_errorMessage.CrossFadeAlpha(1f, 0.6f, false);
        m_errorMessage.text = errorMessage;
        Invoke("fadeInErrorMessage", 1.4f);
    }
    public void fadeInErrorMessage()
    {
        m_errorMessage.CrossFadeAlpha(0f, 0.5f, false);
    }
    //------------------------------------------------------------------
    public IEnumerator sendTransformInfo()
    {
        while (m_isStart == false)
            yield return null;

        while (true)
        {
            InGameServerMgr.getInstance().SendTransformInfo();
            yield return new WaitForSeconds(0.03f);
        }
    }
    //-------------------------------------------------------------
    public GameObject[] GetObjectIndex(CreatureMgr.CreatureMessage _messageType, int _messageInfo, int _index)
    {
        if (-1 == _index)
            return null;
        // MessageType 에 따른 설정
        switch (_messageType)
        {
            // Creature 상태 변환 메세지
            case CreatureMgr.CreatureMessage.SET_STATE:
                // 변환할 상태에 따른 설정
                switch ((CreatureMgr.CreatureState)_messageInfo)
                {
                    case CreatureMgr.CreatureState.NONE:
                    case CreatureMgr.CreatureState.IDLE:
                    case CreatureMgr.CreatureState.BREAKDOWN:
                    case CreatureMgr.CreatureState.DIE:
                    case CreatureMgr.CreatureState.ONLY_VALUE_ANI_STATE:
                        break;
                    case CreatureMgr.CreatureState.SPOT:
                        {
                            int index = _index / 1000;
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
             
        if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.READY:
                        {
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == _index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.MOVE:
                    case CreatureMgr.CreatureState.MOVE_RETRY:
                        {
                            int index = _index / 1000;
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.JUMP:
                        {
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == _index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.PATROL:
                        {
                            // WayPoint 얻기
                            GameObject[] _obj = new GameObject[1];
                            _obj[0] = m_wayPoints[_index];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.PROTECT:
                        {
                            if (_index == -1)
                                return null;
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == _index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                        break;
                    case CreatureMgr.CreatureState.TRACE:
                        {
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == _index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.ROAR:
                        {
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == _index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.ATTACK:
                        {
                            int index = _index / 1000;
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.HIT:
                        {
                            // 공격한 Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            int index = _index / 1000;
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.ONLY_VALUE_HIT:
                        {
                            int index = _index / 1000;
                            // 공격한 Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                    case CreatureMgr.CreatureState.CREATE:
                        {
                            // Player 얻기
                            GameObject[] _obj = new GameObject[1];
                            if (m_userInfo[0].GetComponentInChildren<UserInfo>().getUserID() == _index)
                                _obj[0] = m_userInfo[0];
                            else
                                _obj[0] = m_userInfo[1];
                            return _obj;
                        }
                }
                break;
        }
        return null;
    }

    //getter
    public GameObject getDeadViewCameraCtrl() { return m_deadViewCameraCtrl; }
    public GameObject getDeadViewCameraTaget() { return m_deadViewCameraTarget; }
    public GameObject getDeadViewCamera() { return m_deadViewCamera; }

    public GameObject getCharacterCamera() { return m_characterCamera; }
    public GameObject getCharacterViewTarget() { return m_characterViewTarget; }

    public GameObject getMainUI() { return m_mainUI; }

    void OnApplicationQuit()
    {
        //Application.CancelQuit();
    }
}