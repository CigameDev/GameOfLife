using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval =0.05f;//interval khoang thoi gian

    private HashSet<Vector3Int> aliveCells;//HashSet khong chap nhan cac phan tu trung lap
    private HashSet<Vector3Int> cellToCheck;

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellToCheck = new HashSet<Vector3Int>();
    }
    private void Start()
    {
        SetPattern(pattern);       
    }
    private void SetPattern(Pattern pattern)
    {
        Clear();
        Vector2Int center = pattern.GetCenter();
        for(int i=0;i<pattern.cells.Length;i++)
        {
            Vector3Int cell = (Vector3Int)(pattern.cells[i]-center);
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }    
    }    
    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }
    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }
    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);//vi goi qua nhieu lan new trong while se tao ra rac
        while(enabled)
        {
            UpdateState();
            yield return interval;
            //yield return new WaitForSeconds(updateInterval);
        }    
    }    
    private void UpdateState()
    {
        cellToCheck.Clear();

        foreach(Vector3Int cell in aliveCells)
        {
            for(int x =-1;x<=1;x++)
            {
                for(int y =-1;y<=1;y++)
                {
                    cellToCheck.Add(cell +new Vector3Int(x,y,0));
                }    
            }    
        }    

        foreach(Vector3Int cell in cellToCheck)
        {
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);
            if(!alive && neighbors ==3)//SONG LAI
            {
                //become alive
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            }
            else if(alive && (neighbors <2 || neighbors > 3))//CHET DI
            {
                //become dead
                nextState.SetTile(cell, deadTile);
                aliveCells.Remove(cell);
            }    
            else//no change
            {
                //stays the same
                nextState.SetTile(cell, currentState.GetTile(cell));
            }    
        }    

        Tilemap tempState = currentState;
        currentState = nextState;
        nextState = tempState;

        nextState.ClearAllTiles();
    }    

    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell +new Vector3Int(x,y,0);
                if (x == 0 && y == 0) continue;
                if (IsAlive(neighbor)) count++;
            }
        }
        return count;
    }    
    private bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }    
}
