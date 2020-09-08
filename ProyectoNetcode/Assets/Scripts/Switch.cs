using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Switch : IComponentData
{
    public string switchName;
    public string switchClientorServer;
}
