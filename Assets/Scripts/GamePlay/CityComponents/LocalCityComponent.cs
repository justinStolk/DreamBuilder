using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatClientExample;

public abstract class LocalCityComponent : NetworkCityComponent
{
    public override void MoveToPosition(Vector3 newPosition)
    {
        RPCMessage message = new RPCMessage();
        message.target = this;
        message.MethodName = "MoveToPosition";
    }

    public override void PlaceComponent()
    {
        RPCMessage message = new RPCMessage();
        message.target = this;
        message.MethodName = "PlaceComponent";
    }

    public override void Rotate()
    {
        RPCMessage message = new RPCMessage();
        message.target = this;
        message.MethodName = "Rotate";
    }
}
