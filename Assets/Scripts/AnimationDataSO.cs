using UnityEngine;

[CreateAssetMenu()]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType
    {
        None,
        SoldierIdle,
        SoldierWalk,
        ZombieIdle,
        ZombieWalk,
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;
}
