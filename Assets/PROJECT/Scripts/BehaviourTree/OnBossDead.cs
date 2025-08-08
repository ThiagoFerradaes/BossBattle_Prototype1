using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnBossDead")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnBossDead", message: "Boss Dead", category: "Events", id: "d05c108f69c8dae4935acb780c109427")]
public sealed partial class OnBossDead : EventChannel { }

