using UnityEngine;

[CreateAssetMenu(
    fileName = "CharaData",
    menuName = "Game/CharaData",
    order = 1
    )]
public class CharaData : ScriptableObject
{
    [Header("基本情報")]
    public int charaId;
    public string charaName;
    public Sprite faceIcon;
    public Sprite bodyIcon;
    public Sprite mapSpawnPoint;

    [Header("プロフィール")]
    [TextArea(3,6)]
    public string profileText;

    [Header("共助の情報")]
    [TextArea(3,6)]
    public string Discription;

    [Header("配慮度")]
    public int consideration;

    [Header("共助スコア")]
    public int score;

    [Header("属性")]
    public bool isWheelchairUser;
}

