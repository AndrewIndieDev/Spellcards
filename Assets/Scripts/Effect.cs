using UnityEngine;

public class Effect : ScriptableObject
{
    public AudioClip sound;
    public int damage;
    [TextArea]
    public int description;

    public virtual void OnActivate() { }
}