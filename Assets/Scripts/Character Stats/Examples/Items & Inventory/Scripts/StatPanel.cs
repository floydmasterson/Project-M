﻿using UnityEngine;
using TMPro;

namespace Kryz.CharacterStats.Examples
{
	public class StatPanel : MonoBehaviour
	{
		[SerializeField] StatDisplay[] statDisplays;
		[SerializeField] string[] statNames;
		[SerializeField] TextMeshProUGUI defenseText;

        private CharacterStat[] stats;

		private void OnValidate()
		{
			statDisplays = GetComponentsInChildren<StatDisplay>();
			UpdateStatNames();
		}

		public void SetStats(params CharacterStat[] charStats)
		{
			stats = charStats;

			if (stats.Length > statDisplays.Length)
			{
				Debug.LogError("Not Enough Stat Displays!");
				return;
			}

			for (int i = 0; i < statDisplays.Length; i++)
			{
				statDisplays[i].Stat = i < stats.Length ? stats[i] : null;
				statDisplays[i].gameObject.SetActive(i < stats.Length);
			}
		}

		public void UpdateStatValues()
		{
			for (int i = 0; i < stats.Length; i++)
			{
				statDisplays[i].ValueText.text = stats[i].Value.ToString("0.0");
			}
			PlayerUi.Instance.target.CheckMaxHealth();
            PlayerUi.Instance.target.CheckDefense();
			defenseText.text = PlayerUi.Instance.target.Defense.ToString();

        }

		public void UpdateStatNames()
		{
			for (int i = 0; i < statNames.Length; i++)
			{
				statDisplays[i].NameText.text = statNames[i];
			}
		}
	}
}
