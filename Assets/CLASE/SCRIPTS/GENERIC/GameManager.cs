using Fusion;
using TMPro;
using UnityEngine;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [Header("Configuracion de Tiempo")]
    [SerializeField] private float tiempoDePartida = 120f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private GameObject panelResultado;
    [SerializeField] private TextMeshProUGUI textoResultado;

    [Header("Mensajes")]
    [SerializeField] private string mensajeVictoria = "¡VICTORIA!";
    [SerializeField] private string mensajeDerrota = "DERROTA";
    [SerializeField] private string mensajeEmpate = "¡EMPATE!";

    [Networked] private TickTimer timerPartida { get; set; }
    [Networked] private NetworkBool partidaTerminada { get; set; }
    [Networked] private float tiempoRestante { get; set; }
    [Networked] private PlayerRef jugadorGanador { get; set; }

    private ScoreManager scoreManager;

    private void Awake()
    {
        if (panelResultado != null)
            panelResultado.SetActive(false);
    }

    public override void Spawned()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();

        if (panelResultado != null)
            panelResultado.SetActive(false);

        if (Object.HasStateAuthority)
        {
            timerPartida = TickTimer.CreateFromSeconds(Runner, tiempoDePartida);
            partidaTerminada = false;
            jugadorGanador = PlayerRef.None;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (partidaTerminada) return;

        if (timerPartida.IsRunning)
        {
            tiempoRestante = timerPartida.RemainingTime(Runner) ?? 0f;
        }

        if (Object.HasStateAuthority && timerPartida.Expired(Runner))
        {
            TerminarPartida();
        }
    }

    public override void Render()
    {
        if (textoTiempo != null && !partidaTerminada)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
            textoTiempo.text = $"{minutos:00}:{segundos:00}";
        }
    }

    private void TerminarPartida()
    {
        partidaTerminada = true;

        if (scoreManager != null)
        {
            jugadorGanador = scoreManager.ObtenerGanador();
        }

        RPC_MostrarResultado(jugadorGanador);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_MostrarResultado(PlayerRef ganador)
    {
        if (panelResultado == null) return;

        panelResultado.SetActive(true);

        if (textoResultado != null)
        {
            PlayerRef localPlayer = Runner.LocalPlayer;

            if (ganador == PlayerRef.None)
            {
                textoResultado.text = mensajeEmpate;
            }
            else if (localPlayer == ganador)
            {
                textoResultado.text = mensajeVictoria;
            }
            else
            {
                textoResultado.text = mensajeDerrota;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool PartidaEnCurso()
    {
        return !partidaTerminada;
    }

    public void RegistrarNuevoJugador(PlayerRef jugador)
    {
        if (scoreManager != null)
        {
            scoreManager.RegistrarJugador(jugador);
        }
    }
}