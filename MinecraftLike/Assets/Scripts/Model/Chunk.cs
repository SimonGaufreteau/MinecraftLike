using JetBrains.Annotations;
using System;
using System.Collections.Specialized;
using UnityEngine;

/// <summary>
/// Model class representing a Minecraft Chunk
/// </summary>
public class Chunk
{
    private Block[,,] blocks;
    private int size;
    private int height;
    
    public Chunk(int size,int height)
    {
        this.size = size;
        this.height = height;
        blocks = new Block[size,height,size];
        initBlocks();
    }

    private void initBlocks()
    {
        //Looping over the dimensions and generate a block at each position
        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < height; y++)
            {
                for(int z = 0; z < size; z++)
                {
                    blocks[x, y, z] = new Block();
                }
            }
        }
    }

    public Block[,,] GetNearBlocks(int x,int y,int z)
    {
        int startX = Math.Max(x - 1,0);
        int startY = Math.Max(y - 1, 0); 
        int startZ = Math.Max(z - 1, 0);
        int endX = Math.Min(x + 1, size);
        int endY = Math.Min(y + 1, height);
        int endZ = Math.Min(z + 1, size);
        //Debug.Log("x=" + startX + " y=" + startY + " z=" + startZ);
        //Debug.Log("x'=" + endX + " y'=" + endY + " z'=" + endZ);

        Block[,,] nearBlocks = new Block[3,3,3];
        
        for (int i = startX; i < endX; i++) { 
        
            for (int j = startY; j < endY; j++)
            {
                for (int h = startZ; h < endZ; h++)
                {
                    //Debug.Log((i - startX) + " / " + (j - startY) + " / " + (h - startZ));
                    Block tempBlock = blocks[i, j, h];
                    nearBlocks[i-startX, j-startY, h-startZ] = tempBlock;
                }
            }
        }
        return nearBlocks;
    }

    public Block GetBlock(Vector3 position)
    {
        return GetBlock((int)position.x, (int)position.y, (int)position.z);
    }

    public Block GetBlock(int x,int y,int z)
    {
        return blocks[x, y, z];
    }

    public int GetSize()
    {
        return size;
    }

    public int GetHeight()
    {
        return height;
    }

    
}
