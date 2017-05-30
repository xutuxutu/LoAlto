using UnityEngine;
using System.Collections;

public class DeadViewCameraCtrl : MonoBehaviour
{
    private Camera m_deadViewCamera;
    private Vector3 m_directionPosition;

    void init()
    {
        m_directionPosition = new Vector3(0.49f, 2.71f, -5);
        m_deadViewCamera = InGameMgr.getInstance().getDeadViewCamera().GetComponent<Camera>();
        Debug.Log(m_deadViewCamera);
    }

    public void printDeadViewCameraDirection()
    {
        StartCoroutine("deadViewCameraDirection");
    }

    public IEnumerator deadViewCameraDirection()
    {
        float dist = Vector3.Distance(transform.position, m_directionPosition);
        while(true)
        {
            m_deadViewCamera.transform.localPosition = Vector3.Lerp(m_deadViewCamera.transform.localPosition, m_directionPosition, Time.deltaTime);

            yield return null;
        }
    }
}
