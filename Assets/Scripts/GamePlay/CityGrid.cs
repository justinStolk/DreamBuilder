using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGrid : MonoBehaviour
{
    public static CityGrid GlobalGrid { get; private set; }

    [Min(5)]
    public int GridWidth, GridHeight;

    public Dictionary<Vector2Int, CityComponent> cityComponents = new();

    private void Awake()
    {
        if (GlobalGrid != null)
        {
            Debug.LogWarning("Multiple instances of CityGrid exist, this shouldn't happen!");
            Destroy(gameObject);
            return;
        }
        GlobalGrid = this;
    }
    private void Start()
    {
        GameObject floorPanel = Resources.Load("FloorPanel") as GameObject;
        GameObject panels = new GameObject("Panels");
        for(int x = 0; x < GridWidth; x++)
        {
            for(int y = 0; y < GridHeight; y++)
            {
                Vector2Int componentPosition = new Vector2Int(x, y);
                cityComponents.Add(componentPosition, null);
                Instantiate(floorPanel, new Vector3(x, 0, y), Quaternion.identity, panels.transform);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(new Vector3(x, 0, y), Vector3.one * 0.9f - Vector3.up * 0.8f);
            }
        }
    }
}
