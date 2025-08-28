using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class PoolingManager : MonoBehaviour {
    #region Parameters
    public static PoolingManager Instance;

    // Dicionários
    Dictionary<string, List<GameObject>> listOfHitboxes = new();
    Dictionary<string, List<GameObject>> listOfPreCastingRange = new();
    Dictionary<string, List<GameObject>> listOfVFX = new();
    Dictionary<string, GameObject> listOfManagers = new();

    // Transforms
    public Transform HitboxContainer, ManagerContainer, VFXContainer, PreCastingContainer;

    #endregion

    #region Methods
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public GameObject ReturnPrefabFromPool(string objectName, GameObject prefab, TypeOfSkillPrefab type) {

        Dictionary<string, List<GameObject>> pool = type switch {
            TypeOfSkillPrefab.Hitbox => listOfHitboxes,
            TypeOfSkillPrefab.VFX => listOfVFX,
            _ => listOfPreCastingRange
        };

        if (!pool.ContainsKey(objectName)) {
            pool[objectName] = new List<GameObject>();
        }

        var list = pool[objectName];

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


    public GameObject ReturnManagerFromPool(string managerName, GameObject prefab) {

        if (listOfManagers.TryGetValue(managerName, out GameObject manager)) {
            return manager;
        }

        else {
            GameObject newManager = Instantiate(prefab, ManagerContainer);
            newManager.transform.SetParent(ManagerContainer.transform);
            newManager.SetActive(false);
            listOfManagers[managerName] = newManager;
            return listOfManagers[managerName];
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
            _ => PreCastingContainer
        };

        prefab.transform.SetParent(container);
    }

    #endregion
}
