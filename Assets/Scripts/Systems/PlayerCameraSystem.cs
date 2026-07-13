using Unity.Burst;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class PlayerCameraSystem : SystemBase
{
    private EntityQuery playerQuery;
    private CinemachineCamera cinemachineCamera;

    override protected void OnCreate()
    {
        playerQuery = SystemAPI.QueryBuilder().WithAll<PlayerManagedComponentData>().Build();

        RequireForUpdate(playerQuery);
    }

    protected override void OnUpdate()
    {
        if (cinemachineCamera == null)
            cinemachineCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamera>();

        if (cinemachineCamera.Follow != null)
            return;

        var playerTransform = playerQuery.GetSingleton<PlayerManagedComponentData>();
        cinemachineCamera.Follow = playerTransform.Transform;
    }

    override protected void OnDestroy()
    {
    }
}
