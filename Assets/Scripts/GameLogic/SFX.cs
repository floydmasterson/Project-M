using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SFX
{
    [LabelText("SFX Type")]
    [LabelWidth(100)]
    [OnValueChanged("SFXChange")]
    [InlineButton("StopSFX")]
    [InlineButton("PlaySFX")]
    public SFXManager.SFXType sfxType = SFXManager.SFXType.Ambient;

    [LabelText("$sfxLabel")]
    [LabelWidth(100)]   
    [ValueDropdown("SFXType")]
    [OnValueChanged("SFXChange")]
    [InlineButton("SelectSFX")]
    public SFXClip sfxToPlay;
    private string sfxLabel = "SFX";

#pragma warning disable 0414
    [SerializeField]
    private bool showSettings = true;

    [SerializeField]
    private bool editSettings = true;
#pragma warning restore 0414

    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private SFXClip _sfxBase;

    [Title("Audio Source")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool waitToPlay = false;

    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool useDefualt = false;

    [DisableIf("useDefualt")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private AudioSource audioSource;

    private void SFXChange()
    {
        sfxLabel = sfxType.ToString() + " SFX";

        _sfxBase = sfxToPlay;
    }

    private void SelectSFX()
    {
       // UnityEditor.Selection.activeObject = sfxToPlay;
    }
    private List<SFXClip> SFXType()
    {
        List<SFXClip> sfxList;

        switch (sfxType)
        {

            case SFXManager.SFXType.Ambient:
                sfxList = SFXManager.instance.ambientSFX;
                break;
            case SFXManager.SFXType.Ui:
                sfxList = SFXManager.instance.uiSFX;
                break;
            case SFXManager.SFXType.Player:
                sfxList = SFXManager.instance.playerSFX;
                break;
            case SFXManager.SFXType.Weapon:
                sfxList = SFXManager.instance.WeaponSFX;
                break;
            case SFXManager.SFXType.Enemy:
                sfxList = SFXManager.instance.EnemySFX;
                break;
            default:
                sfxList = SFXManager.instance.ambientSFX;
                break;
        }
        return sfxList;
    }

    public void PlaySFX()
    {
        if (useDefualt || audioSource == null)
            SFXManager.PlaySFX(sfxToPlay, waitToPlay, null);
        else
            SFXManager.PlaySFX(sfxToPlay, waitToPlay, audioSource);
    }

    public void StopSFX()
    {
        if (useDefualt || audioSource == null)
            SFXManager.StopSFX( null);
        else
            SFXManager.StopSFX(audioSource);
    }
}
