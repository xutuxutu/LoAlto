using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommunicateToObject : MonoBehaviour
{
    public struct COLL_BUTTON
    {
        public GameObject button;
        public Button buttonCtrl;
        public int typeID;
        public int objectID;
        public int buttonID;

        public bool init(GameObject _button)
        {
            button = _button;
            buttonCtrl = button.GetComponent<Button>();
            if(buttonCtrl == null)
            {
                Debug.Log("Script is Not Exist : Button");
                return false;
            }

            int[] _objID = ObjectMgr.getInstance().getObjectID(buttonCtrl.m_EventObject.name);

            if (_objID == null)
            {
                Debug.Log("Target ID Error");
                return false;
            }

            typeID = _objID[0];
            objectID = _objID[1];

            buttonID = buttonCtrl.getButtonID();

            return true;
        }

        public void reset()
        {
            button = null;
            buttonCtrl = null;
            typeID = -1;
            objectID = -1;
            buttonID = -1;
        }
    }

    private COLL_BUTTON m_collObject;
    private Camera m_characterCamera;

    private GameObject m_objectUI;
    private GameObject m_revivalUI;
    private GameObject m_revivalBar;
    private Image m_revivalGuage;
    private float m_revivalTime;

    private bool m_revival;

    void Awake()
    {
        m_revivalTime = 5f;

        m_objectUI = GameObject.Find(OBJECT_NAME.OBJECT_UI);
        m_revivalUI = GameObject.Find(OBJECT_NAME.REVIVAL_UI);
        m_revivalBar = GameObject.Find(OBJECT_NAME.REVIVAL_Bar);
        m_revivalGuage = GameObject.Find(OBJECT_NAME.REVIVAL_GUAGE).GetComponent<Image>();
        m_revivalGuage.fillAmount = 0;

        m_objectUI.SetActive(false);
        m_revivalBar.SetActive(false);
        m_revivalUI.SetActive(false);
    }

    void Start()
    {
        m_collObject.reset();
        m_characterCamera = InGameMgr.getInstance().getCharacterCamera().GetComponent<Camera>();
    }

    public void startEvent()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isActive() == false)
            return;

        if (m_collObject.button != null)
        {
            if (m_collObject.button.CompareTag(TAG.QUEST_ITEM))
                activeQuestItem();
            else
                sendEvent();
        }

        if (m_revival == true)
        {
            activeRevivalBar();
            ObjectPool_Common.getInstance().printRevivalEffect(InGameMgr.getInstance().getOtherCharacterCtrl().transform.position + InGameMgr.getInstance().getOtherCharacterCtrl().transform.forward * 0.5f);
        }
    }

    public void endEvent()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isActive() == false)
            return;

        if (m_collObject.button != null)
        {
            if (m_collObject.buttonCtrl.getButtonOperatingType() == BUTTON.OPERATING_TYPE.KEEP_PRESS)
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_DEACTIVE_EM, m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);
        }

        if (m_revival == true)
        {
            deActiveRevivalBar();
            ObjectPool_Common.getInstance().stopRevivalEffect();
        }
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isActive() == false)
            return;

        if (coll.gameObject.CompareTag(TAG.CHARACTER_OTHER))
        {
            if (InGameMgr.getInstance().getOtherCharacterCtrl().isDie() == true)
            {
                activeUI(TAG.CHARACTER_OTHER);
                m_revival = true;
            }
        }

        if (coll.gameObject.CompareTag(TAG.BUTTON))
        {
            m_collObject.init(coll.gameObject);
            if (m_collObject.buttonCtrl.isActive() == true)
                activeUI(TAG.BUTTON);
        }

        if(coll.gameObject.CompareTag(TAG.QUEST_ITEM))
        {
            m_collObject.init(coll.gameObject);
            if (m_collObject.buttonCtrl.isActive() == true)
                activeUI(TAG.QUEST_ITEM);
        }
    }

    public void sendEvent()
    {
#if SERVER_ON
        if (m_collObject.buttonCtrl.checkStartCondition() == true)
        {
            if (m_collObject.buttonCtrl.getButtonState() == BUTTON.STATE.OFF)
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_ACTIVE_EM, m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);
            else if (m_collObject.buttonCtrl.getButtonState() == BUTTON.STATE.ON)
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_DEACTIVE_EM, m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);
        }
#else
        if (m_collObject.buttonCtrl.getButtonState() == BUTTON.STATE.OFF)
            ObjectMgr.getInstance().startEvent(m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);

        else if (m_collObject.buttonCtrl.getButtonState() == BUTTON.STATE.ON)
            ObjectMgr.getInstance().endEvent(m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);
#endif
    }

    public void activeQuestItem()
    {
        if (m_collObject.buttonCtrl.getButtonState() == BUTTON.STATE.OFF)
            ObjectMgr.getInstance().startEvent(m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);

        else if (m_collObject.buttonCtrl.getButtonState() == BUTTON.STATE.ON)
            ObjectMgr.getInstance().endEvent(m_collObject.typeID, m_collObject.objectID, m_collObject.buttonID);
    }

    public void OnTriggerExit(Collider coll)
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isActive() == false)
            return;

        if (coll.gameObject.CompareTag(TAG.BUTTON))
        {
            deActiveUI(TAG.BUTTON);
        }

        if(coll.gameObject.CompareTag(TAG.CHARACTER_OTHER))
        {
            if (m_revival == true)
                deActiveUI(TAG.CHARACTER_OTHER);
        }
        if (coll.gameObject.CompareTag(TAG.QUEST_ITEM))
        {
            deActiveUI(TAG.QUEST_ITEM);
        }

        endEvent();
    }

    public void eventTrigger(string methodName)
    {
        if(m_collObject.button != null)
            m_collObject.button.SendMessage(methodName);
    }

    //----------------------------------------------------------------------------------//
    public void activeUI(string tag)
    {
        if (tag.Contains(TAG.BUTTON))
            m_objectUI.SetActive(true);

        else if (tag.Contains(TAG.CHARACTER_OTHER))
            m_revivalUI.SetActive(true);

        else if (tag.Contains(TAG.QUEST_ITEM))
        {
            m_objectUI.SetActive(true);
            m_objectUI.GetComponentInChildren<Text>().text = "F를 눌러 습득";
        }

        StartCoroutine("printUI", tag);
    }

    public void deActiveUI(string tag)
    {
        StopCoroutine("printUI");

        if (tag.Contains(TAG.BUTTON))
        {
            m_objectUI.SetActive(false);
            m_collObject.reset();
        }

        else if (tag.Contains(TAG.CHARACTER_OTHER))
        {
            if (m_revivalBar.activeSelf == true)
                deActiveRevivalBar();

            ObjectPool_Common.getInstance().stopRevivalEffect();
            m_revivalUI.SetActive(false);
            m_revival = false;
        }

        else if (tag.Contains(TAG.QUEST_ITEM))
        {
            m_objectUI.GetComponentInChildren<Text>().text = "F를 눌러 작동";
            m_objectUI.SetActive(false);
            m_collObject.reset();
        }
    }

    public void setUIPosition(string tag)
    {
        if (m_collObject.button != null)
        {
            Vector2 uiPosition = m_collObject.buttonCtrl.getUI_Position();
            Vector3 pos = m_characterCamera.WorldToScreenPoint(m_collObject.button.transform.position);
            m_objectUI.transform.position = new Vector3(pos.x + uiPosition.x, pos.y + uiPosition.y, 0);
        }
    }

    public IEnumerator printUI(string tag)
    {
        if (tag == TAG.BUTTON || tag == TAG.QUEST_ITEM)
        {
            while (true)
            {
                if (m_collObject.buttonCtrl.isActive() == false || m_collObject.buttonCtrl.getEventState() == ObjectState.EVENT_STATE.WORKING)
                    deActiveUI(tag);

                setUIPosition(tag);
                yield return null;
            }
        }

        if(tag == TAG.CHARACTER_OTHER)
        {
            while (true)
            {
                if (InGameMgr.getInstance().getOtherCharacterCtrl().isDie() == false)
                    deActiveUI(tag);

                yield return null;
            }
        }
    }

    public void activeRevivalBar()
    {
        m_revivalBar.SetActive(true);
        StartCoroutine("revivalOtherCharacter");
    }

    public void deActiveRevivalBar()
    {
        m_revivalGuage.fillAmount = 0;
        m_revivalBar.SetActive(false);
        StopCoroutine("revivalOtherCharacter");
    }

    public IEnumerator revivalOtherCharacter()
    {
        bool revival = false;
        float pressTime = 0;

        while(revival == false)
        {
            pressTime += Time.deltaTime;
            m_revivalGuage.fillAmount = pressTime / m_revivalTime;

            if(pressTime >= m_revivalTime)
            {
                revival = true;
#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DIE_INFO_EM, false);
#endif
                InGameMgr.getInstance().getOtherCharacterCtrl().die(false);

                deActiveUI(TAG.CHARACTER_OTHER);
                ObjectPool_Common.getInstance().printRevivalFinishEffect(InGameMgr.getInstance().getOtherCharacterCtrl().transform.position + InGameMgr.getInstance().getOtherCharacterCtrl().transform.forward * 0.5f);
            }

            yield return null;
        }
    }

    public void stopRevival()
    {
        if(m_revival == true)
        {
            deActiveRevivalBar();
            ObjectPool_Common.getInstance().stopRevivalEffect();
        }
    }

    public bool isRevivalActive() { return m_revival; }
}
