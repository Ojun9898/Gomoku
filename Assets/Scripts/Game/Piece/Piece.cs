using UnityEngine;

public class Piece : HaveHp
{
    public enum PieceType
    {
        WARRIOR,
        RANCER,
        MAGICIAN,
        HEALER,
        ARCHER
    }
    
    public enum AttackType {
        NONE,
        CHOOSE_ATTACK,
        RANGE_ATTACK,
        BUFF
    }
    
    public PieceType pieceType;
    public AttackType attackType;
    public bool isAlreadyAttack;
    public int[] rangeAttackRange;
    public int cost;
    public AudioSource audioSource;
    
    [SerializeField] private int attackPower=1;
    [SerializeField] private int attackRange = 1;
    public Animator animator;
    
    private SPUM_Prefabs _prefab;

    private void Start()
    {
        _prefab = GetComponent<SPUM_Prefabs>();
        animator = _prefab._anim;
    }

    public enum Owner
    {
        NONE,
        PLAYER_A,
        PLAYER_B
    }
    public Owner pieceOwner;

    public Owner GetPieceOwner()
    {
        return pieceOwner;
    }

    public void SetPieceOwner(Owner pieceOwners)
    {
        this.pieceOwner = pieceOwners;
    }


    public int GetAttackRange()
    {
        return attackRange;
    }
    public void SetAttackRange(int attackRanges)
    {
        this.attackRange = attackRanges;
    }


    public int GetAttackPower()
    {
        return attackPower;
    }
    public void SetAttackPower(int attackPowers)
    {
        this.attackPower = attackPowers;
    }






    public void ChoseAttack(Piece piece,int attackPowers) { 
        piece.Hp -= attackPowers;
        isAlreadyAttack = true;
    }
    public void Buff(Piece piece, int attackPowers) {
        piece.Hp += attackPowers;
        isAlreadyAttack = true;
    }
    public void ChoseAttack(Obstacle oc, int attackPowers)
    {
        oc.Hp -= attackPowers;
        isAlreadyAttack = true;
    }


}
