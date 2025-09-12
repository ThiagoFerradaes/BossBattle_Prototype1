using NaughtyAttributes;
using Unity.AppUI.UI;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    #region Parameters

    public static ArenaManager Instance;

    enum TypeOfArena { Ring, Square}

    // Atributos da arena
    [Header("Arena Atributes")]
    [SerializeField] TypeOfArena typeOfArena;
    [ShowIf("typeOfArena", TypeOfArena.Ring), SerializeField] float ringInnerRadius;
    [SerializeField] Vector3 centerOfArena;
    [SerializeField] Vector2 arenaSize;

    #endregion

    #region Initialize

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    #endregion

    #region Get Position

    /// <summary>
    /// Retorna uma posição aleatória válida dentro da arena.
    /// objectRadius garante que a hitbox caiba inteira dentro.
    /// </summary>
    public Vector3 GetRandomPosition(float objectRadius = 0f) {
        return typeOfArena switch {
            TypeOfArena.Square => GetRandomRectPosition(objectRadius),
            TypeOfArena.Ring => GetRandomRingPosition(objectRadius),
            _ => centerOfArena,
        };
    }

    private Vector3 GetRandomRectPosition(float objectRadius) {
        float halfX = arenaSize.x / 2f - objectRadius;
        float halfZ = arenaSize.y / 2f - objectRadius;

        float x = Random.Range(-halfX, halfX);
        float z = Random.Range(-halfZ, halfZ);

        return centerOfArena + new Vector3(x, 0, z);
    }

    private Vector3 GetRandomRingPosition(float objectRadius) {
        Vector2 dir = Random.insideUnitCircle.normalized;

        // garante que o objeto inteiro caiba dentro do anel
        float min = ringInnerRadius + objectRadius;
        float max = arenaSize.x - objectRadius;

        float dist = Mathf.Sqrt(Random.Range(min * min, max * max));
        Vector2 pos = dir * dist;

        return centerOfArena + new Vector3(pos.x, 0, pos.y);
    }

    public bool IsPointInsideArena(Vector3 point, float objectRadius = 0f) {
        switch (typeOfArena) {
            case TypeOfArena.Square:
                Vector3 local = point - centerOfArena;
                return Mathf.Abs(local.x) + objectRadius <= arenaSize.x / 2f &&
                       Mathf.Abs(local.z) + objectRadius <= arenaSize.y / 2f;
            case TypeOfArena.Ring:
                float dist = Vector3.Distance(centerOfArena, point);
                return dist - objectRadius >= ringInnerRadius &&
                       dist + objectRadius <= arenaSize.x;
            default:
                return false;
        }
    }
    #endregion
}
