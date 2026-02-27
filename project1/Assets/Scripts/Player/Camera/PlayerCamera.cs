using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;
    
    private float _xRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;
        
        _player.Rotate(Vector3.up * mouseX);
        
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, _minY, _maxY);
    }
}
