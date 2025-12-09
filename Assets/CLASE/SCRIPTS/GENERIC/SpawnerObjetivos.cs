using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerObjetivos : NetworkBehaviour
{
    [Header("Prefab del Objetivo")]
    [SerializeField] private NetworkPrefabRef objetivoPrefab;

    [Header("Area de Spawn")]
    [SerializeField] private Vector3 areaMin = new Vector3(-10f, 1f, -10f);
    [SerializeField] private Vector3 areaMax = new Vector3(10f, 1f, 10f);

    [Header("Configuracion de Spawn")]
    [SerializeField] private float tiempoEntreOleadasMin = 3f;
    [SerializeField] private float tiempoEntreOleadasMax = 8f;
    [SerializeField] private int objetivosPorOleadaMin = 1;
    [SerializeField] private int objetivosPorOleadaMax = 5;

    [Header("Limites")]
    [SerializeField] private int maxObjetivosEnEscena = 20;

    [Networked] private TickTimer timerProximaOleada { get; set; }

    private List<NetworkObject> objetivosActivos = new List<NetworkObject>();

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            ProgramarProximaOleada();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        LimpiarObjetivosDestruidos();

        if (timerProximaOleada.Expired(Runner))
        {
            SpawnearOleada();
            ProgramarProximaOleada();
        }
    }

    private void ProgramarProximaOleada()
    {
        float tiempoEspera = Random.Range(tiempoEntreOleadasMin, tiempoEntreOleadasMax);
        timerProximaOleada = TickTimer.CreateFromSeconds(Runner, tiempoEspera);
    }

    private void SpawnearOleada()
    {
        int cantidadDisponible = maxObjetivosEnEscena - objetivosActivos.Count;
        if (cantidadDisponible <= 0) return;

        int cantidadASpawnear = Random.Range(objetivosPorOleadaMin, objetivosPorOleadaMax + 1);
        cantidadASpawnear = Mathf.Min(cantidadASpawnear, cantidadDisponible);

        for (int i = 0; i < cantidadASpawnear; i++)
        {
            Vector3 posicionAleatoria = new Vector3(
                Random.Range(areaMin.x, areaMax.x),
                Random.Range(areaMin.y, areaMax.y),
                Random.Range(areaMin.z, areaMax.z)
            );

            Quaternion rotacionAleatoria = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            NetworkObject objetivo = Runner.Spawn(
                objetivoPrefab,
                posicionAleatoria,
                rotacionAleatoria
            );

            if (objetivo != null)
            {
                objetivosActivos.Add(objetivo);
            }
        }

        Debug.Log($"[Spawner] Oleada: {cantidadASpawnear} objetivos. Total: {objetivosActivos.Count}");
    }

    private void LimpiarObjetivosDestruidos()
    {
        objetivosActivos.RemoveAll(obj => obj == null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Vector3 centro = (areaMin + areaMax) / 2f;
        Vector3 tamaño = areaMax - areaMin;
        Gizmos.DrawCube(centro, tamaño);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centro, tamaño);
    }
}