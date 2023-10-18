using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance;

    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private GridXZ<GridObject> grid;

    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    public Action<object, EventArgs> OnSelectedChanged { get; internal set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        int gridWidth = 25;
        int gridHeight = 25;
        float cellSize = 2f;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

        placedObjectTypeSO = placedObjectTypeSOList[0];
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObj;
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObj(PlacedObject placedObj)
        {
            this.placedObj = placedObj;
        }

        public void ClearPlacedObj()
        {
            placedObj = null;
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObj;
        }

        public bool CanBuild()
        {
            return placedObj == null;
        }

        public override string ToString()
        {
            return x + "," + z + "\n" + placedObj;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.GetXZ(Mouse3D.GetMouseWorldPos(), out int x, out int z);

           List<Vector2Int> gridPosList = placedObjectTypeSO.GetGridPosList(new Vector2Int(x, z), dir);
            bool canBuild = true;
            foreach(Vector2Int gridPos in gridPosList)
            {
               if(!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild()) //if cannot build here
                {
                    canBuild = false;
                    break;             
                }
            }


            GridObject gridObject = grid.GetGridObject(x, z);
            if(canBuild)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjWorldPos = grid.GetWorldPos(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

               PlacedObject placedObj = PlacedObject.Create(placedObjWorldPos, new Vector2Int(x, z), dir, placedObjectTypeSO);

                foreach(Vector2Int gridPos in gridPosList)
                {
                    grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObj(placedObj); 
                }
            }
            else
            {
                return;
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placedObjectTypeSO = placedObjectTypeSOList[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placedObjectTypeSO = placedObjectTypeSOList[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            placedObjectTypeSO = placedObjectTypeSOList[2];
        }

        if(Input.GetMouseButtonDown(1))
        {
            GridObject gridObj = grid.GetGridObject(Mouse3D.GetMouseWorldPos());
            PlacedObject placedObj = gridObj.GetPlacedObject();
            if(placedObj != null)
            {
                placedObj.DestroySelf();

                List<Vector2Int> gridPosList = placedObj.GetGridPosList();
                foreach (Vector2Int gridPos in gridPosList)
                {
                    grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObj();
                }
            }
        }

    }

}
