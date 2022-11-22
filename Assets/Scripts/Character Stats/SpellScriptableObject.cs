using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Spells")]
public class SpellScriptableObject : ScriptableObject
{
    public float dmgAmt = 10f;
    public float ManaCost = 5f;
    public float lifeTime = 2f;
    public float speed = 15f;
    public float spellRadius = .5f;

}
