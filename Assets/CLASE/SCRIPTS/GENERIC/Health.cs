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
        Debug.Log($"[HEALTH] {gameObject.name} spawned con {maxHealth} HP. ScoreManager encontrado: {scoreManager != null}");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_TakeDamage(int damage, PlayerRef shooter)
    {
        CurrentHealth -= damage;
        Debug.Log($"[HEALTH] {name} recibio {damage} daño de jugador {shooter.PlayerId}. Vida: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath(shooter);
        }
    }

    private void OnDeath(PlayerRef asesino)
    {
        Debug.Log($"[HEALTH] {gameObject.name} murio. Asesino: {asesino.PlayerId}, Tag: {gameObject.tag}");

        if (gameObject.CompareTag("Enemigo"))
        {
            if (scoreManager == null)
                scoreManager = FindFirstObjectByType<ScoreManager>();

            Debug.Log($"[HEALTH] ScoreManager: {(scoreManager != null ? "OK" : "NULL")}, Asesino valido: {asesino != PlayerRef.None}");

            if (scoreManager != null && asesino != PlayerRef.None)
            {
                Debug.Log($"[HEALTH] Agregando puntaje a jugador {asesino.PlayerId}");
                scoreManager.Rpc_AgregarPuntaje(asesino);
            }
        }

        if (Object != null && Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}