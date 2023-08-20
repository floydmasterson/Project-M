using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPortal : MonoBehaviourPun
{
    [TabGroup("Setup"), SerializeField]
    private bool toShop = true;
    [TabGroup("Setup"), SerializeField]
    SFX portalDrone;
    [TabGroup("Setup"), SerializeField, HideIf("@toShop")]
    List<RespawnPoint> respawnPoints = new List<RespawnPoint>();
    Vector3 shopSpawn;
    Collider col;
    GameObject GFX;

    public Action FirstOut;
    public bool firstHasPassed;

    private IEnumerator OpenPortal(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        StartCoroutine(OpenShopPortal());
    }

    private void Start()
    {
        shopSpawn = new Vector3(1753, -107, -592);
        col = gameObject.GetComponent<CapsuleCollider>();
        col.enabled = false;
        GFX = gameObject.transform.GetChild(0).gameObject;
        GFX.SetActive(false);
        if (!toShop)
            StartCoroutine(OpenPortal(GameManger.Instance.gameTime + 60));
        else if (toShop)
        {
            StartCoroutine(OpenPortal(GameManger.Instance.gameTime / 3));
            FirstOut += () => Destroy(gameObject);
        }

    }

    public IEnumerator OpenShopPortal()
    {
        GFX.SetActive(true);
        portalDrone.PlaySFX();
        yield return new WaitForSeconds(2.5f);
        col.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            Teleport(other.gameObject);
        }
    }
    void Teleport(GameObject player)
    {
        if (toShop)
        {
            PlayerManger manger = player.GetComponent<PlayerManger>();
            if (manger != null)
                manger.inShop = true;
            player.transform.position = shopSpawn;
        }
        else if (!toShop)
        {
            PlayerManger manger = player.GetComponent<PlayerManger>();
            if (manger != null)
            {
                manger.enemyLayers |= LayerMask.GetMask("enenmyMask") | LayerMask.GetMask("target");
                manger.pvp = true;
                manger.inShop = false;
            }
            int selctedSpawn = UnityEngine.Random.Range(0, respawnPoints.Count);
            respawnPoints[selctedSpawn].Selcted(player);
            photonView.RPC("RemoveSpawn", RpcTarget.AllBuffered, selctedSpawn);
            if (!firstHasPassed)
                FirstOut?.Invoke();
        }
        Destroy(gameObject);
    }
    [PunRPC]
    public void RemoveSpawn(int spawnIndex)
    {
        respawnPoints.Remove(respawnPoints[spawnIndex]);
    }

}
