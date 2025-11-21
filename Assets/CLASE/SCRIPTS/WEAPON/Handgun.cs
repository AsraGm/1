using Fusion;
using UnityEngine;

public class Handgun : Weapon
{
    // layerMask
    // range
    /// <summary>
    /// Un RPC es un protocolo para mandar a llamar un metodo en diferentes clientes
    /// 
    /// RpcSources es quien lo manda a llamar
    /// RpcTargets es quien lo ejecuta
    /// </summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public override void RpcRaycastShoot(RpcInfo info = default)
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward,out RaycastHit hit,range,layerMask))
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.TryGetComponent(out Health health))
            {
                health.Rpc_TakeDamage(damage, info.Source);
            }
            else
            {
                // Hacer aparecer un agujero de bala
                Debug.Log("No tiene componente de vida");
            }           
        }
    }

    public override void RigidBodyShoot()
    {
        RpcPhysicShoot(shootPoint.position,shootPoint.rotation);
    }

    /// <summary>
    /// Un RPC se ejecuta 2 veces
    /// La primera vez, tal cual no realiza lo que esta dentro de el metodo
    /// sino que, le manda a quien sea el target la ejecucion de el metodo.
    /// 
    /// Ya que el RpcTarget recibe el metodo, este lo invoca.
    /// 
    /// Cuando el RpcSource le dice al RpcTarget invoca esto, automaticamente
    /// el RpcTarget tambien recibe informacion sobre el Rpc mismo
    /// </summary>
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    private void RpcPhysicShoot(Vector3 pos, Quaternion rot, RpcInfo info = default)
    {
        if (bullet.IsValid)
        {
            NetworkObject bulletInstance = Runner.Spawn(bullet,pos,rot,info.Source);
            bulletInstance.GetComponent<Projetile>().SetProjetile(info.Source,damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.indianRed;
        Gizmos.DrawRay(playerCam.transform.position, playerCam.transform.forward * range);
    }

}
