using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image faceImage;        // 顔アイコン表示用
    [SerializeField] private Image bodyImage;        
    [SerializeField] private TextMeshProUGUI nameText;       // (あれば)名前表示用
    [SerializeField] private TextMeshProUGUI profileText;    // プロフィールテキスト表示用
    [SerializeField] private TextMeshProUGUI descriptionText; // 共助情報(Discription)表示用
    // 必要に応じてパラメータ表示を追加 (例: 配慮度など)

    [Header("MiniMap Settings")]
    [SerializeField] private RectTransform mapArea;         // ミニマップの白い枠 (親)
    [SerializeField] private RectTransform charMarker;      // 赤い点 (Icon)
    [SerializeField] private Vector2 worldMinPos = new Vector2(-10, -10); // ゲーム世界の左下の座標
    [SerializeField] private Vector2 worldMaxPos = new Vector2(10, 10);   // ゲーム世界の右上の座標

    private void OnEnable()
    {
        UpdateProfileWithReset();
    }

    void Start()
    {
        UpdateProfileWithReset();
    }

    public void UpdateProfileWithReset()
    {
        var manager = CharaManager.Instance;
        if (manager == null) manager = FindObjectOfType<CharaManager>();

        if (manager != null)
        {
            manager.ResetIndex();
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        var manager = CharaManager.Instance;
        if (manager == null) manager = FindObjectOfType<CharaManager>();
        if (manager == null) return;

        CharaData currentData = manager.GetCurrent();
        if (currentData == null) return;

        // アイコンの更新
        if (faceImage != null)
        {
            faceImage.sprite = currentData.faceIcon;
            if (faceImage.sprite != null)
                faceImage.enabled = true;
            else
                faceImage.enabled = false;
        }

        //ミニキャラ/全身画像の更新
        if (bodyImage != null)
        {
            bodyImage.sprite = currentData.bodyIcon;
            if (bodyImage.sprite != null)
                bodyImage.enabled = true;
            else
                bodyImage.enabled = false;
        }

        // テキストの更新 (CharaDataの変数名に合わせる)
        if (nameText != null)
        {
            nameText.text = currentData.charaName;
        }

        if (profileText != null)
        {
            profileText.text = currentData.profileText;
        }

        if (descriptionText != null)
        {
            descriptionText.text = currentData.Discription;
        }

        // ▼▽▼ ミニマップ位置の更新処理 ▼▽▼
        UpdateMapPosition(currentData);
    }

    private void Update()
    {
        // 毎フレーム位置を更新（キャラが移動している可能性があるため）
        var manager = CharaManager.Instance;
        if (manager == null) manager = FindObjectOfType<CharaManager>();
        
        if (manager != null)
        {
            CharaData currentData = manager.GetCurrent();
            if (currentData != null)
            {
                UpdateMapPosition(currentData);
            }
        }
    }

    private void UpdateMapPosition(CharaData data)
    {
        if (mapArea == null || charMarker == null) return;

        //CharaManagerから、データに対応する実体(GameObject)を取得
        Chara targetChara = CharaManager.Instance.GetSpawnedChara(data);

        if (targetChara != null)
        {
            charMarker.gameObject.SetActive(true);

            //アイコン画像を更新
            Image markerImg = charMarker.GetComponent<Image>();
            if (markerImg != null)
            {
                if (data.mapSpawnPoint != null) markerImg.sprite = data.mapSpawnPoint;
                else if (data.faceIcon != null) markerImg.sprite = data.faceIcon;
            }

            //ワールド座標を取得
            Vector3 worldPos = targetChara.transform.position;

            //ワールド座標を 0.0 ~ 1.0 の割合(Normalized)に変換
            float normX = Mathf.InverseLerp(worldMinPos.x, worldMaxPos.x, worldPos.x);
            float normY = Mathf.InverseLerp(worldMinPos.y, worldMaxPos.y, worldPos.y);

            //MAPエリアのサイズに合わせて位置を決定
            //MapAreaのPivotが(0.5, 0.5)だと仮定して補正
            Vector2 finalPos = new Vector2(
                (mapArea.rect.width * normX) - (mapArea.rect.width * 0.5f),
                (mapArea.rect.height * normY) - (mapArea.rect.height * 0.5f)
            );
            
            charMarker.anchoredPosition = finalPos;
        }
        else
        {
            // まだスポーンしていない、または見つからない場合は非表示
            charMarker.gameObject.SetActive(false);
        }
    }
}
