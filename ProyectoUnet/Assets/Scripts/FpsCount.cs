using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FpsCount : MonoBehaviour
{
    List<float> fpslist = new List<float>();
    List<float> timeList = new List<float>();
    List<int> ballsList = new List<int>();
    float dtime = 0f;
    float fps = 0f;
    float time = 0f;
    public int balls = 0;

    // Update is called once per frame
    void Update()
    {
        dtime += (Time.deltaTime - dtime) * 0.1f;
        fps = 1.0f / dtime;
        time += Time.deltaTime;
        fpslist.Add(fps);
        timeList.Add(time);
        ballsList.Add(balls);
    }
    private void OnDestroy()
    {
        string pathfps = Application.dataPath + "/fps.txt";
        string pathtime = Application.dataPath + "/time.txt";
        string pathballs = Application.dataPath + "/balls.txt";
        StreamWriter sw;
        sw = File.CreateText(pathfps);
        for (int i = 0; i < fpslist.Count; i++)
        {
            sw.WriteLine(fpslist[i].ToString() + " ");
        }
        sw = File.CreateText(pathtime);
        for (int i = 0; i < timeList.Count; i++)
        {
            sw.WriteLine(timeList[i].ToString() + " ");
        }
        sw = File.CreateText(pathballs);
        for (int i = 0; i < ballsList.Count; i++)
        {
            sw.WriteLine(ballsList[i].ToString() + " ");
        }

    }
}
