using Fusion;
using UnityEngine;

/// <summary>
/// Esta estructura debe contener TODOS los valores que se van a mandar
/// a el servidor. OJO unicamente los valores.
/// 
/// Esto debe de heredar de INetworkInput para que la estructura sea reconocida
/// por el servidor como una serie de inputs a leer
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector2 move;
    public Vector2 look;
    public bool isRunning;    
    public bool shoot;

    public const byte BotonDisparo = 0;
    public const byte BotonCorrer = 1;
    public const byte BotonRecarga = 2;

}
