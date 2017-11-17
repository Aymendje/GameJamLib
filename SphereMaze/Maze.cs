using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
    private int height = 20;
    private int widght = 40;
    private int nTriangles;
    private Triangle[] cells;
    private Wall[] edges;
    private DisjointSet ds;
    private string nameO = "ligne";

    public class Wall
    {
        public Vector3 start;
        public Vector3 end;
        public int cell1;
        public int cell2;
        public bool isLocked;

        public Wall(Vector3 st, Vector3 ed)
        {
            start = st;
            end = ed;
            isLocked = true;
        }
    }

    private class Triangle
    {
        public int id;
        public Dictionary<int,bool> paths;
        public bool isVisited;
        public enum Bones
        {
            DIAGO = 0,
            HORIZ,
            VERTI
        };

        public Triangle(int identifiant)
        {
            id = identifiant;
            //isLinked = new bool[3];
            isVisited = false;
            paths = new Dictionary<int,bool>();
        }
    }
    
    public class DisjointSet
    {
        private int[] s;

        public DisjointSet(int size)
        {
            s = new int[size];
            for (int i = 0; i < size;  i++)
                s[i] = -1;
        }

        public int find(int a)
        {
            if (s[a] == -1)
                return a;
            else
            {
                s[a] = find(s[a]);
                return s[a];
            }
        }

        public void union(int a, int b)
        {
            int roota = find(a);
            int rootb = find(b);
            if (roota == rootb)
                return;

            if (rootb < roota)
                s[roota] = rootb;
            else
                s[rootb] = roota;
        }
    }

    private Wall createWall(int row, int colomn, bool isFirst, Triangle.Bones bone)
    {
        Vector3 start = new Vector3(),
                end = new Vector3();

        switch (bone)
        {
            case Triangle.Bones.DIAGO:
                start = new Vector3(colomn, -row, 0);
                end = new Vector3(start.x + 1, start.y - 1, 0);
                break;

            case Triangle.Bones.HORIZ:
                if (isFirst)
                    start = new Vector3(colomn, -row - 1, 0);
                else
                    start = new Vector3(colomn, -row, 0);

                end = new Vector3(start.x + 1, start.y, 0);
                break;

            case Triangle.Bones.VERTI:
                if (isFirst)
                    start = new Vector3(colomn, -row, 0);
                else
                    start = new Vector3(colomn + 1, -row, 0);

                end = new Vector3(start.x, start.y - 1, 0);
                break;
        }

        return new Wall(start, end);
    }

    private Triangle createTriangle(int id, int row, int colomn, int i)
    {
        bool isFirst = true;
        int wallIndex = row * widght * 3 / 2 + colomn * 3;
        Triangle currCell = new Triangle(id);

        if (i == 2)
            isFirst = false;
        
        if (isFirst)
        {
            Wall diagoE = createWall(row, colomn, isFirst, Triangle.Bones.DIAGO);
            diagoE.cell1 = id;
            diagoE.cell2 = id + 1;
            currCell.paths[id + 1] = false;
            
            edges[wallIndex++] = diagoE;
            
            if (id < nTriangles - widght)
            {
                Wall horizE = createWall(row, colomn, isFirst, Triangle.Bones.HORIZ);
                horizE.cell1 = id;
                horizE.cell2 = id + widght;

                currCell.paths[id + widght] = false;
                edges[wallIndex] = horizE;
            }

            if (id % widght != 1)
                currCell.paths[id - 1] = false;
        }
        else
        {
            if (id % widght != 0)
            {
                Wall vertiE = createWall(row, colomn, isFirst, Triangle.Bones.VERTI);
                vertiE.cell1 = id;
                vertiE.cell2 = id + 1;

                if (id > widght)
                    currCell.paths[id - widght] = false;

                currCell.paths[id - 1] = false;
                currCell.paths[id + 1] = false;
                edges[wallIndex + 2] = vertiE;
            }
        }
        return currCell;
    }

    private void associateTriangles()
    {
        cells = new Triangle[nTriangles + 1];
        int cellIndex;
        for (int row = 0; row < height; row++)
            for (int colomn = 0; colomn < widght / 2; colomn++)
                for (int i = 1; i < 3; i++)
                {
                    cellIndex = widght * row + 2 * colomn + i;
                    cells[cellIndex] = createTriangle(cellIndex, row, colomn, i);
                }
    }

    private void createLine(List<Wall> wall, string name)
    {
        GameObject edge = new GameObject(name);
        LineRenderer line = edge.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(Color.black, Color.black);
        line.SetWidth(0.1F, 0.1F);
        line.SetVertexCount(wall.Count * 2);

        int j;
        for (int i = 0; i < (wall.Count * 2); i++)
        {
            line.SetPosition(i, wall[i / 2].start);
            j = i + 1;
            line.SetPosition(j, wall[i / 2].end);
            i = j;
        }

        //Create Prefab with LineRenderer component
        //AssetDatabase.CreateAsset(lineRenderer, "Asset/lineRenderer.prefab");
        //PrefabUtility.CreatePrefab("Assets/" + name + ".prefab", edge, ReplacePrefabOptions.Default);
    }

    private void generateMaze()
    {
        ds = new DisjointSet(nTriangles + 1);
        
        int random;
        /*for (int i = 0; i < edges.Length; i++)
        {
            random = (int)UnityEngine.Random.value * edges.Length;
            Wall temp = edges[random];
            edges[random] = edges[i];
            edges[i] = temp;
        }*/

        for (int i = 0; i < edges.Length; i++)
        {
            do
                random = (int)(UnityEngine.Random.value * edges.Length);
            while ((random + 1) % (widght * 3 / 2) == 0 || random > (nTriangles - widght) * 3 / 2 && random % (widght * 3 / 2) % 3 == 1);
            int cell1 = edges[random].cell1;
            int cell2 = edges[random].cell2;
            if (ds.find(cell1) != ds.find(cell2))
            {
                ds.union(cell1, cell2);
                cells[cell1].paths[cell2] = true;
                cells[cell2].paths[cell1] = true;
                edges[random].isLocked = false;
            }
        }
        
        //int lastAdded;
        List<Wall> line = new List<Wall>();

        for (int i = 0; i < widght * 3 / 2; i += 3)
        {
            drawLine(line, Triangle.Bones.DIAGO, i);
            line.Clear();
        }

        for (int i = 0; i < edges.Length ; i += widght * 3 / 2)
        {
            drawLine(line, Triangle.Bones.DIAGO, i);
            line.Clear();
        }

        /*for (int i = 1; i < widght; i += widght * 3 / 2)
        {
            drawLine(line, Triangle.Bones.HORIZ, i);
            line.Clear();
        }*/

        for (int i = 2; (i + 1) % (widght * 3 / 2) != 0 ; i += 3)
        {
            drawLine(line, Triangle.Bones.VERTI, i);
            line.Clear();
        }
    }

    private void drawLine(List<Wall> line, Triangle.Bones b, int i)
    {
        switch (b)
        {
            case Triangle.Bones.DIAGO:
                for (; i < widght * height * 3 / 2 - 3 ; i += widght * 3 + 3)
                {
                    if (edges[i].isLocked)
                        line.Add(edges[i]);
                    else
                    {
                        if (line.Count != 0)
                        {
                            createLine(line, nameO + i.ToString());
                            line.Clear();
                        }
                    }
                }
                break;
            case Triangle.Bones.HORIZ:
                for (; (i + 2) % (widght * 3 / 2) != 0 && i < widght * (height - 1) * 3 / 2; i += 3)
                {
                    if (edges[i].isLocked)
                        line.Add(edges[i]);
                    else
                    {
                        if (line.Count != 0)
                        {
                            createLine(line, nameO + i.ToString());
                            line.Clear();
                        }
                    }
                }
                break;
            case Triangle.Bones.VERTI:
                for (; i < edges.Length; i += widght * 3 / 2)
                {
                    if (edges[i].isLocked)
                        line.Add(edges[i]);
                    else
                    {
                        if (line.Count != 0)
                        {
                            createLine(line, nameO + i.ToString());
                            line.Clear();
                        }
                    }
                }
                break;
        }
    }
	// Use this for initialization
	void Start () 
    {
        //gameObject.GetComponent<LineRenderer>().enabled = false;
        nTriangles = height * widght;
        edges = new Wall[nTriangles * 3 / 2];
        associateTriangles();
        generateMaze();
    }
	
	// Update is called once per frame
    void Update()
    {
    }
}
