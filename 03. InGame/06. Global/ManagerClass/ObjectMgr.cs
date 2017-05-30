using UnityEngine;
using System.Collections;

public class ObjectMgr : MonoBehaviour
{
    public int[] m_objectTypeToNum;   //오브젝트 종류 별 갯수 
    public string[] m_objectNaming;

    private int m_objectTypeNum;          //구역 갯수
    private int m_totalObejctNum;  //총 오브젝트 갯수

    private GameObject[][][] m_objectList;

    private static ObjectMgr m_instance;
	// Use this for initialization
    void Awake()
    {
        m_instance = this;
        m_totalObejctNum = 0;
        m_objectTypeNum = m_objectTypeToNum.Length;     //오브젝트 타입 갯수

        //총 오브젝트 갯수
        for (int i = 0; i < m_objectTypeNum; i++)
            m_totalObejctNum += m_objectTypeToNum[i];

        if (m_objectTypeNum == 0)
            m_objectList = null;
        else
            m_objectList = new GameObject[m_objectTypeNum][][];             //타입 갯수만큼 배열 할당

        for (int typeNum = 0; typeNum < m_objectTypeNum; ++typeNum)       //오브젝트들의 갯수만큼 배열 할당
        {
            m_objectList[typeNum] = new GameObject[m_objectTypeToNum[typeNum]][];                                     
        }
    }

    public static ObjectMgr getInstance()
    {
        if (m_instance == null)
            return null;

        return m_instance;
    }

    public void initObject()
    {
        string objectName;
        EventObject objectTarget;
        Button[] buttons;

        if (m_objectList == null)
            return;

        Debug.Log("--------------------------Init Object Manager : Add Object------------------------------");
        for (int typeNum = 0; typeNum < m_objectTypeNum; ++typeNum)                         //오브젝트 종류의 갯수만큼 반복
        {
            for (int obejctNum = 0; obejctNum < m_objectTypeToNum[typeNum]; ++obejctNum)      //종류 별 오브젝트의 갯수만큼 반복
            {
                objectName = m_objectNaming[typeNum] + obejctNum;              //오브젝트 이름 설정.
                objectTarget = GameObject.Find(objectName).GetComponent<EventObject>();                 //이름으로 오브젝트 탐색
                if (objectTarget == null)
                {
                    Debug.Log("non object target : " + objectName);
                    return;
                }

                buttons = objectTarget.getButton();   //타겟 오브젝트의 버튼들을 탐색
                Debug.Log("오브젝트 이름 : " + objectTarget + " 버튼 갯수 : " + buttons.Length);
                if (buttons != null)
                {
                    m_objectList[typeNum][obejctNum] = new GameObject[buttons.Length + 1];   //버튼 갯수 + 1개만큼 배열을 할당

                    m_objectList[typeNum][obejctNum][0] = objectTarget.gameObject;                      //배열의 0번째 원소에 오브젝트를 저장.
                    for (int buttonNum = 0; buttonNum < buttons.Length; buttonNum++)    //배열의 나머지 원소에 버튼을 저장.
                        m_objectList[typeNum][obejctNum][buttonNum + 1] = buttons[buttonNum].gameObject;
                }
                else
                {
                    m_objectList[typeNum][obejctNum] = new GameObject[1];   //1개만큼 배열을 할당
                    m_objectList[typeNum][obejctNum][0] = objectTarget.gameObject;                      //배열의 0번째 원소에 오브젝트를 저장.
                }
            }
        }
        Debug.Log("---------------------------------------------------------------------------------------");
    }

    public void setActiveObjectPool()
    {
        bool isOwner = true;
#if SERVER_ON
        if (ProjectMgr.getInstance().isHost())
            isOwner = false;
#endif
        Debug.Log("-------------------------------------ActiveObject-----------------------------------");
        for (int typeNum = 0; typeNum < m_objectTypeNum; ++typeNum)
        {
            for (int objnum = 0; objnum < m_objectList[typeNum].Length; ++objnum)
            {
                Debug.Log("Acivate Obejct : " + m_objectList[typeNum][objnum][0]);
                m_objectList[typeNum][objnum][0].SendMessage("setActive", true);

                if (m_objectList[typeNum][objnum][0].CompareTag(TAG.MOVING_SCRIPT))
                {
                    m_objectList[typeNum][objnum][0].GetComponent<MovingObject>().setOwner(isOwner);
                }
            }
        }

        Debug.Log("---------------------------------------------------------------------------------------");
    }

    public void startEvent(int typeID, int objID, int buttonID)
    {
        if (isEmpty())
            return;

        for (int objNum = 1; objNum < m_objectList[typeID][objID].Length; ++objNum)
        {
            if (m_objectList[typeID][objID][buttonID].GetComponent<Button>().getButtonID() == buttonID)
            {
                m_objectList[typeID][objID][buttonID].SendMessage("startEvent");
                Debug.Log(m_objectList[typeID][objID][buttonID]);
                return;
            }
        }

        Debug.Log("Not Exist Target Button To able");
    }

    public void endEvent(int typeID, int objID, int buttonID)
    {
        if (isEmpty())
            return;

        for (int objNum = 1; objNum < m_objectList[typeID][objID].Length; ++objNum)
        {
            if (m_objectList[typeID][objID][buttonID].GetComponent<Button>().getButtonID() == buttonID)
            {
                m_objectList[typeID][objID][buttonID].SendMessage("endEvent");
                return;
            }
        }
        Debug.Log("Not Exist Target Button To Enable");
    }

    public void setState(int typeID, int objID, int state)
    {
        if (isEmpty())
            return;

        if (m_objectList[typeID][objID][0].CompareTag(TAG.MOVING_SCRIPT))
            m_objectList[typeID][objID][0].GetComponent<MovingObject>().setState(state);
        else
            Debug.Log("Set State Error : not MovingObejct");
    }

    public GameObject findObject(int typeID, int objID)
    {
        if (isEmpty())
            return null;

        GameObject obj = null;
        try
        {
            obj = m_objectList[typeID][objID][0];
        }
        catch
        {
            Debug.Log("Can't Find Object");
        }

        return obj;
    }

    public int[] getObjectID(string name)
    {
        if (isEmpty())
            return null;

        int[] data = new int[2];
        data[0] = -1;
        data[1] = -1;

        for (int typeNum = 0; typeNum < m_objectTypeNum; ++typeNum)
        {
            for (int objNum = 0; objNum < m_objectList[typeNum].Length; ++objNum)
            {
                if (string.Equals(name, m_objectList[typeNum][objNum][0].name))
                {
                    data[0] = typeNum;
                    data[1] = objNum;
                    return data;
                }
            }
        }
        return null;
    }

    public bool isEmpty()
    {
        if (m_objectList == null)
        {
            Debug.Log("objectList is Empty");
            return true;
        }
        return false;
    }
}
