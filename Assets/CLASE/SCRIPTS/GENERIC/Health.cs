using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
  

    [SerializeField] private int health; // Tendremos una variable local de vida
    [Networked] public int _health { get; set; }

    public override void Spawned() // Cuando haces spawn, la vida se configura
    {
        _health = health;
    }

    /// <summary>
    /// Cualquiera puede recibir daño, pero solo el Host lo ejecuta
    /// porque si en el target pongo All, entonces el objetivo recibira daño
    /// de todos lados
    /// </summary>
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void Rpc_TakeDamage(int damage, PlayerRef shooter)
    {
        _health -= damage;
        Debug.Log($"{name} recibio daño de {shooter}. Vida actual: {_health}");

        if (_health <= 0) 
        { 
            OnDeath();
        }

    }

    private void OnDeath()
    {

    }

}
