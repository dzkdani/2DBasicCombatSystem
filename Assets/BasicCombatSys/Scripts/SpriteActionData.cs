using UnityEngine;

[System.Serializable]
public struct SpriteData
{
    public Sprite sprite;
    public float duration;
    public PlayerAction actionType;
}

[System.Serializable]
public struct ActionData
{
    public string name;
    public SpriteData actionData;
}