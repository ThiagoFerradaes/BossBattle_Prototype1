using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour {

    #region Parameter

    [SerializeField] StatusSO baseStatus;
    Dictionary<StatusType, float> _listOfStatus = new();

    #endregion

    #region Initialize
    private void Awake() {
        PopulateDictionary();
    }

    void PopulateDictionary() {
        foreach (var status in baseStatus.StatusList) {
            _listOfStatus[status.Type] = status.Value;
        }
    }
    #endregion

    #region Getter
    public float ReturnStatusValue(StatusType type) {
        return _listOfStatus.TryGetValue(type, out float value) ? value : 0f;
    }

    #endregion

    #region Change Status Value
    /// <summary>
    /// The percent value has to be between 0 and 1
    /// </summary>
    /// <param name="type"></param>
    /// <param name="percent"></param>
    /// <param name="increase"></param>
    public void ChangeStatus(StatusType type, float percent, bool increase) {
        if (!_listOfStatus.ContainsKey(type)) return;

        percent = Mathf.Abs(percent);

        if (increase) _listOfStatus[type] *= (1 + percent);
        else _listOfStatus[type] /= (1 + percent);

        _listOfStatus[type] = Mathf.Max(0.01f, _listOfStatus[type]);
    }
    public void ChangeStatus(StatusType type, float percent, bool increase, float duration) {
        StartCoroutine(ChangeValueRoutine(type, percent, increase, duration));
    }
    IEnumerator ChangeValueRoutine(StatusType type, float percent, bool increase, float duration) {

        ChangeStatus(type, percent, increase);

        yield return new WaitForSeconds(duration);

        ChangeStatus(type, percent, !increase);
    }

    #endregion
}
