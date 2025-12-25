using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsManager : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public string name;

        [SerializeField] private int _xp;
        public int level;

        public int XP
        {
            get => _xp;
            set
            {
                _xp = value;

                bool leveledUp = false;

                while (level < RequiredExp.Count && _xp >= RequiredExp[level])
                {
                    _xp -= RequiredExp[level];
                    level++;
                    leveledUp = true;
                }

                OnXPChanged?.Invoke(this, leveledUp);
            }
        }

        public static readonly IReadOnlyList<int> RequiredExp = new List<int>
        {
            100, 380, 770, 1300, 2150,
            3300, 4800, 6900, 10000, 15000
        };

        public event Action<Skill, bool> OnXPChanged;
    }

    [HideInInspector] public List<Skill> skills = new List<Skill>();

    public static SkillsManager instance;

    [Header("Sliders")]
    public Slider farmingSlider;
    public Slider miningSlider;
    public Slider combatSlider;

    [Header("Level Texts")]
    public TextMeshProUGUI farmingLvl;
    public TextMeshProUGUI miningLvl;
    public TextMeshProUGUI combatLvl;

    [Header("Notifications")]
    public GameObject needSleepNotification;

    [HideInInspector] public bool isSkillUpgraded;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        skills = new List<Skill>
        {
            new Skill { name = "Farming", XP = 0 },
            new Skill { name = "Mining", XP = 0 },
            new Skill { name = "Combat", XP = 0 }
        };

        foreach (var skill in skills)
        {
            skill.OnXPChanged += Skill_OnXPChanged;
        }

        UpdateVisuals();
    }

    private void Skill_OnXPChanged(Skill skill, bool leveledUp)
    {
        UpdateVisuals();
        if (leveledUp)
        {
            needSleepNotification.SetActive(true);
            isSkillUpgraded = true;
            Skill_OnLevelUp(skills.IndexOf(skill), skills[skills.IndexOf(skill)].level);
        }
    }

    private void Skill_OnLevelUp(int skillId, int lvl)
    {

        if (skillId == 0)
        {
            switch (lvl)
            {
                case 2:
                    Debug.Log("[Farming] <Sprinklers> are open");
                    break;
                case 4:
                    Debug.Log("[Farming] <Cheese Press> are open");
                    break;
                case 6:
                    Debug.Log("[Farming] <Quality Sprinkler> are open");
                    break;
                case 7:
                    Debug.Log("[Farming] <Bee House> are open");
                    break;
                case 8:
                    Debug.Log("[Farming] <Oil Maker> and <Keg> are open");
                    break;
                case 9:
                    Debug.Log("[Farming] <Iridium Sprinkler> are open");
                    break;
            }
        }
    }

    private void UpdateVisuals()
    {
        if (skills.Count < 3) return;

        UpdateSkillUI(skills[0], farmingSlider, farmingLvl);
        UpdateSkillUI(skills[1], miningSlider, miningLvl);
        UpdateSkillUI(skills[2], combatSlider, combatLvl);
    }

    private void UpdateSkillUI(Skill skill, Slider slider, TextMeshProUGUI levelText)
    {
        levelText.text = $"{skill.name} ({skill.level})";

        int nextLevelIndex = Mathf.Min(skill.level, Skill.RequiredExp.Count - 1);
        slider.maxValue = Skill.RequiredExp[nextLevelIndex];
        slider.value = skill.XP;
    }

    public void AddXP(string skillName, int amount)
    {
        var skill = skills.Find(s => s.name == skillName);
        if (skill != null)
        {
            skill.XP += amount;
        }
    }

    public void OnPlayerSlept()
    {
        needSleepNotification.SetActive(false);
    }

    [ContextMenu("TEST Add Farming EXP")]
    public void TEST_AddFarmingExp()
    {
        skills[0].XP += 70;
    }
}
