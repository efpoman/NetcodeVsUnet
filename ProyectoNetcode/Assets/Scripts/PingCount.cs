using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PingCount : MonoBehaviour
{
    Ping ping;
    List<float> pingList;
    
    void Start()
    {
        Debug.Log(GameController.IP);
        pingList = new List<float>();
        ping = new Ping(GameController.IP);

        StartCoroutine(PingUpdate());
    }

    IEnumerator PingUpdate()
    {
        yield return new WaitForSeconds(1f);
        if (ping.isDone)
        {
            pingList.Add(ping.time);
            Debug.Log(ping.time);
            ping = new Ping(GameController.IP);
        }
        StartCoroutine(PingUpdate());
    }

    private void OnDestroy()
    {
        string pathping = Application.dataPath + "/ping.txt";
        StreamWriter sw;
        sw = File.CreateText(pathping);
        for (int i = 0; i < pingList.Count; i++)
        {
            sw.WriteLine(pingList[i].ToString() + " ");
        }

    }
}
