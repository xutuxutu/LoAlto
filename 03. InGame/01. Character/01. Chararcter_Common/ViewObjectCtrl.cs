using UnityEngine;
using System.Collections.Generic;

public class ViewObjectCtrl : MonoBehaviour
{
    public Transform m_viewObject;
    private Transform m_characterTransform;
    private Vector3 m_localPosition;
    private Transform m_cameraTransform;

    private bool m_checkCamera;
    private bool m_checkObject;
    private Vector3 m_hitPoint;

    private LayerMask mask;

    private Vector3 m_fixedPosition = new Vector3(0, 1.354f, 0.585f);
    private SphereCollider m_trigger;
    public void Start()
    {
        m_characterTransform = InGameMgr.getInstance().getOwnCharacterCtrl().transform.parent;
        m_localPosition = transform.localPosition;
        m_cameraTransform = InGameMgr.getInstance().getCharacterCamera().transform;

        mask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14) | ( 1 << 16) | ( 1 << 17);
        mask = ~mask;
    }

    public void Update()
    {
        checkCameraDist();
        checkRightObject();
        checkObjectDist();
        //checkBack();

    }

    public void checkBack()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_characterTransform.position + Vector3.up, -transform.forward, out hit, 0.5f, mask))
        {
            transform.localPosition = m_fixedPosition;
        }
        else
            transform.localPosition = m_localPosition;
    }

    public void checkCameraDist()
    {
        float dist = Vector3.Distance(transform.localPosition, m_cameraTransform.localPosition);
        Debug.DrawRay(m_viewObject.position, m_viewObject.forward, Color.red, 1f);
        float minDist = 1.0f;
        if (m_checkObject == true)
            minDist = 1.5f;
        if (dist < minDist)
        {
            m_checkCamera = true;
            dist = Vector3.Distance(m_viewObject.position, m_cameraTransform.position);
            if (dist < 0.7f)
            {
                Debug.Log("foward");
                RaycastHit hit;
                if (Physics.Raycast(m_viewObject.position, m_viewObject.forward, out hit, 0.2f))
                    return;

                Vector3 targetPosition = new Vector3(m_viewObject.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                dist = Vector3.Distance(m_viewObject.localPosition, targetPosition);
                Debug.Log(dist);
                if (dist < 1)
                {
                    m_viewObject.position += m_viewObject.forward * 5f * Time.deltaTime;
                    Debug.Log("forwardMove");
                }
            }
        }
        else
            m_checkCamera = false;
    }

    public void checkRightObject()
    {
        Debug.DrawRay(m_characterTransform.position + Vector3.up, transform.right, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(m_characterTransform.position + Vector3.up, m_characterTransform.right, out hit, 1f, mask))
        {
            Vector3 hitPoint = hit.point;
            if (Physics.Raycast(m_viewObject.position, m_viewObject.right, out hit, 0.8f, mask))
            {
                m_checkObject = true;
                float dist = Vector3.Distance(m_viewObject.position, hit.point);
                if (dist < 0.8f)
                {
                    Vector3 targetPosition = new Vector3(transform.localPosition.x, m_viewObject.localPosition.y, m_viewObject.localPosition.z);
                    dist = Vector3.Distance(m_viewObject.localPosition, targetPosition);
                    if (dist < 1)
                        m_viewObject.position += -transform.right * 5f * Time.deltaTime;
                }
            }
        }
        else
            m_checkObject = false;
        
        /*
        RaycastHit hit;
        Debug.DrawRay(m_characterTransform.position + Vector3.up, transform.right, Color.red, 1.0f);
        if (m_checkObject == false)
        {
            if (Physics.Raycast(m_characterTransform.position + Vector3.up, m_characterTransform.right, out hit, 1.0f, mask))
            {
                m_checkObject = true;
                //부딪혔을 때 원래의 위치를 저장
                m_hitPoint = transform.position;
            }
        }
        else
        {
            //원래의 위치에서 레이를 쏨
            if (Physics.Raycast(m_hitPoint, transform.right, out hit, 0.8f, mask))
            {
                //현재 위치에서 부딪힌 위치까지의 거리
                float dist = Vector3.Distance(transform.position, hit.point);
                Debug.Log(dist);
                if (dist < 0.9f)
                {
                    Vector3 moveVec = Vector3.Normalize(transform.position - hit.point);
                    transform.position += moveVec * 5f * Time.deltaTime;
                }
                else if (dist > 1.0f)
                    m_checkObject = false;
            }
            else //원래의 위치에서부터 닿는 물체가 없는 경우
                m_checkObject = false;
        }
        */
    }

    public void checkObjectDist()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 0.7f, mask) == false)
        {
            if (m_checkCamera == false)
            {
                Vector3 targetPosition = new Vector3(m_viewObject.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                m_viewObject.localPosition = Vector3.Lerp(m_viewObject.localPosition, targetPosition, Time.deltaTime * 10f);
            }
        }
        
        if (Physics.Raycast(m_characterTransform.position + Vector3.up, m_characterTransform.right, out hit, 0.8f, mask) == false)
        {
            if (m_checkObject == false)
            {
                Vector3 targetPosition = new Vector3(transform.localPosition.x, m_viewObject.localPosition.y, m_viewObject.localPosition.z);
                m_viewObject.localPosition = Vector3.Lerp(m_viewObject.localPosition, targetPosition, Time.deltaTime * 10f);
            }
        }
    }
}
