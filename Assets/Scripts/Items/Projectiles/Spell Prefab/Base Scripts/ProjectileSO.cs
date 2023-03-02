
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Projectile")]
public class ProjectileSO : ScriptableObject
{
    public int dmgAmt = 10;
    public float ManaCost = 5f;
    public float lifeTime = 2f;
    public float speed = 15f;
    public float spellRadius = .5f;

}
