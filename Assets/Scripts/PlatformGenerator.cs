using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

    public GameObject platformPrefab;
    public int spawnCount;

    [Tooltip("How far apart each platform spawned should be in the x-axis.This takes in the platformPrefab's width and adds a percentage to it")]
    [Range(0.0f,1.0f)]
    public float xSpread;

    [Tooltip("The percentage of minHeight and maxHeight needs to be  when spawning platforms of random height")]
    [Range(0.0f, 1.0f)]
    public float ySpread;

    public float moveSpeed;
    public int direction;

    [SerializeField]
    private List<GameObject> platforms;

    private Camera mainCam;

    private void Awake()
    {
        platforms = new List<GameObject>();
        mainCam = Camera.main;
    }

    // Use this for initialization
    void Start () {

        Debug.Assert(platformPrefab != null);

        //set this transform's position to the left edge of the camera's bounding box
        //float camHalfWidth = (mainCam.aspect * mainCam.orthographicSize * 2) / 2;
        //float camLeftEdge = mainCam.transform.position.x - camHalfWidth;
        //transform.position = new Vector3(camLeftEdge, mainCam.transform.position.y, 0.0f);

        BoxCollider2D platformCollider = platformPrefab.GetComponent<BoxCollider2D>();
        float halfPlatformHeight = platformCollider.size.y / 2;
        float xPadding = platformCollider.size.x * xSpread + platformCollider.size.x;

        //Note: the random height values calculated are relative to this transform's position
        //todo: this height should be adjusted in a way such that 2 adjacent platforms can be reached while jumping
        //(in other words it shouldn't be impossible to jump to the next platform due to poor rng)
        float minHeight = (-mainCam.orthographicSize * ySpread) + halfPlatformHeight;
        float maxHeight = (mainCam.orthographicSize * ySpread) - halfPlatformHeight;

        float xPlatformOffset = platformCollider.size.x / 2;
        for(int i = 0;i < spawnCount;++i)
        {
            float randomHeight = Random.Range(minHeight, maxHeight);
            GameObject platform = Instantiate<GameObject>(platformPrefab, transform);
            platform.transform.localPosition = new Vector3(i * xPadding + xPlatformOffset, randomHeight, 0.0f);
            platforms.Add(platform);
        }

	}
	
    private float GenerateRandomPlatformHeight(GameObject prefab,Camera cam)
    {
        Debug.Assert(prefab != null);
        BoxCollider2D platformCollider = prefab.GetComponent<BoxCollider2D>();
        Debug.Assert(platformCollider != null);
        float halfPlatformHeight = platformCollider.size.y / 2;
        float minHeight = (-cam.orthographicSize * ySpread) + halfPlatformHeight;
        float maxHeight = (cam.orthographicSize * ySpread) - halfPlatformHeight;


        return Random.Range(minHeight, maxHeight);
    }

	// Update is called once per frame
	void Update () {

        Vector2 moveVelocity = new Vector2(moveSpeed * direction * Time.deltaTime, 0.0f);
        for(int i = 0;i < platforms.Count;++i)
        {
            GameObject platform = platforms[i];
            Rigidbody2D rb = platform.GetComponent<Rigidbody2D>();
            rb.velocity = moveVelocity;
            BoxCollider2D platformBox = platform.GetComponent<BoxCollider2D>();
            float camHalfWidth = (mainCam.orthographicSize * 2 * mainCam.aspect) / 2;
            float leftCamEdge = mainCam.transform.position.x - camHalfWidth;
            if (platformBox.bounds.center.x + platformBox.bounds.extents.x < leftCamEdge)
            {
                platforms.Remove(platform);
                float rightCamEdge = mainCam.transform.position.x + camHalfWidth;
                // float randomHeight = GenerateRandomPlatformHeight(platformPrefab, mainCam);

                //generate random height based on the right most platform's height and the player's jump height
                //GameObject lastPlatform = platforms[platforms.Count - 1];
                //float randomHeight;
                //if (NinjaController.MinJumpHeight == NinjaController.UNDETERMINED_JUMP_HEIGHT)
                //    randomHeight = GenerateRandomPlatformHeight(platformPrefab, mainCam);
                //else
                //    randomHeight = Random.Range(lastPlatform.transform.position.y, NinjaController.MinJumpHeight);

                GameObject lastPlatform = platforms[platforms.Count - 1];
                float randomHeight = GenerateRandomPlatformHeight(platformPrefab, mainCam);

                float xPadding = platformBox.size.x * xSpread + platformBox.size.x;
                //the new position of the platform that is offscreen will the rightmost platform's x position + some padding.
                rb.position = new Vector2(lastPlatform.transform.position.x + xPadding, randomHeight);
                platforms.Add(platform);
            }
        }
		
	}
}
