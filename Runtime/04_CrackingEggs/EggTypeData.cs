namespace Sobia.CrackingEggs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewEggType", menuName = "Game/Egg Type")]
    public class EggTypeData : ScriptableObject
    {
        public string EggTypeName;
        public int BaseHp;
        public int RewardValue;
        public Sprite EggSprite;
    }
}