using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private string killerId;
    //asi se sincroniza con los clientes tambien
    [SyncVar]
    private int currentHealth;
    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }
    public float GetHealth()
    {
        return currentHealth;
    }
    public string GetKiller()
    {
        return killerId;
    }
    private void Awake()
    {
        SetDefaults();
    }
    [ClientRpc]
    public void RpcTakeDamage(int _amount)//,string _killerId)
    {
        
        if (currentHealth > 0)
        {
            currentHealth -= _amount;
            Debug.Log(transform.name + " now has " + currentHealth + " health.");
            //killerId = _killerId;
        }
        
    }
    public void SetDefaults()
    {
        currentHealth = maxHealth;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }
    public void SetKiller(string _killerId)
    {
        killerId= _killerId;
    }
}
