using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;


//[UpdateInWorld(UpdateInWorld.TargetWorld.Client)]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class HybridMainCameraFollowPlayerSystem : SystemBase
{
    float currentCameraRotationX = 0f;
    protected override void OnUpdate()
    {
        // Camera position default.
        var position = Camera.main.transform.position;
        var camRotation = Camera.main.transform.rotation;
        //GameObject ui = GameObject.FindGameObjectWithTag("UI");
        var health = UI.vida;
        var killer = UI.KillerIdName;
        var playerid = UI.PlayerIdName;
        // Get the player entity.
        var commandTargetComponentEntity = GetSingletonEntity<CommandTargetComponent>();
        var commandTargetComponent = GetComponent<CommandTargetComponent>(commandTargetComponentEntity);

        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;

        Entities.WithoutBurst().
            ForEach(
                    (Entity entity,in PlayerData playerData, in Translation translation, in Rotation rotation,in DynamicBuffer<PlayerInput> inputBuffer,in PredictedGhostComponent prediction) =>
                    {
                        if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                            return;
                        // Only update when the player is the one that is controlled by the client's player.
                        if (entity == commandTargetComponent.targetEntity)
                        {
                            PlayerInput input;
                            inputBuffer.GetDataAtTick(tick, out input);
                            currentCameraRotationX -= input.xRot * 0.0025f;
                            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX,-85f,85f);
                            position.x = translation.Value.x;
                            position.y = 1;
                            position.z = translation.Value.z;
                            camRotation = math.mul(rotation.Value,quaternion.RotateX(currentCameraRotationX));
                            health = playerData.currentHealth;
                            killer = playerData.killedByID;
                            playerid = playerData.playerId;
                        }
                    }
                   ).Run();
        UI.vida = health;
        UI.KillerIdName = killer;
        UI.PlayerIdName = playerid;
        Camera.main.transform.position = position;
        Camera.main.transform.rotation = camRotation;
    }
}
