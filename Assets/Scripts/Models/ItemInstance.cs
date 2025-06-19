using JianAdventure;
using JianAdventure.Managers;
using ProjectJian.SOs;

namespace JianAdventure.Model
{
    /// <summary>
    /// 道具实例，用于运行时背包管理
    /// </summary>
    public class ItemInstance
    {
        /// <summary>
        /// 引用的道具数据对象
        /// </summary>
        public ItemSO Data { get; private set; }

        public ItemInstance(ItemSO so)
        {
            Data = so;
        }

        /// <summary>
        /// 使用该道具时触发效果
        /// </summary>
        public void Use()
        {
            // 调用玩家的健康修改
            GameManager.Instance.Player.ModifyHealth(Data.HealthEffect);
        }
    }
}