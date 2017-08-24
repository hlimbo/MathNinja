using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScroller : MonoBehaviour {

    private Camera mainCam;
    private float camHalfWidth;
    private float leftCamEdge;
    private float rightCamEdge;
    public GameObject[] floors;

    public float moveSpeed;
    public int direction;
    [SerializeField]
    private Vector2 moveVelocity;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    void Start ()
    {
        camHalfWidth = (mainCam.aspect * mainCam.orthographicSize * 2) / 2;
        leftCamEdge = mainCam.transform.position.x - camHalfWidth;
        rightCamEdge = mainCam.transform.position.x + camHalfWidth;
	}

	void Update (){

        moveVelocity = new Vector2(direction * moveSpeed, 0.0f) * Time.deltaTime;
        foreach (GameObject floor in floors)
        {
            Rigidbody2D rb = floor.GetComponent<Rigidbody2D>();
            rb.velocity = moveVelocity;

            BoxCollider2D box = floor.GetComponent<BoxCollider2D>();
            if(box.bounds.center.x + box.bounds.extents.x < leftCamEdge)
            {
                rb.position = new Vector2(rightCamEdge, rb.position.y);
            }

        }

	}
}
