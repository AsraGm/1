using Fusion;
using UnityEngine;

public class Handgun : Weapon
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public override void RpcRaycastShoot(RpcInfo info = default)
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, range, layerMask))
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.TryGetComponent(out Health health))
            {
                PlayerRef shooter = Object.InputAuthority;
                Debug.Log($"[HANDGUN] Raycast - Shooter: {shooter.PlayerId}");
                health.Rpc_TakeDamage(damage, shooter);
            }
            else
            {
                Debug.Log("No tiene componente de vida");
            }
        }
    }

    public override void RigidBodyShoot()
    {        
        RpcPhysicShoot(shootPoint.position, shootPoint.rotation, Object.InputAuthority);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcPhysicShoot(Vector3 pos, Quaternion rot, PlayerRef shooter)
    {
        if (bullet.IsValid)
        {
            NetworkObject bulletInstance = Runner.Spawn(bullet, pos, rot, shooter);

            if (bulletInstance.TryGetComponent(out Projetile projectile))
            {
                projectile.SetProjetile(shooter, damage);
                Debug.Log($"[HANDGUN] Bala creada - Shooter: {shooter.PlayerId}, Damage: {damage}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.indianRed;
        Gizmos.DrawRay(playerCam.transform.position, playerCam.transform.forward * range);
    }
}
