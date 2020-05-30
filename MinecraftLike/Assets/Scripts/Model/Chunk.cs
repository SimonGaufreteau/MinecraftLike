using System;
using UnityEngine;
/// <summary>
/// Model class representing a Minecraft Chunk
/// </summary>
public class Chunk
{
    private Block[,,] blocks;
    private int size;
    private int height;
    public Vector2 pos;
    public int middlePos;

    //Noise used to generate chunks
    private FastNoise fastNoise;
    
    public Chunk(int size,int height,Vector2 pos,int middlePos)
    {
        this.size = size;
        this.height = height;
        this.pos = pos;
        this.middlePos = middlePos;
        blocks = new Block[size,height,size];

        //Init of the noise used to generate chunks
        fastNoise = new FastNoise();
        fastNoise.SetSeed(123456);
        fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);

        //Init the blocks with the newly formed noise
        initBlocks();

        
    }

    private void initBlocks()
    {
        //Looping over the dimensions and generate a block at each position
        for(int x = 0; x < size; x++)
            for(int z = 0; z < size; z++)
            {
                float noiseHeight = Math.Min(Math.Abs(fastNoise.GetNoise(x+(pos.x*size), z+(pos.y*size)))*height/2,height-1)+1;
                //Debug.Log(noiseHeight);
                for (int y = 0; y < noiseHeight; y++)
                { 
                    blocks[x, y, z] = new Block();
                }
            }
    }



    /*
    public Block[,,] GetNearBlocks(int x,int y,int z)
    { 
        //Debug.Log("x=" + startX + " y=" + startY + " z=" + startZ);
        //Debug.Log("x'=" + endX + " y'=" + endY + " z'=" + endZ);

        Block[,,] nearBlocks = new Block[3,3,3];
        
        for (int i = x-1; i < x+2; i++) {
            if (i < 0)
                continue;
            for (int j = y-1; j < y+2; j++)
            {
                if (j < 0)
                    continue;
                for (int h = z-1; h < z+2; h++)
                {
                    if(h<0)
                        continue;
                    if (i >= size || j >= height || h >= size)
                        continue;
                    Debug.Log("Coords : " + i + "/" + j + "/" + h);
                    Block tempBlock = blocks[i, j, h];
                    int tempX = i - x + 1;
                    int tempY = j - y + 1;
                    int tempZ = h - z + 1;
                    Debug.Log("Temps : "+tempX+"/"+tempY+"/"+tempZ);
                    nearBlocks[i-x+1, j-y+1, h-z+1] = tempBlock;
                }
            }
        }
        return nearBlocks;
    }*/




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

    public Block[,,] GetBlocks()
    {
        return blocks;
    }

    public Vector2 GetPosFromMiddle()
    {
        return new Vector2(pos.x - middlePos, pos.y - middlePos);
    }
    
}
