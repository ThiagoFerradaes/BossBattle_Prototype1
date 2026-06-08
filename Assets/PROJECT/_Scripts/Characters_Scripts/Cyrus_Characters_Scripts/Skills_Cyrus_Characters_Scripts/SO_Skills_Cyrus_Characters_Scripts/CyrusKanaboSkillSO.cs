using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Kanabo")]
public class CyrusKanaboSkillSO : CommonSkillSO
{

    [Header("Atributes")]
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;

    [Header("Level One Atributes")]
    [Foldout("Specific")] public float AmountOfExplosionLevelOne;
    [Foldout("Specific")] public float TimeBetweenHitAndExplosion;
    [Foldout("Specific")] public DamageAtributes ExplosionAtributes;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float AmountOfExplosionLevelTwo;
    [Foldout("Specific")] public float ExplosionCritRateLevelTwo;
    [Foldout("Specific")] public float TimeBetweenExplosions;

    [Header("Level Three Atributes")]
    [Foldout("Specific")] public float AmountOfExplosionLevelThree;
    [Foldout("Specific")] public float ExplosionCritDamageLevelThree;
    [Foldout("Specific")] public float ExplosionRadiusLevelThree;
    [Foldout("Specific")] public DamageAtributes ContinuosDamageAreaAtributes;
}
