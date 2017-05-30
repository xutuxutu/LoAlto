using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public struct OBJECT_POOL<T>
{
    public int m_objectNum;
    public List<GameObject> m_list;
    public List<T> m_ctrl;

    public void init(int objNum)
    {
        m_objectNum = objNum;
        m_list = new List<GameObject>();
        m_ctrl = new List<T>();
    }
}

public class ObjectPool_Common : MonoBehaviour
{
    private Image[] m_numberFont;

    private OBJECT_POOL<EffectCtrl> m_revivalEffect;
    private OBJECT_POOL<EffectCtrl> m_revivalFinishEffect;
    private OBJECT_POOL<EffectCtrl> m_damagedEffect;

    private static ObjectPool_Common m_instance;
    public static ObjectPool_Common getInstance() { return m_instance; }

    void Awake()
    {
        m_instance = this;
        //m_damaged = new OBJECT_POOL<EffectCtrl>[3];
    }

    // Use this for initialization
    void Start ()
    {
        initNumberFont();
        initDamagedEffectList();
        initRevivalEffectList();
        initRevivalFinishEffectList();
    }

    public void initNumberFont()
    {
        GameObject temp;
        m_numberFont = new Image[10];

        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_0) as GameObject;
        m_numberFont[0] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_1) as GameObject;
        m_numberFont[1] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_2) as GameObject;
        m_numberFont[2] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_3) as GameObject;
        m_numberFont[3] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_4) as GameObject;
        m_numberFont[4] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_5) as GameObject;
        m_numberFont[5] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_6) as GameObject;
        m_numberFont[6] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_7) as GameObject;
        m_numberFont[7] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_8) as GameObject;
        m_numberFont[8] = temp.GetComponent<Image>();
        temp = Resources.Load(RESOURCE_PATH.FONT_NUMBER_9) as GameObject;
        m_numberFont[9] = temp.GetComponent<Image>();
    }

    private void initDamagedEffectList()
    {
        m_damagedEffect.init(10);

        GameObject damagedEffect = Resources.Load(PREFAB_PATH.CHARACTER_DAMAGED_1, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_damagedEffect.m_objectNum; i++)
        {
            m_damagedEffect.m_list.Add(GameObject.Instantiate(damagedEffect));
            m_damagedEffect.m_ctrl.Add(m_damagedEffect.m_list[i].GetComponent<EffectCtrl>());
            m_damagedEffect.m_ctrl[i].init();
            m_damagedEffect.m_list[i].SetActive(false);
        }
    }

    private void initRevivalEffectList()
    {
        m_revivalEffect.init(1);

        GameObject revivalEffect = Resources.Load(PREFAB_PATH.CHARACTER_REVIVAL_EFFECT, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_revivalEffect.m_objectNum; i++)
        {
            m_revivalEffect.m_list.Add(GameObject.Instantiate(revivalEffect));
            m_revivalEffect.m_ctrl.Add(m_revivalEffect.m_list[i].GetComponent<EffectCtrl>());
            m_revivalEffect.m_ctrl[i].init();
            m_revivalEffect.m_list[i].SetActive(false);
        }
    }

    private void initRevivalFinishEffectList()
    {
        m_revivalFinishEffect.init(1);

        GameObject revivalFinishEffect = Resources.Load(PREFAB_PATH.CHARACTER_REVIVAL_EFFECT_FINISH, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_revivalFinishEffect.m_objectNum; i++)
        {
            m_revivalFinishEffect.m_list.Add(GameObject.Instantiate(revivalFinishEffect));
            m_revivalFinishEffect.m_ctrl.Add(m_revivalFinishEffect.m_list[i].GetComponent<EffectCtrl>());
            m_revivalFinishEffect.m_ctrl[i].init();
            m_revivalFinishEffect.m_list[i].SetActive(false);
        }
    }

    public Image getNumberFont(int num) { return m_numberFont[num]; }

    public void printDamagedEffect(Vector3 position)
    {
        for (int i = 0; i < m_damagedEffect.m_objectNum; i++)
        {
            if (m_damagedEffect.m_list[i].activeSelf == false)
            {
                m_damagedEffect.m_list[i].SetActive(true);
                m_damagedEffect.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void printRevivalEffect(Vector3 position)
    {
        for (int i = 0; i < m_revivalEffect.m_objectNum; i++)
        {
            if (m_revivalEffect.m_list[i].activeSelf == false)
            {
                m_revivalEffect.m_list[i].SetActive(true);
                m_revivalEffect.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void printRevivalFinishEffect(Vector3 position)
    {
        for (int i = 0; i < m_revivalFinishEffect.m_objectNum; i++)
        {
            if (m_revivalFinishEffect.m_list[i].activeSelf == false)
            {
                m_revivalFinishEffect.m_list[i].SetActive(true);
                m_revivalFinishEffect.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void stopRevivalEffect()
    {
        for (int i = 0; i < m_revivalEffect.m_objectNum; i++)
        {
            if (m_revivalEffect.m_list[i].activeSelf == true)
            {
                m_revivalEffect.m_ctrl[i].deActive();
                break;
            }
        }
    }
}
