using System;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class CameraController : NetworkBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity = 1f;
    [FormerlySerializedAs("smooth")]
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private float maxAngleY = 80f;
    [SerializeField] private float minAngleY = -80f;

    
    [Networked] private float CamY { get; set; }
    [Networked] private float CamX { get; set; }

    private Vector2 camVelocity;
    private Vector2 smoothVelocity;
    private Vector3 startPos;
    private KCC kcc;

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
          
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }

       
        if (player == null)
            player = GetComponentInParent<MovementController>()?.transform;

        if (player != null)
            kcc = player.GetComponent<KCC>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            RotateCamera(input);
        }
    }

    private void RotateCamera(NetworkInputData input)
    {
        
        Vector2 rawVelocity = input.look * mouseSensitivity;
        smoothVelocity = Vector2.Lerp(smoothVelocity, rawVelocity, 1f / smoothness);

        
        camVelocity += smoothVelocity;
        camVelocity.y = Mathf.Clamp(camVelocity.y, minAngleY, maxAngleY);

       
        CamY = camVelocity.y;
        CamX = camVelocity.x;

        
        transform.localRotation = Quaternion.AngleAxis(-CamY, Vector3.right);

        
        if (kcc != null)
        {
            Quaternion horizontalRotation = Quaternion.AngleAxis(CamX, Vector3.up);
            kcc.SetLookRotation(horizontalRotation);
        }
    }

    public override void Render()
    {
        
        if (!HasInputAuthority)
        {
            transform.localRotation = Quaternion.AngleAxis(-CamY, Vector3.right);
        }
    }

}