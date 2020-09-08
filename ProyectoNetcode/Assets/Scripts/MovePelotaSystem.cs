using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MovePelotaSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableProyectoNetcodeGhostSendSystemComponent>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation trans, ref PelotaComponent pelota) =>//.WithAll<PredictedGhostComponent>().ForEach((ref Translation trans, ref PelotaComponent pelota) =>
        {

            if (trans.Value.y <= 2.5f)
                trans.Value.y = 9;
            else
                trans.Value.y -= 0.03f;
        }).ScheduleParallel();
    }

}
