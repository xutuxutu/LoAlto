using UnityEngine;
using System.Collections;

public class ExplosionBullet : MonoBehaviour
{
    public static float m_moveSpeed = 40;
    public static float m_gravity = 20;

    private Vector3 m_moveVector;
    private Vector3 m_fireDirection;

    public static void initStatus(float moveSpeed, float gravity)
    {
        m_moveSpeed = moveSpeed;
        m_gravity = gravity;
    }

    void Update()
    {
        transform.position += m_moveVector * Time.deltaTime;
        m_moveVector.y -= m_gravity * Time.deltaTime;

        transform.forward = m_moveVector.normalized;

        LayerMask mask = (1 << 8) | (1 << 10) | (1 << 12) | (1 << 13);
        mask = ~mask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f, mask))
        {
            gameObject.SetActive(false);
            ObjectPool_Sparky.getInstance().printExplosionBulletHit(hit.point + Vector3.up * 0.2f);
            InGameMgr.getInstance().getOwnCharacterCtrl().camShakeEvent(CAM_SHAKE_EVENT.TYPE.SPARKY_EB, 1f, true, 0.15f);
        }
    }

    public void setMoveVector(Vector3 startPosition, Vector3 moveVector)
    {
        transform.position = startPosition;
        m_moveVector = moveVector * m_moveSpeed;

        transform.forward = m_moveVector.normalized;
    }
}
