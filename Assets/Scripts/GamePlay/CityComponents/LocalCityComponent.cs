using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatClientExample;

public class LocalCityComponent : NetworkCityComponent
{

    public override void MoveToPosition(Vector3Int newPosition)
    {
        transform.position = newPosition;
        //RPCMessage message = new RPCMessage();
        //message.target = this;
        //message.MethodName = "MoveToPosition";
    }

    public override void PlaceComponent()
    {
        Debug.Log("Should place at this position");
        //RPCMessage message = new RPCMessage();
        //message.target = this;
        //message.MethodName = "PlaceComponent";
    }

    public override void Rotate()
    {
        transform.Rotate(new Vector3(0, 90, 0));
        //RPCMessage message = new RPCMessage();
        //message.target = this;
        //message.MethodName = "Rotate";
    }
}
