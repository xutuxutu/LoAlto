using UnityEngine;
using System.Collections;
using System;

public class PrintDialugueScript : ObjectEvent
{
    public enum EVENT_TIME { START, END }

    public EVENT_TIME m_eventTime;
    public int m_printDialogueNumber;
	// Use this for initialization
	void Start ()
    {
        init();
	}

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        if (m_eventTime == EVENT_TIME.START)
            Invoke("printDialogueScript", invokeTime);
    }

    public override void endEvent()
    {
        if (isActive() == false)
            return;

        if (m_eventTime == EVENT_TIME.END)
            Invoke("printDialogueScript", invokeTime);
    }

    public void printDialogueScript()
    {
        QuestMgr.getInstance().printDialogueUI(m_printDialogueNumber - 1);
    }
}
