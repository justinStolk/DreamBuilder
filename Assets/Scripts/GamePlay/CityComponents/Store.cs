using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : CityComponent
{
    public override void OnComponentPlaced()
    {
        base.OnComponentPlaced();

        for (int x = -areaWidthOffset; x <= areaWidthOffset + componentGridWidth - 1; x++)
        {
            for (int y = -areaHeightOffset; y <= areaHeightOffset + componentGridHeight - 1; y++)
            {
                Vector2Int evaluatedPosition = new Vector2Int(x, y) + Vector3ToVector2Int(transform.position);
                if (!cityGrid.cityComponents.ContainsKey(evaluatedPosition))
                {
                    continue;
                }
                if (cityGrid.cityComponents[evaluatedPosition] != null)
                {
                    if (cityGrid.cityComponents[evaluatedPosition].GetType() == typeof(SmallBuilding) && cityGrid.cityComponents[evaluatedPosition] != this)
                    {
                        EventSystem.CallEvent(EventSystem.EventType.ON_SCORE_CHANGED, 2);
                        Debug.Log("Found a building nearby!");
                    }
                }
            }
        }
    }
}
