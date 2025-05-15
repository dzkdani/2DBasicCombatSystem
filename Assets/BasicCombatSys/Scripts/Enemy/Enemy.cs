using System.Collections;
using UnityEngine;
using System.Linq;

public enum EnemyAttackType
{
    Normal,
    Heavy,
    ParrySpecial
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public ActionData[] enemyActions;
    private SpriteRenderer sr;
    [SerializeField] private float attackHintDuration; 
    [SerializeField] private float attackCooldown;
    private float timer;
    private EnemyAttackType currentAttack;
    [SerializeField] private GameObject attackHitBox;

    [Header("Enemy Attack Settings")]
    [SerializeField] private Color HeavyAttackColor = Color.red;
    [SerializeField] private Color NormalAttackColor = Color.white;
    [SerializeField] private Color ParryAttackColor = Color.yellow;
    [SerializeField] private Color IdleColor = new Color(0.4f, 0.0f, 0.5f);
    private int attackSinceLastParry = 0;
    private int attacksBeforeParrySpecial; 

    private void Start()
    {
        timer = attackCooldown;

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = enemyActions.FirstOrDefault(x => x.name == "Idle").actionData.sprite; 
        
        attackHitBox = this.transform.GetChild(0).gameObject; 
        attackHitBox.GetComponent<BoxCollider2D>().enabled = false;
    
        attacksBeforeParrySpecial = Random.Range(2, 5); // 2–4 random normal/heavy attacks
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SetAttack();
            timer = attackCooldown;
        }
    }

    void SetAttack()
    {
        if (attackSinceLastParry >= attacksBeforeParrySpecial)
        {
            currentAttack = EnemyAttackType.ParrySpecial;
            attackSinceLastParry = 0;
            attacksBeforeParrySpecial = Random.Range(2, 5); // reset the threshold
        }
        else
        {
            currentAttack = (EnemyAttackType)Random.Range(0, 2); // 0 or 1 -> Normal or Heavy
            attackSinceLastParry++;
        }
        StartCoroutine(DoAttack(currentAttack));
    }

    IEnumerator DoAttack(EnemyAttackType type)
    {
        attackHitBox.GetComponent<EnemyAttackHitBox>().attackType = type;
        attackHitBox.GetComponent<EnemyAttackHitBox>().TriggerHitbox(); 
        // Color hint
        switch (type)
        {
            case EnemyAttackType.Normal: sr.color = NormalAttackColor; break;
            case EnemyAttackType.Heavy: sr.color = HeavyAttackColor; break;
            case EnemyAttackType.ParrySpecial: sr.color = ParryAttackColor; break;
        }
        yield return new WaitForSeconds(attackHintDuration); // hint duration

        sr.sprite = enemyActions.FirstOrDefault(x => x.name == "Attack").actionData.sprite; // set attack sprite
        yield return new WaitForSeconds(enemyActions.FirstOrDefault(x => x.name == "Attack").actionData.duration); // wait for attack animation to finish
        sr.sprite = enemyActions.FirstOrDefault(x => x.name == "Idle").actionData.sprite; // set idle sprite
        sr.color = new Color(0.4f, 0.0f, 0.5f);
    }

    public EnemyAttackType GetCurrentAttackType()
    {
        return currentAttack;
    }
}
