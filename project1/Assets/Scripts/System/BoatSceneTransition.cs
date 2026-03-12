using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 배에 접근하면 상호작용 UI가 뜨고
/// 확인하면 로딩 화면과 함께 씬 전환
/// </summary>
public class BoatSceneTransition : MonoBehaviour
{
    [Header("이동할 씬")]
    [SerializeField] private string targetSceneName = "ForestScene";

    [Header("상호작용 설정")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("로딩 화면")]
    [SerializeField] private GameObject loadingScreenPrefab;  // 로딩 UI 프리팹

    [Header("UI 텍스트 (선택)")]
    [SerializeField] private GameObject interactPromptUI;     // "[F] 배에 탑승" UI
    [SerializeField] private string promptMessage = "[F] 배에 탑승";

    private bool _playerInRange = false;
    private bool _isTransitioning = false;
    private Transform _playerTransform;

    private void Update()
    {
        if (_isTransitioning) return;

        // 범위 내 플레이어 감지
        CheckPlayerRange();

        // 상호작용 입력
        if (_playerInRange && Input.GetKeyDown(interactKey))
            StartCoroutine(TransitionToScene());
    }

    private void CheckPlayerRange()
    {
        // 플레이어 태그로 찾기
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerTransform = player.transform;
        }

        if (_playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        bool inRange = dist <= interactRange;

        // 범위 진입/이탈 시 UI 토글
        if (inRange != _playerInRange)
        {
            _playerInRange = inRange;
            ShowPrompt(_playerInRange);
        }
    }

    private IEnumerator TransitionToScene()
    {
        _isTransitioning = true;
        ShowPrompt(false);

        // 로딩 화면 표시
        GameObject loadingScreen = null;
        if (loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreen);
        }

        // 현재 씬 이름 저장 (돌아올 때 사용)
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(0.5f);

        // 비동기 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);
        op.allowSceneActivation = false;

        // 로딩 진행률 대기
        while (op.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(0.3f);

        op.allowSceneActivation = true;

        // 로딩 화면 제거
        if (loadingScreen != null)
            Destroy(loadingScreen, 0.5f);
    }

    private void ShowPrompt(bool show)
    {
        if (interactPromptUI != null)
            interactPromptUI.SetActive(show);
    }

    // 에디터 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
