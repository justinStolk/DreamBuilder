using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPC_Tester : NetworkedBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestEmptyMessage()
    {
        Debug.Log("Tested empty message!");
    }

    private void TestArgumentedMessage(float f, Vector3 v3, bool b)
    {
        Debug.Log(f);
        Debug.Log(v3);
        Debug.Log(b);
    }
}
