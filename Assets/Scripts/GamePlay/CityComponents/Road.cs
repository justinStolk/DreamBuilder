using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { NORTH, WEST, EAST, SOUTH }
public class Road : RemoteCityComponent
{
    public List<Direction> Directions;

    public override int EvaluateNeighbourScore(List<NetworkCityComponent> components)
    {
        int score = 0;
        foreach(Direction d in Directions)
        {
            foreach(NetworkCityComponent ncc in components)
            {
                if(ncc.GetType() != typeof(Road))
                {
                    continue;
                }
                Road r = ncc as Road;
                switch (d)
                {
                    case Direction.NORTH:
                        if (r.Directions.Contains(Direction.SOUTH) && r.transform.position == transform.position - transform.forward)
                        {
                            score++;
                        }
                        break;
                        
                    case Direction.WEST:
                        if (r.Directions.Contains(Direction.EAST) && r.transform.position == transform.position + transform.right)
                        {
                            score++;
                        }
                        break;
                        
                    case Direction.EAST:
                        if (r.Directions.Contains(Direction.WEST) && r.transform.position == transform.position - transform.right)
                        {
                            score++;
                        }
                        break;
                        
                    case Direction.SOUTH:
                        if (r.Directions.Contains(Direction.NORTH) && r.transform.position == transform.position + transform.forward)
                        {
                            score++;
                        }
                        break;
                }
            }

        }
        return score;

    }

}
