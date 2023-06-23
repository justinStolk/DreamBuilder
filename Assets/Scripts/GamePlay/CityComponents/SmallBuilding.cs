using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBuilding : CityComponent
{
    public override void OnComponentPlaced()
    {
        base.OnComponentPlaced();

        for(int x = -areaWidthOffset; x <= areaWidthOffset + componentGridWidth - 1; x++)
        {
            for(int y = -areaHeightOffset; y <= areaHeightOffset + componentGridHeight - 1 ; y++)
            {
                Vector2Int evaluatedPosition = new Vector2Int(x, y) + Vector3ToVector2Int(transform.position);
                if (!cityGrid.cityComponents.ContainsKey(evaluatedPosition))
                {
                    continue;
                }
                if(evaluatedPosition == Vector3ToVector2Int(transform.position) + Vector3ToVector2Int(transform.forward))
                {
                    Debug.Log("This point is in front: " + evaluatedPosition);
                    if(cityGrid.cityComponents[evaluatedPosition] != null)
                    {
                        if(cityGrid.cityComponents[evaluatedPosition].GetType() == typeof(Road))
                        {
                            EventSystem.CallEvent(EventSystem.EventType.ON_SCORE_CHANGED, 5);
                            Debug.Log("Found a road in front!");
                        }
                    }
                }
                if(cityGrid.cityComponents[evaluatedPosition] != null)
                {
                    if(cityGrid.cityComponents[evaluatedPosition].GetType() == typeof(SmallBuilding) && cityGrid.cityComponents[evaluatedPosition] != this)
                    {
                        EventSystem.CallEvent(EventSystem.EventType.ON_SCORE_CHANGED, 1);
                        Debug.Log("Found a building nearby!");
                    }
                }
                //Debug.Log("Evaluating: " + evaluatedPosition);

            }
        }
        Debug.Log("Added a point for placing the building!");
        EventSystem.CallEvent(EventSystem.EventType.ON_SCORE_CHANGED, 1);
        cityGrid.cityComponents[Vector3ToVector2Int(transform.position)] = this;
    }

}
