using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WalkToLowTidePosition")]
public class CrabWalkToLowTide : EnemyBehaviourSO {
    CrabManager _crabManager;
    Animator _anim;

    [Header("Atributes")]
    [SerializeField] float percentOfHighTide;
    [SerializeField] float lowTideHeight;
    [SerializeField] float offSet;
    [SerializeField] float cooldownBetweenAttacks;
    [SerializedDictionary("Wall, Position"), SerializeField] SerializedDictionary<CrabArenaWall, Vector3> listOfPossibleFinalPositions = new();

    [Header("Animation")]
    [SerializeField] string changeTideAnimationParameter;
    [SerializeField] string changeTideAnimationName;
    [SerializeField] int animationLayer;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WalkToPosition());
    }

    public override bool MeetsCondition() {
        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide && CrabArenaManager.Instance.ReturnCurrentTidePercent() >= percentOfHighTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent) {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
    }

    IEnumerator WalkToPosition() {

        while (true) {
            var activeChannels = _crabManager.ReturnActiveChannels();

            if (!ListOfChannelsToClose.Any(a => activeChannels.ContainsKey(a) && activeChannels[a]))
                break;

            yield return null;
        }

        #region Animate and ChangeTide

        _anim.SetTrigger(changeTideAnimationParameter);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do { // Esperando entrar na animação correta
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
        } while (!stateInfo.IsName(changeTideAnimationName));


        if (prefabs != null) {
            var listOfPreffabs = prefabs;
            listOfPreffabs.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

            int attackHash = stateInfo.fullPathHash;

            for (int i = 0; i < listOfPreffabs.Count; i++) {
                var prefab = listOfPreffabs[i];

                do { // Esperando o tempo pra instanciar
                    yield return null;
                    stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
                } while (stateInfo.fullPathHash == attackHash && stateInfo.normalizedTime < prefab.TimeToSpawnPreFab);

                if (prefab.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefab);
                else InstantiateVFX(prefab);
            }

            do { // Esperando a animação terminar
                yield return null;
                stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            } while (stateInfo.fullPathHash == attackHash && stateInfo.normalizedTime < 1);
        }

        #endregion

        #region WalkTo
        CrabArenaWall currentWall = _crabManager.ReturnCurrentWall();

        Vector3 pos = listOfPossibleFinalPositions[currentWall];
        Vector3 pos2 = new(0, pos.y, 0);
        Vector3 dir = (pos - pos2).normalized;

        Vector3 finalPosition = pos + (dir * offSet);

        _crabManager.WalkToTarget(0, finalPosition, false);
        #endregion

        yield return _crabManager.ReturnWalkCoroutine();

        CrabArenaManager.Instance.ChangeCurrentTide();

        while (CrabArenaManager.Instance.ReturnCurrentTide() > CrabArenaState.LowTide) yield return null;

        #region WalkToFront
        _crabManager.transform.position = new(_crabManager.transform.position.x, lowTideHeight, _crabManager.transform.position.z);

        pos.y = lowTideHeight;

        _crabManager.WalkToTarget(0, pos);

        yield return _crabManager.ReturnWalkCoroutine();

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());
        #endregion
    }

    void InstantiateHitBox(SkillAnimationEvent prefab) { }
    void InstantiateVFX(SkillAnimationEvent prefab) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.VFX);
        hitbox.transform.position = prefab.PreFabPosition;

        hitbox.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }
}
