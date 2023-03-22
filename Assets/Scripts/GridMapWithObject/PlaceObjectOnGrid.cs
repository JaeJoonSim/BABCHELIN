using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlaceObjectOnGrid : MonoBehaviour
{
    public Transform gridCellPrefab;
    public Transform cube;
    public Transform onMousePrefab;
    public Vector3 smoothMousePosition;

    [SerializeField] private int height;
    [SerializeField] private int width;

    private Vector3 mousePosition;
    private Grid[,] grids;
    private Plane plane;

    private void Start()
    {
        CreateGrid();
        plane = new Plane(Vector3.up, transform.position);
    }

    private void Update()
    {
        GetMousePositionOnGrid();
    }

    private void GetMousePositionOnGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (plane.Raycast(ray, out var enter))
        {
            mousePosition = ray.GetPoint(enter);
            smoothMousePosition = mousePosition;
            mousePosition.y = 0;
            mousePosition = Vector3Int.RoundToInt(mousePosition);
            
            foreach(var grid in grids)
            {
                if(grid.cellPosition == mousePosition && grid.isPlaceAble)
                {
                    if(Input.GetMouseButtonUp(0) && onMousePrefab != null)
                    {
                        grid.isPlaceAble = false;
                        onMousePrefab.GetComponent<ObjectFollowMouse>().isOnGrid = true;
                        onMousePrefab.position = grid.cellPosition + new Vector3(0, 0.5f, 0);
                        onMousePrefab = null;
                    }
                }
            }
        }
    }

    public void OnMouseClickUI()
    {
        if (onMousePrefab == null)
        {
            onMousePrefab = Instantiate(cube, mousePosition, Quaternion.identity);
            Debug.Log("클릭이벤트");
        }
    }

    private void CreateGrid()
    {
        grids = new Grid[width, height];
        var name = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector3 worldPosition = new Vector3(i, 0, j);
                Transform obj = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity);
                obj.name = "Cell" + name;
                grids[i, j] = new Grid(true, worldPosition, obj);
                name++;
            }
        }
    }
    
    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    
}

public class Grid
{
    public bool isPlaceAble;
    public Vector3 cellPosition;
    public Transform obj;

    public Grid(bool isPlaceAble, Vector3 cellPosition, Transform obj)
    {
        this.isPlaceAble = isPlaceAble;
        this.cellPosition = cellPosition;
        this.obj = obj;
    }
}