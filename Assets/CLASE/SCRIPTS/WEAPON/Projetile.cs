using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class Projetile : NetworkBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifeTime = 2f;

    [Networked] public int Damage { get; set; }
    [Networked] public PlayerRef Shooter { get; set; }

    private Rigidbody rb;
    private bool yaColisiono = false;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        yaColisiono = false;

        if (Object.HasStateAuthority)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        DespawnAfterTime();
    }

    private async void DespawnAfterTime()
    {
        await Task.Delay((int)(lifeTime * 1000));

        if (Object != null && Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    public void SetProjetile(PlayerRef shooter, int damage)
    {
        Shooter = shooter;
        Damage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Object == null || !Object.IsValid) return;
        if (!Object.HasStateAuthority) return;
        if (yaColisiono) return;

        yaColisiono = true;

        if (collision.collider.TryGetComponent(out Health health))
        {
            health.Rpc_TakeDamage(Damage, Shooter);
        }

        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
    }
}