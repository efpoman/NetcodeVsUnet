using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;
using System;
using System.IO;

[UpdateInWorld(UpdateInWorld.TargetWorld.Server)]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MovementSystem : SystemBase
{
 
    
    

    GameObject objfps;
   

    protected override void OnCreate()
    {
        objfps = GameObject.FindGameObjectWithTag("FPSCount");
    
    }

    protected override void OnUpdate()
    {
        

        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
       
        EntityQuery m_EntitiesToDamageQuery = GetEntityQuery(typeof(PlayerData));
        var entities = m_EntitiesToDamageQuery.ToEntityArray(Allocator.TempJob);
        var ides = new NativeArray<int>(entities.Length, Allocator.TempJob);
        var idUsada = new NativeArray<int>(entities.Length, Allocator.TempJob);
        var component = new NativeArray<PlayerData>(entities.Length, Allocator.TempJob);
        var players = new NativeArray<PlayerData>(entities.Length, Allocator.TempJob);
        

        for (var i = 0; i < entities.Length; ++i)
        {
            ides[i] = EntityManager.GetComponentData<PlayerData>(entities[i]).playerId;
            component[i] = EntityManager.GetComponentData<PlayerData>(entities[i]);
        }
       
        float deltaTime = Time.DeltaTime;
        EntityManager entityManager = EntityManager;
        NativeArray<bool> spawnBallsAuto = new NativeArray<bool>(GameController.instance.spawnBallsAuto, Allocator.TempJob);
        NativeArray<bool> spawnOneBallAuto = new NativeArray<bool>(GameController.instance.spawnOneBallAuto, Allocator.TempJob);
        NativeArray<bool> auxShoot = new NativeArray<bool>(entities.Length, Allocator.TempJob);

        //var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
        Entities.WithoutBurst().ForEach((Entity ent,DynamicBuffer<PlayerInput> inputBuffer,ref LocalToWorld local, ref Translation trans,ref Rotation rotation,
                                         ref PlayerData player, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            //auxShoot[0] = true;
            //recogemos los datos del buffer por cada tick y movemos los entity dependiendo del valor leido
            PlayerInput input;
            inputBuffer.GetDataAtTick(tick, out input);
            if (input.revive > 0)
            {
                player.death = false;
                player.currentHealth = 100;
               // health= player.currentHealth;
            }
            if (player.death)
                return;

            //Habilitar movimiento automatico con la p
            if (input.autoMove > 0)
            {
                player.auto = !player.auto;
            }
            //Eliminar el spawneo automatico con la o
            if (input.autoSpawn > 0)
            {
                player.autoSpawn = !player.autoSpawn;
               
            }
            //Si esta habilitado el automove ...
            if (player.auto)
            {
                float step = 4 * deltaTime;
                auxShoot[player.playerId - 1] = false;

                if (Vector3.Distance(trans.Value, player.dest) == 0)
                    if (Vector3.Distance(player.dest, player.dest2) != 0) { 
                        player.dest = new float3(0f, 1f, -30f);
                        spawnBallsAuto[0] = false;
                        auxShoot[player.playerId - 1] = false;
                    }
                    else
                    {
                        player.dest = new float3(0f, 1f, 30f);
                        if(player.autoSpawn)
                            spawnBallsAuto[0] = true;

                        for (int i = 0; i < ides.Length; ++i)
                        {
                            
                            if (ides[i] == 2 && player.playerId !=2)
                            {
                                var jugador = component[i];
                                auxShoot[player.playerId-1] = true;
                                idUsada[player.playerId-1] = i;
                                
                                jugador.currentHealth -= 10;
                                if (jugador.currentHealth < 0)
                                {
                                    jugador.currentHealth = 0;
                                    jugador.death = true;
                                }
                                jugador.killedBy = ent;
                                jugador.killedByID = player.playerId;


                                //entityManager.SetComponentData<PlayerData>(entities[i], jugador);
                                players[player.playerId - 1] = jugador;
                            }
                            
                        }
                    }

                trans.Value = Vector3.MoveTowards(trans.Value,player.dest,step);
                rotation.Value = Quaternion.LookRotation(player.dest, Vector3.up);
            }
            else
            {
                auxShoot[player.playerId - 1] = false;
                Vector3 _rotation = new Vector3(0f, input.yRot, 0f) * 2f;
                //currentCameraRotationX -= input.xRot * 0.0025f;
                //currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -85f, 85f);
                //rotation.Value=math.mul(rotation.Value, quaternion.RotateX(currentCameraRotationX));
                rotation.Value *= Quaternion.Euler(_rotation);

                Vector3 movHorizontal = local.Right  * input.xMov;

                Vector3 movVertical = local.Forward * input.zMov;
                Vector3 velocity = (movHorizontal + movVertical).normalized * 2f;
                //Almacena el valor de velocidad
                trans.Value.x += velocity.x * deltaTime;
                trans.Value.z += velocity.z * deltaTime;

                
                if (input.shoot > 0)
                {
                   auxShoot[player.playerId-1] = true;
                    for (int i = 0; i < ides.Length; ++i)
                    {
                        
                        if (ides[i] == input.shootID)
                        {
                            var jugador = component[i];
                            idUsada[player.playerId-1] = i;
                            jugador.currentHealth -= 10;
                            if (jugador.currentHealth < 0)
                            {
                                jugador.currentHealth = 0;
                                jugador.death = true;
                            }
                            jugador.killedBy = ent;
                            jugador.killedByID = player.playerId;
                            //entityManager.SetComponentData<PlayerData>(entities[i], jugador);
                            players[player.playerId - 1] = jugador;
                        }
                        
                    }
                }
                
                if (input.check > 0)
                {
                    spawnOneBallAuto[0] = !spawnOneBallAuto[0];
                }
                if (input.ballscheck > 0)
                {
                    spawnBallsAuto[0] = !spawnBallsAuto[0];    
                }

            }
        }).ScheduleParallel();
        this.CompleteDependency();
        
            for (int i = 0; i < ides.Length; ++i)
            {
                if (auxShoot[i])
                {
                    entityManager.SetComponentData<PlayerData>(entities[idUsada[i]], players[i]);
                    auxShoot[i] = false;
                }
            }
        
        
        entities.Dispose();
        ides.Dispose();
        idUsada.Dispose();
        component.Dispose();
        players.Dispose();
        auxShoot.Dispose();
        
        if (spawnBallsAuto[0])
        {
            //for (int i = 0; i < 10; i++)
            //{
            objfps = GameObject.FindGameObjectWithTag("FPSCount");
            objfps.GetComponent<FpsCount>().balls += 1;
            //obtenemos los ghost
            var ghostCollection1 = GetSingleton<GhostPrefabCollectionComponent>();
            //obtenemos el id del ghost que nos interesa 
            var ghostId1 = ProyectoNetcodeGhostSerializerCollection.FindGhostType<PelotaSnapshotData>();
            //su prefab 
            var prefab1 = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection1.serverPrefabs)[ghostId1].Value;
            //y lo instanciamos
            var pelota = EntityManager.Instantiate(prefab1);
            float xa = UnityEngine.Random.Range(-10, 10);
            float ya = 2.5f;
            float za = UnityEngine.Random.Range(-10, 10);
            //le añadimos el componente de posicion
            EntityManager.SetComponentData(pelota, new Unity.Transforms.Translation { Value = new float3(xa, ya, za) });
        }
        GameController.instance.spawnBallsAuto[0] = spawnBallsAuto[0];
        spawnBallsAuto.Dispose();

        if (spawnOneBallAuto[0])
        {
            
            objfps = GameObject.FindGameObjectWithTag("FPSCount");
            objfps.GetComponent<FpsCount>().balls += 1;
            //obtenemos los ghost
            var ghostCollection1 = GetSingleton<GhostPrefabCollectionComponent>();
            //obtenemos el id del ghost que nos interesa
            var ghostId1 = ProyectoNetcodeGhostSerializerCollection.FindGhostType<PelotaSnapshotData>();
            //su prefab 
            var prefab1 = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection1.serverPrefabs)[ghostId1].Value;
            //y lo instanciamos
            var pelota = EntityManager.Instantiate(prefab1);
            float xa = UnityEngine.Random.Range(-10, 10);
            float ya = 2.5f;
            float za = UnityEngine.Random.Range(-10, 10);
            //le añadimos el componente de posicion
            EntityManager.SetComponentData(pelota, new Unity.Transforms.Translation { Value = new float3(xa, ya, za) });
            spawnOneBallAuto[0] = false;
        }
        GameController.instance.spawnOneBallAuto[0] = spawnOneBallAuto[0];
        spawnOneBallAuto.Dispose();
        
        
    }

    

}
