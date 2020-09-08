using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData
{
    [GhostDefaultField(1)]
    [NonSerialized] public int playerId;
    [GhostDefaultField(1)]
    [NonSerialized] public int maxHealth;
    [GhostDefaultField(1)]
    [NonSerialized] public int currentHealth;
    [GhostDefaultField(1)]
    [NonSerialized] public bool death;
    [GhostDefaultField(1)]
    [NonSerialized] public bool auto;
    [GhostDefaultField(1)]
    [NonSerialized] public bool autoSpawn;
    [GhostDefaultField(1)]
    [NonSerialized] public float3 dest;
    [GhostDefaultField(1)]
    [NonSerialized] public float3 dest2;
    [GhostDefaultField(1)]
    [NonSerialized] public Entity killedBy;
    [GhostDefaultField(1)]
    [NonSerialized] public int killedByID;
    public Entity hudEntity;
}
