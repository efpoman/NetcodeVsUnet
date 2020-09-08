using UnityEngine.Networking;
using UnityEngine;
using System;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    Player player;
    [SerializeField]
    GameObject sphere;
    [SerializeField]
    GameObject objFps;
    

    private PlayerMotor motor;

   public bool spawnBallsAuto=false;
    void Start()
    {
        player= this.GetComponent<Player>();
        motor = GetComponent<PlayerMotor>();
        //si no hay camara no hacemos nada
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: no camera");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetHealth() > 0)
        {
            
            if (Input.GetKeyDown("l"))
            {
                objFps = GameObject.FindGameObjectWithTag("FPSCount");
                objFps.GetComponent<FpsCount>().balls += 1;
                SpawnSphere();
            }
            if (Input.GetKeyDown("k"))
            {
                spawnBallsAuto= !spawnBallsAuto;
               
            }
            if (Input.GetKeyDown("p"))
            {
                motor.auto = !motor.auto;
            }
            if (Input.GetKeyDown("o"))
            {
                motor.autoSpawn = !motor.autoSpawn;
            }
            if (Input.GetKeyDown("1"))
            {
                CmdPlayerShot("Player 1", weapon.damage);
            }
            if (Input.GetKeyDown("2"))
            {
                CmdPlayerShot("Player 2", weapon.damage);
            }
            if (Input.GetKeyDown("3"))
            {
                CmdPlayerShot("Player 3", weapon.damage);
            }
            if (Input.GetKeyDown("4"))
            {
                CmdPlayerShot("Player 4", weapon.damage);
            }

        }
        if (Input.GetKeyDown("space"))
            CmdSetDefaults("Player "+GetComponent<NetworkIdentity>().netId.ToString());


        if (spawnBallsAuto)
        {
            //for (int i = 0; i < 10; i++)
            objFps = GameObject.FindGameObjectWithTag("FPSCount");
            objFps.GetComponent<FpsCount>().balls += 1;
            SpawnSphere();
        }
    }


    void SpawnSphere()
    {
       
        CmdSpawnSphere();
    }
    [Command]
    void CmdSpawnSphere()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-10f, 10f), 3f, UnityEngine.Random.Range(-10f, 10f));
        GameObject instantiatedSphere = (GameObject)Instantiate(sphere,pos,Quaternion.identity);
        GameObject owner = this.gameObject;
        NetworkServer.SpawnWithClientAuthority(instantiatedSphere, owner);
    }

    //Solo se ejecuta en el servidor
    [Command]
    public void CmdPlayerShot(string _playerID, int damage)
    {
        //Debug.Log(_playerID + " has been shot.");
        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(damage);//,this.netId.ToString());
        _player.SetKiller(this.netId.ToString());
    }

    public void CmdSetDefaults(string _playerID)
    {
        Player _player = GameManager.GetPlayer(_playerID);
        _player.SetDefaults();
    }
}
