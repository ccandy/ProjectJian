using System;
using System.Collections.Generic;

[Serializable]
public class JsonEventDatabase
{
    public List<JsonEvent> events;
}

[Serializable]
public class JsonEvent
{
    public string id;

    public JsonMoodText description;

    public int minDay = 1;
    public int maxDay = 999999;

    public int minMoney = -999999;
    public int maxMoney = 999999;

    public int minDepression = 0;
    public int maxDepression = 100;

    public int weight = 10;

    public List<JsonChoice> choices;
}

[Serializable]
public class JsonChoice
{
    public string label;
    public JsonMoodText resultText;
    public StatDelta delta;
}

[Serializable]
public class JsonMoodText
{
    public string normal;
    public string gloomy;
    public string meltdown;

    public MoodText ToMoodText()
    {
        return new MoodText { normal = normal, gloomy = gloomy, meltdown = meltdown };
    }
}
