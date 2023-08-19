using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatClientExample;

public class RemoteCityComponent : NetworkCityComponent
{
    public override void MoveToPosition(Vector3Int newPosition)
    {
        transform.position = newPosition;
    }

    public override void PlaceComponent()
    {
        //if (CanPlaceComponent())
        //{
        //    placedComponents.Add(transform.position, this);
        //    // Place the city component
        //    RPCMessage msg = new RPCMessage();
        //    msg.TargetID = NetworkID;
        //    msg.MethodName = "PlaceComponent";
            
        //    // Code to end the turn, called on the server
        //}

    }

    public override void Rotate()
    {
        transform.Rotate(new Vector3(0, 90, 0));
    }
    public virtual int EvaluateNeighbourScore(List<NetworkCityComponent> components)
    {
        return 0;
    }

    //protected bool CanPlaceComponent()
    //{
    //    return !placedComponents.ContainsKey(transform.position);
    //}
}
