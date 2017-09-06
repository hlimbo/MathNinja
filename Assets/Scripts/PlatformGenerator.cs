using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

    public GameObject platformPrefab;
    public int spawnCount;

    [Tooltip("How far apart each platform spawned should be in the x-axis.This takes in the platformPrefab's width and adds a percentage to it")]
    [Range(0.0f, 1.0f)]
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
    private float halfPlatformHeight;
    private float xPadding;//how far apart each platform should be along the x-axis

    private float localTopCamEdge;
    private float localBotCamEdge;

    //debugging
    [SerializeField]
    private float modCamHalfHeight;
    [SerializeField]
    private float modCamHeight;

    private void Awake()
    {
        platforms = new List<GameObject>();
        mainCam = Camera.main;
    }

    void Start () {
        Debug.Assert(platformPrefab != null);
        Debug.Assert(spawnCount > 0);
        ninja = FindObjectOfType<NinjaController>();

        BoxCollider2D platformCollider = platformPrefab.GetComponent<BoxCollider2D>();
        halfPlatformHeight = platformCollider.size.y / 2;
        xPadding = (xSpread + 1) * platformCollider.size.x;

        float platformHalfWidth = platformCollider.size.x / 2;

        //this is in world coordinates
        float topCamEdge = (mainCam.transform.position.y - mainCam.orthographicSize) + (mainCam.orthographicSize * 2 * ySpread);
        float botCamEdge = (mainCam.transform.position.y - mainCam.orthographicSize) + halfPlatformHeight;

        //the top edge of the camera's y location is relative to this game object's y position
        localTopCamEdge = topCamEdge - transform.position.y;
        localBotCamEdge = botCamEdge - transform.position.y;

        //assign the first platform's height to be between the camera's bottom edge and top edge
        //where the y positions of the camera's top and bottom edges are relative to this game object's y position
        //since all platform prefab objects will be parented to this game object
        float randomHeight = Random.Range(localBotCamEdge, localTopCamEdge);

        GameObject platform = Instantiate<GameObject>(platformPrefab, transform, false);
        platform.transform.localPosition = new Vector3(platformHalfWidth, randomHeight, 0.0f);
        platforms.Add(platform);

        //generate all other platform heights based on the previous platform's height and the player's jump height
        for(int i = 1;i < spawnCount;++i)
        {
            GameObject prevPlatform = platforms[i - 1];
            float minHeightMod = prevPlatform.transform.localPosition.y - ninja.jumpHeight;
            float maxHeightMod = prevPlatform.transform.localPosition.y + ninja.jumpHeight;
            if (minHeightMod < localBotCamEdge)
                minHeightMod = localBotCamEdge;
            if (maxHeightMod > localTopCamEdge)
                maxHeightMod = localTopCamEdge;

            randomHeight = Random.Range(minHeightMod, maxHeightMod);

            GameObject platformClone = Instantiate<GameObject>(platformPrefab, transform, false);
            platformClone.transform.localPosition = new Vector3(i * xPadding + platformHalfWidth, randomHeight, 0.0f);
            platforms.Add(platformClone);
        }

        Debug.Log(mainCam.pixelRect.ToString());

    }

    void Update () {
        
        //debugging
        //modCamHalfHeight = mainCam.orthographicSize * ySpread;
        //modCamHeight = modCamHalfHeight * 2;
        //Vector3 screenCoords = mainCam.WorldToScreenPoint(new Vector3((mainCam.orthographicSize * mainCam.aspect * 2) / 2, mainCam.orthographicSize * 2, mainCam.nearClipPlane));
        //Debug.Log(screenCoords);

        //move each platform
        Vector2 moveVelocity = new Vector2(moveSpeed * direction * Time.deltaTime, 0.0f);
        foreach(GameObject platform in platforms)
        {
            platform.GetComponent<Rigidbody2D>().velocity = moveVelocity;
        }

        //set the location of the platform next to the rightmost platform
        //when the platform leaves camera view
        for (int i = 0; i < platforms.Count; ++i)
        {
            GameObject platform = platforms[i];
            BoxCollider2D platformBox = platform.GetComponent<BoxCollider2D>();
            float camHalfWidth = (mainCam.orthographicSize * 2 * mainCam.aspect) / 2;
            float leftCamEdge = mainCam.transform.position.x - camHalfWidth;
            if (platformBox.bounds.center.x + platformBox.bounds.extents.x < leftCamEdge)
            {
                GameObject lastPlatform = platforms[platforms.Count - 1];

                platforms.Remove(platform);

                float minHeightMod = lastPlatform.transform.localPosition.y - ninja.jumpHeight;
                float maxHeightMod = lastPlatform.transform.localPosition.y + ninja.jumpHeight;
                if (minHeightMod < localBotCamEdge)
                    minHeightMod = localBotCamEdge;
                if (maxHeightMod > localTopCamEdge)
                    maxHeightMod = localTopCamEdge;

                float randomHeight = Random.Range(minHeightMod, maxHeightMod);
                platform.transform.localPosition = new Vector2(lastPlatform.transform.localPosition.x + xPadding, randomHeight);

                platforms.Add(platform);
            }
        }
    }
}
