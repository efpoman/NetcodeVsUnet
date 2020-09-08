using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Entities;

public struct SphereSnapshotData : ISnapshotData<SphereSnapshotData>
{
    public uint tick;
    private int PlayerDataplayerId;
    private int PlayerDatamaxHealth;
    private int PlayerDatacurrentHealth;
    private uint PlayerDatadeath;
    private uint PlayerDataauto;
    private uint PlayerDataautoSpawn;
    private int PlayerDatadestX;
    private int PlayerDatadestY;
    private int PlayerDatadestZ;
    private int PlayerDatadest2X;
    private int PlayerDatadest2Y;
    private int PlayerDatadest2Z;
    private int PlayerDatakilledBy;
    private int PlayerDatakilledByID;
    private int RotationValueX;
    private int RotationValueY;
    private int RotationValueZ;
    private int RotationValueW;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public int GetPlayerDataplayerId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDataplayerId;
    }
    public int GetPlayerDataplayerId()
    {
        return (int)PlayerDataplayerId;
    }
    public void SetPlayerDataplayerId(int val, GhostSerializerState serializerState)
    {
        PlayerDataplayerId = (int)val;
    }
    public void SetPlayerDataplayerId(int val)
    {
        PlayerDataplayerId = (int)val;
    }
    public int GetPlayerDatamaxHealth(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatamaxHealth;
    }
    public int GetPlayerDatamaxHealth()
    {
        return (int)PlayerDatamaxHealth;
    }
    public void SetPlayerDatamaxHealth(int val, GhostSerializerState serializerState)
    {
        PlayerDatamaxHealth = (int)val;
    }
    public void SetPlayerDatamaxHealth(int val)
    {
        PlayerDatamaxHealth = (int)val;
    }
    public int GetPlayerDatacurrentHealth(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatacurrentHealth;
    }
    public int GetPlayerDatacurrentHealth()
    {
        return (int)PlayerDatacurrentHealth;
    }
    public void SetPlayerDatacurrentHealth(int val, GhostSerializerState serializerState)
    {
        PlayerDatacurrentHealth = (int)val;
    }
    public void SetPlayerDatacurrentHealth(int val)
    {
        PlayerDatacurrentHealth = (int)val;
    }
    public bool GetPlayerDatadeath(GhostDeserializerState deserializerState)
    {
        return PlayerDatadeath!=0;
    }
    public bool GetPlayerDatadeath()
    {
        return PlayerDatadeath!=0;
    }
    public void SetPlayerDatadeath(bool val, GhostSerializerState serializerState)
    {
        PlayerDatadeath = val?1u:0;
    }
    public void SetPlayerDatadeath(bool val)
    {
        PlayerDatadeath = val?1u:0;
    }
    public bool GetPlayerDataauto(GhostDeserializerState deserializerState)
    {
        return PlayerDataauto!=0;
    }
    public bool GetPlayerDataauto()
    {
        return PlayerDataauto!=0;
    }
    public void SetPlayerDataauto(bool val, GhostSerializerState serializerState)
    {
        PlayerDataauto = val?1u:0;
    }
    public void SetPlayerDataauto(bool val)
    {
        PlayerDataauto = val?1u:0;
    }
    public bool GetPlayerDataautoSpawn(GhostDeserializerState deserializerState)
    {
        return PlayerDataautoSpawn!=0;
    }
    public bool GetPlayerDataautoSpawn()
    {
        return PlayerDataautoSpawn!=0;
    }
    public void SetPlayerDataautoSpawn(bool val, GhostSerializerState serializerState)
    {
        PlayerDataautoSpawn = val?1u:0;
    }
    public void SetPlayerDataautoSpawn(bool val)
    {
        PlayerDataautoSpawn = val?1u:0;
    }
    public float3 GetPlayerDatadest(GhostDeserializerState deserializerState)
    {
        return GetPlayerDatadest();
    }
    public float3 GetPlayerDatadest()
    {
        return new float3(PlayerDatadestX * 1f, PlayerDatadestY * 1f, PlayerDatadestZ * 1f);
    }
    public void SetPlayerDatadest(float3 val, GhostSerializerState serializerState)
    {
        SetPlayerDatadest(val);
    }
    public void SetPlayerDatadest(float3 val)
    {
        PlayerDatadestX = (int)(val.x * 1);
        PlayerDatadestY = (int)(val.y * 1);
        PlayerDatadestZ = (int)(val.z * 1);
    }
    public float3 GetPlayerDatadest2(GhostDeserializerState deserializerState)
    {
        return GetPlayerDatadest2();
    }
    public float3 GetPlayerDatadest2()
    {
        return new float3(PlayerDatadest2X * 1f, PlayerDatadest2Y * 1f, PlayerDatadest2Z * 1f);
    }
    public void SetPlayerDatadest2(float3 val, GhostSerializerState serializerState)
    {
        SetPlayerDatadest2(val);
    }
    public void SetPlayerDatadest2(float3 val)
    {
        PlayerDatadest2X = (int)(val.x * 1);
        PlayerDatadest2Y = (int)(val.y * 1);
        PlayerDatadest2Z = (int)(val.z * 1);
    }
    public Entity GetPlayerDatakilledBy(GhostDeserializerState deserializerState)
    {
        if (PlayerDatakilledBy == 0)
            return Entity.Null;
        if (!deserializerState.GhostMap.TryGetValue(PlayerDatakilledBy, out var ghostEnt))
            return Entity.Null;
        if (Unity.Networking.Transport.Utilities.SequenceHelpers.IsNewer(ghostEnt.spawnTick, Tick))
            return Entity.Null;
        return ghostEnt.entity;
    }
    public void SetPlayerDatakilledBy(Entity val, GhostSerializerState serializerState)
    {
        PlayerDatakilledBy = 0;
        if (serializerState.GhostStateFromEntity.Exists(val))
        {
            var ghostState = serializerState.GhostStateFromEntity[val];
            if (ghostState.despawnTick == 0)
                PlayerDatakilledBy = ghostState.ghostId;
        }
    }
    public void SetPlayerDatakilledBy(int val)
    {
        PlayerDatakilledBy = val;
    }
    public int GetPlayerDatakilledByID(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatakilledByID;
    }
    public int GetPlayerDatakilledByID()
    {
        return (int)PlayerDatakilledByID;
    }
    public void SetPlayerDatakilledByID(int val, GhostSerializerState serializerState)
    {
        PlayerDatakilledByID = (int)val;
    }
    public void SetPlayerDatakilledByID(int val)
    {
        PlayerDatakilledByID = (int)val;
    }
    public quaternion GetRotationValue(GhostDeserializerState deserializerState)
    {
        return GetRotationValue();
    }
    public quaternion GetRotationValue()
    {
        return new quaternion(RotationValueX * 0.001f, RotationValueY * 0.001f, RotationValueZ * 0.001f, RotationValueW * 0.001f);
    }
    public void SetRotationValue(quaternion q, GhostSerializerState serializerState)
    {
        SetRotationValue(q);
    }
    public void SetRotationValue(quaternion q)
    {
        RotationValueX = (int)(q.value.x * 1000);
        RotationValueY = (int)(q.value.y * 1000);
        RotationValueZ = (int)(q.value.z * 1000);
        RotationValueW = (int)(q.value.w * 1000);
    }
    public float3 GetTranslationValue(GhostDeserializerState deserializerState)
    {
        return GetTranslationValue();
    }
    public float3 GetTranslationValue()
    {
        return new float3(TranslationValueX * 0.01f, TranslationValueY * 0.01f, TranslationValueZ * 0.01f);
    }
    public void SetTranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetTranslationValue(val);
    }
    public void SetTranslationValue(float3 val)
    {
        TranslationValueX = (int)(val.x * 100);
        TranslationValueY = (int)(val.y * 100);
        TranslationValueZ = (int)(val.z * 100);
    }

    public void PredictDelta(uint tick, ref SphereSnapshotData baseline1, ref SphereSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        PlayerDataplayerId = predictor.PredictInt(PlayerDataplayerId, baseline1.PlayerDataplayerId, baseline2.PlayerDataplayerId);
        PlayerDatamaxHealth = predictor.PredictInt(PlayerDatamaxHealth, baseline1.PlayerDatamaxHealth, baseline2.PlayerDatamaxHealth);
        PlayerDatacurrentHealth = predictor.PredictInt(PlayerDatacurrentHealth, baseline1.PlayerDatacurrentHealth, baseline2.PlayerDatacurrentHealth);
        PlayerDatadeath = (uint)predictor.PredictInt((int)PlayerDatadeath, (int)baseline1.PlayerDatadeath, (int)baseline2.PlayerDatadeath);
        PlayerDataauto = (uint)predictor.PredictInt((int)PlayerDataauto, (int)baseline1.PlayerDataauto, (int)baseline2.PlayerDataauto);
        PlayerDataautoSpawn = (uint)predictor.PredictInt((int)PlayerDataautoSpawn, (int)baseline1.PlayerDataautoSpawn, (int)baseline2.PlayerDataautoSpawn);
        PlayerDatadestX = predictor.PredictInt(PlayerDatadestX, baseline1.PlayerDatadestX, baseline2.PlayerDatadestX);
        PlayerDatadestY = predictor.PredictInt(PlayerDatadestY, baseline1.PlayerDatadestY, baseline2.PlayerDatadestY);
        PlayerDatadestZ = predictor.PredictInt(PlayerDatadestZ, baseline1.PlayerDatadestZ, baseline2.PlayerDatadestZ);
        PlayerDatadest2X = predictor.PredictInt(PlayerDatadest2X, baseline1.PlayerDatadest2X, baseline2.PlayerDatadest2X);
        PlayerDatadest2Y = predictor.PredictInt(PlayerDatadest2Y, baseline1.PlayerDatadest2Y, baseline2.PlayerDatadest2Y);
        PlayerDatadest2Z = predictor.PredictInt(PlayerDatadest2Z, baseline1.PlayerDatadest2Z, baseline2.PlayerDatadest2Z);
        PlayerDatakilledBy = predictor.PredictInt(PlayerDatakilledBy, baseline1.PlayerDatakilledBy, baseline2.PlayerDatakilledBy);
        PlayerDatakilledByID = predictor.PredictInt(PlayerDatakilledByID, baseline1.PlayerDatakilledByID, baseline2.PlayerDatakilledByID);
        RotationValueX = predictor.PredictInt(RotationValueX, baseline1.RotationValueX, baseline2.RotationValueX);
        RotationValueY = predictor.PredictInt(RotationValueY, baseline1.RotationValueY, baseline2.RotationValueY);
        RotationValueZ = predictor.PredictInt(RotationValueZ, baseline1.RotationValueZ, baseline2.RotationValueZ);
        RotationValueW = predictor.PredictInt(RotationValueW, baseline1.RotationValueW, baseline2.RotationValueW);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
    }

    public void Serialize(int networkId, ref SphereSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (PlayerDataplayerId != baseline.PlayerDataplayerId) ? 1u : 0;
        changeMask0 |= (PlayerDatamaxHealth != baseline.PlayerDatamaxHealth) ? (1u<<1) : 0;
        changeMask0 |= (PlayerDatacurrentHealth != baseline.PlayerDatacurrentHealth) ? (1u<<2) : 0;
        changeMask0 |= (PlayerDatadeath != baseline.PlayerDatadeath) ? (1u<<3) : 0;
        changeMask0 |= (PlayerDataauto != baseline.PlayerDataauto) ? (1u<<4) : 0;
        changeMask0 |= (PlayerDataautoSpawn != baseline.PlayerDataautoSpawn) ? (1u<<5) : 0;
        changeMask0 |= (PlayerDatadestX != baseline.PlayerDatadestX ||
                                           PlayerDatadestY != baseline.PlayerDatadestY ||
                                           PlayerDatadestZ != baseline.PlayerDatadestZ) ? (1u<<6) : 0;
        changeMask0 |= (PlayerDatadest2X != baseline.PlayerDatadest2X ||
                                           PlayerDatadest2Y != baseline.PlayerDatadest2Y ||
                                           PlayerDatadest2Z != baseline.PlayerDatadest2Z) ? (1u<<7) : 0;
        changeMask0 |= (PlayerDatakilledBy != baseline.PlayerDatakilledBy) ? (1u<<8) : 0;
        changeMask0 |= (PlayerDatakilledByID != baseline.PlayerDatakilledByID) ? (1u<<9) : 0;
        changeMask0 |= (RotationValueX != baseline.RotationValueX ||
                                           RotationValueY != baseline.RotationValueY ||
                                           RotationValueZ != baseline.RotationValueZ ||
                                           RotationValueW != baseline.RotationValueW) ? (1u<<10) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<11) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(PlayerDataplayerId, baseline.PlayerDataplayerId, compressionModel);
        if ((changeMask0 & (1 << 1)) != 0)
            writer.WritePackedIntDelta(PlayerDatamaxHealth, baseline.PlayerDatamaxHealth, compressionModel);
        if ((changeMask0 & (1 << 2)) != 0)
            writer.WritePackedIntDelta(PlayerDatacurrentHealth, baseline.PlayerDatacurrentHealth, compressionModel);
        if ((changeMask0 & (1 << 3)) != 0)
            writer.WritePackedUIntDelta(PlayerDatadeath, baseline.PlayerDatadeath, compressionModel);
        if ((changeMask0 & (1 << 4)) != 0)
            writer.WritePackedUIntDelta(PlayerDataauto, baseline.PlayerDataauto, compressionModel);
        if ((changeMask0 & (1 << 5)) != 0)
            writer.WritePackedUIntDelta(PlayerDataautoSpawn, baseline.PlayerDataautoSpawn, compressionModel);
        if ((changeMask0 & (1 << 6)) != 0)
        {
            writer.WritePackedIntDelta(PlayerDatadestX, baseline.PlayerDatadestX, compressionModel);
            writer.WritePackedIntDelta(PlayerDatadestY, baseline.PlayerDatadestY, compressionModel);
            writer.WritePackedIntDelta(PlayerDatadestZ, baseline.PlayerDatadestZ, compressionModel);
        }
        if ((changeMask0 & (1 << 7)) != 0)
        {
            writer.WritePackedIntDelta(PlayerDatadest2X, baseline.PlayerDatadest2X, compressionModel);
            writer.WritePackedIntDelta(PlayerDatadest2Y, baseline.PlayerDatadest2Y, compressionModel);
            writer.WritePackedIntDelta(PlayerDatadest2Z, baseline.PlayerDatadest2Z, compressionModel);
        }
        if ((changeMask0 & (1 << 8)) != 0)
            writer.WritePackedIntDelta(PlayerDatakilledBy, baseline.PlayerDatakilledBy, compressionModel);
        if ((changeMask0 & (1 << 9)) != 0)
            writer.WritePackedIntDelta(PlayerDatakilledByID, baseline.PlayerDatakilledByID, compressionModel);
        if ((changeMask0 & (1 << 10)) != 0)
        {
            writer.WritePackedIntDelta(RotationValueX, baseline.RotationValueX, compressionModel);
            writer.WritePackedIntDelta(RotationValueY, baseline.RotationValueY, compressionModel);
            writer.WritePackedIntDelta(RotationValueZ, baseline.RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(RotationValueW, baseline.RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 11)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref SphereSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            PlayerDataplayerId = reader.ReadPackedIntDelta(baseline.PlayerDataplayerId, compressionModel);
        else
            PlayerDataplayerId = baseline.PlayerDataplayerId;
        if ((changeMask0 & (1 << 1)) != 0)
            PlayerDatamaxHealth = reader.ReadPackedIntDelta(baseline.PlayerDatamaxHealth, compressionModel);
        else
            PlayerDatamaxHealth = baseline.PlayerDatamaxHealth;
        if ((changeMask0 & (1 << 2)) != 0)
            PlayerDatacurrentHealth = reader.ReadPackedIntDelta(baseline.PlayerDatacurrentHealth, compressionModel);
        else
            PlayerDatacurrentHealth = baseline.PlayerDatacurrentHealth;
        if ((changeMask0 & (1 << 3)) != 0)
            PlayerDatadeath = reader.ReadPackedUIntDelta(baseline.PlayerDatadeath, compressionModel);
        else
            PlayerDatadeath = baseline.PlayerDatadeath;
        if ((changeMask0 & (1 << 4)) != 0)
            PlayerDataauto = reader.ReadPackedUIntDelta(baseline.PlayerDataauto, compressionModel);
        else
            PlayerDataauto = baseline.PlayerDataauto;
        if ((changeMask0 & (1 << 5)) != 0)
            PlayerDataautoSpawn = reader.ReadPackedUIntDelta(baseline.PlayerDataautoSpawn, compressionModel);
        else
            PlayerDataautoSpawn = baseline.PlayerDataautoSpawn;
        if ((changeMask0 & (1 << 6)) != 0)
        {
            PlayerDatadestX = reader.ReadPackedIntDelta(baseline.PlayerDatadestX, compressionModel);
            PlayerDatadestY = reader.ReadPackedIntDelta(baseline.PlayerDatadestY, compressionModel);
            PlayerDatadestZ = reader.ReadPackedIntDelta(baseline.PlayerDatadestZ, compressionModel);
        }
        else
        {
            PlayerDatadestX = baseline.PlayerDatadestX;
            PlayerDatadestY = baseline.PlayerDatadestY;
            PlayerDatadestZ = baseline.PlayerDatadestZ;
        }
        if ((changeMask0 & (1 << 7)) != 0)
        {
            PlayerDatadest2X = reader.ReadPackedIntDelta(baseline.PlayerDatadest2X, compressionModel);
            PlayerDatadest2Y = reader.ReadPackedIntDelta(baseline.PlayerDatadest2Y, compressionModel);
            PlayerDatadest2Z = reader.ReadPackedIntDelta(baseline.PlayerDatadest2Z, compressionModel);
        }
        else
        {
            PlayerDatadest2X = baseline.PlayerDatadest2X;
            PlayerDatadest2Y = baseline.PlayerDatadest2Y;
            PlayerDatadest2Z = baseline.PlayerDatadest2Z;
        }
        if ((changeMask0 & (1 << 8)) != 0)
            PlayerDatakilledBy = reader.ReadPackedIntDelta(baseline.PlayerDatakilledBy, compressionModel);
        else
            PlayerDatakilledBy = baseline.PlayerDatakilledBy;
        if ((changeMask0 & (1 << 9)) != 0)
            PlayerDatakilledByID = reader.ReadPackedIntDelta(baseline.PlayerDatakilledByID, compressionModel);
        else
            PlayerDatakilledByID = baseline.PlayerDatakilledByID;
        if ((changeMask0 & (1 << 10)) != 0)
        {
            RotationValueX = reader.ReadPackedIntDelta(baseline.RotationValueX, compressionModel);
            RotationValueY = reader.ReadPackedIntDelta(baseline.RotationValueY, compressionModel);
            RotationValueZ = reader.ReadPackedIntDelta(baseline.RotationValueZ, compressionModel);
            RotationValueW = reader.ReadPackedIntDelta(baseline.RotationValueW, compressionModel);
        }
        else
        {
            RotationValueX = baseline.RotationValueX;
            RotationValueY = baseline.RotationValueY;
            RotationValueZ = baseline.RotationValueZ;
            RotationValueW = baseline.RotationValueW;
        }
        if ((changeMask0 & (1 << 11)) != 0)
        {
            TranslationValueX = reader.ReadPackedIntDelta(baseline.TranslationValueX, compressionModel);
            TranslationValueY = reader.ReadPackedIntDelta(baseline.TranslationValueY, compressionModel);
            TranslationValueZ = reader.ReadPackedIntDelta(baseline.TranslationValueZ, compressionModel);
        }
        else
        {
            TranslationValueX = baseline.TranslationValueX;
            TranslationValueY = baseline.TranslationValueY;
            TranslationValueZ = baseline.TranslationValueZ;
        }
    }
    public void Interpolate(ref SphereSnapshotData target, float factor)
    {
        SetRotationValue(math.slerp(GetRotationValue(), target.GetRotationValue(), factor));
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
