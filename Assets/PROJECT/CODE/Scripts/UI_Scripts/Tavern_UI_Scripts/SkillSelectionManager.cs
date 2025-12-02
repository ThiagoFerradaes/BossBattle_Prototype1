using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionManager : MonoBehaviour {

    [Header("Components")]
    [SerializeField] GameObject SkillSelectionScreen;
    [SerializeField] TextMeshProUGUI SkillName;
    [SerializeField] TextMeshProUGUI SkillLongDescription;
    [SerializedDictionary("Slot", "Conexion"), SerializeField]
    SerializedDictionary<SkillSlot, Image> dictionaryOfConexions = new();
    [SerializedDictionary("Slot", "Button"), SerializeField]
    SerializedDictionary<SkillSlot, Button> dictionaryOfTypeOfIndexers = new();

    public void Initialize(SkillSlot slotInitialized) {

    }

}
