using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Camera_Script : MonoBehaviour
{

    public float scrollAtEdgeBuffer;
    public float scrollSpeed;
    public bool lockedToPlayerMarker;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (lockedToPlayerMarker)
        {
            Vector3 camPos = GameObject.FindGameObjectWithTag("Player Map Marker").transform.position;
            camPos.z = this.gameObject.transform.position.z;
            Camera.main.transform.position = camPos;
        }
        else
        {
            Vector3 targetPos = Camera.main.transform.position;
            if (Input.mousePosition.x >= Screen.width - scrollAtEdgeBuffer)
            {
                // Move the camera
                targetPos += Vector3.right * Time.deltaTime * scrollSpeed;
            }
            if (Input.mousePosition.x <= 0 + scrollAtEdgeBuffer)
            {
                // Move the camera
                targetPos -= Vector3.right * Time.deltaTime * scrollSpeed;
            }
            if (Input.mousePosition.y >= Screen.height - scrollAtEdgeBuffer)
            {
                // Move the camera
                targetPos += Vector3.up * Time.deltaTime * scrollSpeed;
            }
            if (Input.mousePosition.y <= 0 + scrollAtEdgeBuffer)
            {
                // Move the camera
                targetPos -= Vector3.up * Time.deltaTime * scrollSpeed;
            }

            targetPos = limitToMapBounds(targetPos);

            Camera.main.transform.position = targetPos;
        }

    }

    private Vector3 limitToMapBounds(Vector3 cameraPosIn)
    {
        Vector3 cameraPos = cameraPosIn;
        SpriteRenderer background = GameObject.FindGameObjectWithTag("Map Background").GetComponent<SpriteRenderer>();
        float rightBounds = background.bounds.extents.x;
        float leftBounds = -background.bounds.extents.x;
        float topBounds = background.bounds.extents.y;
        float bottomBounds = -background.bounds.extents.y;

        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = Camera.main.aspect * cameraHeight;

        if(cameraPos.x > rightBounds - cameraWidth)
        {
            cameraPos.x = rightBounds - cameraWidth;
            //cameraPos.x = rightBounds;
        }

        if (cameraPos.x < leftBounds + cameraWidth)
        {
            cameraPos.x = leftBounds + cameraWidth;
            //cameraPos.x = leftBounds;
        }

        if (cameraPos.y > topBounds - cameraHeight)
        {
            cameraPos.y = topBounds - cameraHeight;
            //cameraPos.y = topBounds;
        }

        if (cameraPos.y < bottomBounds + cameraHeight)
        {
            cameraPos.y = bottomBounds + cameraHeight;
            //cameraPos.y = bottomBounds;
        }

        return cameraPos;
    }
}
