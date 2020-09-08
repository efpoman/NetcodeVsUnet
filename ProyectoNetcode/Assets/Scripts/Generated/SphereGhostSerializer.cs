using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

public struct SphereGhostSerializer : IGhostSerializer<SphereSnapshotData>
{
    private ComponentType componentTypePlayerData;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerData> ghostPlayerDataType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<SphereSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypePlayerData = ComponentType.ReadWrite<PlayerData>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostPlayerDataType = system.GetArchetypeChunkComponentType<PlayerData>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref SphereSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayerData = chunk.GetNativeArray(ghostPlayerDataType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetPlayerDataplayerId(chunkDataPlayerData[ent].playerId, serializerState);
        snapshot.SetPlayerDatamaxHealth(chunkDataPlayerData[ent].maxHealth, serializerState);
        snapshot.SetPlayerDatacurrentHealth(chunkDataPlayerData[ent].currentHealth, serializerState);
        snapshot.SetPlayerDatadeath(chunkDataPlayerData[ent].death, serializerState);
        snapshot.SetPlayerDataauto(chunkDataPlayerData[ent].auto, serializerState);
        snapshot.SetPlayerDataautoSpawn(chunkDataPlayerData[ent].autoSpawn, serializerState);
        snapshot.SetPlayerDatadest(chunkDataPlayerData[ent].dest, serializerState);
        snapshot.SetPlayerDatadest2(chunkDataPlayerData[ent].dest2, serializerState);
        snapshot.SetPlayerDatakilledBy(chunkDataPlayerData[ent].killedBy, serializerState);
        snapshot.SetPlayerDatakilledByID(chunkDataPlayerData[ent].killedByID, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
