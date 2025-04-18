using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemySO", order = 1)]
public class EnemyData : ScriptableObject
{
    public string id;
    [Header("Stats")] [SerializeField] private float health;
    [Header("Enemy State Type")] [SerializeField] private EnemyType enemyStateType;
    [Header("Player Detection Parameters")]
    [SerializeField] private float angleOfRange;
    [SerializeField] private float distanceOfRange;
    [SerializeField] private float innerSphereRadius;

    [Header("Attack Strategy")]
    //[SerializeField] private GameObject attackPrefab;
    [SerializeField] private int attackRangeInCell;
    [SerializeField] private int retreatActivateDistance;
    [SerializeField] private int retreatDistance;
    public float Health => health;
    public float AngleOfRange => angleOfRange;
    public float DistanceOfRange => distanceOfRange;
    public float InnerSphereRadius => innerSphereRadius;
    //public AttackType AttackStrategy => attackType;
    //public GameObject AttackPrefab => attackPrefab;

    public int AttackRangeInCell => attackRangeInCell;
    public int RetreatDistance => retreatDistance;
    public int RetreatActivateDistance => retreatActivateDistance;
    public AbilityData AbilityData;

    public enum EnemyType
    {
        Berserk,
        Sniper,
        Boomer,
        Assassin,
        Healer,
        Boss,
        Other
    }

    public EnemyType EnemyStateType => enemyStateType;
}