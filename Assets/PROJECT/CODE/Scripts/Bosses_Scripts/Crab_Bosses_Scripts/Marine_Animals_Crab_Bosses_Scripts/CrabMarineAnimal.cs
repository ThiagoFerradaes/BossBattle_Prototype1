using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class CrabMarineAnimal : MonoBehaviour {

    #region Parameters

    [Header("Components")]
    [SerializeField] CrabMarineAnimalSO MarineAnimalInfo;
    [HideInInspector] public CrabManager CrabManager;

    // Coroutines
    Coroutine _durationCoroutine;

    #endregion

    #region Methods
    public void OnStart() {
        gameObject.SetActive(true);

        _durationCoroutine ??= StartCoroutine(Duration());

        if (CrabManager == null ) CrabManager = CrabArenaManager.Instance.CrabM; 
    }

    IEnumerator Duration() {

        yield return new WaitForSeconds(MarineAnimalInfo.Duration);

        _durationCoroutine = null;

        End();
    }

    void End() {
        if (_durationCoroutine != null) StopCoroutine(_durationCoroutine);

        MarineAnimalInfo.OnEnd();

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    private void OnTriggerEnter(Collider other) {

        if (!MarineAnimalInfo.ListOfTags.Any(tag => other.CompareTag(tag.ToString()))) return;

        MarineAnimalInfo.OnTrigger(other, this);

        End();
    }

    #endregion
}
