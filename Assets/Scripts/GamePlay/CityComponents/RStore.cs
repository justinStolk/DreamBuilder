using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RStore : RemoteCityComponent
{

    public override int EvaluateNeighbourScore(List<NetworkCityComponent> components)
    {
        int score = 0;
        foreach(NetworkCityComponent ncc in components)
        {
            if(ncc.GetType() == typeof(Building) && ncc.transform.position != transform.position + transform.forward)
            {
                score++;
            }
        }
        foreach (NetworkCityComponent ncc in components)
        {
            if (ncc.GetType() == typeof(Road) && ncc.transform.position == transform.position + transform.forward)
            {
                score += 2;
            }
        }
        return score;
    }
}
