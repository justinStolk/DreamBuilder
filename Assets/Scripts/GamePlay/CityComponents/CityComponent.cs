using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityComponent : NetworkedBehaviour
{
    public Vector2Int Position { get; set; }

    protected Material defaultMaterial;
    protected Material canPlaceMaterial;
    protected Material cannotPlaceMaterial;

    [Min(1)]
    [SerializeField] protected int componentGridWidth = 1 , componentGridHeight = 1 ;
    [SerializeField] protected int areaWidthOffset, areaHeightOffset;

    protected List<Transform> positionPoints = new();
    protected static CityGrid cityGrid;

    private void Awake()
    {
        if(cityGrid == null)
        {
            cityGrid = CityGrid.GlobalGrid;
        }
    }
    private void Start()
    {
        for (int x = 0; x < componentGridWidth; x++)
        {
            for (int y = 0; y < componentGridHeight; y++)
            {
                Transform point = new GameObject("point").transform;
                point.SetParent(this.transform);
                Vector2Int objectPos = Vector3ToVector2Int(transform.position) + new Vector2Int(x, y);
                point.transform.position = new Vector3(objectPos.x, 0, objectPos.y);
                positionPoints.Add(point);
            }
        }
    }

    public bool CanPlaceComponent()
    {
        foreach(Transform point in positionPoints)
        {
            if(cityGrid.cityComponents[Vector3ToVector2Int(point.position)] != null)
            {
                return false;
            }
        }
        return true;
    }
    public virtual void OnComponentPlaced()
    {
        foreach (Transform point in positionPoints)
        {
            cityGrid.cityComponents[Vector3ToVector2Int(point.position)] = this;
        }
    }
    
    protected Vector2Int Vector3ToVector2Int(Vector3 vector3)
    {
        return new Vector2Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int x = -areaWidthOffset; x <= areaWidthOffset + componentGridWidth - 1; x++)
        {
            for (int y = -areaHeightOffset; y <= areaHeightOffset + componentGridHeight - 1; y++)
            {
                Gizmos.DrawWireCube(new Vector3(x, 0, y) + transform.position, Vector3.one);
            }
        }
        foreach(Transform p in positionPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(p.position, 0.5f);
        }
    }
}
