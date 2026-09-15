using UnityEngine;

[CreateAssetMenu(
    fileName = "NewOtherworld",
    menuName = "Game/Otherworld Data"
)]
public class OtherworldData : ScriptableObject
{
    [Header("기본 정보")]
    public string worldID;

    public string worldName;


    [Header("이미지")]
    public Sprite selectionImage;

    public Sprite roomImage;


    [Header("격리실 내부 오브젝트")]
    public GameObject roomPrefab;


    [Header("선택 전 힌트")]
    [TextArea(2, 5)]
    public string hoverDescription;


    [Header("격리 후 상세 정보")]
    public string infoTitle;

    [TextArea(3, 8)]
    public string infoDescription;


    [Header("작업 정보")]
    public EquipmentType requiredEquipment =
        EquipmentType.None;

    public int powerGain = 1;

    public int hpDamage = 0;

    public int mentalDamage = 0;
}