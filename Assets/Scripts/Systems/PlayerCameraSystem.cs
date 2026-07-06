using Unity.Burst;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class PlayerCameraSystem : SystemBase
{
    private EntityQuery playerQuery;
    private CinemachineCamera cinemachineCamera;

    [BurstCompile]
    override protected void OnCreate()
    {
        playerQuery = SystemAPI.QueryBuilder().WithAll<PlayerComponentData>().WithAll<LocalTransform>().Build();

        RequireForUpdate(playerQuery);
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (cinemachineCamera != null)
            return;

        cinemachineCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamera>();
        var playerTransform = playerQuery.GetSingleton<LocalTransform>();
        // cinemachineCamera.Follow = playerTransform.;
    }

    [BurstCompile]
    override protected void OnDestroy()
    {
    }
}
