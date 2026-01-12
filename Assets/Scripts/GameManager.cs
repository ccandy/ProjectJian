using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("玩家状态")]
    public PlayerStats stats = new PlayerStats();

    [Header("事件来源（优先用 ScriptableObject；想用 JSON 就勾 useJson）")]
    public bool useJson = false;
    public EventDatabaseSO database;   // SO 事件库
    public TextAsset eventsJson;       // JSON 文本（可选）

    [Header("UI 引用")]
    public Text dayText;
    public Text statsText;
    public Text descriptionText;
    public Button[] choiceButtons;

    private System.Random _random = new System.Random();
    private bool _isGameOver = false;

    // ——Runtime 事件结构：用于 JSON / 统一处理——
    [Serializable]
    private class RuntimeEvent
    {
        public string id;
        public MoodText description;

        public int minDay, maxDay, minMoney, maxMoney, minDepression, maxDepression;
        public int weight;

        public List<RuntimeChoice> choices;

        public bool IsAvailable(PlayerStats s)
        {
            if (s.day < minDay || s.day > maxDay) return false;
            if (s.money < minMoney || s.money > maxMoney) return false;
            if (s.depression < minDepression || s.depression > maxDepression) return false;
            return true;
        }
    }

    [Serializable]
    private class RuntimeChoice
    {
        public string label;
        public MoodText resultText;
        public StatDelta delta;
    }

    private List<RuntimeEvent> _events = new List<RuntimeEvent>();
    private RuntimeEvent _currentEvent;

    private void Start()
    {
        LoadEvents();
        UpdateStatsUI();
        ShowRandomEvent();
    }

    private void LoadEvents()
    {
        _events.Clear();

        if (useJson)
        {
            LoadFromJson();
        }
        else
        {
            LoadFromScriptableObjects();
        }

        if (_events.Count == 0)
        {
            Debug.LogError("事件库为空：请检查 EventDatabaseSO 或 JSON 是否配置正确。");
        }
    }

    private void LoadFromScriptableObjects()
    {
        if (database == null || database.events == null) return;

        foreach (var e in database.events)
        {
            if (e == null) continue;

            var re = new RuntimeEvent
            {
                id = e.id,
                description = e.description,

                minDay = e.minDay,
                maxDay = e.maxDay,
                minMoney = e.minMoney,
                maxMoney = e.maxMoney,
                minDepression = e.minDepression,
                maxDepression = e.maxDepression,
                weight = Mathf.Max(1, e.weight),

                choices = new List<RuntimeChoice>()
            };

            foreach (var c in e.choices)
            {
                re.choices.Add(new RuntimeChoice
                {
                    label = c.label,
                    resultText = c.resultText,
                    delta = c.delta
                });
            }

            _events.Add(re);
        }
    }

    private void LoadFromJson()
    {
        if (eventsJson == null) return;

        var db = JsonUtility.FromJson<JsonEventDatabase>(eventsJson.text);
        if (db == null || db.events == null) return;

        foreach (var e in db.events)
        {
            var re = new RuntimeEvent
            {
                id = e.id,
                description = e.description.ToMoodText(),

                minDay = e.minDay,
                maxDay = e.maxDay,
                minMoney = e.minMoney,
                maxMoney = e.maxMoney,
                minDepression = e.minDepression,
                maxDepression = e.maxDepression,
                weight = Mathf.Max(1, e.weight),

                choices = new List<RuntimeChoice>()
            };

            if (e.choices != null)
            {
                foreach (var c in e.choices)
                {
                    re.choices.Add(new RuntimeChoice
                    {
                        label = c.label,
                        resultText = c.resultText.ToMoodText(),
                        delta = c.delta
                    });
                }
            }

            _events.Add(re);
        }
    }

    private void UpdateStatsUI()
    {
        if (dayText != null) dayText.text = $"Day {stats.day}";

        if (statsText != null)
        {
            statsText.text =
                $"精力:{stats.energy}  健康:{stats.health}  家庭:{stats.family}  金钱:{stats.money}  抑郁度:{stats.depression}";
        }
    }

    private void ShowRandomEvent()
    {
        if (_isGameOver) return;

        if (IsGameOver())
        {
            ShowGameOver();
            return;
        }

        var candidates = new List<RuntimeEvent>();
        int totalWeight = 0;

        foreach (var e in _events)
        {
            if (e == null) continue;
            if (!e.IsAvailable(stats)) continue;

            int w = GetMoodAdjustedWeight(e.weight);
            if (w <= 0) continue;

            candidates.Add(e);
            totalWeight += w;
        }

        if (candidates.Count == 0)
        {
            descriptionText.text = "今天什么事都没发生。\n这反而更可怕。";
            return;
        }

        int roll = _random.Next(totalWeight);
        int acc = 0;
        RuntimeEvent chosen = candidates[0];

        foreach (var e in candidates)
        {
            acc += GetMoodAdjustedWeight(e.weight);
            if (roll < acc)
            {
                chosen = e;
                break;
            }
        }

        _currentEvent = chosen;

        if (descriptionText != null)
            descriptionText.text = _currentEvent.description.Resolve(stats.depression);

        SetupChoiceButtons();
    }

    private int GetMoodAdjustedWeight(int baseWeight)
    {
        // 抑郁度越高，事件触发越“密”（更像生活追着你打）
        // 这里是轻微放大，避免变成一秒崩盘
        float k = 1f + stats.depression * 0.005f; // 0..+50%
        return Mathf.Max(1, Mathf.RoundToInt(baseWeight * k));
    }

    private void SetupChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            var btn = choiceButtons[i];
            if (btn == null) continue;

            if (i < _currentEvent.choices.Count)
            {
                var choice = _currentEvent.choices[i];
                btn.gameObject.SetActive(true);

                var t = btn.GetComponentInChildren<Text>();
                if (t != null) t.text = choice.label;

                btn.onClick.RemoveAllListeners();
                int captured = i;
                btn.onClick.AddListener(() => OnChoiceSelected(captured));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    private void OnChoiceSelected(int index)
    {
        if (_isGameOver) return;

        var choice = _currentEvent.choices[index];

        // 新的一天
        stats.day++;

        // 先做“每日抑郁漂移”（生活本身就会把人往下拽）
        ApplyDailyDepressionDrift();

        // 应用选项影响（抑郁度会让同样的行动更难）
        var d = ApplyDepressionToDelta(choice.delta);
        ApplyDelta(d);

        // 文本随抑郁度变化
        if (descriptionText != null)
            descriptionText.text = choice.resultText.Resolve(stats.depression);

        UpdateStatsUI();
        StartCoroutine(NextEventRoutine());
    }

    private void ApplyDailyDepressionDrift()
    {
        int drift = 0;

        // 状态差 → 抑郁度上升
        if (stats.energy < 30) drift += 2;
        if (stats.health < 40) drift += 2;
        if (stats.family < 40) drift += 2;
        if (stats.money < 0) drift += 2;

        // 状态好 → 抑郁度缓慢下降（很慢，符合现实）
        if (stats.energy > 70) drift -= 1;
        if (stats.health > 70) drift -= 1;
        if (stats.family > 70) drift -= 1;

        stats.depression = Mathf.Clamp(stats.depression + drift, 0, 100);
    }

    private StatDelta ApplyDepressionToDelta(StatDelta src)
    {
        // 规则：越抑郁，负向精力/健康消耗放大；正向家庭收益打折
        int dep = stats.depression;

        float costAmp = 1f + dep * 0.003f;      // up to +30%
        float familyBuffNerf = 1f - dep * 0.002f; // up to -20%

        var d = new StatDelta(
            src.energyDelta,
            src.healthDelta,
            src.familyDelta,
            src.moneyDelta,
            src.depressionDelta
        );

        if (d.energyDelta < 0) d.energyDelta = Mathf.RoundToInt(d.energyDelta * costAmp);
        if (d.healthDelta < 0) d.healthDelta = Mathf.RoundToInt(d.healthDelta * costAmp);

        if (d.familyDelta > 0) d.familyDelta = Mathf.RoundToInt(d.familyDelta * familyBuffNerf);

        return d;
    }

    private void ApplyDelta(StatDelta d)
    {
        stats.energy = Mathf.Clamp(stats.energy + d.energyDelta, 0, 100);
        stats.health = Mathf.Clamp(stats.health + d.healthDelta, 0, 100);
        stats.family = Mathf.Clamp(stats.family + d.familyDelta, 0, 100);
        stats.money += d.moneyDelta;

        stats.depression = Mathf.Clamp(stats.depression + d.depressionDelta, 0, 100);
    }

    private IEnumerator NextEventRoutine()
    {
        SetButtonsInteractable(false);
        yield return new WaitForSeconds(1.2f);
        SetButtonsInteractable(true);

        if (IsGameOver()) ShowGameOver();
        else ShowRandomEvent();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in choiceButtons)
            if (btn != null) btn.interactable = interactable;
    }

    private bool IsGameOver()
    {
        if (stats.health <= 0) return true;
        if (stats.energy <= 0) return true;
        if (stats.family <= 0) return true;
        return false;
    }

    private void ShowGameOver()
    {
        _isGameOver = true;

        string reason =
            stats.health <= 0 ? "你在凌晨三点胸口一紧，倒在键盘前。" :
            stats.energy <= 0 ? "你终于在某个早晨再也起不来床。" :
            stats.family <= 0 ? "你回到家，只看到桌上摆着一份离婚协议书。" :
            "人生数据崩盘。";

        descriptionText.text =
            $"游戏结束。\n\n{reason}\n\n（这只是游戏，但它为什么这么像现实？）";

        foreach (var btn in choiceButtons)
            if (btn != null) btn.gameObject.SetActive(false);
    }
}
