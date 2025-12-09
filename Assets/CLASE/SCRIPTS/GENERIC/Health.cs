using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [Networked] public int CurrentHealth { get; set; }

    private ScoreManager scoreManager;

    public override void Spawned()
    {
        CurrentHealth = maxHealth;
        scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_TakeDamage(int damage, PlayerRef shooter)
    {
        CurrentHealth -= damage;
        Debug.Log($"{name} recibio daño de {shooter}. Vida actual: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath(shooter);
        }
    }

    private void OnDeath(PlayerRef asesino)
    {
        if (gameObject.CompareTag("Enemigo"))
        {
            if (scoreManager == null)
                scoreManager = FindFirstObjectByType<ScoreManager>();

            if (scoreManager != null && asesino != PlayerRef.None)
            {
                scoreManager.Rpc_AgregarPuntaje(asesino);
            }
        }

        if (Object != null && Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}