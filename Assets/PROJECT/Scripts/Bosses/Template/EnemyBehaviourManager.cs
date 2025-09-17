using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviourManager : MonoBehaviour {
    [SerializeField] EnemyBehaviourSO initialState;
    [SerializeField] ListOfEnemyBehaviourSO listOfBehaviour;

    EnemyBehaviourSO _currentBehaviour;
    List<EnemyBehaviourSO> _actualListOfBehaviours = new();
    [HideInInspector] public EnemyCooldownManager CooldownManager;

    bool _hasStarted;

    public virtual IEnumerator Start() {
        try {

            foreach (var behaviour in listOfBehaviour.ListOfEnemyBehaviours) {
                EnemyBehaviourSO behaviourClone = Instantiate(behaviour);
                _actualListOfBehaviours.Add(behaviourClone);
            }
        }
        catch { Debug.LogWarning("No listOfBehaviours"); }

        try {
            CooldownManager = EnemyCooldownManager.Instance;
            CooldownManager.Initiate(_actualListOfBehaviours);

        }
        catch { Debug.LogWarning("No CooldownManager"); }

        _currentBehaviour = Instantiate(initialState);
        _currentBehaviour.StartState(this);

        yield return null;

        _hasStarted = true;
    }

    public virtual void Update() {
        if (!_hasStarted) return;

        try {
            _currentBehaviour.UpdateState();
        }
        catch { Debug.LogWarning("No _currentBehaviour"); }
    }

    public void ChangeBehaviourAtRandom() {
        EnemyBehaviourSO behaviour = ChooseAnAttack();
        _currentBehaviour.ExitState();
        _currentBehaviour = behaviour;
        _currentBehaviour.StartState(this);

    }

    EnemyBehaviourSO ChooseAnAttack() {
        var sortedSkills = _actualListOfBehaviours.OrderByDescending(skill => skill.Priority);

        foreach (var skill in sortedSkills) {
            if (!CooldownManager.SkillInCooldown(skill)) {
                return skill;
            }
        }

        return null;
    }
}
