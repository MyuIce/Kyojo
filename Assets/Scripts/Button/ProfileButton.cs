using UnityEngine;

public class ProfileButton : MonoBehaviour
{
    [SerializeField] private ProfileUIManager profileUIManager;

    // ＞ ボタン
    public void OnClickNext()
    {
        if (CharaManager.Instance != null)
        {
            CharaManager.Instance.Next();
            RefreshUI();
        }
    }

    // ＜ ボタン
    public void OnClickPrev()
    {
        if (CharaManager.Instance != null)
        {
            CharaManager.Instance.Prev();
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        if (profileUIManager != null)
        {
            profileUIManager.UpdateUI();
        }
        else
        {
            Debug.LogWarning("ProfileUIManager is not assigned in ProfileButton.");
        }
    }
}
