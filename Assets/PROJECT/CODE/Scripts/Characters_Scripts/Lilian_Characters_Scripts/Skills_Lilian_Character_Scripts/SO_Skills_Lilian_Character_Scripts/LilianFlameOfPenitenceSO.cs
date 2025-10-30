using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ FlameOfPenitence")]
public class LilianFlameOfPenitenceSO : CommonSkillSO
{

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Candle Atributes")]
    [Foldout("Specific")] public int CandleInitialLimit;
    [Foldout("Specific")] public float CandleTributeGeneration;
    [Foldout("Specific")] public float CandleTributeGenerationCooldown;
    [Foldout("Specific")] public float CandleHeight;
    [Foldout("Specific")] public float CandleFowardDistance;

    [Header("Candle Continuos Damage Atributes")]
    [Foldout("Specific")] public float CandleContinuosDamage;
    [Foldout("Specific")] public float CandleContinuosDamageCooldown;
    [Foldout("Specific"), Range(0,100)] public float CandleContinuosDamageCorruptionMultiplier;
    [Foldout("Specific")] public float CandleContinuosDamageSize;

    [Header("Candle Explosion Damage Atributes")]
    [Foldout("Specific")] public float CandleExplosionDamage;
    [Foldout("Specific")] public float CandleExplosionHealthMultiplier;
    [Foldout("Specific")] public float CandleExplosionDamageSize;

    [Header("Candle Object")]
    [Foldout("Specific")] public GameObject CandlePrefab;
    [Foldout("Specific")] public GameObject ContinuosHitBox;
    [Foldout("Specific")] public GameObject ExplosionHitBox;
    [Foldout("Specific")] public string CandlePrefabName;
    [Foldout("Specific")] public string ContinuosHitBoxName;
    [Foldout("Specific")] public string ExplosionHitBoxName;
}
