using UnityEngine;

[CreateAssetMenu(menuName = "Skills / BaseAttack")]
public class WeaponMasterBaseAttackSO : SkillSO {

    [Header("Animation")]
    public string FirstBaseAttackParameter;
    public string FirstBaseAttackAnimationName;
    public string SecondBaseAttackParameter;
    public string SecondtBaseAttackAnimationName;

    [Header("Atributes")]
    public float FirstAttackDamage;
    public float SecondAttackDamage;
    public float CooldownBetweenAttacks;
    public float MaxTimeBetweenAttacks;
    public bool IsTrueDamage;
    public Tags EnemyTag;
    public Vector3 FirstBaseAttackHitBoxPosition;
    public Vector3 SecondtBaseAttackHitBoxPosition;
}
