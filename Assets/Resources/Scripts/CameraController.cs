using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera camera;
    public Rigidbody2D playerRB;

    public float normalZoom = 5f;
    public float zoomedIn = 3f;
    public float zoomSpeed = 2f;

    private void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
    }
    private void Update()
    {
        float targetRoom = (playerRB.velocity.magnitude < 0.1f ) ? zoomedIn : normalZoom;
        camera.m_Lens.OrthographicSize = Mathf.Lerp(camera.m_Lens.OrthographicSize, targetRoom, zoomSpeed * Time.deltaTime);
    }
}
