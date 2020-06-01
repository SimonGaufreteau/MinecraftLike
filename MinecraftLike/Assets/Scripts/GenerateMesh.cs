using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    public Material material;
    public Vector3 pos;
    public int size;
    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        GenerateBlock(pos, verts, uvs, triangles);         

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
    }

    public void GenerateBlock(Vector3 pos, List<Vector3> verts, List<Vector2> uvs, List<int> triangles)
    {
        GenerateBlock(pos.x, pos.y, pos.z, verts, uvs, triangles);
    }
    public void GenerateBlock(float x, float y, float z, List<Vector3> verts, List<Vector2> uvs, List<int> triangles)
    {
        Vector3 blockPos = new Vector3(x, y, z);
        Vector2 block2D = new Vector2(x, z);


        int listPos = verts.Count();


        //Top 
        verts.Add(blockPos + new Vector3(0, 1, 1));
        verts.Add(blockPos + new Vector3(1, 1, 1));
        verts.Add(blockPos + new Vector3(0, 1, 0));
        verts.Add(blockPos + new Vector3(1, 1, 0));

        uvs.Add(block2D + new Vector2(0, 1));
        uvs.Add(block2D + new Vector2(1, 1));
        uvs.Add(block2D + new Vector2(0, 0));
        uvs.Add(block2D + new Vector2(1, 0));

        addTriangles(triangles,listPos);
        listPos+=4;

        //Right
        verts.Add(blockPos + new Vector3(0, 1, 0));
        verts.Add(blockPos + new Vector3(1, 1, 0));
        verts.Add(blockPos + new Vector3(0, 0, 0));
        verts.Add(blockPos + new Vector3(1, 0, 0));

        uvs.Add(block2D + new Vector2(0, 0));
        uvs.Add(block2D + new Vector2(1, 0));
        uvs.Add(block2D + new Vector2(0, 0));
        uvs.Add(block2D + new Vector2(1, 0));

        addTriangles(triangles, listPos);
        listPos += 4;


        //Front
        verts.Add(blockPos + new Vector3(1, 1, 0));
        verts.Add(blockPos + new Vector3(1, 1, 1));
        verts.Add(blockPos + new Vector3(1, 0, 0));
        verts.Add(blockPos + new Vector3(1, 0, 1));

        uvs.Add(block2D + new Vector2(1, 0));
        uvs.Add(block2D + new Vector2(1, 1));
        uvs.Add(block2D + new Vector2(0, 0));
        uvs.Add(block2D + new Vector2(0, 1));

        addTriangles(triangles, listPos);
        listPos += 4;

        
        //Left 
        verts.Add(blockPos + new Vector3(1, 1, 1));
        verts.Add(blockPos + new Vector3(0, 1, 1));
        verts.Add(blockPos + new Vector3(1, 0, 1));
        verts.Add(blockPos + new Vector3(0, 0, 1));

        uvs.Add(block2D + new Vector2(1, 1));
        uvs.Add(block2D + new Vector2(0, 1));
        uvs.Add(block2D + new Vector2(1, 0));
        uvs.Add(block2D + new Vector2(0, 0));

        addTriangles(triangles, listPos);
        listPos += 4;

        
        //Back
        verts.Add(blockPos + new Vector3(0, 1, 1));
        verts.Add(blockPos + new Vector3(0, 1, 0));
        verts.Add(blockPos + new Vector3(0, 0, 1));
        verts.Add(blockPos + new Vector3(0, 0, 0));

        uvs.Add(block2D + new Vector2(1, 1));
        uvs.Add(block2D + new Vector2(1, 0));
        uvs.Add(block2D + new Vector2(0, 1));
        uvs.Add(block2D + new Vector2(0, 0));

        addTriangles(triangles, listPos);
        listPos += 4;


        //Bottom
        verts.Add(blockPos + new Vector3(0, 0, 1));
        verts.Add(blockPos + new Vector3(0, 0, 0));
        verts.Add(blockPos + new Vector3(1, 0, 1));
        verts.Add(blockPos + new Vector3(1, 0, 0));

        uvs.Add(block2D + new Vector2(0, 0));
        uvs.Add(block2D + new Vector2(0, 1));
        uvs.Add(block2D + new Vector2(1, 0));
        uvs.Add(block2D + new Vector2(1, 1));

        addTriangles(triangles, listPos);

    }

    private void addTriangles(List<int> triangles,int listPos)
    {
        int[] triPos = { 0, 1, 2, 2, 1, 3 };
        for (int i = 0; i < 6; i++)
            triangles.Add(listPos + triPos[i]);
    }

}
