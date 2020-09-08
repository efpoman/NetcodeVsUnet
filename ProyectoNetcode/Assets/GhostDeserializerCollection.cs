using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct ProyectoNetcodeGhostDeserializerCollection : IGhostDeserializerCollection
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
    public void Initialize(World world)
    {
        var curSphereGhostSpawnSystem = world.GetOrCreateSystem<SphereGhostSpawnSystem>();
        m_SphereSnapshotDataNewGhostIds = curSphereGhostSpawnSystem.NewGhostIds;
        m_SphereSnapshotDataNewGhosts = curSphereGhostSpawnSystem.NewGhosts;
        curSphereGhostSpawnSystem.GhostType = 0;
        var curPelotaGhostSpawnSystem = world.GetOrCreateSystem<PelotaGhostSpawnSystem>();
        m_PelotaSnapshotDataNewGhostIds = curPelotaGhostSpawnSystem.NewGhostIds;
        m_PelotaSnapshotDataNewGhosts = curPelotaGhostSpawnSystem.NewGhosts;
        curPelotaGhostSpawnSystem.GhostType = 1;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_SphereSnapshotDataFromEntity = system.GetBufferFromEntity<SphereSnapshotData>();
        m_PelotaSnapshotDataFromEntity = system.GetBufferFromEntity<PelotaSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<ProyectoNetcodeGhostDeserializerCollection>.InvokeDeserialize(m_SphereSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
                return GhostReceiveSystem<ProyectoNetcodeGhostDeserializerCollection>.InvokeDeserialize(m_PelotaSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_SphereSnapshotDataNewGhostIds.Add(ghostId);
                m_SphereSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProyectoNetcodeGhostDeserializerCollection>.InvokeSpawn<SphereSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_PelotaSnapshotDataNewGhostIds.Add(ghostId);
                m_PelotaSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProyectoNetcodeGhostDeserializerCollection>.InvokeSpawn<PelotaSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<SphereSnapshotData> m_SphereSnapshotDataFromEntity;
    private NativeList<int> m_SphereSnapshotDataNewGhostIds;
    private NativeList<SphereSnapshotData> m_SphereSnapshotDataNewGhosts;
    private BufferFromEntity<PelotaSnapshotData> m_PelotaSnapshotDataFromEntity;
    private NativeList<int> m_PelotaSnapshotDataNewGhostIds;
    private NativeList<PelotaSnapshotData> m_PelotaSnapshotDataNewGhosts;
}
public struct EnableProyectoNetcodeGhostReceiveSystemComponent : IComponentData
{}
public class ProyectoNetcodeGhostReceiveSystem : GhostReceiveSystem<ProyectoNetcodeGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableProyectoNetcodeGhostReceiveSystemComponent>();
    }
}
