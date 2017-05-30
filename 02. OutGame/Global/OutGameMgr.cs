using UnityEngine;
using System.Collections;

public class OutGameMgr : MonoBehaviour
{
    private static OutGameMgr m_instance;

    void Awake()
    {
        m_instance = this;
        Debug.Log("OutGameManager : Awake");
    }
	// Use this for initialization
	void Start ()
    {
        Debug.Log("OutGameManager : Start");

        createProjectManager();
	}
	
	// Update is called once per frame
	void Update ()
    {
        OutGameServerMgr.getInstance().analysePacket();
        HTTPManager.getInstance().analysePacket();
	}

    public void createProjectManager()
    {
        GameObject projectMgr = Resources.Load("01. Prefab/05. Global/ProjectManager", typeof(GameObject)) as GameObject;
        projectMgr = GameObject.Instantiate(projectMgr, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
        projectMgr.transform.parent = transform.parent;
    }

    public static OutGameMgr getInstance()
    {
        return m_instance;
    }

    public void startGame()
    {
        ProjectMgr.getInstance().transform.parent = null;
        DontDestroyOnLoad(ProjectMgr.getInstance().gameObject);
        Application.LoadLevel(1);
    }
}
