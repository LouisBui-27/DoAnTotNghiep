[System.Serializable]
public class RewardData
{
    public string type; // Gold, Gem, Hero, Item, etc.
    public string itemId; // Optional, e.g., hero_knight
    public int amount;    // Optional for things like Hero, Item
}