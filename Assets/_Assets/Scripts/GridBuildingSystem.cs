using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private GridXZ<GridObject> grid;

    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    private void Awake()
    {
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
        private Transform transform;
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }

        public void ClearTransform(Transform transform)
        {
            transform = null;
        }

        public bool CanBuild()
        {
            return transform == null;
        }

        public override string ToString()
        {
            return x + "," + z + "\n" + transform;
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

                Transform builtTrans = Instantiate(placedObjectTypeSO.prefab, placedObjWorldPos, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

                foreach(Vector2Int gridPos in gridPosList)
                {
                    grid.GetGridObject(gridPos.x, gridPos.y).SetTransform(builtTrans); 
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

    }

}
