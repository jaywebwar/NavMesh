using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _maxHeight;
    [SerializeField] float _minHeight;
    [SerializeField] float _zoomInterval;
    [SerializeField] float _zoomAngle;
    [SerializeField] float _panSpeed;

    Rigidbody _rb;
    Camera _camera;

    private void Awake()
    {
        _rb = transform.parent.GetComponent<Rigidbody>();
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Move();
        Zoom();
    }

    void Zoom()
    {
        Vector2 zoomInput = Input.mouseScrollDelta;
        float zoomDirection = -1f*zoomInput.y;
        _camera.orthographicSize += zoomDirection * _zoomInterval;
        _camera.transform.localRotation = Quaternion.Euler(zoomDirection*_zoomAngle + _camera.transform.localEulerAngles.x, 0f, 0f);
        if (_camera.orthographicSize > _maxHeight)
        {
            _camera.orthographicSize = _maxHeight;
            _camera.transform.localRotation = Quaternion.Euler(50f, 0f, 0f);
        }
        else if (_camera.orthographicSize < _minHeight)
        {
            _camera.orthographicSize = _minHeight;
            _camera.transform.localRotation = Quaternion.Euler(38f, 0f, 0f);
        }
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 horizontalMoveVector = transform.parent.right * horizontalInput * _panSpeed;
        Vector3 verticalMoveVector = transform.parent.forward * verticalInput * _panSpeed;

        _rb.velocity = horizontalMoveVector + verticalMoveVector;
    }
}
