using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities.Serialization;
using UnityEngine;

public class LevelReferences : MonoBehaviour
{
    [SerializeField] private List<EntitySceneReference> sceneReferences;

    public List<EntitySceneReference> SceneReferences => sceneReferences;

    public static LevelReferences Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
    }
}
