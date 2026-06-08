using UnityEngine;

public class Enums { }

public enum TypesOfAudio {
    Global,
    Music,
    SFX,
    Ambient
}

public enum ConfigurationScreen {
    Gameplay,
    Graphics,
    Audio,
    Language,
    Tutorial
}

public enum InputType {
    MouseKeyboard,
    Gamepad
}

public enum ProgressBools {
    IsKrakenDefeated, HasTalkedToLilianBGFDemo, LilianTwo, TalkedToBastian, TalkedToCyrus, AskedAboutLilian, TalkedtoLilianORBastian,
    hasTalkedToBertrand
}

public enum Bosses { Kraken, Crab, Thalassia, Voodoo, Birds, Ecdna }

public enum RotationType { MouseRotation, MoveRotation }

public enum SkillSlot {
    BaseAttack = 0,
    SkillOne = 1,
    SkillTwo = 2,
    Ultimate = 3,
    Dash = 4,
    Passive = 5
}

public enum Character { Cyrus, Bastian, Lilian, Gracia, Julian, Betrand, Carmen, Null }

public enum TypeOfArena { Ring, Square, Paths }

public enum CrabArenaState { LowTide, IncomingTide, HighTide, OutgoingTide }

public enum CrabArenaWall { Up, Left, Right, None }

public enum BastianHeatArea { CoolArea = 0, HeatArea = 1, OverHeatArea = 2 }

public enum BattleRank { E, D, C, B, A, S, SS }

public enum GraciaTypeOfSkill { Left, Right };
public enum GraciaAura { Blue, Yellow, Red, Green, Null };

public enum StatusType {
    MaxHealth, MaxAmountOfShield, BaseAttack, Defense, MoveSpeed, AttackSpeed, EnergyRecharge
}

public enum TypeOfSkillPrefab { Hitbox, VFX, PreCastRange, Manager }
public enum TypeOfCollider { Instant, Continuos, Projectile, Boomerang }
public enum ExtraDamageContextAtributes { Penetration }

public enum TypeOfDialogueSpritePosition { Left, Right }
public enum ExpressionTypeDialogue {
    Angry_S, Angry_N, Anxious_S, Anxious_N, Curious_S, Curious_N,
    Default_S, Default_N, Intense_S, Intense_N, Laughing_S, Laughing_N, Satisfied_S, Satisfied_N, Surprise_S, Surprised_N, Vulnerable_S,
    Vulnerable_N, Special_1, Special_2, Special_3
}
