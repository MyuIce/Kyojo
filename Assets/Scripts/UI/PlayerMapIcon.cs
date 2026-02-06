using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMapIcon : MonoBehaviour
{
    [SerializeField] RectTransform mapRect;
    [SerializeField] RectTransform playerIcon;
    [SerializeField] GameObject npcIconPrefab; // 他キャラ用のアイコンプレファブ

    [SerializeField] Vector2 worldMin; // マップ左下
    [SerializeField] Vector2 worldMax; // マップ右上
    [SerializeField] Transform player;

    private Dictionary<Chara, RectTransform> npcIcons = new Dictionary<Chara, RectTransform>();

    void Update()
    {
        // 自キャラの位置を更新
        UpdateIconPosition(player, playerIcon);

        // 他キャラの位置を更新
        UpdateNPCIcons();
    }

    private void UpdateIconPosition(Transform target, RectTransform icon)
    {
        if (target == null || icon == null) return;

        float x = Mathf.InverseLerp(worldMin.x, worldMax.x, target.position.x);
        float y = Mathf.InverseLerp(worldMin.y, worldMax.y, target.position.y);

        Vector2 mapSize = mapRect.sizeDelta;
        icon.anchoredPosition = new Vector2(
            (x - 0.5f) * mapSize.x,
            (y - 0.5f) * mapSize.y
        );
    }

    private void UpdateNPCIcons()
    {
        if (CharaManager.Instance == null || npcIconPrefab == null) return;

        var spawnedMap = CharaManager.Instance.SpawnedCharacterMap;
        CharaData currentData = CharaManager.Instance.GetCurrent();

        foreach (var kvp in spawnedMap)
        {
            Chara chara = kvp.Value;
            CharaData data = kvp.Key;
            
            if (chara == null) continue;

            if (!npcIcons.ContainsKey(chara))
            {
                // アイコンを生成
                GameObject iconObj = Instantiate(npcIconPrefab, mapRect);
                RectTransform rt = iconObj.GetComponent<RectTransform>();
                npcIcons.Add(chara, rt);
                
                // キャラのアイコンを設定（Imageコンポーネントがあれば）
                Image img = iconObj.GetComponent<Image>();
                if (img != null)
                {
                    // mapSpawnPointがあればそれを使う、なければfaceIconを使う
                    if (data.mapSpawnPoint != null) img.sprite = data.mapSpawnPoint;
                    else if (data.faceIcon != null) img.sprite = data.faceIcon;
                }
                
                iconObj.name = $"MapIcon_{data.charaName}";
            }

            RectTransform iconRect = npcIcons[chara];
            UpdateIconPosition(chara.transform, iconRect);

            // プロフィールで開いている人物を強調（少し大きくする）
            if (data == currentData)
            {
                iconRect.localScale = new Vector3(1.5f, 1.5f, 1f);
            }
            else
            {
                iconRect.localScale = Vector3.one;
            }
        }
    }
}
