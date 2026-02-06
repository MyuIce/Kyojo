using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float carryingDashSpeed = 12f;
    [SerializeField] private GameObject charaPrefab; // キャラを配置する時用のプレファブ

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer spriteRenderer;
    private CharaData carriedCharaData;

    public bool IsCarrying => carriedCharaData != null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        CheckSafeZoneDrop();
        HandleDash();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, z).normalized;
        
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed = IsCarrying ? carryingDashSpeed : dashSpeed;
        }

        transform.Translate(move * currentSpeed * Time.deltaTime);

        if (x > 0) spriteRenderer.sprite = rightSprite;
        else if (x < 0) spriteRenderer.sprite = leftSprite;
        else if (z > 0) spriteRenderer.sprite = upSprite;
        else if (z < 0) spriteRenderer.sprite = downSprite;
    }

    private void CheckSafeZoneDrop()
    {
        if (carriedCharaData != null)
        {
            // GameManagerの機能を利用して安全地帯にいるかチェック
            if (GameManager.Instance != null && IsInSafeZone())
            {
                DropChara();
            }
        }
    }

    private bool IsInSafeZone()
    {
        // GameManagerのIsPlayerInSafeZoneを公開するか、ここでも同様のチェックを行う
        // 現状GameManagerのはprivateなので、タグで簡易チェック
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("SafeZone")) return true;
        }
        return false;
    }

    public void CarryChara(CharaData data)
    {
        carriedCharaData = data;
        Debug.Log($"Carrying: {data.charaName}");
    }

    private void DropChara()
    {
        if (carriedCharaData == null) return;

        Debug.Log($"Dropped {carriedCharaData.charaName} in Safe Zone");

        // 安全地帯にキャラを再配置
        if (charaPrefab != null)
        {
            GameObject obj = Instantiate(charaPrefab, transform.position, Quaternion.identity);
            Chara charaScript = obj.GetComponent<Chara>();
            if (charaScript != null)
            {
                // UI表示用のテキストなどはマネージャーから取得するか、Nullでも動くように調整
                charaScript.SetData(carriedCharaData, null); 
                charaScript.ShowBubble(); // 吹き出しを表示
                
                // スコア加算
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(carriedCharaData.score);
                }
            }
        }

        carriedCharaData = null;
    }

    // HandleDash is now integrated into HandleMovement for cleaner speed calculation
    private void HandleDash()
    {
        // No longer used separately
    }
}
