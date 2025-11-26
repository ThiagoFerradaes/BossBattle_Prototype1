using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ MarineAnimals/ Starfish")]
public class CrabStarFishSO : CrabMarineAnimalSO {
    [SerializeField] float healAmount;
    [SerializeField] float explosionRadius;
    [SerializeField] SkillAnimationEvent _vfx;
    Transform _parent;

    public override void OnTrigger(Collider other, CrabMarineAnimal parent) {
        base.OnTrigger(other, parent);

        _parent = parent.transform;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (_vfx.PreFab != null) InstantiateVFX(_vfx);

        health.Heal(healAmount);
    }

    void InstantiateVFX(SkillAnimationEvent prefab) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.VFX);
        hitbox.transform.position = _parent.position;
        hitbox.transform.localScale = explosionRadius * Vector3.one;

        VFXPreFabStatic damage = hitbox.GetComponent<VFXPreFabStatic>();
        damage.Initialize(prefab.VFXDuration);
    }
}
