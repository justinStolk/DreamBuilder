using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RemoteCityComponent : NetworkCityComponent
{
    public override void MoveToPosition(Vector3 newPosition)
    {
        throw new System.NotImplementedException();
    }

    public override void PlaceComponent()
    {
        throw new System.NotImplementedException();
    }

    public override void Rotate()
    {
        throw new System.NotImplementedException();
    }

}
