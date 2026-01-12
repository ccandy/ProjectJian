using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jianjian/Event Database", fileName = "EDB_Main")]
public class EventDatabaseSO : ScriptableObject
{
    public List<GameEventSO> events = new List<GameEventSO>();
}
