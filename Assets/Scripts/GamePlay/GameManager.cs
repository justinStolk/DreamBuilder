using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public CityComponent smallBuildingPrefab;
    public CityComponent storePrefab;
    public CityComponent roadPrefab;
    public LayerMask floorMask;

    [SerializeField] private Text scoreText;
    private CityComponent activeComponent;
    private CityGrid cityGrid;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        //smallBuildingPrefab = Resources.Load("SmallBuilding") as CityComponent;
        cityGrid = FindObjectOfType<CityGrid>();
        EventSystem.Subscribe(EventSystem.EventType.ON_SCORE_CHANGED, ChangeScore);
    }

    // Update is called once per frame
    void Update()
    {
        if(activeComponent != null)
        {
            MoveComponentGridWise();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(activeComponent != null)
            {
                Destroy(activeComponent.gameObject);
            }
            activeComponent = Instantiate(smallBuildingPrefab);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (activeComponent != null)
            {
                Destroy(activeComponent.gameObject);
            }
            activeComponent = Instantiate(roadPrefab);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (activeComponent != null)
            {
                Destroy(activeComponent.gameObject);
            }
            activeComponent = Instantiate(storePrefab);
        }

        if (Input.GetMouseButtonDown(0) && activeComponent != null)
        {
            // Instead request to place this component by sending a request to the server, through the client
            if(activeComponent.CanPlaceComponent())
            {
                activeComponent.OnComponentPlaced();
                activeComponent = null;
            }
        }

    }

    private void MoveComponentGridWise()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay, out RaycastHit hit, floorMask))
        {
            Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(hit.transform.position.x), Mathf.RoundToInt(hit.transform.position.z));
            activeComponent.transform.position = new Vector3(targetPosition.x, 0, targetPosition.y);
            activeComponent.Position = targetPosition;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            activeComponent.transform.Rotate(new Vector3(0, 90, 0));  
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            activeComponent.transform.Rotate(new Vector3(0, -90, 0));
        }
    }

    private void ChangeScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

}
