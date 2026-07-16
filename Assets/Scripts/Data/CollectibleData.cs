using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleData", menuName = "ECS/CollectibleData")]
public class CollectibleData : ScriptableObject
{
    public CollectibleType CollectibleType;
    public float Points;    
}
