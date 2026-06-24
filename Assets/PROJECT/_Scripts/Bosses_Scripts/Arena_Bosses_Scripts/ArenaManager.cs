using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ArenaManager : MonoBehaviour
{
    #region Parameters

    public static ArenaManager Instance;

    // Atributos da arena
    [Header("Arena Atributes")]
    [SerializeField] TypeOfArena typeOfArena;
    [ShowIf("typeOfArena", TypeOfArena.Ring), SerializeField] float ringInnerRadius;
    [SerializeField] Vector3 centerOfArena;
    [SerializeField] Vector2 arenaSize;
    List<PathWay> paths = new();

    #endregion

    #region Initialize

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    #endregion

    #region Get Random Position

    /// <summary>
    /// Retorna uma posição aleatória válida dentro da arena.
    /// objectRadius garante que a hitbox caiba inteira dentro.
    /// </summary>
    public Vector3 GetRandomPosition(float objectRadius = 0f) {
        return typeOfArena switch {
            TypeOfArena.Square => GetRandomRectPosition(objectRadius),
            TypeOfArena.Ring => GetRandomRingPosition(objectRadius),
            TypeOfArena.Paths => GetRandomPointInPath(),
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
        float outerRadius = arenaSize.x;                // usando arenaSize.x como raio externo
        float ringWidth = outerRadius - ringInnerRadius;

        // Se o objeto não cabe em nenhuma posição do anel, colocamos no "meio da parede"
        if (objectRadius * 2f > ringWidth) {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float mid = (ringInnerRadius + outerRadius) / 2f;

            Vector2 pos = dir * mid;
            return centerOfArena + new Vector3(pos.x, 0, pos.y);
        }

        // Caso normal sorteia dentro do anel
        Vector2 dirNormal = Random.insideUnitCircle.normalized;
        float min = ringInnerRadius + objectRadius;
        float max = outerRadius - objectRadius;

        float dist = Mathf.Sqrt(Random.Range(min * min, max * max));
        Vector2 posNormal = dirNormal * dist;

        return centerOfArena + new Vector3(posNormal.x, 0, posNormal.y);
    }

    private Vector3 GetRandomPointInPath() {

        // Escolhendo um ponto aleatório do caminho
        int randomPath = Random.Range(0, paths.Count);
        List<Transform> pathPoints = paths[randomPath].pathPoint;

        // Declarando variáveis
        float totalLenght = 0f;
        int amountOfSegmentes = pathPoints.Count - 1;
        float[] segments = new float[amountOfSegmentes];

        // Definindo o tamanho total do caminho
        for (int i = 0; i < amountOfSegmentes; i++) {
            float dis = Vector3.Distance(pathPoints[i].position, pathPoints[i + 1].position);
            segments[i] = dis;
            totalLenght += dis;
        }

        // Escolhendo um ponto aleatório ao longo da distancia
        float randomPoint = Random.Range(0, totalLenght);

        // Econtrando qual segmento o ponto pertence
        for (int i = 0; i < segments.Length; i++) {

            if (randomPoint < segments[i]) {
                float percent = randomPoint / segments[i];
                return Vector3.Lerp(pathPoints[i].position, pathPoints[i + 1].position, percent);
            }

            randomPoint -= segments[i];
        }

        return centerOfArena;
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

    public float FindGroundHeight(Vector3 originalPosition) {
        Vector3 startPos = new(originalPosition.x, 1000f, originalPosition.z);

        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 1000f, LayerMask.GetMask("Floor"))){
            return hit.point.y; 
        }

        return 0;
    }

    #endregion

    #region Setter

    public void SetTypeOfArena(TypeOfArena type) => typeOfArena = type; 

    public void SetPathPoints(List<PathWay> paths) => this.paths = paths;

    #endregion
}
