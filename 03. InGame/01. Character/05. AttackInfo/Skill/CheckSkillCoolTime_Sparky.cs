using UnityEngine;
using System.Collections;
using System;

public class CheckSkillCoolTime_Sparky : CheckSkillCoolTime
{
    private COOL_TIME m_explosionBulletCoolTime;
    private COOL_TIME m_c4BombCoolTime;
    private COOL_TIME m_teslaBulletCoolTime;
    private COOL_TIME m_rapidFireCoolTime;


    public CheckSkillCoolTime_Sparky()
    {
        m_explosionBulletCoolTime = new COOL_TIME();
        m_c4BombCoolTime = new COOL_TIME();
        m_teslaBulletCoolTime = new COOL_TIME();
        m_rapidFireCoolTime = new COOL_TIME();

        initComponent(OBJECT_NAME.COOLTIME_EXPLOSION_BULLET_UI, m_explosionBulletCoolTime);
        initComponent(OBJECT_NAME.COOLTIME_C4BOMB_UI, m_c4BombCoolTime);
    }

    public override void initSkillInfo(int coolTime_Explosion_B, int coolTime_C4_Bomb, int coolTime_Tesla_b, int coolTime_Rapid_Fire,
                                        bool active_Explosion_B, bool active_C4Bomb, bool active_Tesla_B, bool active_RapidFire)
    {

        setCoolTimeInfo(coolTime_Explosion_B, active_Explosion_B, m_explosionBulletCoolTime);
        setCoolTimeInfo(coolTime_C4_Bomb, active_C4Bomb, m_c4BombCoolTime);
        //setCoolTimeInfo(coolTime_Tesla_b, active_Tesla_B, m_teslaBulletCoolTime);
        //setCoolTimeInfo(coolTime_Rapid_Fire, active_RapidFire, m_rapidFireCoolTime);
    }

    public override void initSkillInfo(int type, int coolTime, bool isActive)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)type;

        switch(skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B :
                setCoolTimeInfo(coolTime, isActive, m_explosionBulletCoolTime);
                break;
            case CHARACTER_SPARKY.SKILL.C4_BOMB :
                setCoolTimeInfo(coolTime, isActive, m_c4BombCoolTime);
                break;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE :
                //setCoolTimeInfo(coolTime, isActive, m_teslaBulletCoolTime);
                break;
            case CHARACTER_SPARKY.SKILL.TESLA_B :
                //setCoolTimeInfo(coolTime, isActive, m_rapidFireCoolTime);
                break;  
        }
    }
    public override void setActiveSkill(bool active, int type)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)type;

        switch(skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B :
                setActiveSkill(true, m_explosionBulletCoolTime);
                break;
            case CHARACTER_SPARKY.SKILL.C4_BOMB :
                setActiveSkill(true, m_c4BombCoolTime);
                break;
            case CHARACTER_SPARKY.SKILL.TESLA_B :
                setActiveSkill(true, m_teslaBulletCoolTime);
                break;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE :
                setActiveSkill(true, m_rapidFireCoolTime);
                break;  
        }
    }

    public override IEnumerator printSkillCoolTime(int type)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)type;

        int curTime = 0;
        COOL_TIME skillCoolTime = null;
        switch (skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B:
                skillCoolTime = m_explosionBulletCoolTime;
                break;
            case CHARACTER_SPARKY.SKILL.C4_BOMB:
                skillCoolTime = m_c4BombCoolTime;
                detonateC4Bomb();
                break;
            case CHARACTER_SPARKY.SKILL.TESLA_B:
                skillCoolTime = m_teslaBulletCoolTime;
                break;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE:
                skillCoolTime = m_rapidFireCoolTime;
                break;
        }

        skillCoolTime.m_isCoolTime = true;
        skillCoolTime.m_animator.SetBool("isCoolTime", true);

        while (curTime < skillCoolTime.m_coolTime)
        {
            setCoolTime(skillCoolTime, skillCoolTime.m_coolTime - curTime);
            curTime += 1;
            yield return new WaitForSeconds(1.0f);
        }

        skillCoolTime.m_isCoolTime = false;
        skillCoolTime.m_animator.SetBool("isCoolTime", false);
    }

    public override bool checkSkillCoolTime(int skillNum)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)skillNum;
        switch (skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B:
                return m_explosionBulletCoolTime.m_isCoolTime;
            case CHARACTER_SPARKY.SKILL.C4_BOMB:
                return m_c4BombCoolTime.m_isCoolTime;
            case CHARACTER_SPARKY.SKILL.TESLA_B:
                return m_teslaBulletCoolTime.m_isCoolTime;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE:
                return m_rapidFireCoolTime.m_isCoolTime;
        }
        return false;
    }

    public override void resetSkillAnimation()
    {
        m_explosionBulletCoolTime.m_animator.SetBool("isActive", m_explosionBulletCoolTime.m_isActive);
        m_c4BombCoolTime.m_animator.SetBool("isActive", m_c4BombCoolTime.m_isActive);
        //m_teslaBulletCoolTime.m_animator.SetBool("isActive", m_teslaBulletCoolTime.m_isActive);
        //m_rapidFireCoolTime.m_animator.SetBool("isActive", m_rapidFireCoolTime.m_isActive);
    }

    public void throwC4Bomb() { m_c4BombCoolTime.m_animator.SetBool("isUsing", true); }
    public void detonateC4Bomb() { m_c4BombCoolTime.m_animator.SetBool("isUsing", false); }

    public override bool isUsebleSkill(int type)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)type;
        switch (skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B:
                return m_explosionBulletCoolTime.m_isActive;
            case CHARACTER_SPARKY.SKILL.C4_BOMB:
                return m_c4BombCoolTime.m_isActive;
            case CHARACTER_SPARKY.SKILL.TESLA_B:
                return m_teslaBulletCoolTime.m_isActive;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE:
                return m_rapidFireCoolTime.m_isActive;
        }

        return false;
    }
}
