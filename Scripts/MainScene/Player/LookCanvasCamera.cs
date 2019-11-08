using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCanvasCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(new Vector3(0f, -180f, 0f));

    }
}
