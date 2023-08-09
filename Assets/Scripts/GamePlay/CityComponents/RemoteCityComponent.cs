using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RemoteCityComponent : NetworkCityComponent
{
    private static Dictionary<Vector3, NetworkCityComponent> placedComponents;
    public override void MoveToPosition(Vector3Int newPosition)
    {
        transform.position = newPosition;
    }

    public override void PlaceComponent()
    {
        if (CanPlaceComponent())
        {
            // Place the city component
            // Code to end the turn, called on the server
        }

    }

    public override void Rotate()
    {
        transform.Rotate(new Vector3(0, 90, 0));
    }
    protected bool CanPlaceComponent()
    {
        return !placedComponents.ContainsKey(transform.position);
    }
}
