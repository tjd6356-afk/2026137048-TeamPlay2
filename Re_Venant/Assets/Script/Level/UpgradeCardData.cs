using UnityEngine;

[CreateAssetMenu(
    fileName = "NewUpgradeCard",
    menuName = "Level Up/Upgrade Card"
)]
public class UpgradeCardData : ScriptableObject
{
    [Header("Card")]
    public string cardName;

    [TextArea(2, 5)]
    public string description;

    public Sprite icon;

    [Header("Upgrade")]
    public UpgradeType upgradeType;

    [Header("Value")]
    public int intValue = 1;
    public float floatValue = 1f;
}