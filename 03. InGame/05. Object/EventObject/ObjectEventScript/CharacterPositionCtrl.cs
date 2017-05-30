using UnityEngine;
using System.Collections;
using System;

public class CharacterPositionCtrl : ObjectEvent
{
    public enum CTRL_TYPE { CHARACTER, USER }
    public CTRL_TYPE m_ctrlType;
    public GameObject[] m_position;

	// Use this for initialization
	void Start ()
    {
        init();
	}

    public override void startEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        switch(m_ctrlType)
        {
            case CTRL_TYPE.CHARACTER :
                setPositionCharacter();
                break;
            case CTRL_TYPE.USER :
                setPositionUser();
                break;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public override void endEvent()
    {
    }

    public void setPositionCharacter()
    {
        InGameMgr.getInstance().getOwnCharacterCtrl().gameObject.transform.parent.position =
            m_position[(int)InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterType() - 1].transform.position;

        InGameMgr.getInstance().getOwnCharacterCtrl().gameObject.transform.parent.rotation =
             m_position[(int)InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterType() - 1].transform.rotation;

#if SERVER_ON
        InGameMgr.getInstance().getOtherCharacterCtrl().gameObject.transform.parent.position =
             m_position[(int)InGameMgr.getInstance().getOtherCharacterCtrl().getCharacterType() - 1].transform.position;
        
        InGameMgr.getInstance().getOtherCharacterCtrl().gameObject.transform.parent.rotation =
             m_position[(int)InGameMgr.getInstance().getOtherCharacterCtrl().getCharacterType() - 1].transform.rotation;
#endif
    }

    public void setPositionUser()
    {
        InGameMgr.getInstance().getOwnCharacterCtrl().gameObject.transform.parent.position = m_position[0].transform.position;
#if SERVER_ON
        InGameMgr.getInstance().getOwnCharacterCtrl().gameObject.transform.parent.position = m_position[1].transform.position;
#endif
    }
}
