using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;


//estructura que engloba los movimientos que introduce el player
public struct PlayerInput : ICommandData<PlayerInput>
{
    public uint Tick => tick;
    public uint tick;
    public int ballscheck;
    public int vertical;
    public float zMov;
    public float xMov;
    public float yRot;
    public float xRot;
    public float currentCameraRotationX;
    public int shoot;
    public int shootID;
    public int check;
    public int autoMove;
    public int autoSpawn;
    public int revive;

    public void Deserialize(uint tick, ref DataStreamReader reader)
    {
        this.tick = tick;
        shoot = reader.ReadInt();
        shootID = reader.ReadInt();
        check = reader.ReadInt();
        autoMove = reader.ReadInt();
        autoSpawn = reader.ReadInt();
        revive = reader.ReadInt();
        ballscheck = reader.ReadInt();
        vertical = reader.ReadInt();
        zMov = reader.ReadFloat();
        xMov = reader.ReadFloat();
        yRot = reader.ReadFloat();
        xRot = reader.ReadFloat();
        currentCameraRotationX = reader.ReadFloat();
        
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteInt(shoot);
        writer.WriteInt(shootID);
        writer.WriteInt(check);
        writer.WriteInt(autoMove);
        writer.WriteInt(autoSpawn);
        writer.WriteInt(revive);
        writer.WriteInt(ballscheck);
        writer.WriteInt(vertical);
        writer.WriteFloat(zMov);
        writer.WriteFloat(xMov);
        writer.WriteFloat(yRot);
        writer.WriteFloat(xRot);
        writer.WriteFloat(currentCameraRotationX);
    }

    public void Deserialize(uint tick, ref DataStreamReader reader, PlayerInput baseline,
        NetworkCompressionModel compressionModel)
    {
        Deserialize(tick, ref reader);
    }

    public void Serialize(ref DataStreamWriter writer, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Serialize(ref writer);
    }
}

public class PlayerInputSendCommandSystem : CommandSendSystem<PlayerInput>
{
}
public class PlayerInputReceiveCommandSystem : CommandReceiveSystem<PlayerInput>
{
}


[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[UpdateBefore(typeof(GhostSimulationSystemGroup))]
public class SamplePlayerInput : ComponentSystem
{
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<EnableProyectoNetcodeGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
        if (localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref PlayerData player) =>
            {
                if (player.playerId == localPlayerId)
                {
                    PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                    PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent { targetEntity = ent });
                }
            });
            return;
        }

        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        // inicializa un input con la estructura creada y recoge las pulsaciones 
        var input = default(PlayerInput);
        input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;

        input.xMov = Input.GetAxisRaw("Horizontal");
        input.zMov = Input.GetAxisRaw("Vertical");
        input.yRot = Input.GetAxisRaw("Mouse X");
        input.xRot = Input.GetAxisRaw("Mouse Y");

        if (Input.GetKeyDown("space"))
        {
            input.revive += 1;
                //playerData.currentHealth = 100;
        }
        if (Input.GetKeyDown("k"))
        {
            input.ballscheck += 1;
        }
        if (Input.GetKeyDown("p"))
        {
            input.autoMove += 1;
        }
        if (Input.GetKeyDown("o"))
        {
            input.autoSpawn += 1;
        }
        if (Input.GetKeyDown("l"))
        {
            input.check += 1;
        }
        if (Input.GetKeyDown("1"))
        {
            input.shoot += 1;
            input.shootID = 1;
        }
        if (Input.GetKeyDown("2"))
        {
            input.shoot += 1;
            input.shootID = 2;
        }
        if (Input.GetKeyDown("3"))
        {
            input.shoot += 1;
            input.shootID = 3;
        }
        else if (inputBuffer.GetDataAtTick(input.tick, out var dupCmd) && dupCmd.Tick == input.tick)
            return;

        inputBuffer.AddCommandData(input);
        return;
    }
}
