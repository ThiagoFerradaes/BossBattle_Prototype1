using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviourManager : MonoBehaviour
{
    [SerializeField] EnemyBehaviourSO initialState;
    [SerializeField] ListOfEnemyBehaviourSO listOfBehaviour;

    Dictionary<int, EnemyBehaviourSO> _dictionaryOfBehaviours = new();
    EnemyBehaviourSO _currentBehaviour;
    List<EnemyBehaviourSO> _actualListOfBehaviours = new();
    [HideInInspector] public EnemyCooldownManager CooldownManager;

    public virtual IEnumerator Start()
    {
        foreach (var behaviour in listOfBehaviour.ListOfEnemyBehaviours)
        {
            EnemyBehaviourSO behaviourClone = Instantiate(behaviour);
            _actualListOfBehaviours.Add(behaviourClone);
        }

        CooldownManager = EnemyCooldownManager.Instance;
        CooldownManager.Initiate(_actualListOfBehaviours);

        _dictionaryOfBehaviours[0] = Instantiate(initialState);
        _dictionaryOfBehaviours[0].StartState(this);

        yield return null;
    }

    public void ChangeBehaviourAtRandom(int behaviourChannel = 0)
    {
        EnemyBehaviourSO behaviour = ChooseAnAttack(behaviourChannel);

        if (_dictionaryOfBehaviours.ContainsKey(behaviourChannel)) _dictionaryOfBehaviours[behaviourChannel].ExitState();

        _dictionaryOfBehaviours[behaviourChannel] = behaviour;
        _dictionaryOfBehaviours[behaviourChannel].StartState(this);

    }

    EnemyBehaviourSO ChooseAnAttack(int behaviourChannel)
    {
        var validSkills = _actualListOfBehaviours
            .Where(skill => skill.Channel == behaviourChannel)
            .Where(skill => !CooldownManager.SkillInCooldown(skill) && skill.MeetsCondition())
            .OrderByDescending(skill => skill.Priority);

        return validSkills.FirstOrDefault();
    }
}
