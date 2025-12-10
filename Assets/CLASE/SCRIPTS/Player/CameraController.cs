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
    [SerializeField] private float mouseSensitivity = .2f;
    [FormerlySerializedAs("smooth")]
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private float maxAngleY = 80f;
    [SerializeField] private float minAngleY = -80f;

    [Networked] private float CamY { get; set; }
    [Networked] private float CamX { get; set; }

    private Vector2 camVelocity;
    private KCC kcc;
    private bool inicializado = false;
        
    public float ObtenerRotacionY()
    {
        if (HasInputAuthority)
            return camVelocity.x;
        else
            return CamX;
    }

    public override void Spawned()
    {
        if (player == null)
            player = GetComponentInParent<MovementController>()?.transform;

        if (player != null)
            kcc = player.GetComponent<KCC>();

        if (HasInputAuthority)
        {
            GetComponent<Camera>().enabled = true;
            GetComponent<AudioListener>().enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float rotacionInicial = player != null ? player.eulerAngles.y : 0f;
            camVelocity.x = rotacionInicial;
            camVelocity.y = 0f;
                        
            if (Object.HasStateAuthority)
            {
                CamX = rotacionInicial;
                CamY = 0f;
            }
            else
            {
                RPC_EnviarRotacion(rotacionInicial, 0f);
            }

            Debug.Log($"[CAM] InputAuthority. Rotacion inicial: {rotacionInicial}");
        }
        else
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }

        inicializado = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!inicializado) return;

        if (HasInputAuthority)
        {
            if (GetInput(out NetworkInputData input))
            {
                RotateCamera(input);
            }
        }

        AplicarRotacion();
    }

    private void RotateCamera(NetworkInputData input)
    {
        camVelocity.x += input.look.x * mouseSensitivity;
        camVelocity.y += input.look.y * mouseSensitivity;
        camVelocity.y = Mathf.Clamp(camVelocity.y, minAngleY, maxAngleY);

        if (Object.HasStateAuthority)
        {
            CamX = camVelocity.x;
            CamY = camVelocity.y;
        }
        else
        {
            RPC_EnviarRotacion(camVelocity.x, camVelocity.y);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_EnviarRotacion(float x, float y)
    {
        CamX = x;
        CamY = y;
    }

    private void AplicarRotacion()
    {
        transform.localRotation = Quaternion.AngleAxis(-CamY, Vector3.right);

        if (player != null)
        {
            player.rotation = Quaternion.Euler(0f, CamX, 0f);
        }

        if (kcc != null)
        {
            kcc.SetLookRotation(Quaternion.Euler(0f, CamX, 0f));
        }
    }

    public override void Render()
    {
        if (!inicializado) return;

        if (HasInputAuthority)
        {
            transform.localRotation = Quaternion.AngleAxis(-camVelocity.y, Vector3.right);
            if (player != null)
            {
                player.rotation = Quaternion.Euler(0f, camVelocity.x, 0f);
            }
        }
        else
        {
            transform.localRotation = Quaternion.AngleAxis(-CamY, Vector3.right);
            if (player != null)
            {
                player.rotation = Quaternion.Euler(0f, CamX, 0f);
            }
        }
    }
}