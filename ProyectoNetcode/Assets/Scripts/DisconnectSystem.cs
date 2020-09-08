using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateAfter(typeof(MovePelotaSystem))]
public class DisconnectSystem : JobComponentSystem
{
    private BeginSimulationEntityCommandBufferSystem m_Barrier;
    [RequireComponentTag(typeof(NetworkStreamDisconnected))]
    [System.Obsolete]
    struct DisconnectJob : IJobForEach<CommandTargetComponent>
    {
        public EntityCommandBuffer commandBuffer;
        public void Execute(ref CommandTargetComponent state)
        {
            if (state.targetEntity != Entity.Null)
            {
                commandBuffer.DestroyEntity(state.targetEntity);
                state.targetEntity = Entity.Null;
            }
        }
    }

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DisconnectJob { commandBuffer = m_Barrier.CreateCommandBuffer() };
        var handle = job.ScheduleSingle(this, inputDeps);
        m_Barrier.AddJobHandleForProducer(handle);
        return handle;
    }
}