using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Sizing_Script : MonoBehaviour
{
    //public float orthographicSize = 13.04766f;
    //public float aspect = 1.33333f;

    /*void Start()
    {
        Camera mainCam = this.gameObject.GetComponent<Camera>();

        Camera.main.projectionMatrix = Matrix4x4.Ortho(
                -orthographicSize * aspect, orthographicSize * aspect,
                -orthographicSize, orthographicSize,
                mainCam.nearClipPlane, mainCam.farClipPlane);
    }*/

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 10;

    Camera _camera;

    void Start()
    {
        _camera = this.gameObject.GetComponent<Camera>();
        Debug.Log("Camera aspect ratio = " + _camera.aspect);
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update()
    {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        _camera.orthographicSize = desiredHalfHeight;
    }

}
