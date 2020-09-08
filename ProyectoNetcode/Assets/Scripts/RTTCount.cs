using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Entities;
using System.IO;

[UpdateInWorld(UpdateInWorld.TargetWorld.Client)]
public class RTTCount : SystemBase
{
    List<float> rttList = new List<float>();
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((ref NetworkSnapshotAckComponent ack) =>
        {
            var rtt = ack.EstimatedRTT;
            //Debug.Log(rtt);
            rttList.Add(rtt);
            
        }).Run();
    }
    protected override void OnDestroy()
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
