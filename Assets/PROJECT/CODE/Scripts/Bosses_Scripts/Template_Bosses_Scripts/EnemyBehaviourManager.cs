using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviourManager : MonoBehaviour
{
    #region Parameters

    [Header("Behaviours")]
    [SerializeField] EnemyBehaviourSO initialState;
    [SerializeField] ListOfEnemyBehaviourSO listOfBehaviour;

    Dictionary<int, EnemyBehaviourSO> _dictionaryOfBehaviours = new();
    List<EnemyBehaviourSO> _actualListOfBehaviours = new();
    [HideInInspector] public EnemyCooldownManager CooldownManager;
    Dictionary<int, bool> _activeChannels = new();
    Dictionary<int, bool> _openChannels = new();

    #endregion

    #region Initialize
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
    #endregion

    #region Change Behaviour
    public void ChangeBehaviourAtRandom(int behaviourChannel = 0)
    {
        if (!_openChannels.ContainsKey(behaviourChannel) || !_openChannels[behaviourChannel]) return;

        EnemyBehaviourSO behaviour = ChooseAnAttack(behaviourChannel);

        if (_dictionaryOfBehaviours.ContainsKey(behaviourChannel)) _dictionaryOfBehaviours[behaviourChannel].ExitState();

        if (behaviour != null) {
            _dictionaryOfBehaviours[behaviourChannel] = behaviour;
            _dictionaryOfBehaviours[behaviourChannel].StartState(this);
            ActivateChannel(behaviourChannel);
        }
    }

    EnemyBehaviourSO ChooseAnAttack(int behaviourChannel) {
        var validSkills = _actualListOfBehaviours
            .Where(skill => skill.Channel == behaviourChannel)
            .Where(skill => !CooldownManager.SkillInCooldown(skill))
            .Where(skill => skill.MeetsCondition())
            .Where(skill => skill.MeetsCondition(this))
            .OrderByDescending(skill => skill.Priority);

        return validSkills.FirstOrDefault();
    }
    #endregion

    #region Channel Region
    public void ActivateChannel(int channel) => _activeChannels[channel] = true;
    public void DesactivateChannel(int channel) {
        if (!_activeChannels.ContainsKey(channel)) return;

        _activeChannels[channel] = false;
    }
    public void OpenChannel(int channel) => _openChannels[channel] = true;

    public void CloseChannel(int channel) {
        if (!_openChannels.ContainsKey(channel)) return;

        _openChannels[channel] = false;
    }

    public Dictionary<int, bool> ReturnActiveChannels() => _activeChannels;

    public bool ReturnIfChannelIsOpen(int channel)
    {
        if (!_openChannels.ContainsKey(channel)) return false;

        return _openChannels[channel];
    }
    #endregion

}
