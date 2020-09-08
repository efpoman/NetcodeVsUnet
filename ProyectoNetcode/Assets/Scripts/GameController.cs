using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField]public static string IP;
    [SerializeField]public static ushort port;
    public bool[] spawnBallsAuto;
    public bool[] spawnOneBallAuto;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        spawnBallsAuto = new bool[1];
        spawnOneBallAuto = new bool[1];
        spawnBallsAuto[0] = false;
        spawnOneBallAuto[0] = false;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        IP="127.0.0.1";
        port = 7979;
    }
    public void changeIP(string ip)
    {
        IP = ip;
        
    }
    public void changePort(string puerto)
    {
        port = ushort.Parse(puerto);
    }
    public string GetIP()
    {
        return IP;
    }
    
}
