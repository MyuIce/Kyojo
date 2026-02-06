using UnityEngine;
using System.Collections.Generic; // Listを使うために追加

public class CharaManager : MonoBehaviour
{
    public static CharaManager Instance;

    [Header("Character Database")]
    [SerializeField] private CharaData[] charaList;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject charaPrefab; // キャラのプレファブ
    [SerializeField] private Transform[] spawnPoints; // 配置されたスポーン地点

    private int currentIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // ゲーム開始時にランダム配置を実行
        SpawnCharactersRandomly();
    }

    [SerializeField] private GameObject interactionTextSceneObject; // インスペクターからドラッグ＆ドロップで設定・もしくはシーン上の参照

    // 生成されたキャラを管理する辞書
    private Dictionary<CharaData, Chara> spawnedCharacterMap = new Dictionary<CharaData, Chara>();

    public Dictionary<CharaData, Chara> SpawnedCharacterMap => spawnedCharacterMap;

    private void SpawnCharactersRandomly()
    {
        if (charaPrefab == null || spawnPoints.Length == 0) return;

        spawnedCharacterMap.Clear(); // リストクリア

        // 1. まずインスペクター設定を確認し、無ければタグで探す
        GameObject playerText = interactionTextSceneObject;
        if (playerText == null)
        {
            playerText = GameObject.FindWithTag("InteractionText");
        }

        if (playerText == null)
        {
            Debug.LogError("CharaManager: まだテキストオブジェクトが見つかりません！インスペクターで設定するか、タグ'InteractionText'を設定してください。");
        }

        List<CharaData> randomList = new List<CharaData>(charaList);

        // リストシャッフル
        for (int i = randomList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            CharaData temp = randomList[i];
            randomList[i] = randomList[j];
            randomList[j] = temp;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i >= randomList.Count) break;

            GameObject obj = Instantiate(charaPrefab, spawnPoints[i].position, Quaternion.identity);
            
            Chara charaScript = obj.GetComponent<Chara>();
            if (charaScript != null)
            {
                CharaData data = randomList[i];
                // ここでテキストオブジェクトも一緒に渡す
                charaScript.SetData(data, playerText);
                
                // 辞書に登録
                if (!spawnedCharacterMap.ContainsKey(data))
                {
                    spawnedCharacterMap.Add(data, charaScript);
                }
            }
        }
    }

    /// <summary>
    /// 指定したデータを持つ、実際に生成されたキャラを取得する
    /// </summary>
    public Chara GetSpawnedChara(CharaData data)
    {
        if (data == null) return null;
        if (spawnedCharacterMap.ContainsKey(data))
        {
            return spawnedCharacterMap[data];
        }
        return null;
    }

    public CharaData GetCurrent()
    {
        if (charaList == null || charaList.Length == 0) return null;
        return charaList[currentIndex];
    }

    public CharaData Next()
    {
        if (charaList == null || charaList.Length == 0) return null;
        currentIndex = (currentIndex + 1) % charaList.Length;
        return GetCurrent();
    }

    public CharaData Prev()
    {
        if (charaList == null || charaList.Length == 0) return null;
        currentIndex = (currentIndex - 1 + charaList.Length) % charaList.Length;
        return GetCurrent();
    }

    public void ResetIndex()
    {
        currentIndex = 0;
    }
}
