using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] Weapon actualWeapon;


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            if (input.shoot)
            {
                actualWeapon.RpcRaycastShoot();
            }
        }
    }
}
