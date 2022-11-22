using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Spell : MonoBehaviour
{
  public SpellScriptableObject spellToCast;
    private SphereCollider sphereCollider;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = spellToCast.spellRadius;

        Destroy(this.gameObject, spellToCast.lifeTime);
    }

    private void Update()
    {
        if (spellToCast.speed > 0) transform.Translate(Vector3.forward * spellToCast.speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        int Obstruction = LayerMask.NameToLayer("Obstruction");
     
        if (other.gameObject.layer == Obstruction)
        {
            Destroy(this.gameObject);
        }
        if (other.CompareTag("enemy"))
        {
            other.GetComponent<Enemys>().TakeDamge(spellToCast.dmgAmt + (Mathf.RoundToInt(Character.Instance.Intelligence.Value / Mathf.Pow(2f, (other.GetComponent<Enemys>().Defense / Character.Instance.Intelligence.Value)))));
            if(!other.GetComponent<Enemys>().isDead)
            other.GetComponent<Enemys>().Target = PlayerUi.Instance.target.transform;
            Destroy(this.gameObject);
        }
    }

}
