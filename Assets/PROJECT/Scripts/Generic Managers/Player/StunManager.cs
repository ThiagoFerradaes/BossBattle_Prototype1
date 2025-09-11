using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StunManager : MonoBehaviour
{
    // Events
    public event Action<bool> OnStun;

    // Components
    Animator anim;

    // Booleans
    bool _isStunned;

    // Animation
    [Header("Animation")]
    [SerializeField] string stunParamenter;

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// stun value equals if character is going to be stunned
    /// </summary>
    /// <param name="stun"></param>
    public void StunCharacter(bool stun) {
        _isStunned = stun;

        OnStun?.Invoke(_isStunned);

        anim.SetBool(stunParamenter, _isStunned);
    }

    private void Update() {
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            StunCharacter(!_isStunned);
        }
    }
}
