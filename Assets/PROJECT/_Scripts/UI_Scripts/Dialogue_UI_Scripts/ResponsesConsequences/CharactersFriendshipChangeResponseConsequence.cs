using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Response Consequences/Characters Friendship Change")]
public class CharactersFriendshipChangeResponseConsequence : ResponseConsequence {

    [SerializedDictionary("Character", "Amount Of Friendship"), SerializeField]
    SerializedDictionary<Character, float> charactersFriendshipChanges = new();

    public override void ExecuteConsequence() {
        foreach (var pair in charactersFriendshipChanges) {
            Debug.Log($"{pair.Key} added: {pair.Value} friendship points");
        }
    }
}
