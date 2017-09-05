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

    private NinjaController ninja;

    private void Awake()
    {
        platforms = new List<GameObject>();
        mainCam = Camera.main;
    }

    // Use this for initialization
    void Start () {

        Debug.Assert(platformPrefab != null);

        ninja = FindObjectOfType<NinjaController>();

        BoxCollider2D platformCollider = platformPrefab.GetComponent<BoxCollider2D>();
        float halfPlatformHeight = platformCollider.size.y / 2;
        float xPadding = platformCollider.size.x * xSpread + platformCollider.size.x;

        float xPlatformOffset = platformCollider.size.x / 2;
        for(int i = 0;i < spawnCount;++i)
        {
            //the issue with this code is that the top edge and bottom edge locations of the camera are not
            //assigned to their proper locations. That is, it only takes the minHeight and maxHeight of the
            //camera and centers those y-positions around this gameobject's position. I want the top edge and bottom
            //edge y positions of the camera to remain  relative to this game-object's coordinates without changing the 
            //camera's top and bottom edge y positions in world space
            float randomHeight;
            float minHeight = (-mainCam.orthographicSize * ySpread) + halfPlatformHeight;
            float maxHeight = (mainCam.orthographicSize * ySpread) - halfPlatformHeight;
            if (i == 0)
            {
                //generate the first platform's height between the camera's y boundaries
                randomHeight = Random.Range(minHeight, maxHeight);
            }
            else
            {
                //generate all other platform heights based on the previous platform's position and and player's jump height
                GameObject prevPlatform = platforms[i - 1];
                float minHeightMod = prevPlatform.transform.localPosition.y - ninja.jumpHeight * 2;
                float maxHeightMod = prevPlatform.transform.localPosition.y + ninja.jumpHeight * 2;
                if (minHeightMod < minHeight)
                    minHeightMod = minHeight;
                if (maxHeightMod > maxHeight)
                    maxHeightMod = maxHeight;

                randomHeight = Random.Range(minHeightMod, maxHeightMod);
            }

            //set the location of each platform relative to this gameobject's transform position.
            GameObject platform = Instantiate<GameObject>(platformPrefab, transform);
            platform.transform.localPosition = new Vector3(i * xPadding + xPlatformOffset, randomHeight, 0.0f);
            platforms.Add(platform);
        }

        DisplayWorldandLocalCoords();

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
                float halfPlatformHeight = platformPrefab.GetComponent<BoxCollider2D>().size.y / 2;
                float minHeight = (-mainCam.orthographicSize * ySpread) + halfPlatformHeight;
                float maxHeight = (mainCam.orthographicSize * ySpread) - halfPlatformHeight;
                GameObject lastPlatform = platforms[platforms.Count - 1];

                platforms.Remove(platform);

                //make the height of the next platform to appear to have a random height that can be reached by the player when jumping and or falling
                float minHeightMod = lastPlatform.transform.localPosition.y - ninja.jumpHeight * 2;
                float maxHeightMod = lastPlatform.transform.localPosition.y + ninja.jumpHeight * 2;
                
                if (minHeightMod < minHeight)
                    minHeightMod = minHeight;
                if (maxHeightMod > maxHeight)
                    maxHeightMod = maxHeight;

                float randomHeight = Random.Range(minHeightMod, maxHeightMod);
                float xPadding = platformBox.size.x * xSpread + platformBox.size.x;
                //the new position of the platform that is offscreen will the rightmost platform's x position + some padding.
                platform.transform.localPosition = new Vector2(lastPlatform.transform.localPosition.x + xPadding, randomHeight);
                platforms.Add(platform);
            }
        }
		
	}

    //debug function
    private void DisplayWorldandLocalCoords()
    {
        Debug.Log("World Coordinates::");
        foreach(GameObject platform in platforms)
        {
            Debug.Log(platform.name + ": " + platform.transform.position);
        }

        Debug.Log("LocalCoordinates::");
        foreach(GameObject platform in platforms)
        {
            Debug.Log(platform.name + ": " + platform.transform.localPosition);
        }
    }
}
