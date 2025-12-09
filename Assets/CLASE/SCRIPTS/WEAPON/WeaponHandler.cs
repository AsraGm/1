using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] private Weapon actualWeapon;

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;

        if (GetInput(out NetworkInputData input))
        {
            if (input.shoot)
            {
                switch (actualWeapon.Type)
                {
                    case ShootType.RigidBody:
                        actualWeapon.RigidBodyShoot();
                        break;
                    case ShootType.Raycast:
                        actualWeapon.RpcRaycastShoot();
                        break;
                }
            }
        }
    }
}
