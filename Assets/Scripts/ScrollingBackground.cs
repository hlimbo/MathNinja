using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed;
    public int scrollDirection;

    [SerializeField]
    private Vector2 scrollVelocity;
    private Camera mainCam;
    private float camHalfWidth;
    private float camHalfHeight;

    private float leftCamEdge;
    private float rightCamEdge;

    public GameObject[] backgrounds;

    void Start ()
    {
        mainCam = Camera.main;
        scrollVelocity = new Vector2(scrollDirection * scrollSpeed, 0.0f) * Time.deltaTime;
        camHalfWidth = (mainCam.aspect * mainCam.orthographicSize * 2) / 2;
        camHalfHeight = mainCam.orthographicSize;
        leftCamEdge = mainCam.transform.position.x - camHalfWidth;
        rightCamEdge = mainCam.transform.position.x + camHalfWidth;
    }
	
	void Update ()
    {
        scrollVelocity = new Vector2(scrollDirection * scrollSpeed, 0.0f) * Time.deltaTime;
        foreach (GameObject background in backgrounds)
        {
            background.transform.Translate((Vector3)scrollVelocity);
            SpriteRenderer backgroundSR = background.GetComponent<SpriteRenderer>();
        }

        //check if 0th background is in front of 1th background.
        if (backgrounds[0].transform.position.x > backgrounds[1].transform.position.x)
        {
            //goal: to set the position of the 1th background to the right edge of the 0th background
            SpriteRenderer bgSR1 = backgrounds[1].GetComponent<SpriteRenderer>();
            if (leftCamEdge >= backgrounds[1].transform.position.x + bgSR1.bounds.extents.x)
            {
                SpriteRenderer bgSR = backgrounds[0].GetComponent<SpriteRenderer>();
                float tempX = backgrounds[0].transform.position.x + bgSR.bounds.extents.x * 1.95f;
                backgrounds[1].transform.position = new Vector3(tempX, backgrounds[1].transform.position.y, backgrounds[1].transform.position.z);
            }
        }
        else if (backgrounds[0].transform.position.x < backgrounds[1].transform.position.x)
        {
            //goal to set the position of the 0th background to the right edge of the 1th background
            SpriteRenderer bgSR0 = backgrounds[0].GetComponent<SpriteRenderer>();
            if (leftCamEdge >= backgrounds[0].transform.position.x + bgSR0.bounds.extents.x)
            {
                SpriteRenderer bgSR = backgrounds[1].GetComponent<SpriteRenderer>();
                float tempX = backgrounds[1].transform.position.x + bgSR.bounds.extents.x * 1.95f;
                backgrounds[0].transform.position = new Vector3(tempX, backgrounds[0].transform.position.y, backgrounds[0].transform.position.z);
            }
        }
    }

    private void PrintDebugInfo()
    {
        Debug.Log("Camera Viewport rect position: " + mainCam.rect.center);
        Debug.Log("Camera Viewport rect width and height" + mainCam.rect.width + ", " + mainCam.rect.height);
        Vector2 camWorldPos = mainCam.ViewportToWorldPoint(Camera.main.rect.center);
        Debug.Log("Camera WorldPoint position " + camWorldPos);
        Debug.Log("Orthographic size: " + mainCam.orthographicSize);
        float cameraWidth = mainCam.aspect * mainCam.orthographicSize;
        Debug.Log("camera width and height: " + cameraWidth * 2 + "," + mainCam.orthographicSize * 2);
        Debug.Log("Aspect Ratio: " + mainCam.aspect);

        float cameraHalfWidth = (mainCam.aspect * mainCam.orthographicSize * 2) / 2;
        float cameraHalfHeight = mainCam.orthographicSize;
        Debug.Log("camera half width and half height: " + cameraHalfWidth + ", " + cameraHalfHeight);
        Debug.Log("Camera in pixel coordinates" + mainCam.pixelRect);
    }
}
