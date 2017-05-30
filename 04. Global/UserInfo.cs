using UnityEngine;
using System.Collections;

namespace USER_INFO
{
    public enum TYPE { OWN, OTHER }
}

public class UserInfo : MonoBehaviour
{
    private int m_userID = 0;//-1;
    private string m_userName;
    private USER_INFO.TYPE m_type;

    public void setUserID(int pID) { m_userID = pID; }
    public void setUserName(string pName) { m_userName = pName; }
    public void setUserType(USER_INFO.TYPE type) { m_type = type; }

    public int getUserID() { return m_userID; }
    public string getUserName() { return m_userName; }
    public USER_INFO.TYPE getUserType() { return m_type; }
}
