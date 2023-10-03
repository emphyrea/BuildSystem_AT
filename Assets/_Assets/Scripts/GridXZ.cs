using System;
using UnityEngine;

public class GridXZ<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPos;

    public GridXZ(int gridWidth, int gridHeight, float cellSize, Vector3 originPos, Func<GridXZ<TGridObject>, int, int, TGridObject> CreateGridObject)
    {
        this.width = gridWidth;
        this.height = gridHeight;
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x+1, y), Color.white, 100f);
                gridArray[x, y] = CreateGridObject(this, x, y);
            }
        }
    }

    private Vector3 GetWorldPos(int x, int y) //convert grid pos to world pos
    {
        return new Vector3(x, y) * cellSize + originPos;
    }

    private void GetXY(Vector3 worldPos, out int x, out int y) //convert world pos to grid pos
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPos).y / cellSize);
    }

    public void SetGridObject(Vector3 worldPos, TGridObject val)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetGridObject(x, y, val);
    }

    public void SetGridObject(int x, int y, TGridObject val) //set a single value
    {
        if(x >= 0 && y >= 0 && x < width && y < height) //if x and y within grid parameters
        {
            gridArray[x, y] = val;
            if(OnGridValueChanged != null)
            {
                OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
            }
        }
    }

    public TGridObject GetGridObject(int x, int y) //get grid pos
    {
        if (x >= 0 && y >= 0 && x < width && y < height) //if x and y within grid parameters
        {
            return gridArray[x, y];
        }
        else
        {
            return default;
        }
    }
    public TGridObject GetGridObject(Vector3 worldPos) //get world pos
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetGridObject(x, y);
    }

    public int GetHeight()
    {
        return height;
    }
    public int GetWidth()
    {
        return width;
    }
    public float GetCellSize()
    {
        return cellSize;
    }
}