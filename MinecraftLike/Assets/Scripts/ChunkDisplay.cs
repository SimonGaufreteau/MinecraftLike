using UnityEngine;

public class ChunkDisplay : MonoBehaviour
{
    //Game objects cloned in the game
    private static GameObject blockPrefabs;
    private static GameObject planePrefabs;
    private static Material borderMaterial;

    [SerializeField] private int chunkSize=4;
    [SerializeField] private int chunkHeight=1;
    [SerializeField] private int viewDistance=4;
    [SerializeField] private int blockSize=1;


    //Starting point on chunk loading
    public Vector2 startPoint;

    private World world;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting the chunkDisplay");
        //Loading the prefabs 
        blockPrefabs = (GameObject)Resources.Load("Prefabs/Block");
        borderMaterial = (Material)Resources.Load("Materials/RedMaterial");
        planePrefabs = (GameObject)Resources.Load("Prefabs/Plane");
        world = new World(chunkSize, chunkHeight);

        for (int i = 0; i < viewDistance; i++)
        {
            for (int j = 0; j < viewDistance; j++)
            {
                Chunk chunk = world.GenerateOrGetFromMiddle(i, j);
                LoadChunk(chunk, i, j);
            }
        }
    }

    private void LoadChunk(Chunk chunk, int i, int j)
    {
        Debug.Log("Loading the chunk : " + i + " / " + j);
        Vector3 offset = new Vector3(i * chunkSize * blockSize, 0, j * chunkSize * blockSize);
        int size = chunk.GetSize();
        int height = chunk.GetHeight();
        for (int x = 0; x < size; x++)
        {
            //Debug.Log("size :" + size + " / height :" + height);
            for(int y = 0; y < height; y++)
            {
                for(int z = 0; z < size; z++)
                {
                    Block block = chunk.GetBlock(x, y, z);
                    //Debug.Log("Drawing the block : " + x + " / " + y + " / " + z);
                    Block[,,] nearBlocks = chunk.GetNearBlocks(x, y, z);
                    GameObject gameBlock = DrawBlock(block, nearBlocks,new Vector3(x,y,z)+offset);
                    if(x%(size-1)==0 || z % (size - 1) == 0)
                        gameBlock.GetComponent<Renderer>().material = borderMaterial;

                }
            }
        }
    }

    private GameObject DrawBlock(Block block, Block[,,] nearBlocks,Vector3 position)
    {
        return LoadBlock(position);
    }


    //A variation of LoadBlock(Vector3) with coordinates
    private GameObject LoadBlock(float x, float y, float z)
    {
        return LoadBlock(new Vector3(x, y, z));
    }

    //Loads a block defined by the blockPrefabs GameObject at the specified position
    private GameObject LoadBlock(Vector3 position)
    {
        return (GameObject)Instantiate(blockPrefabs, position, Quaternion.identity);
    }
}
