using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public delegate void RespawnDelegate();
    public static event RespawnDelegate onRespawn;


    // Start is called before the first frame update
    void Start()
    {
        PlayerMove.onDeath += Respawn;       
    }

    void Respawn()
    {
        onRespawn.Invoke();
        onRespawn += Respawn;
        onRespawn();
    }
}
