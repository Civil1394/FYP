using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemySO", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Player Detection Parameters")]
    [SerializeField] private float angleOfRange;
    [SerializeField] private float distanceOfRange;
    [SerializeField] private float innerSphereRadius;

    [Header("Attack Strategy")]
    [SerializeField] private AttackType attackType;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private float dashSpeed;

    public float AngleOfRange => angleOfRange;
    public float DistanceOfRange => distanceOfRange;
    public float InnerSphereRadius => innerSphereRadius;
    public AttackType AttackStrategy => attackType;
    public GameObject AttackPrefab => attackPrefab;
    public float DashSpeed => dashSpeed;
    public enum AttackType
    {
        DirectionTargetting,
        GroundTargetting,
        Dash
    }
}