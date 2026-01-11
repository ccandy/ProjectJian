using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 《剑剑历险记》Demo 版核心管理器：
/// - 管理玩家四大系统数值
/// - 随机事件 + 选项
/// - Game Over 判定
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("玩家状态")]
    public PlayerStats stats = new PlayerStats();

    [Header("UI 引用")]
    public Text dayText;
    public Text statsText;
    public Text descriptionText;
    public Button[] choiceButtons; // 建议数组长度为 3

    private System.Random _random = new System.Random();

    // 事件与选项的数据结构（简单写在代码里，后面可改成 ScriptableObject）
    private List<GameEvent> _events;
    private GameEvent _currentEvent;

    private bool _isGameOver = false;

    #region 内部数据结构

    [System.Serializable]
    public class Choice
    {
        public string label;        // 按钮显示文本
        public string resultText;   // 选完后的结果描述
        public StatDelta delta;     // 对四大系统的影响

        public Choice(string label, StatDelta delta, string resultText)
        {
            this.label = label;
            this.delta = delta;
            this.resultText = resultText;
        }
    }

    [System.Serializable]
    public class GameEvent
    {
        public string id;
        public string description;  // 事件描述
        public Choice[] choices;

        public GameEvent(string id, string description, Choice[] choices)
        {
            this.id = id;
            this.description = description;
            this.choices = choices;
        }
    }

    #endregion

    private void Start()
    {
        InitEvents();
        UpdateStatsUI();
        ShowRandomEvent();
    }

    /// <summary>
    /// 初始化若干个写死的事件，用于 Demo。
    /// 正式版本可以替换为 JSON / ScriptableObject 数据驱动。
    /// </summary>
    private void InitEvents()
    {
        _events = new List<GameEvent>();

        // 事件 1：早上咖啡
        _events.Add(new GameEvent(
            "morning_coffee",
            "早上 7:30，闹钟第 3 遍响。你昨晚为了改 bug 熬到 2 点。" +
            "\n桌子上剩下一口昨晚的冰咖啡。",
            new Choice[]
            {
                new Choice(
                    "硬撑起床去投简历",
                    new StatDelta(-20, -5, 0, 0),
                    "你强行把自己从床上撬起来，洗了把脸，打开招聘网站。" +
                    "\n精力 -20，健康 -5。"
                ),
                new Choice(
                    "把剩下那口咖啡干了",
                    new StatDelta(+10, -15, 0, 0),
                    "咖啡因直冲大脑，你清醒了，但心跳开始加速。" +
                    "\n精力 +10，健康 -15（心悸 3 小时）。"
                ),
                new Choice(
                    "装死再睡一小时",
                    new StatDelta(+5, 0, -5, 0),
                    "你选择对世界按下暂停键。老婆在门外重重地叹了口气。" +
                    "\n精力 +5，家庭 -5。"
                )
            }
        ));

        // 事件 2：投简历 vs 摸鱼 vs 外包
        _events.Add(new GameEvent(
            "send_cv",
            "中午 12:00，你盯着电脑上的简历，不知道该从哪一段被裁经历写起。",
            new Choice[]
            {
                new Choice(
                    "认真改简历并广撒网投递",
                    new StatDelta(-15, -5, 0, 0),
                    "你细细修改每一行项目经历，认真投了十几封。" +
                    "\n精力 -15，健康 -5，但你感觉至少做了点什么。"
                ),
                new Choice(
                    "随便投两份就去刷视频",
                    new StatDelta(-5, 0, -5, 0),
                    "你象征性地投了两份，随后在短视频里度过了一个下午。" +
                    "\n精力 -5，家庭 -5（晚上又没空陪孩子了）。"
                ),
                new Choice(
                    "接个低价外包先糊口",
                    new StatDelta(-25, -10, 0, 200),
                    "你接下了一个压榨到离谱的外包。" +
                    "\n精力 -25，健康 -10，金钱 +200。" +
                    "\n赚钱像拔牙，花钱像拉稀。"
                )
            }
        ));

        // 事件 3：老婆 QTE
        _events.Add(new GameEvent(
            "family_qte",
            "晚上 10:30，你刚想打开 Steam，微信弹出一条消息：\n『你打算怎么办？』——来自你老婆。",
            new Choice[]
            {
                new Choice(
                    "认真聊聊自己的打算",
                    new StatDelta(-10, -5, +15, 0),
                    "你关掉游戏，认真和她聊到半夜。" +
                    "\n精力 -10，健康 -5，家庭 +15。" +
                    "\n她的表情似乎稍微柔和了一点。"
                ),
                new Choice(
                    "假装没看到，继续刷技术文章",
                    new StatDelta(-5, -5, -15, 0),
                    "你对着技术博客发呆，装作很忙。" +
                    "\n家庭 -15，健康 -5（心里更焦虑了）。"
                ),
                new Choice(
                    "回复『我会想办法的』然后秒消失",
                    new StatDelta(-5, 0, -10, 0),
                    "你发了一句标准模板回复，然后把自己交给了屏幕。" +
                    "\n家庭 -10。"
                )
            }
        ));

        // 事件 4：狗粮危机
        _events.Add(new GameEvent(
            "dog_food",
            "月底了，银行卡余额亮起红灯。\n" +
            "狗子悠闲地看着你，把空空的进口狗粮袋子叼到你脚边。",
            new Choice[]
            {
                new Choice(
                    "咬牙继续买进口狗粮",
                    new StatDelta(-5, 0, +5, -200),
                    "你选择牺牲自己，成全狗子和家庭形象。" +
                    "\n精力 -5（继续省吃俭用），家庭 +5，金钱 -200。"
                ),
                new Choice(
                    "换成普通狗粮，省点钱",
                    new StatDelta(0, 0, -5, -50),
                    "狗子盯着新狗粮闻了半天，叹了口气。" +
                    "\n家庭 -5（被老婆吐槽你对狗都抠），金钱 -50。"
                ),
                new Choice(
                    "自己啃馒头，把剩下的钱都给狗粮",
                    new StatDelta(-10, -5, +10, -150),
                    "你决定用馒头度过本月剩下时间。" +
                    "\n精力 -10，健康 -5，家庭 +10，金钱 -150。"
                )
            }
        ));
    }

    /// <summary>
    /// 更新顶部 UI 上的数值显示。
    /// </summary>
    private void UpdateStatsUI()
    {
        if (dayText != null)
        {
            dayText.text = $"Day {stats.day}";
        }

        if (statsText != null)
        {
            statsText.text = $"精力: {stats.energy}   健康: {stats.health}   家庭: {stats.family}   金钱: {stats.money}";
        }
    }

    /// <summary>
    /// 从事件列表中随机挑一个事件展示。
    /// </summary>
    private void ShowRandomEvent()
    {
        if (_isGameOver)
            return;

        if (IsGameOver())
        {
            ShowGameOver();
            return;
        }

        int index = _random.Next(_events.Count);
        _currentEvent = _events[index];

        if (descriptionText != null)
        {
            descriptionText.text = _currentEvent.description;
        }

        SetupChoiceButtons();
    }

    /// <summary>
    /// 配置三个按钮的文字和点击事件。
    /// </summary>
    private void SetupChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button btn = choiceButtons[i];

            if (i < _currentEvent.choices.Length)
            {
                var choice = _currentEvent.choices[i];
                btn.gameObject.SetActive(true);

                Text btnText = btn.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = choice.label;
                }

                btn.onClick.RemoveAllListeners();
                int capturedIndex = i;
                btn.onClick.AddListener(() => OnChoiceSelected(capturedIndex));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 当玩家选择了某个选项。
    /// </summary>
    /// <param name="index">选项索引</param>
    private void OnChoiceSelected(int index)
    {
        if (_isGameOver)
            return;

        var choice = _currentEvent.choices[index];

        // 天数前进
        stats.day++;

        // 应用数值变化，并进行 Clamp
        ApplyDelta(choice.delta);

        // 显示结果文案
        if (descriptionText != null)
        {
            descriptionText.text = choice.resultText;
        }

        UpdateStatsUI();

        // 短暂停顿后进入下一天
        StartCoroutine(NextEventRoutine());
    }

    private void ApplyDelta(StatDelta delta)
    {
        stats.energy = Mathf.Clamp(stats.energy + delta.energyDelta, 0, 100);
        stats.health = Mathf.Clamp(stats.health + delta.healthDelta, 0, 100);
        stats.family = Mathf.Clamp(stats.family + delta.familyDelta, 0, 100);
        stats.money += delta.moneyDelta;
    }

    private IEnumerator NextEventRoutine()
    {
        // 暂时禁用按钮，避免连点
        SetButtonsInteractable(false);
        yield return new WaitForSeconds(1.5f);
        SetButtonsInteractable(true);

        if (IsGameOver())
        {
            ShowGameOver();
        }
        else
        {
            ShowRandomEvent();
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in choiceButtons)
        {
            btn.interactable = interactable;
        }
    }

    /// <summary>
    /// Game Over 判定：任一核心系统归零。
    /// </summary>
    private bool IsGameOver()
    {
        if (stats.health <= 0) return true;
        if (stats.energy <= 0) return true;
        if (stats.family <= 0) return true;

        // 如果希望加上“经济崩盘”结局，可以打开：
        // if (stats.money <= -20000) return true;

        return false;
    }

    private void ShowGameOver()
    {
        _isGameOver = true;

        string reason;

        if (stats.health <= 0)
        {
            reason = "你在凌晨三点胸口一紧，倒在键盘前。\n第二天 HR 发现你最后一封邮件还卡在草稿箱里。";
        }
        else if (stats.energy <= 0)
        {
            reason = "你终于在某个早晨再也起不来床。\n手机还在播着昨晚没看完的技术分享直播。";
        }
        else if (stats.family <= 0)
        {
            reason = "你回到家，只看到桌上摆着一份离婚协议书和一只吃完最后一口狗粮的狗。";
        }
        else
        {
            reason = "你的人生数据已经崩盘。";
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                "游戏结束。\n\n" +
                reason +
                "\n\n《剑剑历险记》只是个模拟器。\n" +
                "如果现实里的你也觉得快扛不住了，\n" +
                "请先关掉游戏，好好休息，或者去找可以聊聊的人。";
        }

        // 隐藏按钮，避免继续操作
        foreach (var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }
}
