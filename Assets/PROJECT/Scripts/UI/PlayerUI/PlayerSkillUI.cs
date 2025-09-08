using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerSkillUI : MonoBehaviour {

    #region Parameters

    [Header("Skill Image")]
    [SerializeField] private Image dashSkillImage;
    [SerializeField] private Image skillOneImage;
    [SerializeField] private Image skillTwoImage;
    [SerializeField] private Image ultimateImage;

    [Header("Cooldown Image")]
    [SerializeField] private Image dashCooldown;
    [SerializeField] private Image skillOneCooldown;
    [SerializeField] private Image skillTwoCooldown;
    [SerializeField] private Image ultimateEnergy;

    Action<float, float> _energyGainAction;

    private Dictionary<SkillSlot, Coroutine> cooldownCoroutines;
    private Dictionary<SkillSlot, Image> cooldownImages;

    #endregion

    #region Methods
    private void Start() {

        StartDictionary();
        SetSkillsImage();
        SetCooldownImagesOff();
        SubscribeEvents();
    }

    void StartDictionary() {
        cooldownCoroutines = new Dictionary<SkillSlot, Coroutine>();
        cooldownImages = new Dictionary<SkillSlot, Image>
        {
            { SkillSlot.Dash, dashCooldown },
            { SkillSlot.SkillOne, skillOneCooldown },
            { SkillSlot.SkillTwo, skillTwoCooldown },
            { SkillSlot.Ultimate, ultimateEnergy }
        };
    }
    private void SubscribeEvents() {
        PlayerSkillCooldownManager.OnCooldownSet -= StartCooldownUI;
        PlayerSkillCooldownManager.OnCooldownSet += StartCooldownUI;

        _energyGainAction = (currentEnergy, maxEnergy) => UpdateUltimateEnergyCost(currentEnergy, maxEnergy);

        EnergyManager.OnEnergyValueChanged -= _energyGainAction;
        EnergyManager.OnEnergyValueChanged += _energyGainAction;
    }

    private void StartCooldownUI(SkillSlot slot, float cooldown) {
        if (cooldownCoroutines.TryGetValue(slot, out Coroutine currentRoutine) && currentRoutine != null) {
            StopCoroutine(currentRoutine);
        }

        Coroutine newRoutine = StartCoroutine(CooldownRoutine(slot, cooldown));
        cooldownCoroutines[slot] = newRoutine;
    }

    void UpdateUltimateEnergyCost(float currentEnergy, float maxEnergy) {
        cooldownImages[SkillSlot.Ultimate].fillAmount = 1 - (currentEnergy / maxEnergy);
    }

    private IEnumerator CooldownRoutine(SkillSlot slot, float cooldown) {
        Image cooldownImage = cooldownImages[slot];
        float timer = cooldown;
        cooldownImage.fillAmount = 1f;

        while (timer > 0) {
            timer -= Time.deltaTime;
            cooldownImage.fillAmount = timer / cooldown;
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        cooldownCoroutines[slot] = null;
    }

    private void SetSkillsImage() {
        Debug.Log("Skills have image");
    }

    private void SetCooldownImagesOff() {
        foreach (var image in cooldownImages.Values) {
            image.fillAmount = 0f;
        }

        cooldownImages[SkillSlot.Ultimate].fillAmount = 1;
    }

    private void OnDisable() {
        PlayerSkillCooldownManager.OnCooldownSet -= StartCooldownUI;
        EnergyManager.OnEnergyValueChanged -= _energyGainAction;
    }

    #endregion
}

