using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OutGameInputManager : MonoBehaviour
{
    private enum OUTGAME { NONE = -1, LOGIN, JOIN, DEVELPER, EXIT, CHAR_SELECT }
    private enum MESSAGE { SERVER, LOGIN, WAIT }
    private enum CHARACTER_SELECT { SELECT_HOST, SELECT_PEER }

    public PrintVideo m_staffRoll;
    private OUTGAME m_state;
    private UnityEngine.UI.Button[] m_mainButton;
    private UnityEngine.UI.Button[] m_loginButton;
    private UnityEngine.UI.Button[] m_joinButton;
    private InputField[] m_inputField_ID;
    private InputField[] m_inputField_PW;

    private GameObject m_loginUI;
    private GameObject m_joinUI;
    private GameObject m_gameMessageUI;
    public GameObject m_exitUI;
    public Sprite[] m_messageImage;
    private UnityEngine.UI.Button m_gameMsgConfirmButton;

    public GameObject m_characterSelectUI;
    public UnityEngine.UI.Button m_selectCharacterSam_UI;
    public UnityEngine.UI.Button m_selectCharacterSparky_UI;

    public Animator[] m_selectCharacterSamImage;
    public Animator[] m_selectCharacterSparkyImage;
    public Animator[] m_selectGear;
    public Toggle m_gameReadyButton;

    public Sprite[] m_startCountImage;
    public Image m_startCountUI;

    public Animator[] m_gearCenter;

    public UnityEngine.UI.Button m_selectRoomExitButton;

    private AudioSource m_audioSource;
    public AudioClip[] m_audioClip;

    private CHARACTER.TYPE m_ownSelectCharacter;
    private CHARACTER.TYPE m_otherSelectCharacter;

    private bool m_isReady;
    private bool m_gameStart;
    private int m_gameStartTime;

    private static OutGameInputManager m_instance;
    public static OutGameInputManager getInstance() { return m_instance; }

    private string m_loginPW;
    void Awake()
    {
        m_instance = this;
        initData();
        initComponent();

        m_staffRoll.stopVideo();
        m_staffRoll.gameObject.SetActive(false);
    }

    public void initData()
    {
        m_state = OUTGAME.NONE;
        m_isReady = false;
        m_gameStart = false;
        m_gameStartTime = 8;

        m_ownSelectCharacter = CHARACTER.TYPE.NONE;
        m_otherSelectCharacter = CHARACTER.TYPE.NONE;
    }

    public void initComponent()
    {
        m_audioSource = GetComponent<AudioSource>();

        m_mainButton = new UnityEngine.UI.Button[4];
        m_joinButton = new UnityEngine.UI.Button[2];
        m_loginButton = new UnityEngine.UI.Button[2];
        m_inputField_ID = new InputField[2];
        m_inputField_PW = new InputField[3];

        m_mainButton[(int)OUTGAME.LOGIN] = GameObject.Find(OBJECT_NAME.BUTTON_LOGIN).GetComponent<UnityEngine.UI.Button>();
        m_mainButton[(int)OUTGAME.JOIN] = GameObject.Find(OBJECT_NAME.BUTTON_JOIN).GetComponent<UnityEngine.UI.Button>();
        m_mainButton[(int)OUTGAME.DEVELPER] = GameObject.Find(OBJECT_NAME.BUTTON_DEVELOPER_INFO).GetComponent<UnityEngine.UI.Button>();
        m_mainButton[(int)OUTGAME.EXIT] = GameObject.Find(OBJECT_NAME.BUTTON_EXIT).GetComponent<UnityEngine.UI.Button>();

        m_inputField_ID[(int)OUTGAME.LOGIN] = GameObject.Find(OBJECT_NAME.LOGIN_ID_INPUT_FIELD).GetComponent<InputField>();
        m_inputField_PW[(int)OUTGAME.LOGIN] = GameObject.Find(OBJECT_NAME.LOGIN_PW_INPUT_FIELD).GetComponent<InputField>();

        m_inputField_ID[(int)OUTGAME.JOIN] = GameObject.Find(OBJECT_NAME.JOIN_ID_INPUT_FIELD).GetComponent<InputField>();
        m_inputField_PW[(int)OUTGAME.JOIN] = GameObject.Find(OBJECT_NAME.JOIN_PW_INPUT_FIELD).GetComponent<InputField>();
        m_inputField_PW[(int)OUTGAME.JOIN + 1] = GameObject.Find(OBJECT_NAME.JOIN_PW_INPUT_FIELD_CF).GetComponent<InputField>();

        m_joinUI = GameObject.Find(OBJECT_NAME.JOIN_UI);
        m_joinButton[0] = GameObject.Find(OBJECT_NAME.JOIN_CONFIRM_BUTTON).GetComponent<UnityEngine.UI.Button>();
        m_joinButton[1] = GameObject.Find(OBJECT_NAME.JOIN_CANCLE_BUTTON).GetComponent<UnityEngine.UI.Button>();
        m_loginButton[0] = GameObject.Find(OBJECT_NAME.LOGIN_CONFIRM_BUTTON).GetComponent<UnityEngine.UI.Button>();
        m_loginButton[1] = GameObject.Find(OBJECT_NAME.LOGIN_CANCLE_BUTTON).GetComponent<UnityEngine.UI.Button>();
        m_gameMessageUI = GameObject.Find(OBJECT_NAME.GAME_MESSAGE_UI);
        //m_gameMessageText = GameObject.Find(OBJECT_NAME.GAME_MESSAGE_TEXT).GetComponent<Text>();
        m_gameMsgConfirmButton = GameObject.Find(OBJECT_NAME.GAME_MESSAGE_CONFIRM_BUTTON).GetComponent<UnityEngine.UI.Button>();

        m_loginUI = GameObject.Find(OBJECT_NAME.LOGIN_UI);

        m_loginUI.SetActive(false);
        m_joinUI.SetActive(false);
        m_exitUI.SetActive(false);
        m_gameMessageUI.SetActive(false);

        m_gameReadyButton.interactable = false;
        m_characterSelectUI.SetActive(false);
        m_startCountUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkUserInput();
    }

    public void checkUserInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch(m_state)
            {
                case OUTGAME.JOIN :
                    if (m_inputField_ID[(int)OUTGAME.JOIN].isFocused == true)
                        m_inputField_PW[(int)OUTGAME.JOIN].Select();
                    else if (m_inputField_PW[(int)OUTGAME.JOIN].isFocused == true)
                        m_inputField_PW[(int)OUTGAME.JOIN + 1].Select();
                    else if (m_inputField_PW[(int)OUTGAME.JOIN + 1].isFocused == true)
                        m_inputField_ID[(int)OUTGAME.JOIN].Select();
                    else
                        m_inputField_ID[(int)OUTGAME.JOIN].Select();
                    break;
                case OUTGAME.LOGIN :
                    if (m_inputField_ID[(int)OUTGAME.LOGIN].isFocused == true)
                        m_inputField_PW[(int)OUTGAME.LOGIN].Select();
                    else if (m_inputField_PW[(int)OUTGAME.LOGIN].isFocused == true)
                        m_inputField_ID[(int)OUTGAME.LOGIN].Select();
                    else
                        m_inputField_ID[(int)OUTGAME.LOGIN].Select();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (m_gameMessageUI.activeSelf == true)
            {
                pressGameMessageConfirmButton();
                return;
            }

            switch (m_state)
            {
                case OUTGAME.JOIN:
                    pressJoin_CreateButton();
                    break;
                case OUTGAME.LOGIN:
                    pressLogin_LoginButton();
                    break;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_gameMessageUI.activeSelf == true)
            {
                pressGameMessageConfirmButton();
                return;
            }

            switch (m_state)
            {
                case OUTGAME.JOIN:
                    pressJoin_CancleButton();
                    break;
                case OUTGAME.LOGIN:
                    pressLogin_CancleButton();
                    break;
                case OUTGAME.DEVELPER :
                    m_staffRoll.GetComponent<PrintVideo>().stopVideo();
                    m_staffRoll.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public void printClickSound()
    {
        m_audioSource.PlayOneShot(m_audioClip[0], 1f);
    }

    public void printSelectSound()
    {
        m_audioSource.PlayOneShot(m_audioClip[1], 1f);
    }

    public void pressMainButton_Login()
    {
        m_state = OUTGAME.LOGIN;
        m_loginUI.SetActive(true);
        disableMainButton();
        printClickSound();
    }

    public void pressMainButton_Join()
    {
        m_state = OUTGAME.JOIN;
        m_joinUI.SetActive(true);
        disableMainButton();
        printClickSound();
    }

    public void pressMainButton_DeveloperInfo()
    {
        m_state = OUTGAME.DEVELPER;
        m_staffRoll.gameObject.SetActive(true);
        m_staffRoll.playVideo();
        printClickSound();
    }

    public void pressMainButton_Exit()
    {
        m_state = OUTGAME.EXIT;
        m_exitUI.SetActive(true);
        disableMainButton();
        printClickSound();
    }

    public void pressLogin_LoginButton()
    {
        string text_ID = m_inputField_ID[(int)OUTGAME.LOGIN].text;
        string text_PW = m_inputField_PW[(int)OUTGAME.LOGIN].text;

        NET_HTTP.SEND.JOIN_ACCOUNT_INFO packet;
        packet.USER_IDNT = text_ID;
        packet.USER_PSWD = text_PW;

        HTTPManager.getInstance().SEND_HTTP(packet, NET_HTTP.URL_NAME.LOGIN);
        disableLoginUIButton();
        openGameMessageUI();
        printClickSound();
    }

    public void pressLogin_CancleButton()
    {
        closeLoginUI();
        enableMainButton();
        m_state = OUTGAME.NONE;
        printClickSound();
    }

    public void pressJoin_CreateButton()
    {
        disableJoinUIButton();
        string text_ID = m_inputField_ID[(int)OUTGAME.JOIN].text;
        string text_PW = m_inputField_PW[(int)OUTGAME.JOIN].text;
        string text_PW_CF = m_inputField_PW[(int)OUTGAME.JOIN + 1].text;

        Debug.Log(text_PW);
        if (text_ID.Length < 6 || text_ID.Length > 20)
        {
            openGameMessageUI();
            //m_gameMessageText.text = "아이디는 6 ~ 20자만 가능합니다.";
            enableGameMessageConfirmButton();
            return;
        }
        if (text_PW.Length < 6 || text_PW.Length > 20)
        {
            openGameMessageUI();
            //m_gameMessageText.text = "비밀번호는 6 ~ 20자만 가능합니다.";
            enableGameMessageConfirmButton();
            return;
        }
        if (text_PW != text_PW_CF)
        {
            openGameMessageUI();
            //m_gameMessageText.text = "비밀번호가 일치하지 않습니다.";
            enableGameMessageConfirmButton();
            return;
        }

        NET_HTTP.SEND.JOIN_ACCOUNT_INFO packet;
        packet.USER_IDNT = text_ID;
        packet.USER_PSWD = text_PW;

        HTTPManager.getInstance().SEND_HTTP(packet, NET_HTTP.URL_NAME.JOIN);
        openGameMessageUI();
        printClickSound();
    }

    public void pressJoin_CancleButton()
    {
        closeJoinUI();
        enableMainButton();
        m_state = OUTGAME.NONE;
        printClickSound();
    }

    public void pressGameMessageConfirmButton()
    {
        m_gameMsgConfirmButton.gameObject.SetActive(false);
        m_gameMessageUI.SetActive(false);
        switch (m_state)
        {
            case OUTGAME.LOGIN:
                break;
            case OUTGAME.JOIN:
                enableJoinUIButton();
                break;
        }
        printClickSound();
    }

    public void pressCharacterSelect_SamButton()
    {
        Debug.Log(m_isReady);
        if (m_isReady || m_ownSelectCharacter == CHARACTER.TYPE.SPARKY || m_otherSelectCharacter == CHARACTER.TYPE.SAM)
        {
            Debug.Log("return");
            return;
        }

        Debug.Log("Press Sam");
        m_selectCharacterSam_UI.interactable = false;
        m_selectCharacterSparky_UI.interactable = false;
        m_gameReadyButton.interactable = false;

        printSelectSound();
        OutGameServerMgr.getInstance().SendPacket(NET_OUTGAME.SEND.PACKET_TYPE.SEND_CHARACTER_SELECT_INFO_EM, (int)CHARACTER.TYPE.SAM);
    }

    public void pressCharacterSelect_SparkyButton()
    {
        if (m_isReady || m_ownSelectCharacter == CHARACTER.TYPE.SAM || m_otherSelectCharacter == CHARACTER.TYPE.SPARKY)
            return;

        Debug.Log("Press Sparky");
        m_selectCharacterSam_UI.interactable = false;
        m_selectCharacterSparky_UI.interactable = false;
        m_gameReadyButton.interactable = false;

        printSelectSound();
        OutGameServerMgr.getInstance().SendPacket(NET_OUTGAME.SEND.PACKET_TYPE.SEND_CHARACTER_SELECT_INFO_EM, (int)CHARACTER.TYPE.SPARKY);
    }

    public void pressSelectReadyButton()
    {
        m_isReady = !m_isReady;
        m_selectRoomExitButton.interactable = !m_isReady;
        printClickSound();

        OutGameServerMgr.getInstance().SendPacket(NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_READY_EM, m_isReady);
    }

    public void pressSelectRoomExitButton()
    {
        disableCharacterSelectUI();
        OutGameServerMgr.getInstance().SendPacket(NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_EXIT_ROOM_EM);
    }
    public void pressExitConfirm()
    {
        printClickSound();
        Application.Quit();
    }

    public void pressExitCancle()
    {
        printClickSound();
        m_exitUI.SetActive(false);
        enableMainButton();
    }
    public void recvJoinResult(string isSuccess, string text)
    {
        if (isSuccess.Contains("success"))
        {
            closeJoinUI();
            enableMainButton();
            //m_gameMessageUI.GetComponentInChildren<Text>().text = "계정 생성 완료.";
            m_state = OUTGAME.NONE;
        }
        else
        {
            enableGameMessageConfirmButton();
        }
        enableJoinUIButton();
        enableGameMessageConfirmButton();
    }

    public void recvLoginResult(string pID, string isSuccess, string text)
    {
        if (isSuccess.Contains("success"))
        {
            if (OutGameServerMgr.getInstance().connectToServer())
            {
                int playerID = int.Parse(pID);
                ProjectMgr.getInstance().setOwnID(playerID);
                OutGameServerMgr.getInstance().SendPacket(NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_ID_EM, playerID);
            }
            else
            {
                enableGameMessageConfirmButton();
                Debug.Log("서버 접속 실패 " + text);
            }
        }
        else
        {
            enableGameMessageConfirmButton();
            m_gameMessageUI.GetComponent<Image>().sprite = m_messageImage[(int)MESSAGE.LOGIN];
            Debug.Log("DB 연결 실패 " + text);
        }
        enableLoginUIButton();
    }

    public void RequierDBCreatureData() { HTTPManager.getInstance().SEND_HTTP(NET_HTTP.URL_NAME.CREATURE_DATA); }

    public void recvCharacterSelectInfo_Own(bool isSuccess, bool isSelect, CHARACTER.TYPE type)
    {
        int ownID = (int)CHARACTER_SELECT.SELECT_HOST;
        if (ProjectMgr.getInstance().isHost() == false)
            ownID = (int)CHARACTER_SELECT.SELECT_PEER;

        //선택 취소
        if (isSuccess == false || isSelect == false)
        {
            switch (type)
            {
                case CHARACTER.TYPE.SAM:
                    m_selectCharacterSamImage[ownID].SetBool("isSelect", false);
                    m_selectGear[0].SetBool("isSelect", false);
                    break;
                case CHARACTER.TYPE.SPARKY:
                    m_selectCharacterSparkyImage[ownID].SetBool("isSelect", false);
                    m_selectGear[1].SetBool("isSelect", false);
                    break;
            }

            switch (m_otherSelectCharacter)
            {
                case CHARACTER.TYPE.NONE:
                    Invoke("activeSelectSam", 1.2f);
                    Invoke("activeSelectSparky", 1.2f);
                    break;
                case CHARACTER.TYPE.SAM:
                    Invoke("activeSelectSparky", 1.2f);
                    break;
                case CHARACTER.TYPE.SPARKY:
                    Invoke("activeSelectSam", 1.2f);
                    break;
            }

            m_ownSelectCharacter = CHARACTER.TYPE.NONE;
        }
        else //선택
        {
            switch (type)
            {
                case CHARACTER.TYPE.SAM:
                    m_ownSelectCharacter = CHARACTER.TYPE.SAM;
                    Invoke("activeSelectSam", 1f);
                    m_selectCharacterSamImage[ownID].SetBool("isSelect", true);
                    m_selectGear[0].SetBool("isSelect", true);
                    break;
                case CHARACTER.TYPE.SPARKY:
                    m_ownSelectCharacter = CHARACTER.TYPE.SPARKY;
                    Invoke("activeSelectSparky", 1f);
                    m_selectCharacterSparkyImage[ownID].SetBool("isSelect", true);
                    m_selectGear[1].SetBool("isSelect", true);
                    break;
            }
            m_gameReadyButton.interactable = true;
        }
    }

    public void recvCharacterSelectInfo_Other(bool isSelect, CHARACTER.TYPE type)
    {
        bool interactable = false;

        int otherID = (int)CHARACTER_SELECT.SELECT_HOST;
        if (ProjectMgr.getInstance().isHost() == true)
            otherID = (int)CHARACTER_SELECT.SELECT_PEER;

        if (isSelect == true)
            m_otherSelectCharacter = type;
        else
        {
            m_otherSelectCharacter = CHARACTER.TYPE.NONE;
            if (m_ownSelectCharacter == CHARACTER.TYPE.NONE)
                interactable = true;
        }

        switch (type)
        {
            case CHARACTER.TYPE.SAM:
                if (interactable)
                    Invoke("activeSelectSam", 1.2f);
                else
                    deActiveSelectSam();
                m_selectCharacterSamImage[otherID].SetBool("isSelect", isSelect);
                m_selectGear[0].SetBool("isSelect", isSelect);
                break;
            case CHARACTER.TYPE.SPARKY:
                if (interactable)
                    Invoke("activeSelectSparky", 1.2f);
                else
                    deActiveSelectSparky();
                m_selectCharacterSparkyImage[otherID].SetBool("isSelect", isSelect);
                m_selectGear[1].SetBool("isSelect", isSelect);
                break;
        }
        if (isSelect == true)
            m_audioSource.PlayOneShot(m_audioClip[1]);
        //m_selectCharacterSam_UI.
    }

    public void activeSelectSam() { m_selectCharacterSam_UI.interactable = true; }
    public void deActiveSelectSam() { m_selectCharacterSam_UI.interactable = false; }

    public void activeSelectSparky() { m_selectCharacterSparky_UI.interactable = true; }
    public void deActiveSelectSparky() { m_selectCharacterSparky_UI.interactable = false; }

    public void recvSelectReady(bool isReady)
    {
        Debug.Log("playerReady : " + isReady);
    }

    public void recvGameStart()
    {
        if (m_gameStart == false)
        {
            m_gameStart = true;
            for(int i = 0; i < m_gearCenter.Length; ++i)
                m_gearCenter[i].SetTrigger("startCount");

            disableCharacterSelectUI();
            disableGameReadyButton();
        }

        m_gameStartTime -= 1;
        if (m_gameStartTime <= 5 && m_gameStartTime > 0)
        {
            if(m_startCountUI.gameObject.activeSelf == false)
                m_startCountUI.gameObject.SetActive(true);

            m_audioSource.PlayOneShot(m_audioClip[2], 1f);
            m_startCountUI.sprite = m_startCountImage[m_gameStartTime - 1];

            for (int i = 0; i < m_gearCenter.Length; ++i)
                m_gearCenter[i].SetTrigger("startCount");
        }

        if (m_gameStartTime <= 0)
        {
            m_gameStartTime = 0;
            openGameMessageUI();
        }
    }

    public void recvExitCharacterSelectRoom()
    {
        m_gameReadyButton.isOn = false;
        m_selectRoomExitButton.interactable = true;
        m_isReady = false;
        closeCharacterSelectUI();
        OutGameServerMgr.getInstance().disConnectServer();
    }

    //UI 열기 / 닫기
    public void closeLoginUI()
    {
        m_inputField_ID[(int)OUTGAME.LOGIN].text = "";
        m_inputField_PW[(int)OUTGAME.LOGIN].text = "";
        m_loginUI.SetActive(false);
    }
    public void closeJoinUI()
    {
        m_inputField_ID[(int)OUTGAME.JOIN].text = "";
        m_inputField_PW[(int)OUTGAME.JOIN].text = "";
        m_inputField_PW[(int)OUTGAME.JOIN + 1].text = "";
        m_joinUI.SetActive(false);
    }
    public void closeCharacterSelectUI()
    {
        deActiveCharacterSelectUI();
        enableMainButton();
    }
    public void openGameMessageUI()
    {
        m_gameMsgConfirmButton.gameObject.SetActive(false);
        m_gameMessageUI.SetActive(true);
        disableGameMessageConfirmButton();
        m_gameMessageUI.GetComponent<Image>().sprite = m_messageImage[(int)MESSAGE.SERVER];
    }
    public void closeGameMessageUI() { m_gameMessageUI.SetActive(false); }
    //사용 가능/불가능
    public void enableMainButton()
    {
        for (int i = 0; i < m_mainButton.Length; ++i)
            m_mainButton[i].enabled = true;
    }
    public void disableMainButton()
    {
        for (int i = 0; i < m_mainButton.Length; ++i)
            m_mainButton[i].enabled = false;
    }
    public void enableJoinUIButton()
    {
        for (int i = 0; i < m_joinButton.Length; ++i)
            m_joinButton[i].enabled = true;
    }
    public void disableJoinUIButton()
    {
        for (int i = 0; i < m_joinButton.Length; ++i)
            m_joinButton[i].enabled = false;
    }
    public void enableLoginUIButton()
    {
        for (int i = 0; i < m_loginButton.Length; ++i)
            m_loginButton[i].enabled = true;
    }
    public void disableLoginUIButton()
    {
        for (int i = 0; i < m_loginButton.Length; ++i)
            m_loginButton[i].enabled = false;
    }

    public void enableGameMessageConfirmButton()
    {
        m_gameMsgConfirmButton.gameObject.SetActive(true);
        m_gameMsgConfirmButton.enabled = true;
    }
    public void disableGameMessageConfirmButton() { m_gameMsgConfirmButton.enabled = false; }

    public void enableLoginUI(bool enable)
    {
        m_inputField_ID[(int)OUTGAME.LOGIN].enabled = enable;
        m_inputField_PW[(int)OUTGAME.LOGIN].enabled = enable;
    }

    public void enableCharacterSelectUI()
    {
        m_selectCharacterSam_UI.interactable = true;
        m_selectCharacterSparky_UI.interactable = true;
    }

    public void disableCharacterSelectUI()
    {
        m_selectCharacterSam_UI.interactable = false;
        m_selectCharacterSparky_UI.interactable = false;
    }

    public void disableGameReadyButton()
    {
        m_gameReadyButton.interactable = false;
    }

    public void waitOtherUser()
    {
        m_gameMessageUI.GetComponent<Image>().sprite = m_messageImage[(int)MESSAGE.WAIT];
    }

    public void changeCharacterSelectUI()
    {
        Debug.Log("들어올때 : " + m_isReady);
        m_state = OUTGAME.CHAR_SELECT;
        closeLoginUI();
        disableMainButton();
        closeGameMessageUI();
        activeCharacterSelectUI();
        enableCharacterSelectUI();
    }

    public void activeCharacterSelectUI()
    {
        m_characterSelectUI.SetActive(true);
    }

    public void deActiveCharacterSelectUI()
    {
        m_ownSelectCharacter = CHARACTER.TYPE.NONE;
        m_otherSelectCharacter = CHARACTER.TYPE.NONE;
        m_characterSelectUI.SetActive(false);
    }

    public void setCharacterType()
    {
        ProjectMgr.getInstance().setOwnCharacterType(m_ownSelectCharacter);
        ProjectMgr.getInstance().setOtherCharacterType(m_otherSelectCharacter);

        Debug.Log("own : " + m_ownSelectCharacter);
        Debug.Log("other : " + m_otherSelectCharacter);
    }
}
