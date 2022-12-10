using Photon.Pun;
using SmartConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectMCommands : CommandBehaviour
{
    PlayerManger localPlayer;
    GameManger GameManger;
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
        GameObject gm = GameObject.FindGameObjectWithTag("GameManger");
        GameManger = gm.GetComponent<GameManger>();
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
            Debug.Log("Local Player does not exist yet");
    }
    [Command]
    private void max_heal()
    {
        if (localPlayer != null)
            localPlayer.CurrentHealth = localPlayer.MaxHealth;
        else
            Debug.Log("Local Player does not exist yet");
    }
    [Command]
    private void god_mode(bool state)
    {
        if (localPlayer != null)
        {
            if (state == true)
            {
                localPlayer.DefenseMod += 100000;
                localPlayer.CheckDefense();
                Character.Instance.statPanel.UpdateStatValues();
            }
            else if (state == false)
            {

                localPlayer.DefenseMod -= 100000;
                localPlayer.CheckDefense();
                Character.Instance.statPanel.UpdateStatValues();
            }
        }
        else
            Debug.Log("Local Player does not exist yet");
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
            Debug.Log("Local Player does not exist yet");
    }
    #endregion
    #region Spawn Things
    [Command]
    private void spawn_chest(string chestlevel)
    {
        if (localPlayer != null)
        {
            Vector3 spawnPoint = new Vector3(0, 0, 5) + localPlayer.transform.position;
            PhotonNetwork.Instantiate("Lv" + chestlevel + " Chest", spawnPoint, Quaternion.Euler(-90, 0, 0));
        }
        else
            Debug.Log("Local Player does not exist yet");
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
            Debug.Log("Local Player does not exist yet");
    }
    #endregion
    #region Game Manger
    [Command]
    private void set_gametime(float timeinseconds)
    {
        if (GameManger != null)
        {
            GameManger.GameTimeLeft = timeinseconds;
            if (timeinseconds > 0)
                GameManger.timerOn = true;
        }
        else
            Debug.Log("Game Manger does not exist yet");
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
        Debug.Log("Zǎoshang hǎo zhōngguó xiànzài wǒ yǒu BING CHILLING  wǒ hěn xǐhuān BING CHILLING  dànshì sùdù yǔ jīqíng 9 bǐ BING CHILLING  sùdù yǔ jīqíng sùdù yǔ jīqíng 9 wǒ zuì xǐhuān suǒyǐ…xiànzài shì yīnyuè shíjiān zhǔnbèi 1 2 3 liǎng gè lǐbài yǐhòu sùdù yǔ jīqíng 9 ×3 bùyào wàngjì bùyào cu òguò jìdé qù diànyǐngyuàn kàn sùdù yǔ jīqíng 9 yīn wéi fēicháng hǎo diànyǐng dòngzuò fēicháng hǎo chàbùduō yīyàng BING CHILLING zàijiàn ");
    }
    #endregion
    #endregion
}
