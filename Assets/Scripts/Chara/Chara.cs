using UnityEngine;
using TMPro;
using System.Collections;

public class Chara : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharaData charaData;

    [Header("UI")]
    [SerializeField] private GameObject buttonText;
    [SerializeField] private GameObject bubbleCanvas; // キャラについている吹き出し用Canvas
    
    private bool isPlayerInRange = false;
    private bool isShowingQuestion = false;
    private bool hasScored = false;
    private TextMeshProUGUI interactionTextMesh;
    private PlayerController playerController;
    private Coroutine bubbleCoroutine;

    void Start()
    {
        if (buttonText != null) 
        {
            buttonText.SetActive(false);
            interactionTextMesh = buttonText.GetComponentInChildren<TextMeshProUGUI>();
        }

        // ShowBubbleがInstantiate直後に呼ばれた場合、Startでの非表示化を防ぐ
        if (bubbleCanvas != null && bubbleCoroutine == null)
        {
            bubbleCanvas.SetActive(false);
        }

        if (charaData != null)
        {
            UpdateVisual();
        }
    }

    // 外部（マネージャーなど）からデータを注入するための関数
    public void SetData(CharaData data, GameObject playerTextObject)
    {
        charaData = data;
        buttonText = playerTextObject; 
        
        if (buttonText != null) 
        {
            buttonText.SetActive(false); 
            interactionTextMesh = buttonText.GetComponentInChildren<TextMeshProUGUI>();
        }

        UpdateVisual();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            UpdateInteraction();
        }
    }

    private void UpdateInteraction()
    {
        if (interactionTextMesh == null || charaData == null || playerController == null) return;

        if (playerController.IsCarrying)
        {
            interactionTextMesh.text = "今は手を貸せません";
            return;
        }

        if (charaData.isWheelchairUser)
        {
            if (!isShowingQuestion)
            {
                interactionTextMesh.text = "連れていきますか？(もう一度F)";
                isShowingQuestion = true;
            }
            else
            {
                CarryThisChara();
            }
        }
        else
        {
            // 健常者の場合
            ShowBubble(); // 統一メソッドを使用
            
            // スコア加算（一回のみ）
            if (!hasScored && charaData != null && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(charaData.score);
                hasScored = true;
            }

            if (buttonText != null) buttonText.SetActive(false);
            Debug.Log($"{charaData.charaName} に避難を呼びかけました");
        }
    }

    public void ShowBubble()
    {
        if (bubbleCanvas != null)
        {
            // Canvasの描画順を強制的に手前に持ってくる
            Canvas cv = bubbleCanvas.GetComponent<Canvas>();
            if (cv != null)
            {
                cv.overrideSorting = true;
                cv.sortingOrder = 1000;
            }

            // UIが重なりで消えないよう位置を微調整（Z=0を保証）
            bubbleCanvas.transform.localPosition = new Vector3(
                bubbleCanvas.transform.localPosition.x,
                bubbleCanvas.transform.localPosition.y,
                0f
            );

            if (bubbleCoroutine != null) StopCoroutine(bubbleCoroutine);
            bubbleCoroutine = StartCoroutine(ShowBubbleRoutine());
        }
        else
        {
            Debug.LogWarning($"[Chara] {gameObject.name} の bubbleCanvas が未設定です！");
        }
    }

    private IEnumerator ShowBubbleRoutine()
    {
        // 1フレーム待機（Instantiate直後のUI初期化待ち）
        yield return null;

        if (bubbleCanvas != null)
        {
            bubbleCanvas.SetActive(true);
            yield return new WaitForSeconds(3f);
            bubbleCanvas.SetActive(false);
        }
        bubbleCoroutine = null;
    }

    private void CarryThisChara()
    {
        if (playerController != null)
        {
            playerController.CarryChara(charaData);
            
            // オブジェクトを消す
            if (buttonText != null) buttonText.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    // 見た目をデータに合わせて更新する処理
    private void UpdateVisual()
    {
        if (charaData != null && charaData.bodyIcon != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = charaData.bodyIcon;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && buttonText != null)
        {
            isPlayerInRange = true;
            playerController = other.GetComponent<PlayerController>();
            buttonText.SetActive(true);
            isShowingQuestion = false; // 離れて戻ってきたらリセット
            
            if (interactionTextMesh != null)
            {
                // 車いす非使用者の場合は最初から「避難を呼びかける」と表示する
                if (charaData != null && !charaData.isWheelchairUser)
                {
                    string suffix = hasScored ? " (呼びかけ済み)" : "";
                    interactionTextMesh.text = "F：避難を呼びかける" + suffix;
                }
                else
                {
                    interactionTextMesh.text = "F：話しかける";
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && buttonText != null)
        {
            isPlayerInRange = false;
            buttonText.SetActive(false);
            isShowingQuestion = false;
        }
    }
}
