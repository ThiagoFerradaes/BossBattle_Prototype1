using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour {

    #region Parameter

    [SerializeField] StatusSO baseStatus;
    [SerializeField] SerializedDictionary<StatusType, float> _listOfBaseStatus = new();
    [SerializeField] SerializedDictionary<StatusType, float> _listOfStatusMultiplicator = new();

    #endregion

    #region Initialize
    private void Awake() {
        PopulateDictionary();
    }

    void PopulateDictionary() {
        foreach (var status in baseStatus.StatusList) {
            _listOfBaseStatus[status.Type] = status.Value;
        }
        foreach (var status in baseStatus.StatusList) {
            _listOfStatusMultiplicator[status.Type] = 1;
        }
    }
    #endregion

    #region Getter
    public float ReturnStatusValue(StatusType type) {
        float baseStatus = _listOfBaseStatus.TryGetValue(type, out float value) ? value : 0f;
        float statusMultiplicator = _listOfStatusMultiplicator.TryGetValue(type, out float multiplicator) ? multiplicator : 0f;
        float finalStatus = baseStatus * statusMultiplicator;
        return finalStatus;
    }

    #endregion

    #region Change Status Multiplier Value
    /// <summary>
    /// The percent value has to be between 0 and 1
    /// </summary>
    /// <param name="type"></param>
    /// <param name="percent"></param>
    /// <param name="increase"></param>
    public void ChangeStatusMultiplier(StatusType type, float percent, bool increase) {
        if (!_listOfStatusMultiplicator.ContainsKey(type)) return;

        percent = Mathf.Abs(percent);

        if (increase) _listOfStatusMultiplicator[type] *= (1 + percent);
        else _listOfStatusMultiplicator[type] /= (1 + percent);
        _listOfStatusMultiplicator[type] = Mathf.Max(0.01f, _listOfStatusMultiplicator[type]);

    }
    public void ChangeStatusMultiplier(StatusType type, float percent, bool increase, float duration) {
        StartCoroutine(ChangeMultiplierValueRoutine(type, percent, increase, duration));
    }
    IEnumerator ChangeMultiplierValueRoutine(StatusType type, float percent, bool increase, float duration) {

        ChangeStatusMultiplier(type, percent, increase);

        yield return new WaitForSeconds(duration);

        ChangeStatusMultiplier(type, percent, !increase);
    }


    #endregion

    #region Change Base Status Value

    public void SetBaseStatus(StatusType type, float newValue) {
        if (!_listOfBaseStatus.ContainsKey(type)) return;

        _listOfBaseStatus[type] = newValue;
    }

    #endregion
}
