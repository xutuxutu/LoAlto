using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class CheckSkillCoolTime
{
    public class COOL_TIME
    {
        public bool m_isActive;
        public bool m_isCoolTime;
        public Image m_ten;
        public Image m_one;
        public int m_coolTime;
        public Animator m_animator;
    };

    // Use this for initialization
    public abstract void initSkillInfo(int type, int coolTime, bool isActive);
    public abstract void initSkillInfo(int skill_first, int skill_second, int skill_third, int skill_fourth, bool active_first, bool active_second, bool active_third, bool active_fourth);

    protected void initComponent(string name, COOL_TIME coolTimeInfo)
    {
        GameObject coolTimeImage = GameObject.Find(name);
        coolTimeInfo.m_animator = coolTimeImage.GetComponentInParent<Animator>();

        Image[] coolTImeImageChild = coolTimeImage.GetComponentsInChildren<Image>();

        for (int i = 0; i < coolTImeImageChild.Length; i++)
        {
            if (coolTImeImageChild[i].name.Equals("Ten"))
            {
                coolTimeInfo.m_ten = coolTImeImageChild[i];
                continue;
            }
            if (coolTImeImageChild[i].name.Equals("One"))
            {
                coolTimeInfo.m_one = coolTImeImageChild[i];
                continue;
            }
        }
        coolTimeImage.SetActive(false);
    }

    protected void setCoolTimeInfo(int coolTime, bool isActive, COOL_TIME coolTimeInfo)
    {
        coolTimeInfo.m_isCoolTime = false;
        coolTimeInfo.m_isActive = isActive;

        coolTimeInfo.m_coolTime = coolTime;
        coolTimeInfo.m_animator.SetBool("isActive", isActive);
    }

    protected void setCoolTime(COOL_TIME coolTimeImg, int coolTime)
    {
        int ten = coolTime / 10;
        int one = coolTime % 10;

        coolTimeImg.m_ten.sprite = ObjectPool_Common.getInstance().getNumberFont(ten).sprite;
        coolTimeImg.m_one.sprite = ObjectPool_Common.getInstance().getNumberFont(one).sprite;
    }

    protected void setActiveSkill(bool active, COOL_TIME coolTimeInfo)
    {
        coolTimeInfo.m_isActive = active;
        coolTimeInfo.m_animator.SetBool("isActive", active);
    }

    public abstract void setActiveSkill(bool active, int type);
    public abstract void resetSkillAnimation();
    public abstract bool checkSkillCoolTime(int skillNum);
    public abstract IEnumerator printSkillCoolTime(int type);

    public abstract bool isUsebleSkill(int type);
}
