using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkCityComponent : NetworkedBehaviour
{

    public abstract void Rotate();
    public abstract void PlaceComponent();
    public abstract void MoveToPosition(Vector3Int newPosition);
}
