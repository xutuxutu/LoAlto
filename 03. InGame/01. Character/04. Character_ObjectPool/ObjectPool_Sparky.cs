using UnityEngine;

public class ObjectPool_Sparky : MonoBehaviour
{
    //일반탄
    private OBJECT_POOL<Bullet> m_normalBullet;
    //일반탄 착탄이펙트_일반
    private OBJECT_POOL<BulletHit> m_normalBulletHit_Ground;
    //일반탄 착탄이펙트_생명체
    private OBJECT_POOL<EffectCtrl> m_normalBulletHit_Creature;
    //폭발탄
    private OBJECT_POOL<ExplosionBullet> m_explosionBullet;
    //폭발탄 착탄이펙트
    private OBJECT_POOL<ExplosionBulletHit> m_explosionBulletHit;
    //속사 총알
    private OBJECT_POOL<Bullet> m_rapidFireBullet;
    //C4 폭탄
    private OBJECT_POOL<C4_Bomb> m_c4Bomb_Object;
    private OBJECT_POOL<EffectCtrl> m_c4Bomb_Explosion;
    //발자국 이펙트
    private OBJECT_POOL<EffectCtrl> m_runningEffect_right;
    private OBJECT_POOL<EffectCtrl> m_runningEffect_left;
    //점프 이펙트
    private OBJECT_POOL<EffectCtrl> m_jumpEffect;
    private OBJECT_POOL<EffectCtrl> m_landingEffect;

    private static ObjectPool_Sparky m_instance;
    void Awake()
    {
        m_instance = this;
    }

    void Start()
    {
        initNormalBulletList();
        initNormalBulleHitList();
        initExplosionBulletList();
        initExplosionBulletHitList();
        //initRapidFireBulletList();
        initC4Bomb_Object_List();
        initC4Bomb_ExplosionEffect();
        initRunningEffectList();
        initJumpEffectList();
        initLandingEffectList();
    }

    public static ObjectPool_Sparky getInstance() { return m_instance; }

    private void initNormalBulletList()
    {
        m_normalBullet.init(10);

        GameObject bullet = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_BULLET, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_normalBullet.m_objectNum; i++)
        {
            m_normalBullet.m_list.Add(GameObject.Instantiate(bullet));
            m_normalBullet.m_ctrl.Add(m_normalBullet.m_list[i].GetComponent<Bullet>());
            m_normalBullet.m_list[i].SetActive(false);
        }
    }

    private void initNormalBulleHitList()
    {
        m_normalBulletHit_Ground.init(10);

        GameObject bulletHit = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_BULLET_HIT_EFFECT_GROUND_1, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_normalBulletHit_Ground.m_objectNum; i++)
        {
            m_normalBulletHit_Ground.m_list.Add(GameObject.Instantiate(bulletHit));
            m_normalBulletHit_Ground.m_ctrl.Add(m_normalBulletHit_Ground.m_list[i].GetComponent<BulletHit>());
            m_normalBulletHit_Ground.m_ctrl[i].init();
            m_normalBulletHit_Ground.m_list[i].SetActive(false);
        }

        m_normalBulletHit_Creature.init(10);

        bulletHit = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_BULLET_HIT_EFFECT_CREATURE, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_normalBulletHit_Creature.m_objectNum; i++)
        {
            m_normalBulletHit_Creature.m_list.Add(GameObject.Instantiate(bulletHit));
            m_normalBulletHit_Creature.m_ctrl.Add(m_normalBulletHit_Creature.m_list[i].GetComponent<EffectCtrl>());
            m_normalBulletHit_Creature.m_ctrl[i].init();
            m_normalBulletHit_Creature.m_list[i].SetActive(false);
        }
    }

    private void initExplosionBulletList()
    {
        m_explosionBullet.init(4);

        GameObject bullet = Resources.Load(PREFAB_PATH.EXPLOSION_BULLET, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_explosionBullet.m_objectNum; i++)
        {
            m_explosionBullet.m_list.Add(GameObject.Instantiate(bullet));
            m_explosionBullet.m_ctrl.Add(m_explosionBullet.m_list[i].GetComponent<ExplosionBullet>());
            m_explosionBullet.m_list[i].SetActive(false);
        }
    }

    private void initExplosionBulletHitList()
    {
        m_explosionBulletHit.init(8);

        GameObject bulletHit = Resources.Load(PREFAB_PATH.EXPLOSION_BUULET_HIT, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_explosionBullet.m_objectNum; i++)
        {
            m_explosionBulletHit.m_list.Add(GameObject.Instantiate(bulletHit));
            m_explosionBulletHit.m_ctrl.Add(m_explosionBulletHit.m_list[i].GetComponent<ExplosionBulletHit>());
            m_explosionBulletHit.m_ctrl[i].init();
            m_explosionBulletHit.m_list[i].SetActive(false);
        }
    }

    private void initRapidFireBulletList()
    {
        m_rapidFireBullet.init(15);

        GameObject[] bullet = new GameObject[2];

        bullet[0] = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_RAPID_FIRE_BULLET_1, typeof(GameObject)) as GameObject;
        bullet[1] = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_RAPID_FIRE_BULLET_2, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_rapidFireBullet.m_objectNum; i++)
        {
            m_rapidFireBullet.m_list.Add(GameObject.Instantiate(bullet[i % 2]));
            m_rapidFireBullet.m_ctrl.Add(m_rapidFireBullet.m_list[i].GetComponent<Bullet>());
            m_rapidFireBullet.m_list[i].SetActive(false);
        }
    }

    private void initC4Bomb_Object_List()
    {
        m_c4Bomb_Object.init(5);

        GameObject c4Bomb = Resources.Load(PREFAB_PATH.C4_BOMB, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_c4Bomb_Object.m_objectNum; i++)
        {
            m_c4Bomb_Object.m_list.Add(GameObject.Instantiate(c4Bomb));
            m_c4Bomb_Object.m_ctrl.Add(m_c4Bomb_Object.m_list[i].GetComponent<C4_Bomb>());
            m_c4Bomb_Object.m_ctrl[i].init();
            m_c4Bomb_Object.m_list[i].SetActive(false);
        }
    }

    private void initC4Bomb_ExplosionEffect()
    {
        m_c4Bomb_Explosion.init(5);

        GameObject c4Effect = Resources.Load(PREFAB_PATH.C4_BOMB_EXPLOSION, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_c4Bomb_Explosion.m_objectNum; i++)
        {
            m_c4Bomb_Explosion.m_list.Add(GameObject.Instantiate(c4Effect));
            m_c4Bomb_Explosion.m_ctrl.Add(m_c4Bomb_Explosion.m_list[i].GetComponent<EffectCtrl>());
            m_c4Bomb_Explosion.m_ctrl[i].init();
            m_c4Bomb_Explosion.m_list[i].SetActive(false);
        }
    }

    private void initRunningEffectList()
    {
        m_runningEffect_right.init(5);
        m_runningEffect_left.init(5);

        GameObject runningDust_left = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_RUNNING_EFFECT_LEFT, typeof(GameObject)) as GameObject;
        GameObject runningDust_right = Resources.Load(PREFAB_PATH.CHARACTER_SPARKY_RUNNING_EFFECT_RIGHT, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_runningEffect_left.m_objectNum; i++)
        {
            m_runningEffect_left.m_list.Add(GameObject.Instantiate(runningDust_left));
            m_runningEffect_left.m_ctrl.Add(m_runningEffect_left.m_list[i].GetComponent<EffectCtrl>());
            m_runningEffect_left.m_ctrl[i].init();
            m_runningEffect_left.m_list[i].SetActive(false);

            m_runningEffect_right.m_list.Add(GameObject.Instantiate(runningDust_right));
            m_runningEffect_right.m_ctrl.Add(m_runningEffect_right.m_list[i].GetComponent<EffectCtrl>());
            m_runningEffect_right.m_ctrl[i].init();
            m_runningEffect_right.m_list[i].SetActive(false);
        }
    }

    private void initJumpEffectList()
    {
        m_jumpEffect.init(4);

        GameObject jumpEffect = Resources.Load(PREFAB_PATH.CHARACTER_SPARKT_JUMP_EFFECT, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_jumpEffect.m_objectNum; i++)
        {
            m_jumpEffect.m_list.Add(GameObject.Instantiate(jumpEffect));
            m_jumpEffect.m_ctrl.Add(m_jumpEffect.m_list[i].GetComponent<EffectCtrl>());
            m_jumpEffect.m_ctrl[i].init();
            m_jumpEffect.m_list[i].SetActive(false);
        }
    }

    private void initLandingEffectList()
    {
        m_landingEffect.init(4);

        GameObject landingEffect = Resources.Load(PREFAB_PATH.CHARACTER_SPARKT_LANDING_EFFECT, typeof(GameObject)) as GameObject;
        for (int i = 0; i < m_landingEffect.m_objectNum; i++)
        {
            m_landingEffect.m_list.Add(GameObject.Instantiate(landingEffect));
            m_landingEffect.m_ctrl.Add(m_landingEffect.m_list[i].GetComponent<EffectCtrl>());
            m_landingEffect.m_ctrl[i].init();
            m_landingEffect.m_list[i].SetActive(false);
        }
    }

    public void fireNormalBullet(Vector3 firePosition, Vector3 fireDirection)
    {
        for (int i = 0; i < m_normalBullet.m_objectNum; i++)
        {
            if (m_normalBullet.m_list[i].active == false)
            {
                m_normalBullet.m_list[i].SetActive(true);
                m_normalBullet.m_ctrl[i].setStartPosition(firePosition, fireDirection);
                break;
            }
        }
    }

    public void printNormalBulletHit_Ground(Vector3 hitPoint, Vector3 hitPointNormal)
    {
        for (int i = 0; i < m_normalBulletHit_Ground.m_objectNum; i++)
        {
            if (m_normalBulletHit_Ground.m_list[i].activeSelf == false)
            {
                m_normalBulletHit_Ground.m_list[i].SetActive(true);
                m_normalBulletHit_Ground.m_ctrl[i].setPosition(hitPoint, hitPointNormal);
                return;
            }
        }
    }

    public void printNormalBulletHit_Creature(Vector3 hitPoint)
    {
        for (int i = 0; i < m_normalBulletHit_Creature.m_objectNum; i++)
        {
            if (m_normalBulletHit_Creature.m_list[i].activeSelf == false)
            {
                m_normalBulletHit_Creature.m_list[i].SetActive(true);
                m_normalBulletHit_Creature.m_ctrl[i].setPosition_RandomRotScale(hitPoint);
                return;
            }
        }
    }

    public void fireExplosionBullet(Vector3 firePosition, Vector3 moveVector)
    {
        for (int i = 0; i < m_explosionBullet.m_objectNum; i++)
        {
            if (m_explosionBullet.m_list[i].active == false)
            {
                m_explosionBullet.m_list[i].SetActive(true);
                m_explosionBullet.m_ctrl[i].setMoveVector(firePosition, moveVector);
                break;
            }
        }
    }

    public void printExplosionBulletHit(Vector3 position)
    {
        for (int i = 0; i < m_explosionBulletHit.m_objectNum; i++)
        {
            if (m_explosionBulletHit.m_list[i].active == false)
            {
                m_explosionBulletHit.m_list[i].SetActive(true);
                m_explosionBulletHit.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void fireRapidFireBullet(Vector3 firePosition, Vector3 fireDirection)
    {
        for (int i = 0; i < m_rapidFireBullet.m_objectNum; i++)
        {
            if (m_rapidFireBullet.m_list[i].active == false)
            {
                m_rapidFireBullet.m_list[i].SetActive(true);
                m_rapidFireBullet.m_ctrl[i].setStartPosition(firePosition, fireDirection);
                break;
            }
        }
    }

    public C4_Bomb throwC4Bomb(Vector3 firePosition, Vector3 moveVector)
    {
        for (int i = 0; i < m_c4Bomb_Object.m_objectNum; i++)
        {
            if (m_c4Bomb_Object.m_list[i].active == false)
            {
                m_c4Bomb_Object.m_list[i].SetActive(true);
                m_c4Bomb_Object.m_ctrl[i].setMoveVector(firePosition, moveVector);
                return m_c4Bomb_Object.m_ctrl[i];
            }
        }

        return null;
    }

    public void printC4BombExplosion(Vector3 position)
    {
        for (int i = 0; i < m_c4Bomb_Explosion.m_objectNum; i++)
        {
            if (m_c4Bomb_Explosion.m_list[i].active == false)
            {
                m_c4Bomb_Explosion.m_list[i].SetActive(true);
                m_c4Bomb_Explosion.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void printRunningEffect(Vector3 position, SOUND_POOL.SPARKY.FOOT_STEP footType)
    {
        switch (footType)
        {
            case SOUND_POOL.SPARKY.FOOT_STEP.LEFT:
                for (int i = 0; i < m_runningEffect_left.m_objectNum; i++)
                {
                    if (m_runningEffect_left.m_list[i].active == false)
                    {
                        m_runningEffect_left.m_list[i].SetActive(true);
                        m_runningEffect_left.m_ctrl[i].setPosition(position);
                        return;
                    }
                }
                break;
            case SOUND_POOL.SPARKY.FOOT_STEP.RIGHT:
                for (int i = 0; i < m_runningEffect_right.m_objectNum; i++)
                {
                    if (m_runningEffect_right.m_list[i].active == false)
                    {
                        m_runningEffect_right.m_list[i].SetActive(true);
                        m_runningEffect_right.m_ctrl[i].setPosition(position);
                        return;
                    }
                }
                break;
        }
    }

    public void printJumpEffect(Vector3 position)
    {
        for (int i = 0; i < m_jumpEffect.m_objectNum; i++)
        {
            if (m_jumpEffect.m_list[i].active == false)
            {
                m_jumpEffect.m_list[i].SetActive(true);
                m_jumpEffect.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }

    public void printLandingEffect(Vector3 position)
    {
        for (int i = 0; i < m_landingEffect.m_objectNum; i++)
        {
            if (m_landingEffect.m_list[i].active == false)
            {
                m_landingEffect.m_list[i].SetActive(true);
                m_landingEffect.m_ctrl[i].setPosition(position);
                break;
            }
        }
    }
}
