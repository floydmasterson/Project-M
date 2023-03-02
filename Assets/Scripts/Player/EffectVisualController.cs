using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class EffectVisualController : MonoBehaviourPun
{
    [SerializeField]
 private List<GameObject> effect = new List<GameObject>();
    [SerializeField, HideIf("@EffectParent != null")] private Transform EffectParent;
    public bool posioned = false;
    public bool slowed = false;
    public bool burning = false;
    public enum Effects
    {
        Invincible,
        Poisoned,
        Slowed,
        Mana_Pulse,
        Rage_Aura,
        Burned,
    }
    private void OnValidate()
    {
        if (EffectParent != null)
        {
            effect.Clear();
            for (int i = 0; i < EffectParent.childCount; i++)
            {
                effect.Add(EffectParent.GetChild(i).gameObject);
            }
        }
    }
    public void EnableEffect(Effects effectNum)
    {
        photonView.RPC("EnableEffectRPC", RpcTarget.All, (int)effectNum);
    }
    public void DisableEffect(Effects effectNum)
    {
        photonView.RPC("DisableEffectRPC", RpcTarget.All, (int)effectNum);
    }

    [PunRPC]
    public void EnableEffectRPC(Effects effectNum)
    {
        effect[(int)effectNum].SetActive(true);
    }

    [PunRPC]
    public void DisableEffectRPC(Effects effectNum)
    {
        effect[(int)effectNum].SetActive(false);
    }


}
