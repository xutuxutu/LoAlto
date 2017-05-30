using UnityEngine;
using System.Collections;

public class ObjectPool_Sam : MonoBehaviour
{
    private static ObjectPool_Sam m_instance;
    public static ObjectPool_Sam getInstance() { return m_instance; }

    private OBJECT_POOL<EffectCtrl> m_normalAttackEffect;
    private OBJECT_POOL<EffectCtrl> m_normalAttackHitEffect_1;
    private OBJECT_POOL<EffectCtrl> m_normalAttackHitEffect_2;
    private OBJECT_POOL<EffectCtrl> m_normalAttackHitEffect_3;

    void Awake()
    {
        m_instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        initNormalAttackHitEffect_1();
        initNormalAttackHitEffect_2();
        initNormalAttackHitEffect_3();
	}

    private void initNormalAttackEffect()
    {

    }

    private void initNormalAttackHitEffect_1()
    {
        m_normalAttackHitEffect_1.init(10);

        GameObject nomalAttackEffect = Resources.Load(PREFAB_PATH.CHARACTER_SAM_NORMAL_ATTACK_HIT_1, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_normalAttackHitEffect_1.m_objectNum; i++)
        {
            m_normalAttackHitEffect_1.m_list.Add(GameObject.Instantiate(nomalAttackEffect));
            m_normalAttackHitEffect_1.m_ctrl.Add(m_normalAttackHitEffect_1.m_list[i].GetComponent<EffectCtrl>());
            m_normalAttackHitEffect_1.m_list[i].SetActive(false);
        }
    }

    private void initNormalAttackHitEffect_2()
    {
        m_normalAttackHitEffect_2.init(10);

        GameObject nomalAttackEffect = Resources.Load(PREFAB_PATH.CHARACTER_SAM_NORMAL_ATTACK_HIT_2, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_normalAttackHitEffect_2.m_objectNum; i++)
        {
            m_normalAttackHitEffect_2.m_list.Add(GameObject.Instantiate(nomalAttackEffect));
            m_normalAttackHitEffect_2.m_ctrl.Add(m_normalAttackHitEffect_2.m_list[i].GetComponent<EffectCtrl>());
            m_normalAttackHitEffect_2.m_list[i].SetActive(false);
        }
    }

    private void initNormalAttackHitEffect_3()
    {
        m_normalAttackHitEffect_3.init(10);

        GameObject nomalAttackEffect = Resources.Load(PREFAB_PATH.CHARACTER_SAM_NORMAL_ATTACK_HIT_3, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_normalAttackHitEffect_3.m_objectNum; i++)
        {
            m_normalAttackHitEffect_3.m_list.Add(GameObject.Instantiate(nomalAttackEffect));
            m_normalAttackHitEffect_3.m_ctrl.Add(m_normalAttackHitEffect_3.m_list[i].GetComponent<EffectCtrl>());
            m_normalAttackHitEffect_3.m_list[i].SetActive(false);
        }
    }

    public void printNormalAttackEffect(Vector3 position, int atkType)
    {
        float size = UnityEngine.Random.Range(0.2f, 1.0f);
        switch (atkType)
        {
            case 1 :
                printNormalAttackHitEffect_1(position, size);
                break;
            case 2 :
                printNormalAttackHitEffect_2(position, size);
                break;
            case 3 :
                printNormalAttackHitEffect_3(position, size);
                break;
        }
    }

    public void printNormalAttackHitEffect_1(Vector3 position, float size)
    {
        for (int i = 0; i < m_normalAttackHitEffect_1.m_objectNum; i++)
        {
            if (m_normalAttackHitEffect_1.m_list[i].activeSelf == false)
            {
                m_normalAttackHitEffect_1.m_list[i].SetActive(true);
                m_normalAttackHitEffect_1.m_list[i].transform.localScale = new Vector3(size, size, 1);
                m_normalAttackHitEffect_1.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void printNormalAttackHitEffect_2(Vector3 position, float size)
    {
        for (int i = 0; i < m_normalAttackHitEffect_2.m_objectNum; i++)
        {
            if (m_normalAttackHitEffect_2.m_list[i].activeSelf == false)
            {
                m_normalAttackHitEffect_2.m_list[i].SetActive(true);
                m_normalAttackHitEffect_1.m_list[i].transform.localScale = new Vector3(size, size, 1);
                m_normalAttackHitEffect_2.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void printNormalAttackHitEffect_3(Vector3 position, float size)
    {
        for (int i = 0; i < m_normalAttackHitEffect_3.m_objectNum; i++)
        {
            if (m_normalAttackHitEffect_3.m_list[i].activeSelf == false)
            {
                m_normalAttackHitEffect_3.m_list[i].SetActive(true);
                m_normalAttackHitEffect_1.m_list[i].transform.localScale = new Vector3(size, size, 1);
                m_normalAttackHitEffect_3.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }
}
