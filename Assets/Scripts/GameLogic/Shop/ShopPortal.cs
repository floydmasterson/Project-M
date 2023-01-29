using Photon.Pun;
using Sirenix.OdinInspector;
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
    private IEnumerator OpenPortal(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        OpenShopPortal();
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
            Destroy(gameObject, GameManger.Instance.gameTime);
        }
    }

    private void OpenShopPortal()
    {
        col.enabled = true;
        GFX.SetActive(true);
        portalDrone.PlaySFX();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
            Destroy(gameObject);
        }
        else if (!toShop)
        {
            PlayerManger manger = player.GetComponent<PlayerManger>();
            if (manger != null)
            {
                manger.pvp = true;
                manger.inShop = false;
            }
            int selctedSpawn = Random.Range(0, respawnPoints.Count);
            respawnPoints[selctedSpawn].Selcted(player);
            photonView.RPC("RemoveSpawn", RpcTarget.All, selctedSpawn);
        }
    }
    [PunRPC]
    public void RemoveSpawn(int spawnIndex)
    {
        respawnPoints.Remove(respawnPoints[spawnIndex]);
    }

}
