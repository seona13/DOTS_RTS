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
        SoldierAim,
        SoldierShoot,
        zombieAttack,
        ScoutIdle,
        ScoutWalk,
        ScoutShoot,
        ScoutAim,
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;


    public static bool IsAnimationUninterruptible(AnimationType animationType)
    {
        switch (animationType)
        {
            default:
                return false;
            case AnimationType.SoldierShoot:
            case AnimationType.ScoutShoot:
            case AnimationType.zombieAttack:
                return true;
        }
    }
}
