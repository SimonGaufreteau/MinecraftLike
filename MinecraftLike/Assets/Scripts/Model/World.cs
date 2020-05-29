
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Model class representing a Minecraft world
/// </summary>
public class World
{
    private Chunk[,] chunks;
    private int worldSize;
    public readonly static int defaultMaxSize = 512;

    private int chunkSize;
    public readonly static int defaultChunkSize = 16;

    private int chunkHeight;
    public readonly static int defaultChunkHeight = 60;
    
    private int middle;

    public World() {
        chunks = new Chunk[defaultMaxSize, defaultMaxSize];
        chunkSize = defaultChunkSize;
        chunkHeight = defaultChunkHeight;
        worldSize = defaultMaxSize;
        middle = worldSize / 2;
    }

    public World(int chunkSize,int chunkHeight)
    {
        if (chunkSize <= 0)
            chunkSize = defaultChunkSize;
        if (chunkHeight <= 0)
            chunkHeight = defaultChunkHeight;
        
        worldSize = defaultMaxSize;
        chunks = new Chunk[worldSize, worldSize];
        this.chunkSize = chunkSize;
        this.chunkHeight = chunkHeight;
        middle = worldSize / 2;
    }

    public Chunk GetFromMiddle(int i, int j)
    {
        return GetChunk(middle + i, middle + j);
    }

    public Chunk GenerateOrGetFromMiddle(int i, int j)
    {
        return GenerateOrGet(middle + i, middle + j);
    }

    /// <summary>
    /// Returns the chunk at (i,j) pos. If no chunk exist at this position, generates a new one
    /// </summary>
    /// <param name="i">X position</param>
    /// <param name="j">Z position</param>
    /// <returns></returns>
    public Chunk GenerateOrGet(int i,int j)
    {
        if (chunks[i, j] == null)
            GenerateChunk(i, j);
        return GetChunk(i, j);
    }

    public Chunk GetChunk(int i, int j)
    {
        return chunks[i, j];
    }

    /// <summary>
    /// Returns a reference to the generated chunks at i/j coordinates in the world
    /// </summary>
    public Chunk GenerateChunk(int i,int j)
    {
        chunks[i, j] = new Chunk(chunkSize, chunkHeight,new Vector2(i,j));
        return GetChunk(i, j);
    }

    /*
    public void GenerateTerrain(int basex,int finalx)
    {
        //By default, a Chunk is composed of 16*16*60 blocks 
        if (globalChunkSize <= 0)
            globalChunkSize = World.defaultChunkSize;

        if (globalChunkHeight <= 0)
            globalChunkHeight = World.defaultChunkHeight;

        //Loading the chunks in a square of X*X size
        for (int i = 0; i < numberOfChunks; i++)
        {
            for (int j = 0; j < numberOfChunks; j++)
            {
                //Loading a chunk at the given position. (the offset+startPoint represents the current position)
                int spawnPoint = world.GetMiddle();
                world.GenerateChunk(spawnPoint + i, spawnPoint + j);
            }
        }
    }*/

    public int GetMiddle()
    {
        return middle;
    }
}