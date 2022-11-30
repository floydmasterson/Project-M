using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectVisualController : MonoBehaviourPun
{
    public GameObject[] Effects = new GameObject[2];
    public bool posioned = false;
    public void EnableEffect(int number)
    {
        photonView.RPC("EnableEffectRPC", RpcTarget.All, number);
    }
    public void DisableEffect(int number)
    {
        photonView.RPC("DisableEffectRPC", RpcTarget.All, number);
    }

    [PunRPC]
    public void EnableEffectRPC(int number)
    {
        Effects[number].SetActive(true);
    }

    [PunRPC]
    public void DisableEffectRPC(int number)
    {
        Effects[number].SetActive(false);
    }

}
