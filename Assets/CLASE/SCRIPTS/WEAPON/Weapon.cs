using Fusion;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected ShootType type;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected NetworkPrefabRef bullet;
    [SerializeField] protected Camera playerCam;
    [SerializeField] protected LayerMask layerMask;

    [SerializeField] protected int damage;
    [SerializeField] protected float range;
    [SerializeField] protected int actualAmmo;

    public ShootType Type => type;

    public abstract void RigidBodyShoot();
    public abstract void RpcRaycastShoot(RpcInfo info = default);
}

public enum ShootType
{
    RigidBody,Raycast
}