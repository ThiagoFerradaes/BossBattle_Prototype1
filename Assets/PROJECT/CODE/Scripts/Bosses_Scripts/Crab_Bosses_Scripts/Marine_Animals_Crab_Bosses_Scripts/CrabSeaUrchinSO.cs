using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "Bosses/ MarineAnimals/ SeaUrchin")]
public class CrabSeaUrchinSO : CrabMarineAnimalSO {

    [Header("Explosion atributes")]
    [SerializeField] float explosionRadius;
 
    [Header("Damage atributes")]
    [SerializeField] List<SkillAnimationEvent> prefabs;

    CrabMarineAnimal _parent;

    public override void OnTrigger(Collider other, CrabMarineAnimal parent) {
        base.OnTrigger(other, parent);

        _parent = parent;

        if (prefabs != null) {
            for (int i = 0; i < prefabs.Count; i++) {
                if (prefabs[i].PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefabs[i]);
                else InstantiateVFX(prefabs[i]);
            }
        }
    }
    
    void InstantiateHitBox(SkillAnimationEvent prefab) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.position = _parent.transform.position;
        hitbox.transform.localScale = explosionRadius * Vector3.one;

        DamageContext context = new(
            Atributes,
            _parent.CrabManager.GetComponent<StatusManager>()
            );

        InstantDamageHitBox damage = hitbox.GetComponent<InstantDamageHitBox>();
        damage.Initialize(context);
    }

    void InstantiateVFX(SkillAnimationEvent prefab) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.VFX);
        hitbox.transform.position = _parent.transform.position;
        hitbox.transform.localScale = explosionRadius * Vector3.one;

        VFXPreFabStatic damage = hitbox.GetComponent<VFXPreFabStatic>();
        damage.Initialize(prefab.VFXAtribute);
    }
}
