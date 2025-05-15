using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;

public class EnemyAttackHitBox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    [SerializeField] private float windupDuration = 0.5f; // duration of wind-up before hitbox activation
    [SerializeField] private float hitboxDuration = 0.2f; // duration of hitbox active time
    public TextMeshProUGUI warningText;
    public EnemyAttackType attackType;

    BoxCollider2D col;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    public void TriggerHitbox()
    {
        StartCoroutine(WindupDelay(windupDuration, hitboxDuration));
    }

    IEnumerator WindupDelay(float windupDelay, float activeDuration)
    {
        if (warningText != null)
            warningText.text = AttackWarning(attackType);
        yield return new WaitForSeconds(windupDelay); // wind-up phase
        if (warningText != null)
            warningText.text = ""; 

        col.enabled = false;
        yield return null;
        col.enabled = true;

        yield return new WaitForSeconds(activeDuration);
        col.enabled = false;
    }

    string AttackWarning(EnemyAttackType type)
    {
        switch (type)
        {
            case EnemyAttackType.Normal: return "Normal Attack Incoming!";
            case EnemyAttackType.Heavy: return "Heavy Attack! Prepare to Dodge!";
            case EnemyAttackType.ParrySpecial: return "Parry Opportunity!";
            default: return "Unknown Attack!";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                PlayerAction action = player.GetCurrentAction();
                bool success = false;

                switch (attackType)
                {
                    case EnemyAttackType.Normal:
                        success = action == PlayerAction.Blocking || action == PlayerAction.Dodging;
                        break;
                    case EnemyAttackType.Heavy:
                        success = action == PlayerAction.Dodging;
                        break;
                    case EnemyAttackType.ParrySpecial:
                        success = player.IsParryInWindow();
                        break;
                }

                Debug.Log($"[HIT] Player action at {Time.time}: {player.GetCurrentAction()} (Started at: {player.actionStartTime})");
                
                if (success)
                {
                    Debug.Log("Player successfully defended against: " + attackType);
                    if (attackType == EnemyAttackType.ParrySpecial)
                    {
                        // StartCoroutine(player.ParrySlowMo(0.3f, 0.3f));
                        StartCoroutine(player.FreezeFrame(0.3f));
                        StartCoroutine(player.ZoomPunch(Camera.main, 0.3f));
                        GameManager.Instance.RegisterParrySuccess();
                    }
                }
                else
                {
                    Debug.Log("Player failed to defend: " + attackType);
                    GameManager.Instance.RegisterHit();
                    StartCoroutine(player.FlashColor(Color.red, 0.1f));
                    Camera.main.transform.DOShakePosition(0.1f, 0.3f);
                }
            }
        }
    }

    void OnDisable() => col.enabled = false;
}
