using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserConnectionHandler : MonoBehaviour
{
    public static UserConnectionHandler Instance { get; private set; }

    public UserData userData { get; private set; } 

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("A connection handler already exists!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        userData = ScriptableObject.CreateInstance<UserData>();
    }
    public void SetUsername(string username)
    {
        userData.Username = username;
    }
    public void SetPassword(string password)
    {
        userData.Password = password;
    }

}
