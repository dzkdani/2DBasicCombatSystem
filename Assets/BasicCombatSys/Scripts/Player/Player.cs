using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerAction
{
    Idle,
    Attacking,
    Dodging,
    Blocking,
    Parrying
}

public class Player : MonoBehaviour
{
    [Header("Player Components")]
    public ActionData[] playerActions; // array of player actions
    private SpriteRenderer sr;
    private Coroutine currentActionRoutine = null; 

    [Header("Player Settings")]
    private float parryTimer = 0f;
    [SerializeField] private float parryWindow = 0.4f;
    [SerializeField] private PlayerAction currentAction = PlayerAction.Idle;
    [SerializeField] private float actionDuration = 0.4f; // duration of action before auto-resetting to Idle
    public float actionStartTime;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = playerActions.FirstOrDefault(x => x.name == "Idle").actionData.sprite; // set default sprite
    }

    void Update()
    {
        if (parryTimer > 0f)
            parryTimer -= Time.deltaTime;
        
        GetInput();

        // Timer to auto-reset action back to Idle after duration per-action
        if (currentAction != PlayerAction.Idle && Time.time - actionStartTime > actionDuration)
        {
            sr.sprite = playerActions.FirstOrDefault(x => x.name == "Idle").actionData.sprite; // reset to idle sprite
            currentAction = PlayerAction.Idle;
        }
    }

#region Control
    void GetInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            ProcessInput("Parry"); // parry action
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ProcessInput("Block"); // block action
        } 
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessInput("Dodge"); // dodge action
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            ProcessInput("Attack"); // attack action
        }
    }

    public bool IsParryInWindow() => currentAction == PlayerAction.Parrying && parryTimer > 0f;

    void ProcessInput(string currentActionName)
    {
        TriggerAction(playerActions.FirstOrDefault(x => x.name == currentActionName).actionData); // Trigger action
        actionStartTime = Time.time; 
    }

    void TriggerAction(SpriteData data)
    {
        Debug.Log($"[INPUT] Player started action: {data.actionType} at {Time.time}");

        if (currentActionRoutine != null)
            StopCoroutine(currentActionRoutine); //make sure coroutine didnt overkap

        currentActionRoutine = StartCoroutine(DoAction(data.actionType, data.sprite, data.duration));
    }   

    private IEnumerator DoAction(PlayerAction action, Sprite sprite, float duration)
    {
        currentAction = action;
        sr.sprite = sprite;

        if (action == PlayerAction.Parrying)
            parryTimer = parryWindow;

        yield return new WaitForSeconds(duration);
    }
#endregion

#region VFX
    public IEnumerator FlashColor(Color flashColor, float duration)
    {
        Color original = sr.color;
        sr.color = flashColor;
        yield return new WaitForSeconds(duration);
        sr.color = original;
    }
    
    public IEnumerator ParrySlowMo(float duration, float slowdownFactor)
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = 0.02f * slowdownFactor; // keeps physics in sync
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    public IEnumerator FreezeFrame(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    public IEnumerator ZoomPunch(Camera cam, float duration)
    {
        float originalSize = cam.orthographicSize;
        cam.orthographicSize = originalSize * 0.5f;
        yield return new WaitForSeconds(duration);
        cam.orthographicSize = originalSize;
    }
#endregion

    public PlayerAction GetCurrentAction() => currentAction;
}
