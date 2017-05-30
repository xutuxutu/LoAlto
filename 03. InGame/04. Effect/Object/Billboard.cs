

using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    private Transform m_mainCamera = null;
	public bool LockX = false;
	public bool LockY = true;
	public bool LockZ = false;

	float xlock = 0.0f;
	float ylock = 0.0f;
	float zlock = 0.0f;

    void Start()
    {
        m_mainCamera = InGameMgr.getInstance().getCharacterCamera().transform;
    }

    void Update()
    {
		if (m_mainCamera != null)
        {
			if (LockX == true)
				xlock = transform.eulerAngles.x;
            else
				xlock = m_mainCamera.eulerAngles.x;

			if (LockY == true)
				ylock = transform.eulerAngles.y;
            else
				ylock = m_mainCamera.eulerAngles.y;

			if (LockZ == true)
				zlock = transform.eulerAngles.z;
            else
				zlock = m_mainCamera.eulerAngles.z;

//            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
//											 _mainCameraTransf.eulerAngles.y,
//                                             transform.eulerAngles.z);

			transform.eulerAngles = new Vector3(xlock,ylock,zlock);
        }
        else
            m_mainCamera = InGameMgr.getInstance().getCharacterCamera().transform;

        //transform.forward = -m_mainCamera.forward;
    }
}
