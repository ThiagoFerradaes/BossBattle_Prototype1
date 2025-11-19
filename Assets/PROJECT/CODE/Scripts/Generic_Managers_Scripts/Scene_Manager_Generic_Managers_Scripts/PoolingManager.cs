using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour {
    #region Parameters
    public static PoolingManager Instance;

    // Dicionários
    Dictionary<GameObject, List<GameObject>> listOfHitboxes = new();
    Dictionary<GameObject, List<GameObject>> listOfPreCastingRange = new();
    Dictionary<GameObject, List<GameObject>> listOfVFX = new();
    Dictionary<GameObject, GameObject> listOfManagers = new();

    // Transforms
    public Transform HitboxContainer, ManagerContainer, VFXContainer, PreCastingContainer;

    #endregion

    #region Methods
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public GameObject ReturnPrefabFromPool(GameObject prefab, TypeOfSkillPrefab type) {

        Dictionary<GameObject, List<GameObject>> pool = type switch {
            TypeOfSkillPrefab.Hitbox => listOfHitboxes,
            TypeOfSkillPrefab.VFX => listOfVFX,
            _ => listOfPreCastingRange
        };

        if (!pool.ContainsKey(prefab)) {
            pool[prefab] = new List<GameObject>();
        }

        var list = pool[prefab];

        for (int i = 0; i < list.Count; i++) {
            if (!list[i].activeInHierarchy) return list[i];
        }

        Transform container = type switch {
            TypeOfSkillPrefab.Hitbox => HitboxContainer,
            TypeOfSkillPrefab.VFX => VFXContainer,
            _ => PreCastingContainer
        };

        GameObject newObject = Instantiate(prefab, container);
        newObject.SetActive(false);
        list.Add(newObject);
        return newObject;
    }


    public GameObject ReturnManagerFromPool(GameObject prefab) {

        if (listOfManagers.TryGetValue(prefab, out GameObject manager)) {
            return manager;
        }

        else {
            GameObject newManager = Instantiate(prefab, ManagerContainer);
            newManager.transform.SetParent(ManagerContainer.transform);
            newManager.SetActive(false);
            listOfManagers[prefab] = newManager;
            return listOfManagers[prefab];
        }
    }

    public void ReturnObjectToPool(GameObject prefab, TypeOfSkillPrefab type) {
        if(prefab.TryGetComponent<ParticleSystem>(out ParticleSystem ps)) {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        prefab.SetActive(false);

        Transform container = type switch {
            TypeOfSkillPrefab.Hitbox => HitboxContainer,
            TypeOfSkillPrefab.VFX => VFXContainer,
            TypeOfSkillPrefab.Manager => ManagerContainer,
            _ => PreCastingContainer
        };

        prefab.transform.SetParent(container);
    }

    #endregion
}
