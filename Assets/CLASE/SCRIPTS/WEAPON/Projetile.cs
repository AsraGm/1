using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class Projetile : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private int lifeTime;
    private PlayerRef shooter;

    private Rigidbody rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;

        DespawnAfterTime();
    }

    private async void DespawnAfterTime()
    {
        await Task.Delay(lifeTime * 1000);
        if (Object != null)
            Runner.Despawn(Object);
    }

    public void SetProjetile(PlayerRef shooter, int damage)
    {
        this.shooter = shooter;
        this.damage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Health health))
        {
            health.Rpc_TakeDamage(damage, shooter);
        }
        else
        {
            // Hacer aparecer un agujero de bala
            Debug.Log("No tiene componente de vida");
        }
        Runner.Despawn(Object);
    }

}
