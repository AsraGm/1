using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    [Networked, Capacity(16)]
    private NetworkDictionary<PlayerRef, int> Puntajes => default;

    private GameManager gameManager;

    public override void Spawned()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_AgregarPuntaje(PlayerRef jugador)
    {
        if (gameManager != null && !gameManager.PartidaEnCurso()) return;

        if (Puntajes.ContainsKey(jugador))
        {
            Puntajes.Set(jugador, Puntajes[jugador] + 1);
        }
        else
        {
            Puntajes.Add(jugador, 1);
        }

        Debug.Log($"[SCORE] Jugador {jugador.PlayerId} anoto. Puntaje: {Puntajes[jugador]}");
    }

    public int ObtenerPuntaje(PlayerRef jugador)
    {
        if (Puntajes.ContainsKey(jugador))
            return Puntajes[jugador];
        return 0;
    }

    public PlayerRef ObtenerGanador()
    {
        PlayerRef ganador = PlayerRef.None;
        int maxPuntaje = 0;
        bool empate = false;

        Debug.Log("[SCORE] === Calculando ganador ===");

        foreach (var kvp in Puntajes)
        {
            Debug.Log($"[SCORE] Jugador {kvp.Key.PlayerId}: {kvp.Value} puntos");

            if (kvp.Value > maxPuntaje)
            {
                maxPuntaje = kvp.Value;
                ganador = kvp.Key;
                empate = false;
            }
            else if (kvp.Value == maxPuntaje && kvp.Value > 0)
            {
                empate = true;
            }
        }

        Debug.Log($"[SCORE] Ganador: {(empate ? "EMPATE" : ganador.PlayerId.ToString())} con {maxPuntaje} puntos");

        return empate ? PlayerRef.None : ganador;
    }

    public void RegistrarJugador(PlayerRef jugador)
    {
        if (Object.HasStateAuthority)
        {
            if (!Puntajes.ContainsKey(jugador))
            {
                Puntajes.Add(jugador, 0);
                Debug.Log($"[SCORE] Jugador {jugador.PlayerId} registrado con 0 puntos");
            }
        }
    }
}