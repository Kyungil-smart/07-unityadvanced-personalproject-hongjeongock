using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 파밍씬에서 배를 타고 거점(MainScene)으로 귀환
/// </summary>
public class BoatReturn : MonoBehaviour
{
    [Header("귀환 설정")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private GameObject loadingScreenPrefab;
    [SerializeField] private GameObject interactPromptUI;

    private bool _playerInRange = false;
    private bool _isTransitioning = false;
    private Transform _playerTransform;

    private void Update()
    {
        if (_isTransitioning) return;

        CheckPlayerRange();

        if (_playerInRange && Input.GetKeyDown(interactKey))
            StartCoroutine(ReturnToBase());
    }

    private void CheckPlayerRange()
    {
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerTransform = player.transform;
        }

        if (_playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        bool inRange = dist <= interactRange;

        if (inRange != _playerInRange)
        {
            _playerInRange = inRange;
            if (interactPromptUI != null)
                interactPromptUI.SetActive(_playerInRange);
        }
    }

    private IEnumerator ReturnToBase()
    {
        _isTransitioning = true;

        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);

        // 로딩 화면
        GameObject loadingScreen = null;
        if (loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreen);
        }

        yield return new WaitForSeconds(0.5f);

        // 이전 씬으로 복귀 (없으면 MainScene으로)
        string returnScene = PlayerPrefs.GetString("PreviousScene", "MainScene");

        AsyncOperation op = SceneManager.LoadSceneAsync(returnScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(0.3f);
        op.allowSceneActivation = true;

        if (loadingScreen != null)
            Destroy(loadingScreen, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
