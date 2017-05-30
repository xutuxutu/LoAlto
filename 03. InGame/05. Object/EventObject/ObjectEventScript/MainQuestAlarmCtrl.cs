using UnityEngine;
using System.Collections;
using System;

public class MainQuestAlarmCtrl : ObjectEvent
{
	// Use this for initialization
	void Start ()
    {
        init();
	}

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        Invoke("activeMainQuestAlarm", invokeTime);
    }

    public override void endEvent()
    {
    }

    public void activeMainQuestAlarm() { QuestMgr.getInstance().activeMainQuestAlarm(); }
}
