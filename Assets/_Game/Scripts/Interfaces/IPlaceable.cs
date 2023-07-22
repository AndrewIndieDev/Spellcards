using UnityEngine;

public interface IPlaceable
{
    Vector2Int Size { get; }
    Vector2Int GridPosition { get; }
    void OnPlace();
    void OnKill();
    void OnInteract();
    void OnExecute();
    void OnSpawn();
}