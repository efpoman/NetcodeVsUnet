using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct ProyectoNetcodeGhostSerializerCollection : IGhostSerializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "SphereGhostSerializer",
            "PelotaGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(SphereSnapshotData))
            return 0;
        if (typeof(T) == typeof(PelotaSnapshotData))
            return 1;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_SphereGhostSerializer.BeginSerialize(system);
        m_PelotaGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_SphereGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_PelotaGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_SphereGhostSerializer.SnapshotSize;
            case 1:
                return m_PelotaGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<ProyectoNetcodeGhostSerializerCollection>.InvokeSerialize<SphereGhostSerializer, SphereSnapshotData>(m_SphereGhostSerializer, ref dataStream, data);
            }
            case 1:
            {
                return GhostSendSystem<ProyectoNetcodeGhostSerializerCollection>.InvokeSerialize<PelotaGhostSerializer, PelotaSnapshotData>(m_PelotaGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private SphereGhostSerializer m_SphereGhostSerializer;
    private PelotaGhostSerializer m_PelotaGhostSerializer;
}

public struct EnableProyectoNetcodeGhostSendSystemComponent : IComponentData
{}
public class ProyectoNetcodeGhostSendSystem : GhostSendSystem<ProyectoNetcodeGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableProyectoNetcodeGhostSendSystemComponent>();
    }
}
