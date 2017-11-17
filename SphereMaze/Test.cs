using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    int i;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        i++;
        if (i % 500 == 0)
            gameObject.GetComponent<LineRenderer>().SetPosition(i / 500, gameObject.GetComponent<MeshFilter>().mesh.vertices[i / 500]);
    }
}
