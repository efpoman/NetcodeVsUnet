using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

// Systema que se actualiza en el mundo por defecto
[UpdateInWorld(UpdateInWorld.TargetWorld.Default)]
public class Game : SystemBase
{

    struct InitGameComponent : IComponentData
    {
    }
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<InitGameComponent>();
      
        //Crea el singleton
        EntityManager.CreateEntity(typeof(InitGameComponent));
    }

    protected override void OnUpdate()
    {
        
        // Destruye el singleton para no ejecutarlo otra vez
        EntityManager.DestroyEntity(GetSingletonEntity<InitGameComponent>());
        foreach (var world in World.All)
        {
            var network = world.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
            {
                NetworkEndPoint ep = NetworkEndPoint.Parse(GameController.IP ,GameController.port);
                
                
                // El cliente se conecta automaticamente a localhost
                //NetworkEndPoint ep = NetworkEndPoint.LoopbackIpv4;
                ep.Port = 7979;
                network.Connect(ep);
            }
            

            else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
            {
                
                // El servidor espera conexiones de cualquier host
                NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                ep.Port = 7979;
                network.Listen(ep);
            }

        }
    }
}

// RPC request from client to server for game to go "in game" and send snapshots / inputs
[BurstCompile]
public struct GoInGameRequest : IRpcCommand
{
    // Unused integer for demonstration
    public int value;
    public void Deserialize(ref DataStreamReader reader)
    {
       value = reader.ReadInt();
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteInt(value);
    }
    [BurstCompile]
    private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
    {
        RpcExecutor.ExecuteCreateRequestComponent<GoInGameRequest>(ref parameters);
    }

    static PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
        new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);
    public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
    {
        return InvokeExecuteFunctionPointer;
    }
}

// The system that makes the RPC request component transfer
public class GoInGameRequestSystem : RpcCommandRequestSystem<GoInGameRequest>
{
}


//System del cliente 
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class GoInGameClientSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableProyectoNetcodeGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        //Busca las conexiones que no estan en el juego aun
        Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
        {
            
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent<GoInGameRequest>(req);
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
        });
    }
    protected override void OnDestroy()
    {

        base.OnDestroy();
    }
}


//System del servidor 
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class GoInGameServerSystem : ComponentSystem
{
    public bool instanciarUno = true;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableProyectoNetcodeGhostSendSystemComponent>();
        
        instanciarUno = true;
    }

    protected override void OnUpdate()
    {
        

        Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref GoInGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
            UnityEngine.Debug.Log(String.Format("El server establecio la conexion {0} ", EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value));
#if true
            var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
            var ghostId = ProyectoNetcodeGhostSerializerCollection.FindGhostType<SphereSnapshotData>();
            var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
            var player = EntityManager.Instantiate(prefab);
            
            EntityManager.SetComponentData(player, new PlayerData
            {
                playerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value,
                maxHealth = 100,
                currentHealth = 100,
                death = false,
                auto=false,
                autoSpawn = true,
                dest= new float3(0f, 1f, 30f),
                dest2= new float3(0f, 1f, -30f),
                killedBy = Entity.Null,
                hudEntity = Entity.Null
            });
            

            //Posiciones aleatorias
            float x = 0;
            float y = 1;
            float z = 0;
            EntityManager.SetComponentData(player, new Unity.Transforms.Translation { Value = new float3(x, y, z) });
          
            PostUpdateCommands.AddBuffer<PlayerInput>(player);
            PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });
#endif
            

            PostUpdateCommands.DestroyEntity(reqEnt);
          
        });

       
    }
}