using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CooldownManager
{
    private static float[] cooldownTimers = new float[10];
    private static IEnumerator applyPotionCooldown(Image cooldownImage, TextMeshProUGUI cooldownText, float cooldownTime, MonoBehaviour caller, int threadToUse)
    {
        cooldownTimers[threadToUse] -= Time.deltaTime;
        if (cooldownTimers[threadToUse] < 0f)
        {
            if (cooldownText != null)
                cooldownText.gameObject.SetActive(false);
            if (cooldownImage != null)
                cooldownImage.gameObject.SetActive(false);
            cooldownImage.fillAmount = 0f;
            cooldownTimers[threadToUse] = 0;
        }
        else
        {
            if (cooldownText != null)
                cooldownText.text = Mathf.RoundToInt(cooldownTimers[threadToUse]).ToString();
            if (cooldownImage != null)
                cooldownImage.fillAmount = cooldownTimers[threadToUse] / cooldownTime;
            yield return new WaitForEndOfFrame();
            caller.StartCoroutine(applyPotionCooldown(cooldownImage, cooldownText, cooldownTime, caller, threadToUse));
        }
    }
    public static void CooldownGFX(Image cooldownImage, TextMeshProUGUI cooldownText, float cooldownTime, MonoBehaviour caller)
    {
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(true);
        if (cooldownImage != null)
            cooldownImage.gameObject.SetActive(true);
        caller.StartCoroutine(applyPotionCooldown(cooldownImage, cooldownText, cooldownTime, caller, ThreadCheck(cooldownTime)));
    }
    private static int ThreadCheck(float time)
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] == 0)
            {
                cooldownTimers[i] = time;
                return i;
            }
        }
        Debug.LogError("Too many cooldown timers are running");
        return 0;

    }
}
