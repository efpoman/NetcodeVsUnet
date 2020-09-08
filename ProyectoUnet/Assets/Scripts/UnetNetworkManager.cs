using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class UnetNetworkManager : NetworkManager
{
 
    public static string currentIp;
    public static bool cliente;
    List<float> rttList = new List<float>();
    public void Start()
    {
        currentIp="localhost";
        cliente = false;
    }
    public void ChangeIp(string ip)
      {
          currentIp = ip;
      }
      public void Join()
      {
          NetworkManager.singleton.networkAddress = currentIp;
          NetworkManager.singleton.networkPort = 7979;
          NetworkManager.singleton.StartClient();
          cliente = true;
      }
      public void Host()
      {
          NetworkManager.singleton.networkAddress = "87.220.40.163";
          NetworkManager.singleton.networkPort = 7979;
          NetworkManager.singleton.StartHost();

      }
    void Update()
    {
        
        rttList.Add(client.GetRTT());
    }
    private void OnDestroy()
    {
        string pathping = Application.dataPath + "/rtt.txt";
        StreamWriter sw;
        sw = File.CreateText(pathping);
        for (int i = 0; i < rttList.Count; i++)
        {
            sw.WriteLine(rttList[i].ToString() + " ");
        }

    }

}
