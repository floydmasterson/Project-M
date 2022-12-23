using Kryz.CharacterStats;
using Photon.Pun;
using SmartConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectMCommands : CommandBehaviour
{
    [SerializeField] PlayerManger localPlayer;
    [SerializeField] PhotonView photonView;
    #region SetUp
    private void OnEnable()
    {
        PlayerUi.targetSet += SetLocalPlayer;
    }
    private void OnDisable()
    {
        PlayerUi.targetSet -= SetLocalPlayer;
    }
    protected override void Start()
    {
        base.Start();
    }
    [Command]
    private void SetLocalPlayer()
    {
        localPlayer = PlayerUi.Instance.target;
        if (localPlayer != null)
            Debug.Log("Local Player Set");
        else if (localPlayer == null)
            Debug.LogError("Failed to set Local Player");

    }
    #endregion
    #region Commands
    #region Defulat Commands

    [Command]
    public void load_scene(int index)
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(index);
    }
    [Command]
    public void main_menu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
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
            localPlayer.TakeDamge(999, null);
        else
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
    }
    [Command]
    private void damage_player(int damage)
    {
        if (localPlayer != null)
            localPlayer.TakeDamge(damage, null);
        else
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
    }
    [Command]
    private void max_heal()
    {
        if (localPlayer != null)
            localPlayer.CurrentHealth = localPlayer.MaxHealth;
        else
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
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
                Character.Instance.Strength.AddModifier((new StatModifier(1000, StatModType.Flat, godMode)));
                Character.Instance.Intelligence.AddModifier((new StatModifier(1000, StatModType.Flat, godMode)));
                localPlayer.CheckDefense();
            }
            else if (state == false)
            {

                localPlayer.DefenseMod -= 100000;
                Character.Instance.Strength.RemoveAllModifiersFromSource(godMode);
                Character.Instance.Intelligence.RemoveAllModifiersFromSource(godMode);
                localPlayer.CheckDefense();
            }
        }
        else
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
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
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
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
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
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
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
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
            Debug.LogWarning("Local Player is not set. Try SetLocalPLayer");
    }
    #endregion
    #region Game Manger
    [Command]
    private void set_gametime(float timeinseconds)
    {
        if (GameManger.Instance != null)
        {
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("SetGameTimeRPC", RpcTarget.All, timeinseconds);
            else
                Debug.LogError("You must be master client to use this command");
        }
        else
            Debug.LogWarning("Game Manager is not set.");
    }

    [PunRPC]
    public void SetGameTimeRPC(float time)
    {
        GameManger.Instance.GameTimeLeft = time;
        if (time > 0)
            GameManger.Instance.timerOn = true;
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
    #endregion
}
