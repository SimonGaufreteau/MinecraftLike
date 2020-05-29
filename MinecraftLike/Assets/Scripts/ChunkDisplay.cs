using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;
using System.Collections;

public class ChunkDisplay : MonoBehaviour
{
    enum Faces
    {
        Bottom,
        Back,
        Right,   
        Left,
        Front,
        Top
    }

    //Materials used in the game (pre charged in the build)
    private static Material borderMaterial;
    private static Material defaultMaterial;

    //Parameters used in the terrain generation
    [SerializeField] private int chunkSize=4;
    [SerializeField] private int chunkHeight=1;
    [SerializeField] private int viewDistance=4;
    [SerializeField] private int blockSize=1;

    private int chunkCount;

    //Starting point on chunk loading
    public Vector2 startPoint;

    //World used to display chunk
    private World world;

    //Transform used to represent the player
    [SerializeField] private Transform player;

    //Priority queue used to load chunks
    SimplePriorityQueue<Chunk> chunkPriority;

    // Init : world + materials
    void Start()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        chunkCount = 0;
        chunkPriority = new SimplePriorityQueue<Chunk>();

        Debug.Log("Starting the chunkDisplay");

        //Loading the prefabs 
        borderMaterial = (Material)Resources.Load("Materials/RedMaterial");
        defaultMaterial = (Material)Resources.Load("Materials/SquareMaterial");

        world = new World(chunkSize, chunkHeight);

        for (int i = -viewDistance; i < viewDistance; i++)
        {
            for (int j = -viewDistance; j < viewDistance; j++)
            {
                Chunk chunk = world.GenerateOrGetFromMiddle(i, j);
                LoadChunk(chunk, i, j);
                chunkCount++;
            }
        }
        stopwatch.Stop();
        Debug.Log("Build finished in : "+stopwatch.ElapsedMilliseconds+"ms");
    }


    private void Update()
    {
        //Updating the chunks to be updated in the player's view
        //--> Adding the elements in a priority queue which will be updated with a coroutine
        for(int i=-viewDistance+(int)player.position.x;i< viewDistance + player.position.x;i++)
            for(int j=-viewDistance + (int)player.position.z;j< viewDistance + (int)player.position.z;j++)
            {
                chunkPriority.Enqueue(world.GenerateOrGetFromMiddle(i, j), chunkPriority.Count);
            }

        StartCoroutine(LoadAwaitingChunks());
    }

    private IEnumerator LoadAwaitingChunks()
    {
        while (chunkPriority.Count > 0)
        {
            Chunk chunk = chunkPriority.Dequeue();
            LoadChunk(chunk, (int)chunk.pos.x, (int)chunk.pos.y);
            yield return new WaitForSeconds(0.3f);
        }
         
    }

    private void LoadChunk(Chunk chunk, int i, int j)
    {
        //Debug.Log("Loading the chunk : " + i + " / " + j);
        Vector3 offset = new Vector3(i * chunkSize * blockSize, 0, j * chunkSize * blockSize);
        int size = chunk.GetSize();
        int height = chunk.GetHeight();

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        Block[,,] blocks = chunk.GetBlocks();

        for (int x = 0; x < size; x++)
        {
            //Debug.Log("size :" + size + " / height :" + height);
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Block block = chunk.GetBlock(x, y, z);
                    if (block != null)
                    {
                        Vector3 blockPos = new Vector3(x, y, z)+offset;
                        if (y == height - 1 || y<height-1 && blocks[x, y + 1, z] == null)
                            LoadFace(Faces.Top, blockPos, verts, uvs, triangles);

                        if (y == 0 ||  y>0 && blocks[x, y - 1, z] == null)
                            LoadFace(Faces.Bottom, blockPos, verts, uvs, triangles);

                        if (z == size - 1 || z<size-1 && blocks[x, y, z + 1] == null)
                            LoadFace(Faces.Left, blockPos, verts, uvs, triangles);

                        if (z == 0 || z>0 && blocks[x, y, z - 1] == null)
                            LoadFace(Faces.Right, blockPos, verts, uvs, triangles);                       

                        if (x == size - 1 || x<size-1 && blocks[x + 1, y, z] == null)
                            LoadFace(Faces.Front, blockPos, verts, uvs, triangles);

                        if (x == 0 || x >0 && blocks[x - 1, y, z] == null)
                            LoadFace(Faces.Back, blockPos, verts, uvs, triangles);
                    }
                }
            }
        }

        Mesh mesh = new Mesh
        {
            vertices = verts.ToArray(),
            uv = uvs.ToArray(),
            triangles = triangles.ToArray()
        };

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        gameObject.name = "Chunk n°" + chunkCount;
    }

        /*
    /// <summary>
    /// Generates a Mesh for the chunk
    /// </summary>
    private void DrawBlockInChunk(Block block, Chunk chunk,Vector3 position, List<Vector3> verts, List<Vector2> uvs, List<int> triangles)
    {
        
            if (nearBlocks[1, 0, 1] == null)
            LoadFace(Faces.Bottom,position,verts,uvs,triangles);
        if (nearBlocks[0, 1, 1] == null)
            LoadFace(Faces.Back, position, verts, uvs, triangles);
        if (nearBlocks[1, 1, 0] == null)
            LoadFace(Faces.Right, position, verts, uvs, triangles);
        if (nearBlocks[1, 1, 2] == null)
            LoadFace(Faces.Left, position, verts, uvs, triangles);
        if (nearBlocks[2, 1, 1] == null)
            LoadFace(Faces.Front, position, verts, uvs, triangles);
        if (nearBlocks[1, 2, 1] == null)
            LoadFace(Faces.Top, position, verts, uvs, triangles);
    }*/

    private void LoadFace(Faces face,Vector3 blockPos, List<Vector3> verts, List<Vector2> uvs, List<int> triangles) 
    {
        Vector2 block2D = new Vector2(blockPos.x, blockPos.z);
        int listPos = verts.Count;
        switch (face)
        {
            case Faces.Bottom:
                verts.Add(blockPos + new Vector3(0, 0, 1));
                verts.Add(blockPos + new Vector3(0, 0, 0));
                verts.Add(blockPos + new Vector3(1, 0, 1));
                verts.Add(blockPos + new Vector3(1, 0, 0));

                uvs.Add(block2D + new Vector2(0, 0));
                uvs.Add(block2D + new Vector2(0, 1));
                uvs.Add(block2D + new Vector2(1, 0));
                uvs.Add(block2D + new Vector2(1, 1));

                addTriangles(triangles, listPos);
                break;
            case Faces.Back:
                verts.Add(blockPos + new Vector3(0, 1, 1));
                verts.Add(blockPos + new Vector3(0, 1, 0));
                verts.Add(blockPos + new Vector3(0, 0, 1));
                verts.Add(blockPos + new Vector3(0, 0, 0));

                uvs.Add(block2D + new Vector2(1, 1));
                uvs.Add(block2D + new Vector2(1, 0));
                uvs.Add(block2D + new Vector2(0, 1));
                uvs.Add(block2D + new Vector2(0, 0));

                addTriangles(triangles, listPos);
                break;
            case Faces.Right:
                verts.Add(blockPos + new Vector3(0, 1, 0));
                verts.Add(blockPos + new Vector3(1, 1, 0));
                verts.Add(blockPos + new Vector3(0, 0, 0));
                verts.Add(blockPos + new Vector3(1, 0, 0));

                uvs.Add(block2D + new Vector2(0, 0));
                uvs.Add(block2D + new Vector2(1, 0));
                uvs.Add(block2D + new Vector2(0, 0));
                uvs.Add(block2D + new Vector2(1, 0));

                addTriangles(triangles, listPos);
                break;
            case Faces.Left:
                verts.Add(blockPos + new Vector3(1, 1, 1));
                verts.Add(blockPos + new Vector3(0, 1, 1));
                verts.Add(blockPos + new Vector3(1, 0, 1));
                verts.Add(blockPos + new Vector3(0, 0, 1));

                uvs.Add(block2D + new Vector2(1, 1));
                uvs.Add(block2D + new Vector2(0, 1));
                uvs.Add(block2D + new Vector2(1, 0));
                uvs.Add(block2D + new Vector2(0, 0));

                addTriangles(triangles, listPos);
                break;
            case Faces.Front:
                verts.Add(blockPos + new Vector3(1, 1, 0));
                verts.Add(blockPos + new Vector3(1, 1, 1));
                verts.Add(blockPos + new Vector3(1, 0, 0));
                verts.Add(blockPos + new Vector3(1, 0, 1));

                uvs.Add(block2D + new Vector2(1, 0));
                uvs.Add(block2D + new Vector2(1, 1));
                uvs.Add(block2D + new Vector2(0, 0));
                uvs.Add(block2D + new Vector2(0, 1));

                addTriangles(triangles, listPos);
                break;
            case Faces.Top:
                verts.Add(blockPos + new Vector3(0, 1, 1));
                verts.Add(blockPos + new Vector3(1, 1, 1));
                verts.Add(blockPos + new Vector3(0, 1, 0));
                verts.Add(blockPos + new Vector3(1, 1, 0));

                uvs.Add(block2D + new Vector2(0, 1));
                uvs.Add(block2D + new Vector2(1, 1));
                uvs.Add(block2D + new Vector2(0, 0));
                uvs.Add(block2D + new Vector2(1, 0));

                addTriangles(triangles, listPos);
                break;
        }
    }

    private void addTriangles(List<int> triangles, int listPos)
    {
        int[] triPos = { 0, 1, 2, 2, 1, 3 };
        for (int i = 0; i < 6; i++)
            triangles.Add(listPos + triPos[i]);
    }
}
