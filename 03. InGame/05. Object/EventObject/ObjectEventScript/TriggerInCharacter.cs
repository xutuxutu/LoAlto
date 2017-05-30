using UnityEngine;
using System.Collections;
using System;

public class TriggerInCharacter : ObjectEvent
{
    public CHARACTER.TYPE m_characterType;

    public GameObject m_targetEvent;
	// Use this for initialization
	void Start ()
    {
        init();
    }

    public override void startEvent()
    {
        switch(m_characterType)
        {
            case CHARACTER.TYPE.SAM :
                m_targetEvent.SendMessage("arriveSam");
                break;
            case CHARACTER.TYPE.SPARKY :
                m_targetEvent.SendMessage("arriveSparky");
                break;
        }
    }

    public override void endEvent()
    {
    }
}
