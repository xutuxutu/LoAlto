using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace QUEST
{
    public enum QUEST_TYPE { MAIN, SUB };
}

public class QuestMgr : MonoBehaviour
{
    private AudioSource[] m_audioSource;

    private Camera m_characterCamera;
    private Canvas m_dialogueUI;
    private Image m_dialogueLayOut;
    private Text m_dialogueScript;
    private Text m_dialogueScript_Back;
    private Image m_questTagetUI;                //타겟 UI 이미지
    private Image m_questTargetUI_Arrow;

    public QuestUI[] m_targetObject;            //타겟 오브젝트
    private int m_curTargetObjectNum;
    private int m_targetObjectNum;

    private string[] m_mainQuest;               //메인 퀘스트 정보
    private string[][][] m_script;          //스크립트 정보
    private float[][] m_scriptShowTime;
    private int m_curTextNum;               //현재 스크립스 텍스트 Index

    private QuestCondition[] m_questTrigger;   //이벤트 트리거
    private int m_curTriggerNum;
    private int m_triggerNum;

    private Animator m_mainQuestAlarm;
    private Text m_mainQuestAlarmText;
    private Text m_questText;               //좌측 상단 목표 텍스트
    private int m_questTextNum;             //목표 텍스트 갯수

    private Vector3 m_questTargetUIPosition; 

    private float SCREEN_HEIGHT;
    private float SCREEN_WITDH;

    Vector3 ScreenCenter;
    Vector3 ScreenBounds;

    private float m_fadeInTime;
    private float m_fadeOutTime;

    private static QuestMgr m_instance; 
    // Use this for initialization
    void Awake()
    {
        m_instance = this;
        initQuestInfo((SCENE.INDEX)Application.loadedLevel);
        initChild();
        setScreenInfo();

        m_fadeInTime = 0.5f;
        m_fadeOutTime = 1f;
    }

    public void setScreenInfo()
    {
        SCREEN_HEIGHT = Screen.height;
        SCREEN_WITDH = Screen.width;
        ScreenCenter = new Vector3(SCREEN_WITDH, SCREEN_HEIGHT, 0) / 2;
        ScreenBounds = ScreenCenter * 0.9f;
    }

    void Start()
    {
        initQuestNum();
        initQuestInfo();

        m_characterCamera = InGameMgr.getInstance().getCharacterCamera().GetComponent<Camera>();
    }

    public void initChild()
    {
        m_audioSource = GetComponents<AudioSource>();
        GameObject questTrigger = GameObject.Find(OBJECT_NAME.QUEST_TRIGGER);
        m_questTrigger = questTrigger.GetComponentsInChildren<QuestCondition>();

        m_dialogueUI = GameObject.Find(OBJECT_NAME.DIALOGUE_SCREEN).GetComponent<Canvas>();
        m_dialogueScript = GameObject.Find(OBJECT_NAME.DIALOGUE_SCRIPT).GetComponent<Text>();
        m_dialogueScript_Back = GameObject.Find(OBJECT_NAME.DIALOGUE_SCRIPT_BACK).GetComponent<Text>();
        m_dialogueLayOut = GameObject.Find(OBJECT_NAME.DIALOGUE_LAYOUT).GetComponent<Image>();

        m_questTagetUI = GameObject.Find(OBJECT_NAME.QUEST_TARGET_UI).GetComponent<Image>();
        m_questTargetUI_Arrow = GameObject.Find(OBJECT_NAME.QUEST_TARGET_UI_ARROW).GetComponentInChildren<Image>();
        m_questText = GameObject.Find(OBJECT_NAME.QUEST_UI_TEXT).GetComponent<Text>();

        m_questTagetUI.gameObject.SetActive(false);
        m_questTargetUI_Arrow.transform.parent.gameObject.SetActive(false);
        m_questText.transform.parent.gameObject.SetActive(false);

        m_dialogueLayOut.canvasRenderer.SetAlpha(0f);
        m_dialogueScript.canvasRenderer.SetAlpha(0f);
        m_dialogueScript_Back.canvasRenderer.SetAlpha(0f);
        m_dialogueScript.text = "";
        m_dialogueScript_Back.text = "";

        m_questTargetUIPosition = m_questTrigger[0].transform.position;
        m_questText.text = m_mainQuest[m_curTextNum];
        m_mainQuestAlarm = GameObject.Find(OBJECT_NAME.QUEST_UI_MAIN_ALARM).GetComponent<Animator>();
        m_mainQuestAlarmText = GameObject.Find(OBJECT_NAME.QUEST_UI_MAIN_ALARM_TEXT).GetComponent<Text>();
    }

    public void initQuestNum()
    {
        for (int i = 0; i < m_questTrigger.Length; ++i)
            m_questTrigger[i].setQuestNum(i);
    }

    public void initQuestInfo()
    {
        m_curTargetObjectNum = -1;
        m_curTextNum = 0;
        m_curTriggerNum = 0;

        m_questTextNum = m_mainQuest.Length;
        m_targetObjectNum = m_targetObject.Length;
        m_triggerNum = m_questTrigger.Length;
    }

    public void addQuestTargetObject(QuestUI newTarget)
    {
        if (newTarget != null)
        {
            int newQuestNum = m_targetObject.Length + 1;
            QuestUI[] newQuestList = new QuestUI[newQuestNum];
            for(int i = 0; i < newQuestNum - 1; i++)
                newQuestList[i] = m_targetObject[i];

            newQuestList[newQuestNum - 1] = newTarget;

            m_targetObject = newQuestList;
        }
        else
            Debug.Log("newQuest is Null");
    }

    public void addQuestTargetObject(QuestUI newTarget, int num)
    {
        if (newTarget != null)
        {
            int newQuestNum = m_targetObject.Length + 1;
            QuestUI[] newQuestList = new QuestUI[newQuestNum];
            for (int i = 0; i < num - 1; i++)
                newQuestList[i] = m_targetObject[i];

            newQuestList[num -1] = newTarget;

            for (int i = num ; i < newQuestNum; ++i)
                newQuestList[i] = m_targetObject[i - 1];

            m_targetObject = newQuestList;
        }
        else
            Debug.Log("newQuest is Null");
    }

    public void activeMainQuestAlarm()
    {
        Invoke("playMainQuestAlarmSound", 0.5f);
        m_mainQuestAlarm.SetTrigger("active");
    }
    public void playMainQuestAlarmSound()
    {
        m_audioSource[1].Play();
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            m_questTargetUIPosition = m_targetObject[m_curTargetObjectNum].transform.position + m_targetObject[m_curTargetObjectNum].m_uiPosition;
        }
        catch
        {
            return;
        }

        setScreenInfo();

        Vector3 cameraForward = m_characterCamera.transform.forward;
        Vector3 cameraToTarget = (m_questTargetUIPosition - m_characterCamera.transform.position).normalized;

        float degree = Vector3.Dot(cameraForward, cameraToTarget);
        degree = Mathf.Acos(degree);
        degree = Mathf.Rad2Deg * degree;

        Vector3 pos = m_characterCamera.WorldToScreenPoint(m_questTargetUIPosition);

        RaycastHit hit;
        if (Physics.Raycast(m_characterCamera.ScreenPointToRay(pos), out hit, 20f))
        {
            if (hit.collider.CompareTag(TAG.CHARACTER_OWN))
            {
                m_questTagetUI.canvasRenderer.SetAlpha(0.5f);
                m_questTargetUI_Arrow.canvasRenderer.SetAlpha(0.5f);
            }
            else
            {
                m_questTagetUI.canvasRenderer.SetAlpha(1f);
                m_questTargetUI_Arrow.canvasRenderer.SetAlpha(1f);
            }
        }

        if (pos.z > 0 && pos.x > 0 && pos.x < SCREEN_WITDH && pos.y > 0 && pos.y < SCREEN_HEIGHT)
        {
            m_questTagetUI.transform.position = pos;
            m_questTargetUI_Arrow.transform.parent.position = pos;
            m_questTargetUI_Arrow.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_questTargetUI_Arrow.transform.parent.gameObject.SetActive(true);
            if (pos.z < 0)
                pos *= -1;

            //make 00 center of screen instead of botton left
            pos -= ScreenCenter;

            //fine angle from center of screen to mouse position
            //화면 중심에서 마우스 위치까지의 각도
            float angle = Mathf.Atan2(pos.y, pos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            pos = ScreenCenter + new Vector3(sin * 150, cos * 150, 0);

            //y = mx * b format
            float m = cos / sin;

            //check up and down first
            //먼저 위 아래를 확인.
            if (cos > 0)
                pos = new Vector3(-ScreenBounds.y / m, ScreenBounds.y, 0);
            else //down
                pos = new Vector3(ScreenBounds.y / m, -ScreenBounds.y, 0);
            
            //if out of bounds, get point on appropriate side
            //좌 우 확인
            if (pos.x > ScreenBounds.x)
                pos = new Vector3(ScreenBounds.x, -ScreenBounds.x * m, 0);
            else if (pos.x < -ScreenBounds.x)
                pos = new Vector3(-ScreenBounds.x, ScreenBounds.x * m, 0);
                
            pos += ScreenCenter;

            m_questTagetUI.transform.position = pos;
            m_questTargetUI_Arrow.transform.parent.position = pos;
            m_questTargetUI_Arrow.transform.parent.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }

    public static QuestMgr getInstance() { return m_instance; }
    
    public void fadeInDialogueUI() //밝아짐
    {
        m_dialogueLayOut.CrossFadeAlpha(0, m_fadeInTime, false);
        m_dialogueScript.CrossFadeAlpha(0, m_fadeInTime, false);
        m_dialogueScript_Back.CrossFadeAlpha(0, m_fadeInTime, false);
    }

    public void fadeOutDialogueUI() //밝아짐
    {
        m_dialogueLayOut.CrossFadeAlpha(1, m_fadeOutTime, false);
        m_dialogueScript.CrossFadeAlpha(1, m_fadeOutTime, false);
        m_dialogueScript_Back.CrossFadeAlpha(1, m_fadeOutTime, false);
    }

    public void printDialogueUI(int questNum)
    {
        m_curTriggerNum = questNum;
        m_dialogueUI.gameObject.SetActive(true);

        if(questNum < m_triggerNum)
            if (m_questTrigger[m_curTriggerNum].preventCharacterCtrl == true)
                InGameMgr.getInstance().getOwnCharacterCtrl().deActive();

        StopCoroutine("printDialogueScript");
        StartCoroutine("printDialogueScript");
    }

    private IEnumerator printDialogueScript()
    {
        string[][] script = m_script[m_curTriggerNum];
        int curScriptPage = 0;

        int scriptPage = script.Length;
        int[] scriptLine = new int[scriptPage];
 
        for(int i = 0; i < scriptPage; ++i)
            scriptLine[i] = script[i].Length;

        m_dialogueScript.text = script[curScriptPage][0];
        m_dialogueScript_Back.text = script[curScriptPage][0];
        for (int i = 1; i < scriptLine[curScriptPage]; ++i)
        {
            m_dialogueScript.text += "\n" + script[curScriptPage][i];
            m_dialogueScript_Back.text = "\n" + script[curScriptPage][i];
        }

        //첫 스크립트 설정 후 페이드 아웃
        fadeOutDialogueUI();
        printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET.START);
        yield return new WaitForSeconds(m_scriptShowTime[m_curTriggerNum][curScriptPage]);
        printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET.END);

        while (true)
        {
            curScriptPage += 1;

            if (curScriptPage >= scriptPage)
                break;

            else
            {
                m_dialogueScript.text = script[curScriptPage][0];
                m_dialogueScript_Back.text = script[curScriptPage][0];
                for (int i = 1; i < scriptLine[curScriptPage]; ++i)
                {
                    m_dialogueScript.text += "\n" + script[curScriptPage][i];
                    m_dialogueScript_Back.text = "\n" + script[curScriptPage][i];
                }
            }

            printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET.START);
            yield return new WaitForSeconds(m_scriptShowTime[m_curTriggerNum][curScriptPage]);
            printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET.END);
        }

        fadeInDialogueUI();
        yield return new WaitForSeconds(m_fadeInTime);
        resetDialogueInfo();

        if (m_curTriggerNum < m_triggerNum)
        {
            if (m_questTrigger[m_curTriggerNum].m_mainQuest == QUEST.QUEST_TYPE.MAIN)
                m_questText.transform.parent.gameObject.SetActive(true);

            if (m_questTrigger[m_curTriggerNum].m_questTarget == QUEST.QUEST_TYPE.MAIN)
            {
                m_questTagetUI.gameObject.SetActive(true);
                m_questTargetUI_Arrow.transform.parent.gameObject.SetActive(false);
            }

            if (m_questTrigger[m_curTriggerNum].m_activeNextQuest == QuestCondition.ACITIVE_NEXT.ALL_EVENT_END)
                m_questTrigger[m_curTriggerNum].activeNextQuest();
        }
    }

    public void resetDialogueInfo()
    {
        m_dialogueScript.text = "";
        m_dialogueScript_Back.text = "";

        if (m_curTriggerNum < m_triggerNum)
            if (m_questTrigger[m_curTriggerNum].preventCharacterCtrl == true)
                InGameMgr.getInstance().getOwnCharacterCtrl().active();
    }

    public bool setNextQuestTrigger(int questNum)
    {
        if (questNum >= m_triggerNum)
        {
            Debug.Log("Error : NextQuestTrigger Number Overflow");
            return false;
        }

        Debug.Log(m_questTrigger[questNum].gameObject);
        m_questTrigger[questNum].setActive(true);

        return true;
    }

    public bool setNextTargetUI()
    {
        m_curTargetObjectNum += 1;

        if (m_curTargetObjectNum >= m_targetObjectNum)
        {
            m_questTagetUI.gameObject.SetActive(false);
            m_questTargetUI_Arrow.transform.parent.gameObject.SetActive(false);
            return false;
        }
        else
        {
            if (m_questTagetUI.gameObject.activeSelf == false)
                m_questTagetUI.gameObject.SetActive(true);
        }
        return true;
    }

    public bool setNextQuest()
    {
        if (m_curTextNum >= m_questTextNum)
        {
            m_questText.transform.parent.gameObject.SetActive(false);
            return false;
        }

        if (m_questText.transform.parent.gameObject.activeSelf == false)
            m_questText.transform.parent.gameObject.SetActive(true);

        m_questText.text = m_mainQuest[m_curTextNum];
        m_mainQuestAlarmText.text = m_questText.text;
        m_curTextNum += 1;
        return true;
    }

    public void activeQuestTrigger(int num)
    {
        Debug.Log("recvTriggerActive : " + num);
        if (num > 0 && num < m_questTrigger.Length)
            m_questTrigger[num].startEvent();
    }

    public void initQuestInfo(SCENE.INDEX sceneNum)
    {
        switch (sceneNum)
        {
            case SCENE.INDEX.Ar1St1 :
                //--------------------------------------------------------------------------//
                m_mainQuest = new string[5];

                m_mainQuest[0] = "보급품을 획득하라!";
                m_mainQuest[1] = "연습용 적을 처치하라!";
                m_mainQuest[2] = "문을 열고 팀과 합류하라!";
                m_mainQuest[3] = "적들을 물리쳐라!";
                m_mainQuest[4] = "2층 발전소로 이동하라!";
                //--------------------------------------------------------------------------//
                m_scriptShowTime = new float[3][];
                //--------------------------------------------------------------------------//
                m_scriptShowTime[0] = new float[6];
                m_scriptShowTime[0][0] = 3f;
                m_scriptShowTime[0][1] = 3f;
                m_scriptShowTime[0][2] = 3f;
                m_scriptShowTime[0][3] = 3f;
                m_scriptShowTime[0][4] = 3f;
                m_scriptShowTime[0][5] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[1] = new float[2];
                m_scriptShowTime[1][0] = 3f;
                m_scriptShowTime[1][1] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[2] = new float[2];
                m_scriptShowTime[2][0] = 3f;
                m_scriptShowTime[2][1] = 3f;
                //--------------------------------------------------------------------------//
                //--------------------------------------------------------------------------//
                m_script = new string[3][][];   //스크립트의 총 갯수만큼 할당
                //--------------------------------------------------------------------------//
                m_script[0] = new string[6][];
                m_script[0][0] = new string[1];
                m_script[0][1] = new string[1];
                m_script[0][2] = new string[1];
                m_script[0][3] = new string[1];
                m_script[0][4] = new string[1];
                m_script[0][5] = new string[1];

                m_script[0][0][0] = "들리나? 송신 확인 바란다.";
                m_script[0][1][0] = "아, 좋아. 제대로 들어왔구만.";
                m_script[0][2][0] = "여기가 바로 제국군들의 전술기지 'LO ALTO'라네.";
                m_script[0][3][0] = "녀석들은 벌써 우리의 침입을 감지한 모양이야..";
                m_script[0][4][0] = "무기는 가지고 있겠지? 지나가는 길에 보급품을 배치 해 두었으니 반드시 챙겨가게나.";
                m_script[0][5][0] = "그럼 무운을 비네!";
                //--------------------------------------------------------------------------//
                m_script[1] = new string[2][];
                m_script[1][0] = new string[1];
                m_script[1][1] = new string[1];

                m_script[1][0][0] = "이런! 적들이 수상한 냄새를 맡았나 보네!";
                m_script[1][1][0] = "경계령이 발령되기 시작했어. 조심하게나!";
                //--------------------------------------------------------------------------//
                m_script[2] = new string[2][];
                m_script[2][0] = new string[1];
                m_script[2][1] = new string[1];

                m_script[2][0][0] = "여긴 어느정도 정리됬네만, 그쪽은 괜찮은가?";
                m_script[2][1][0] = "뭐, 자네들이라면 무사하겠지만 말이야. 우리 먼저 올라가겠네!";
                //--------------------------------------------------------------------------//
                break;
            case SCENE.INDEX.Ar1St2:
                //--------------------------------------------------------------------------//
                m_mainQuest = new string[3];

                //메인 퀘스트
                m_mainQuest[0] = "테슬라 발전소에 침입하라!";
                m_mainQuest[1] = "회전통로를 작동시켜라!";
                m_mainQuest[2] = "검문소로 올라가라!";
                //--------------------------------------------------------------------------//
                m_scriptShowTime = new float[6][];  //스크립트의 총 갯수 만큼 할당

                //스크립트 시간 정보
                //--------------------------------------------------------------------------//
                m_scriptShowTime[0] = new float[2]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[0][0] = 3f;
                m_scriptShowTime[0][1] = 5f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[1] = new float[2]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[1][0] = 3f;
                m_scriptShowTime[1][1] = 5f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[2] = new float[1];

                m_scriptShowTime[2][0] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[3] = new float[2];

                m_scriptShowTime[3][0] = 3f;
                m_scriptShowTime[3][1] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[4] = new float[1];

                m_scriptShowTime[4][0] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[5] = new float[2];

                m_scriptShowTime[5][0] = 3f;
                m_scriptShowTime[5][1] = 3f;
                //--------------------------------------------------------------------------//

                //스크립트 정보
                m_script = new string[6][][];   //스크립트의 총 갯수만큼 할당
                //--------------------------------------------------------------------------//
                m_script[0] = new string[2][];
                m_script[0][0] = new string[1];
                m_script[0][1] = new string[1];

                m_script[0][0][0] = "발전소로 가는 복도에 도착했구만.";
                m_script[0][1][0] = "녀석들이 자네를 잡으려고 본격적으로 병사들을 풀기 시작했으니 조심하게나.";
                //--------------------------------------------------------------------------//
                m_script[1] = new string[2][]; //대화문 수
                m_script[1][0] = new string[1]; //첫번째 대화문 줄 수
                m_script[1][1] = new string[1]; //두번째 대화문 줄 수

                m_script[1][0][0] = "우리쪽 정보원에 따르면 발전소를 들어가지 못하게 막고있는 녀석들이 있다고 하더군.";
                m_script[1][1][0] = "그 녀석들을 처치해야 발전소로 침입할 수 있을 걸세.";
                //--------------------------------------------------------------------------//
                m_script[2] = new string[1][];
                m_script[2][0] = new string[1];

                m_script[2][0][0] = "";
                //--------------------------------------------------------------------------//
                m_script[3] = new string[2][];
                m_script[3][0] = new string[1];
                m_script[3][1] = new string[1];

                m_script[3][0][0] = "좋아, 제대로 들어갔군 그래.";
                m_script[3][1][0] = "계단 위를 올라 가면 뒷편에 컨트롤 패널이 있을 걸세.";
                //--------------------------------------------------------------------------//
                m_script[4] = new string[1][];
                m_script[4][0] = new string[1];

                m_script[4][0][0] = "바로 저기 보이는게 컨트롤 패널이네. 작동시켜 보게나.";
                //--------------------------------------------------------------------------//
                m_script[5] = new string[2][];
                m_script[5][0] = new string[1];
                m_script[5][1] = new string[1];

                m_script[5][0][0] = "이  소리가 들리는걸 보니, 회전통로가 작동하기 시작했구만!";
                m_script[5][1][0] = "좋아, 그대로 계속 전진하게나!";
                //--------------------------------------------------------------------------//
                break;

            case SCENE.INDEX.Ar1St3 :
                //--------------------------------------------------------------------------//
                m_mainQuest = new string[4];

                //메인 퀘스트
                m_mainQuest[0] = "검문소의 적들을 물리쳐라!";
                m_mainQuest[1] = "지상감시장비 Owl MK-II를 격퇴하라!";
                m_mainQuest[2] = "적들을 섬멸하라!";
                m_mainQuest[3] = "연구소로 올라가라!";
                //--------------------------------------------------------------------------//
                m_scriptShowTime = new float[4][];  //스크립트의 총 갯수 만큼 할당

                //스크립트 시간 정보
                //--------------------------------------------------------------------------//
                m_scriptShowTime[0] = new float[3]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[0][0] = 3f;
                m_scriptShowTime[0][1] = 3f;
                m_scriptShowTime[0][2] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[1] = new float[3]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[1][0] = 4f;
                m_scriptShowTime[1][1] = 4f;
                m_scriptShowTime[1][2] = 4f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[2] = new float[1]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[2][0] = 4f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[3] = new float[2]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[3][0] = 4f;
                m_scriptShowTime[3][1] = 4f;
                //--------------------------------------------------------------------------//

                m_script = new string[4][][];   //스크립트의 총 갯수만큼 할당
                //--------------------------------------------------------------------------//
                m_script[0] = new string[3][];
                m_script[0][0] = new string[1];
                m_script[0][1] = new string[1];
                m_script[0][2] = new string[1];

                m_script[0][0][0] = "발전소를 지났다면 아마 자네들이 도착한 곳은 검문소 일 것 같구만.";
                m_script[0][1][0] = "놈들이 자네들을 잡으려고 단단히 열이 올랐어, 이쪽도 마찬가질세.";
                m_script[0][2][0] = "부디 조심하게나. 놈들의 감시도 점점 강도가 올라가고 있어. 슬슬 전면전을 준비해야 할 걸세.";
                //--------------------------------------------------------------------------//
                m_script[1] = new string[3][];
                m_script[1][0] = new string[1];
                m_script[1][1] = new string[1];
                m_script[1][2] = new string[1];

                m_script[1][0][0] = "이 소리 ··· 틀림없어, 거대 기계 부엉이. 지상감시장비 OWL MK-II !!";
                m_script[1][1][0] = "지상감시장비 OWL MK-II는 드론을 사출하여 공격하는 무인기계장치일세!";
                m_script[1][2][0] = "OWL MK-II의 기능을 정지시키면 연구소로 올라갈 수 있을걸세! 하지만 너무 무리하지 말게나!";
                //--------------------------------------------------------------------------//
                m_script[2] = new string[1][];
                m_script[2][0] = new string[1];

                m_script[2][0][0] = "좋아, 무사히 OWL MK - II의 기능을 정지시켰구만!";
                //--------------------------------------------------------------------------//
                m_script[3] = new string[2][];
                m_script[3][0] = new string[1];
                m_script[3][1] = new string[1];

                m_script[3][0][0] = "제국군 일개 소대가 그쪽으로 투입됬다고하니 조심하게!";
                m_script[3][1][0] = "도대체 이 탑에 얼마나 많은 병력들이 있는지 가늠하기조차 어렵구만..";
                //--------------------------------------------------------------------------//
                break;
            case SCENE.INDEX.Ar1St4 :
                //--------------------------------------------------------------------------//
                m_mainQuest = new string[3];

                //메인 퀘스트
                m_mainQuest[0] = "연구소 안으로 진입하라!";
                m_mainQuest[1] = "제국군 실험체 「프랑키」를 제압하라!";
                m_mainQuest[2] = "다음 층으로 올라가라!";
                //--------------------------------------------------------------------------//
                m_scriptShowTime = new float[2][];  //스크립트의 총 갯수 만큼 할당

                //스크립트 시간 정보
                //--------------------------------------------------------------------------//
                m_scriptShowTime[0] = new float[5]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[0][0] = 3f;
                m_scriptShowTime[0][1] = 3f;
                m_scriptShowTime[0][2] = 3f;
                m_scriptShowTime[0][3] = 3f;
                m_scriptShowTime[0][4] = 3f;
                //--------------------------------------------------------------------------//
                m_scriptShowTime[1] = new float[4]; //첫번째 스크립트 대화문 수 만큼 할당

                m_scriptShowTime[1][0] = 3f;
                m_scriptShowTime[1][1] = 3f;
                m_scriptShowTime[1][2] = 3f;
                m_scriptShowTime[1][3] = 3f;
                //--------------------------------------------------------------------------//
                m_script = new string[2][][];   //스크립트의 총 갯수만큼 할당
                //--------------------------------------------------------------------------//
                m_script[0] = new string[5][];
                m_script[0][0] = new string[1];
                m_script[0][1] = new string[1];
                m_script[0][2] = new string[1];
                m_script[0][3] = new string[1];
                m_script[0][4] = new string[1];

                m_script[0][0][0] = "이 진동은 대체 뭐지?!";
                m_script[0][1][0] = "이 정도 진동.. 틀림없어! 폐기된 줄만 알았던 제국군 실험체 '프랑키'인가!";
                m_script[0][2][0] = "자세한 이야기는 나중에 하고, 일단 '프랑키'를 제압해야겠네!";
                m_script[0][3][0] = "녀석은 등에 달린 발전기를 이용해 전기 에너지를 모아 공격 할 거야!";
                m_script[0][4][0] = "한 방 한 방이 강력한 녀석이니 조심해서 싸우게나! ";
                //--------------------------------------------------------------------------//
                m_script[1] = new string[4][];
                m_script[1][0] = new string[1];
                m_script[1][1] = new string[1];
                m_script[1][2] = new string[1];
                m_script[1][3] = new string[1];

                m_script[1][0][0] = "녀석을 잡았나 보구만! 제국군 녀석들, 적잖이 당황해하고 있어";
                m_script[1][1][0] = "제국군 녀석들, 한 방 먹으니 어안이 벙벙한가 보군!";
                m_script[1][2][0] = "덕분에 이쪽은 조금 수월하게 올라 갈 수 있겠구만.";
                m_script[1][3][0] = "좋아! 프랑키도 무사히 처리한 것 같으니 승강기를 타고 다음 구역으로 이동하게나!";
                //--------------------------------------------------------------------------//
                break;
        }
    }

    public void printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET radioState)
    {
        m_audioSource[0].Stop();
        switch(radioState)
        {
            case SOUND_POOL.AMBIENT.RADIO_SET.START :
                m_audioSource[0].PlayOneShot(SoundMgr.getInstance().getAmbientAudioClip((int)SOUND_POOL.AMBIENT.RADIO_SET.START), 1);
                //Invoke("startLoopSound", 0.1f);
                break;
            case SOUND_POOL.AMBIENT.RADIO_SET.LOOP :
                m_audioSource[0].Play();
                break;
            case SOUND_POOL.AMBIENT.RADIO_SET.END :
                m_audioSource[0].PlayOneShot(SoundMgr.getInstance().getAmbientAudioClip((int)SOUND_POOL.AMBIENT.RADIO_SET.END), 1);
                break;
        }
    }

    public void startLoopSound()
    {
        printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET.LOOP);
    }

    public void startEndSound()
    {
        printRadioSetSound(SOUND_POOL.AMBIENT.RADIO_SET.END);
    }
}


