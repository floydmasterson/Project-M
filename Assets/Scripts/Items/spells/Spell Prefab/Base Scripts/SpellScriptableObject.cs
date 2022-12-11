using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Spells")]
public class SpellScriptableObject : ScriptableObject
{
    public int dmgAmt = 10;
    public float ManaCost = 5f;
    public float lifeTime = 2f;
    public float speed = 15f;
    public float spellRadius = .5f;

}
