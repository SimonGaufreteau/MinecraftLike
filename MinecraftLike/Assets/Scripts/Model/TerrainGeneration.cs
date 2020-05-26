using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Model class used to generate a Minecraft World and keep generating it over time
/// </summary>
public class TerrainGeneration
{
    //Sizes of a chunk
    private int globalChunkSize;
    private int globalChunkHeight;

    //Number of chunks loaded. (will be squared later)
    public int numberOfChunks;

    //Array keeping all the chunks in memory
    private World world;

    public TerrainGeneration(int chunkSize,int chunkHeight,int numberOfChunks)
    {
        world = new World(chunkSize,chunkHeight);
        this.globalChunkSize = chunkSize;
        this.globalChunkHeight = chunkHeight;
        this.numberOfChunks = numberOfChunks;

    }

    public TerrainGeneration(int numberOfChunks)
    {
        world = new World();
        this.globalChunkSize = World.defaultChunkSize;
        this.globalChunkHeight = World.defaultChunkHeight;
        this.numberOfChunks = numberOfChunks;
    }



    public void generateBaseTerrain()
    {
       
    }

}
