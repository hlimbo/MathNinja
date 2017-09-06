using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryScript : MonoBehaviour {

    public GameObject[] walls;
	void Start ()
    {
        walls = new GameObject[2];
        for(int i = 0;i < 2;++i)
        {
            walls[i] = new GameObject("Wall" + i.ToString());
            //walls[i].transform.parent = transform;
            walls[i].AddComponent<BoxCollider2D>();
            BoxCollider2D wallBox = walls[i].GetComponent<BoxCollider2D>();
            wallBox.size = new Vector2(1.0f, Camera.main.orthographicSize * 2);
        }

        float camHalfWidth = (Camera.main.aspect * 2 * Camera.main.orthographicSize) / 2;
        //left wall
        walls[0].transform.position = new Vector2(Camera.main.transform.position.x - camHalfWidth - 0.5f, Camera.main.transform.position.y);
        //right wall
        walls[1].transform.position = new Vector2(Camera.main.transform.position.x + camHalfWidth + 0.5f,Camera.main.transform.position.y);

        walls[0].transform.SetParent(transform);
        walls[1].transform.SetParent(transform);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
