using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WalkToHighTidePosition")]
public class CrabWalkToHighTide : EnemyBehaviourSO
{

    CrabManager _crabManager;
    Animator _anim;

    [Header("Atributes")]
    [SerializeField] float percentOfLowTide;
    [SerializeField] float highTideHeight;
    [SerializeField] float offSet;
    [SerializeField] float cooldownBetweenAttacks;
    [SerializedDictionary("Wall, Position"), SerializeField] SerializedDictionary<CrabArenaWall, Vector3> listOfPossibleFinalPositions = new();
    [SerializeField] List<int> listOfNextAttacksChannels;

    [Header("Animation")]
    [SerializeField] string changeTideAnimationParameter;
    [SerializeField] string changeTideAnimationName;
    [SerializeField] int animationLayer;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    #region Initialize
    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WalkToPosition());
    }

    public override bool MeetsCondition()
    {
        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.LowTide && CrabArenaManager.Instance.ReturnCurrentTidePercent() >= percentOfLowTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
    }

    #endregion
    IEnumerator WalkToPosition()
    {

        #region Animate and ChangeTide

        _anim.SetTrigger(changeTideAnimationParameter);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do
        { // Esperando entrar na animação correta
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
        } while (!stateInfo.IsName(changeTideAnimationName));


        if (prefabs != null)
        {
            var listOfPreffabs = prefabs;
            listOfPreffabs.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

            int attackHash = stateInfo.fullPathHash;

            for (int i = 0; i < listOfPreffabs.Count; i++)
            {
                var prefab = listOfPreffabs[i];

                do
                { // Esperando o tempo pra instanciar
                    yield return null;
                    stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
                } while (stateInfo.fullPathHash == attackHash && stateInfo.normalizedTime < prefab.TimeToSpawnPreFab);

                if (prefab.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefab);
                else InstantiateVFX(prefab);
            }

            do
            { // Esperando a animação terminar
                yield return null;
                stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            } while (stateInfo.fullPathHash == attackHash && stateInfo.normalizedTime < 1);
        }

        #endregion

        #region WalkToBack
        int rng = Random.Range(0, listOfPossibleFinalPositions.Count);

        CrabArenaWall randomWall = (CrabArenaWall)rng;

        _crabManager.SetCurrentArenaWall(randomWall);

        Vector3 pos = listOfPossibleFinalPositions[randomWall];

        Vector3 dir = (pos - Vector3.zero).normalized;

        Vector3 finalPosition = pos + (dir * offSet);

        _crabManager.WalkToTarget(0, finalPosition);
        #endregion

        yield return _crabManager.ReturnWalkCoroutine();

        CrabArenaManager.Instance.ChangeCurrentTide();

        while (CrabArenaManager.Instance.ReturnCurrentTide() < CrabArenaState.HighTide) yield return null;

        #region WalkToFront
        _crabManager.transform.position = new(_crabManager.transform.position.x, highTideHeight, _crabManager.transform.position.z);

        pos.y = highTideHeight;

        _crabManager.WalkToTarget(0, pos);

        yield return _crabManager.ReturnWalkCoroutine();

        Vector3 zeroDir = Vector3.zero - _crabManager.transform.position;

        zeroDir.y = 0;

        zeroDir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(zeroDir, Vector3.up);

        yield return _crabManager.transform.DORotateQuaternion(targetRotation, 0.2f).WaitForCompletion();

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());

        #endregion
    }

    void InstantiateHitBox(SkillAnimationEvent prefab) { }
    void InstantiateVFX(SkillAnimationEvent prefab)
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.VFX);
        hitbox.transform.position = prefab.PreFabPosition;

        hitbox.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }

    public override IEnumerator CooldownBetweenAttacksRoutine()
    {
        enemyBehaviourManager.DesactivateChannel(Channel);
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        foreach (var channel in listOfNextAttacksChannels)
        {
            _crabManager.OpenChannel(channel);
            _crabManager.ChangeBehaviourAtRandom(channel);
        }
    }
}
