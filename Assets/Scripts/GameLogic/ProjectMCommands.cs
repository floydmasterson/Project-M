﻿using Cinemachine;
using Kryz.CharacterStats;
using Photon.Pun;
using SmartConsole;
using System;
using UnityEngine;

public class ProjectMCommands : CommandBehaviour
{
    [SerializeField] PlayerManger localPlayer;
    [SerializeField] GameManger gameManger;
    [SerializeField] PhotonView photonView;
    [SerializeField] TextAsset database;
    const string MISSING_PLAYER = "Local Player is not set. Try setup first";
    #region SetUp

    protected override void Start()
    {
        base.Start();
        photonView = gameObject.GetComponent<PhotonView>();

    }
    private void Update()
    {
        if (localPlayer == null && PlayerUi.Instance != null && PlayerUi.Instance.target != null)
        {
            setup();
        }
    }
    [Command]
    private void setup()
    {
        localPlayer = PlayerUi.Instance.target;
        gameManger = GameManger.Instance;
        if (localPlayer != null && gameManger != null)
            Debug.Log("Setup has succeeded");
        else if (localPlayer == null || gameManger == null)
            Debug.LogError("Setup failed: Localplayer = " + localPlayer + " Game Manager = " + gameManger);
    }
    #endregion
    #region Commands
    #region Defulat Commands

    [Command]
    public void main_menu()
    {
        PhotonNetwork.Disconnect();
        Loader.Load(Loader.Scene.Lobby);
    }
    [Command]
    public void help()
    {
        for (int i = 0; i < Command.List.Count; i++)
        {
            Debug.Log(Command.List[i].MethodInfo.Name);
        }
    }
    #endregion
    #region LocalPlayerCommands
    [Command]
    private void kill()
    {
        if (localPlayer != null)
            localPlayer.TakeDamge(999);
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void damage_player(int damage)
    {
        if (localPlayer != null)
            localPlayer.TakeDamge(damage);
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void max_heal()
    {
        if (localPlayer != null)
            localPlayer.CurrentHealth = localPlayer.MaxHealth;
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [SerializeField] Item godMode;
    [Command]
    private void god_mode(bool state)
    {
        if (localPlayer != null)
        {
            if (state == true)
            {
                localPlayer.DefenseMod += 100000;
                localPlayer.character.Strength.AddModifier((new StatModifier(1000, StatModType.Flat, godMode)));
                localPlayer.character.Intelligence.AddModifier((new StatModifier(1000, StatModType.Flat, godMode)));
                localPlayer.CheckDefense();
            }
            else if (state == false)
            {

                localPlayer.DefenseMod -= 100000;
                localPlayer.character.Strength.RemoveAllModifiersFromSource(godMode);
                localPlayer.character.Intelligence.RemoveAllModifiersFromSource(godMode);
                localPlayer.CheckDefense();
            }
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void set_spawn_to_self()
    {
        if (localPlayer != null)
        {
            Transform newSpawn = localPlayer.transform;
            localPlayer.spawnPoint = newSpawn;
            Debug.Log("Spawn has been set to current location");
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }

    [Command]
    private void tp_to(int POIid)
    {
        if (localPlayer != null)
        {
            switch (POIid)
            {
                case 0:
                    localPlayer.transform.position = new Vector3(-147, 0, 180);
                    Debug.Log("TP to Debug Shop");
                    break;
                case 1:
                    localPlayer.transform.position = new Vector3(25, 3, -45);
                    Debug.Log("TP to Sector 1 spawn");
                    break;
                case 2:
                    localPlayer.transform.position = new Vector3(286, 3, -744);
                    Debug.Log("TP to Sector 3 spawn");
                    break;
                case 3:
                    localPlayer.transform.position = new Vector3(302, 3, -1095);
                    Debug.Log("TP to Sector 5 spawn");
                    break;
                case 4:
                    localPlayer.transform.position = new Vector3(1753, -107, -592);
                    Debug.Log("TP to Shop Spawn");
                    break;
                case 5:
                    localPlayer.transform.position = new Vector3(662, 3, -422);
                    Debug.Log("TP to Sector 4 = spawn");
                    break;

                default:
                    Debug.LogWarning("Unknown or unassiged ID");
                    break;
            }
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void tp(float xCord, float yCord, float zCord)
    {
        if (localPlayer != null)
        {
            localPlayer.transform.position = new Vector3(xCord, yCord, zCord);
        }
        else
            Debug.LogWarning("Local Player is not set. Try setup");
    }
    [Command]
    private void give(int itemId)
    {
        if (localPlayer != null)
        {
            Character character = InventoryUi.Instance.gameObject.GetComponent<Character>();
            if (character != null)
            {
                character.Inventory.AddItem(DataBaseCache.instance.GetItem(itemId));
                Debug.Log("Added " + DataBaseCache.instance.GetItem(itemId).ItemName + " to inventory");
            }
            else
                throw new NullReferenceException("Character is missing on local player");
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void no_clip()
    {
        if (localPlayer != null)
        {
            if (localPlayer.noClip == false)
            {
                localPlayer.gameObject.layer = 31;
                localPlayer.noClip = true;
                localPlayer.lockCamera.GetComponent<CinemachineCollider>().enabled = false;
                Debug.Log("No clip On");

            }
            else
            {
                localPlayer.gameObject.layer = 9;
                localPlayer.noClip = false;
                localPlayer.lockCamera.GetComponent<CinemachineCollider>().enabled = true;
                Debug.Log("No clip Off");
            }
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }

    [Command]
    private void sens()
    {
        if (localPlayer != null)
            Debug.Log(localPlayer.turnSens);
        else
            Debug.LogWarning(MISSING_PLAYER);
    }


    [Command]
    private void give_gold(int amount)
    {
        if (localPlayer != null)
        {
            InventoryUi player = localPlayer.character.GetComponent<InventoryUi>();
            player.UpdateGold(amount);
            Debug.Log("Added " + amount + " gold.");
        }
        else
            Debug.LogWarning(MISSING_PLAYER);

    }


    #endregion
    #region Spawn Things
    [Command]
    private void spawn_chest(string chesttier)
    {
        if (localPlayer != null)
        {
            Vector3 spawnPoint = new Vector3(0, 0, 5) + localPlayer.transform.position;
            PhotonNetwork.Instantiate("T" + chesttier + " Chest", spawnPoint, Quaternion.Euler(-90, 0, 0));
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void spawn_dropbag(string bagtier)
    {
        if (localPlayer != null)
        {
            Vector3 spawnPoint = new Vector3(0, 0, 5) + localPlayer.transform.position;
            PhotonNetwork.Instantiate("T" + bagtier + " DropPouch", spawnPoint, Quaternion.identity);
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    [Command]
    private void spawn_enemy(string enemyname)
    {
        if (localPlayer != null)
        {
            Vector3 spawnPoint = new Vector3(0, 0, 5) + localPlayer.transform.position;
            PhotonNetwork.Instantiate("E-" + enemyname, spawnPoint, Quaternion.identity);
        }
        else
            Debug.LogWarning(MISSING_PLAYER);
    }
    #endregion
    #region Game Manger
    [Command]
    private void set_gametime(float timeinseconds)
    {
        if (gameManger != null)
        {
            if (PhotonNetwork.IsMasterClient)
                gameManger.SetGameTime(timeinseconds);
            else
                Debug.LogError("You must be master client to use this command");
        }
        else
            Debug.LogWarning("Game Manager is not set. Try setup");
    }



    #endregion
    #region Misc
    [Command]
    private void noah()
    {
        Debug.Log("Noah is a homosexual");
    }
    [Command]
    private void trevor()
    {
        Debug.Log("Trevor is a homosexual pagan");
    }
    [Command]
    private void bing_qi_lin()
    {
        Debug.LogError("Zǎoshang hǎo zhōngguó xiànzài wǒ yǒu BING CHILLING  wǒ hěn xǐhuān BING CHILLING  dànshì sùdù yǔ jīqíng 9 bǐ BING CHILLING  sùdù yǔ jīqíng sùdù yǔ jīqíng 9 wǒ zuì xǐhuān suǒyǐ…xiànzài shì yīnyuè shíjiān zhǔnbèi 1 2 3 liǎng gè lǐbài yǐhòu sùdù yǔ jīqíng 9 ×3 bùyào wàngjì bùyào cu òguò jìdé qù diànyǐngyuàn kàn sùdù yǔ jīqíng 9 yīn wéi fēicháng hǎo diànyǐng dòngzuò fēicháng hǎo chàbùduō yīyàng BING CHILLING zàijiàn ");
    }
    #endregion
    #region Lists 
    [Command]
    private void tp_id_list()
    {
        Debug.Log("0:Debug Shop");
        Debug.Log("1:Sector 1 spawn");
        Debug.Log("2:Sector 3 spawn");
        Debug.Log("3:Sector 5 spawn");
        Debug.Log("4:Shop Spawn");
        Debug.Log("3:Sector 4 spawn");

    }
    [Command]
    private void item_id_list()
    {
        Debug.Log(database);
    }
    #endregion
    #endregion
}
