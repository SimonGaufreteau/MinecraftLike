using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Collections;
using System;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

using System.Numerics;

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

    //Last chunk position
    private Vector2 lastChunkPos;

    //World used to display chunk
    private World world;

    //Transform used to represent the player
    [SerializeField] private Transform player;

    //Priority queues used to load and destroy chunks
    SimplePriorityQueue<Chunk> chunkPriority;
    SimplePriorityQueue<Chunk> chunkToDestroy;

    //List containing the chunks loaded in the game
    List<Chunk> loadedChunks;

    //Dictionnary containing the meshes loaded in the game
    Dictionary<Chunk, GameObject> loadedMeshes;

    // Init : world + materials
    void Start()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        chunkCount = 0;
        chunkPriority = new SimplePriorityQueue<Chunk>();
        loadedChunks = new List<Chunk>();
        loadedMeshes = new Dictionary<Chunk, GameObject>();
        chunkToDestroy = new SimplePriorityQueue<Chunk>();
        int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
        int chunkZ = Mathf.FloorToInt(player.position.z / chunkSize);
        lastChunkPos = new Vector2(chunkX, chunkZ);

        Debug.Log("Starting the chunkDisplay");

        //Loading the prefabs 
        borderMaterial = (Material)Resources.Load("Materials/RedMaterial");
        defaultMaterial = (Material)Resources.Load("Materials/SquareMaterial");

        world = new World(chunkSize, chunkHeight);

        for (int i = -viewDistance + (int)(player.position.x / chunkSize); i < viewDistance + (int)(player.position.x / chunkSize); i++)
            for (int j = -viewDistance + (int)(player.position.z / chunkSize); j < viewDistance + (int)(player.position.z / chunkSize); j++)
            {
                Chunk chunk = world.GenerateOrGetFromMiddle(i, j);
                GameObject chunkMesh = LoadChunk(chunk, i, j);
                loadedChunks.Add(chunk);
                loadedMeshes.Add(chunk, chunkMesh);
                chunkCount++;
            }
        stopwatch.Stop();
        Debug.Log("Build finished in : "+stopwatch.ElapsedMilliseconds+"ms");
    }


    private void Update()
    {
        int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
        int chunkZ = Mathf.FloorToInt(player.position.z / chunkSize);

        //Updating the chunks to be updated in the player's view
        //--> Adding the elements in a priority queue which will be updated with a coroutine
        if (chunkX!=lastChunkPos.x || chunkZ!=lastChunkPos.y)
        {
            //Removing the chunks out of player's view
            foreach (Chunk chunk in loadedChunks)
            {
                Vector2 chunkPos = chunk.GetPosFromMiddle();
                /*if (player.position.x / 16 != lastChunkPos.x || player.position.z / 16 != lastChunkPos.y)
                {
                    Debug.Log(Math.Abs(chunkPos.x - player.position.x / 16));
                    Debug.Log(Math.Abs(chunkPos.y - player.position.z / 16));
                }*/


                if (Math.Abs(chunkPos.x - player.position.x / chunkSize) > viewDistance+2 || Math.Abs(chunkPos.y - player.position.z / chunkSize) > viewDistance+2)
                    chunkToDestroy.Enqueue(chunk, chunkToDestroy.Count);
            }

            int minx = -viewDistance + (int)(player.position.x / chunkSize);
            int maxx = viewDistance + (int)(player.position.x / chunkSize);
            int minz = -viewDistance + (int)(player.position.z / chunkSize);
            int maxz = viewDistance + (int)(player.position.z / chunkSize);

            for (int i = minx; i < maxx; i++)
                for (int j = minz; j < maxz; j++)
                {
                    Chunk toLoad = world.GenerateOrGetFromMiddle(i, j);
                    if (chunkToDestroy.Contains(toLoad))
                    {
                        chunkToDestroy.Remove(toLoad);
                        //Debug.Log("ToBeDestroyed removed chunk : " + toLoad.GetPosFromMiddle().x + " / " + toLoad.GetPosFromMiddle().y);
                    }

                    if (!loadedChunks.Contains(toLoad) && !loadedMeshes.ContainsKey(toLoad))
                    {
                        chunkPriority.Enqueue(toLoad, chunkPriority.Count);
                        

                    }
                }
            lastChunkPos.x = chunkX;
            lastChunkPos.y = chunkZ;


            //Debug.Log("To be destroyed : " + chunkToDestroy.Count);
            //Debug.Log("To be loaded : " + chunkPriority.Count);

            while (chunkToDestroy.Count > 0)
            {
                Chunk chunk = chunkToDestroy.Dequeue();
                loadedChunks.Remove(chunk);
                if (loadedMeshes.ContainsKey(chunk))
                {
                    GameObject mesh = loadedMeshes[chunk];
                    Destroy(mesh);
                }
                loadedMeshes.Remove(chunk);
                //Debug.Log("Removed chunk : " + chunk.GetPosFromMiddle().x + " / " + chunk.GetPosFromMiddle().y);
            }
            StartCoroutine(LoadAwaitingChunks());
        }
    }

   
    private IEnumerator LoadAwaitingChunks()
    {
        
        while (chunkPriority.Count > 0)
        {
            Chunk chunk = chunkPriority.Dequeue();
            if (loadedChunks.Contains(chunk))
                continue;
            Vector2 chunkPos = chunk.GetPosFromMiddle();
            GameObject chunkMesh = LoadChunk(chunk, (int)chunkPos.x, (int)chunkPos.y);
            loadedChunks.Add(chunk);
            if(!loadedMeshes.ContainsKey(chunk))
                loadedMeshes.Add(chunk, chunkMesh);
            int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
            int chunkZ = Mathf.FloorToInt(player.position.z / chunkSize);

            //Debug.Log("Player : " + chunkX + "/" + chunkZ + " ... Chunk :" + chunk.GetPosFromMiddle().x + "/" + chunk.GetPosFromMiddle().y);

            double distance = Math.Sqrt(Math.Pow(chunk.GetPosFromMiddle().x - chunkX, 2) + Math.Pow(chunk.GetPosFromMiddle().y - chunkZ, 2));
            //Debug.Log((float)(1 / distance)/2);

            yield return new WaitForSeconds(0.1f);
        }
        
    }

    //Returns the gameObject representing the chunk's mesh
    private GameObject LoadChunk(Chunk chunk, int i, int j)
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

        return gameObject;
    }



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
