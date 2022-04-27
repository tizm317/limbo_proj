using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
class Board
{
    public TileType[,] Tile {get; private set;}//배열
    public int Size_X {get; private set;}
    public int Size_Z {get; private set;}
    public int DestY{get; private set;}
    public int DestX{get; private set;}

    public enum TileType
    {
        Empty,
        Wall,
    }

    public void Initialize(int size_x, int size_z)
    {
        if(Size_X == 0 || Size_Z == 0)
            return;

        Tile = new TileType[size_x,size_z];
        Size_X = size_x;
        Size_Z = size_z;
        
        for(int y = 0; y < Size_Z; y++)
        {
            for(int x = 0; x < Size_X; x++)
            {
                if(x == 0 || x == Size_X - 1 || y == 0 || y == Size_Z -1)
                    Tile[y, x] = TileType.Wall;
                else
                    Tile[y, x] = TileType.Empty;
            }
        }
    }

    public void SetDestination(Vector3 dest)
    {
        DestX = (int)dest.x;
        DestY = (int)dest.z;
    }
}
