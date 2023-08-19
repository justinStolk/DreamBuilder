using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatClientExample;

public class Building : RemoteCityComponent
{
    public override int EvaluateNeighbourScore(List<NetworkCityComponent> components)
    {
        int score = 0;
        foreach(NetworkCityComponent ncc in components)
        {
            if (ncc.GetType() == typeof(Building))
            {
                score++;
            } else if (ncc.GetType() == typeof(Road) && ncc.transform.position == transform.position + transform.forward)
            {
                score += 3;
            }
        }

        return score;
    }
}
