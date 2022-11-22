using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] private int value;
    private InventoryUi inventory;
   
    private void OnTriggerEnter(Collider other)
    {
        
        //if (other.CompareTag("Player"))
        //{
        //    InventoryUi.Instance.gotCoin(value);
        //    Destroy(gameObject);

        //}
    }
}
