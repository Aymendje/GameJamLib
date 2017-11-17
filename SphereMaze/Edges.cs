using UnityEngine;
using System.Collections;

public class Edges : MonoBehaviour
{
    public static int distLimit = 5, nVertices = 80;
    public Vector3[] plan;
    private enum Building { FIRSTHALF, LASTHALF };
    private enum Position { EDGERIGHT, EDGELEFT, EDGEUP, EDGEDOWN };
    private void createEdges(LineRenderer line)
    {
        plan = new Vector3[nVertices];
        Vector3 coord = new Vector3(distLimit, distLimit, 0);
        float temp;
        Building currentState = Building.FIRSTHALF;
        Position position = Position.EDGEUP;

        line.SetVertexCount(nVertices);
        line.SetPosition(0, coord);
        plan[0] = coord;

        for (int i = 1; i < nVertices; i++)
        {
            switch (currentState)
            {
                case Building.FIRSTHALF:
                    switch (position)
                    {
                        case Position.EDGERIGHT:
                            coord.y--;
                            line.SetPosition(i, coord);
                            plan[i] = coord;
                            i++;
                            if (i < nVertices)
                            {
                                temp = coord.x;
                                coord.x = coord.y;
                                coord.y = temp;
                                line.SetPosition(i, coord);
                                plan[i] = coord;
                                position = Position.EDGEUP;
                            }
                            break;

                        case Position.EDGELEFT:
                            if (coord.y == -distLimit)
                            {
                                currentState = Building.LASTHALF;
                                i--;
                            }
                            else
                            {
                                coord.y--;
                                line.SetPosition(i, coord);
                                plan[i] = coord;
                                i++;
                                if (i < nVertices)
                                {
                                    temp = coord.x;
                                    coord.x = coord.y;
                                    coord.y = temp;
                                    line.SetPosition(i, coord);
                                    plan[i] = coord;

                                    position = Position.EDGEDOWN;
                                }
                            }
                            break;

                        case Position.EDGEUP:
                            if (coord.x == -distLimit)
                            {
                                position = Position.EDGELEFT;
                                i--;
                            }

                            else
                            {
                                coord.x--;
                                line.SetPosition(i, coord);
                                plan[i] = coord;

                                i++;
                                if (i < nVertices)
                                {
                                    temp = coord.x;
                                    coord.x = coord.y;
                                    coord.y = temp;
                                    line.SetPosition(i, coord);
                                    plan[i] = coord;

                                    position = Position.EDGERIGHT;
                                }
                            }
                            break;

                        case Position.EDGEDOWN:
                            coord.x--;
                            line.SetPosition(i, coord);
                            plan[i] = coord;
                            i++;
                            if (i < nVertices)
                            {
                                temp = coord.x;
                                coord.x = coord.y;
                                coord.y = temp;
                                line.SetPosition(i, coord);
                                plan[i] = coord;

                                position = Position.EDGELEFT;
                            }
                            break;
                    }
                    break;

                case Building.LASTHALF:
                    switch (position)
                    {
                        case Position.EDGERIGHT:
                            coord.y++;
                            line.SetPosition(i, coord);
                            plan[i] = coord;
                            i++;
                            if (i < nVertices)
                            {
                                coord.x = -coord.x;
                                line.SetPosition(i, coord);
                                plan[i] = coord;

                                position = Position.EDGELEFT;
                            }
                            break;

                        case Position.EDGELEFT:
                            if (coord.y == distLimit)
                            {
                                position = Position.EDGEUP;
                                i--;
                            }
                            else
                            {
                                coord.y++;
                                line.SetPosition(i, coord);
                                plan[i] = coord;
                                i++;
                                if (i < nVertices)
                                {
                                    coord.x = -coord.x;
                                    line.SetPosition(i, coord);
                                    plan[i] = coord;

                                    position = Position.EDGERIGHT;
                                }
                            }
                            break;

                        case Position.EDGEUP:
                            coord.x++;
                            line.SetPosition(i, coord);
                            plan[i] = coord;
                            i++;
                            if (i < nVertices)
                            {
                                coord.y = -coord.y;
                                line.SetPosition(i, coord);
                                plan[i] = coord;

                                position = Position.EDGEDOWN;
                            }
                            break;

                        case Position.EDGEDOWN:
                            coord.x++;
                            line.SetPosition(i, coord);
                            plan[i] = coord;
                            i++;
                            if (i < nVertices)
                            {
                                coord.y = -coord.y;
                                line.SetPosition(i, coord);
                                plan[i] = coord;

                                position = Position.EDGEUP;
                            }
                            break;
                    }
                    break;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        //LineRenderer lineRenderer = GetComponent<LineRenderer>();

        //createEdges(lineRenderer);
    }

    int i;
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<LineRenderer>().SetVertexCount(++i / 20 * 2);
        if (i % 20 == 0)
        {
            Vector3 vertice = gameObject.GetComponent<MeshFilter>().mesh.vertices[i / 20];
            gameObject.GetComponent<LineRenderer>().SetPosition(i / 20, vertice);
        }
    }
}