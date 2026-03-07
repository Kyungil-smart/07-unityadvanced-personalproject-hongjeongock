using System.Collections;
using System.Reflection;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    [Header("로드 타이밍")]
    [SerializeField] private bool loadOnStart = true;
    [SerializeField] private float loadDelay = 0.1f;

    [Header("플레이어 연결")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("플레이어 체력을 가지고 있는 스크립트 컴포넌트")]
    [SerializeField] private MonoBehaviour playerHpTarget;

    [Tooltip("플레이어 현재 체력 변수명")]
    [SerializeField] private string playerHpFieldName = "_playerCurrentHp";

    [Header("골드 연결")]
    [Tooltip("골드 값을 가지고 있는 스크립트 컴포넌트")]
    [SerializeField] private MonoBehaviour goldTarget;

    [Tooltip("골드 변수명")]
    [SerializeField] private string goldFieldName = "gold";

    [Header("집 레벨 연결")]
    [Tooltip("집 레벨을 가지고 있는 스크립트 컴포넌트")]
    [SerializeField] private MonoBehaviour houseLevelTarget;

    [Tooltip("집 레벨 변수명")]
    [SerializeField] private string houseLevelFieldName = "currentLevel";

    [Header("자원 연결")]
    [Tooltip("반드시 ResourceInventory 컴포넌트를 넣으세요.")]
    [SerializeField] private ResourceInventory resourceInventoryTarget;

    [Header("자원 Definition")]
    [SerializeField] private ResourceDefinition woodResource;
    [SerializeField] private ResourceDefinition stoneResource;
    [SerializeField] private ResourceDefinition ironResource;
    [SerializeField] private ResourceDefinition coinResource;

    [Header("추가 반영 메서드 (선택)")]
    [Tooltip("로드 후 플레이어 체력 UI 갱신용 메서드 이름 (없으면 비워둠)")]
    [SerializeField] private string playerHpRefreshMethodName = "";

    [Tooltip("로드 후 골드 UI 갱신용 메서드 이름 (없으면 비워둠)")]
    [SerializeField] private string goldRefreshMethodName = "";

    [Tooltip("로드 후 집 외형 갱신용 메서드 이름 (없으면 비워둠)")]
    [SerializeField] private string houseRefreshMethodName = "";

    [Header("디버그")]
    [SerializeField] private bool showDebugLog = true;

    [Header("새 게임 기본 시작 위치")]
    [SerializeField] private Transform defaultSpawnPoint;

    private void Start()
    {
        if (loadOnStart)
            StartCoroutine(LoadRoutine());
    }

    /// <summary>
    /// 씬 시작 후 잠깐 기다렸다가 저장 데이터를 로드합니다.
    /// 다른 오브젝트 초기화가 끝난 뒤 적용하려고 코루틴 사용.
    /// </summary>
    private IEnumerator LoadRoutine()
    {
        if (loadDelay > 0f)
            yield return new WaitForSeconds(loadDelay);
        else
            yield return null;

        LoadFromLastSelectedSlot();
    }

    /// <summary>
    /// 마지막 선택 슬롯 기준으로 저장 데이터를 불러옵니다.
    /// 저장이 없으면 기본 시작 위치로 이동합니다.
    /// </summary>
    public void LoadFromLastSelectedSlot()
    {
        int slot = PlayerPrefs.GetInt("LastSelectedSlot", 0);

        if (slot <= 0 || !SaveManager.HasSave(slot))
        {
            Log("유효한 저장 슬롯이 없어 기본 시작 위치를 사용합니다.");
            MovePlayerToDefaultSpawnPoint();
            return;
        }

        SaveData data = SaveManager.Load(slot);

        if (data == null)
        {
            Log($"로드 실패: 슬롯 {slot} 저장 데이터가 null입니다.");
            MovePlayerToDefaultSpawnPoint();
            return;
        }

        ApplySaveData(data);
        Log($"로드 완료: 슬롯 {slot}");
    }

    /// <summary>
    /// 저장 데이터를 실제 게임 오브젝트에 반영합니다.
    /// </summary>
    public void ApplySaveData(SaveData data)
    {
        if (data == null)
        {
            Log("ApplySaveData 실패: data가 null입니다.");
            return;
        }

        ApplyPlayerPosition(data);
        ApplyPlayerHp(data);
        ApplyGold(data);
        ApplyHouseLevel(data);
        ApplyResources(data);
    }

    /// <summary>
    /// 현재 게임 상태를 마지막 선택 슬롯에 저장합니다.
    /// </summary>
    public void SaveCurrentGame()
    {
        int slot = PlayerPrefs.GetInt("LastSelectedSlot", 0);

        if (slot <= 0)
        {
            Log("저장 실패: LastSelectedSlot 값이 없습니다.");
            return;
        }

        SaveCurrentGameToSlot(slot);
    }

    /// <summary>
    /// 특정 슬롯에 현재 게임 상태를 저장합니다.
    /// </summary>
    public void SaveCurrentGameToSlot(int slot)
    {
        SaveData data = new SaveData();

        // 닉네임
        data.nickname = PlayerPrefs.GetString("PlayerNickname", "플레이어");

        // 플레이어 위치
        if (playerTransform != null)
        {
            data.posX = playerTransform.position.x;
            data.posY = playerTransform.position.y;
            data.posZ = playerTransform.position.z;
        }
        else if (defaultSpawnPoint != null)
        {
            data.posX = defaultSpawnPoint.position.x;
            data.posY = defaultSpawnPoint.position.y;
            data.posZ = defaultSpawnPoint.position.z;
        }

        // 플레이어 체력
        data.playerHp = GetFloatValue(playerHpTarget, playerHpFieldName, 100f);

        // 골드
        data.gold = GetIntValue(goldTarget, goldFieldName, 0);

        // 집 레벨
        data.houseLevel = GetIntValue(houseLevelTarget, houseLevelFieldName, 1);

        // 자원
        if (resourceInventoryTarget != null)
        {
            data.wood = resourceInventoryTarget.GetAmount(woodResource);
            data.stone = resourceInventoryTarget.GetAmount(stoneResource);
            data.iron = resourceInventoryTarget.GetAmount(ironResource);
            data.coin = resourceInventoryTarget.GetAmount(coinResource);
        }
        else
        {
            Log("저장 시 자원 생략: resourceInventoryTarget이 비어 있습니다.");
        }

        SaveManager.Save(slot, data);
        Log($"현재 게임 저장 완료: 슬롯 {slot} / wood={data.wood}, stone={data.stone}, iron={data.iron}, coin={data.coin}");
    }

    #region 적용 함수

    private void ApplyPlayerPosition(SaveData data)
    {
        if (playerTransform == null)
        {
            Log("플레이어 위치 적용 생략: playerTransform이 비어 있습니다.");
            return;
        }

        Vector3 loadPosition = new Vector3(data.posX, data.posY, data.posZ);

        // 저장 좌표가 0,0,0이면 기본 시작 위치 사용
        if (loadPosition == Vector3.zero && defaultSpawnPoint != null)
        {
            loadPosition = defaultSpawnPoint.position;
            Log("저장 좌표가 0,0,0 이라 기본 시작 위치를 사용합니다.");
        }

        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        CapsuleCollider capsule = playerTransform.GetComponent<CapsuleCollider>();

        // 캡슐 높이만큼 위로 보정
        if (capsule != null)
        {
            float halfHeight = capsule.height * 0.5f;
            loadPosition.y += halfHeight + 0.1f;
        }
        else
        {
            loadPosition.y += 1f;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            bool prevKinematic = rb.isKinematic;
            rb.isKinematic = true;
            rb.position = loadPosition;
            rb.isKinematic = prevKinematic;
        }
        else
        {
            playerTransform.position = loadPosition;
        }

        Log($"플레이어 위치 적용: {loadPosition}");
    }

    private void ApplyPlayerHp(SaveData data)
    {
        if (playerHpTarget == null)
        {
            Log("플레이어 체력 적용 생략: playerHpTarget이 비어 있습니다.");
            return;
        }

        bool success = SetFloatValue(playerHpTarget, playerHpFieldName, data.playerHp);

        if (!success)
        {
            Log($"플레이어 체력 적용 실패: 변수명 [{playerHpFieldName}] 확인 필요");
            return;
        }

        TryInvokeMethod(playerHpTarget, playerHpRefreshMethodName);
        Log($"플레이어 체력 적용: {data.playerHp}");
    }

    private void ApplyGold(SaveData data)
    {
        if (goldTarget == null)
        {
            Log("골드 적용 생략: goldTarget이 비어 있습니다.");
            return;
        }

        bool success = SetIntValue(goldTarget, goldFieldName, data.gold);

        if (!success)
        {
            Log($"골드 적용 실패: 변수명 [{goldFieldName}] 확인 필요");
            return;
        }

        TryInvokeMethod(goldTarget, goldRefreshMethodName);
        Log($"골드 적용: {data.gold}");
    }

    private void ApplyHouseLevel(SaveData data)
    {
        if (houseLevelTarget == null)
        {
            Log("집 레벨 적용 생략: houseLevelTarget이 비어 있습니다.");
            return;
        }

        bool success = SetIntValue(houseLevelTarget, houseLevelFieldName, data.houseLevel);

        if (!success)
        {
            Log($"집 레벨 적용 실패: 변수명 [{houseLevelFieldName}] 확인 필요");
            return;
        }

        TryInvokeMethod(houseLevelTarget, houseRefreshMethodName);
        Log($"집 레벨 적용: {data.houseLevel}");
    }

    private void ApplyResources(SaveData data)
    {
        if (resourceInventoryTarget == null)
        {
            Log("자원 적용 생략: resourceInventoryTarget이 비어 있습니다.");
            return;
        }

        SetResource(resourceInventoryTarget, woodResource, data.wood);
        SetResource(resourceInventoryTarget, stoneResource, data.stone);
        SetResource(resourceInventoryTarget, ironResource, data.iron);
        SetResource(resourceInventoryTarget, coinResource, data.coin);

        Log($"자원 적용 완료: wood={data.wood}, stone={data.stone}, iron={data.iron}, coin={data.coin}");
    }

    private void MovePlayerToDefaultSpawnPoint()
    {
        if (playerTransform == null)
        {
            Log("기본 시작 위치 적용 실패: playerTransform이 비어 있습니다.");
            return;
        }

        if (defaultSpawnPoint == null)
        {
            Log("기본 시작 위치 적용 실패: defaultSpawnPoint가 비어 있습니다.");
            return;
        }

        Vector3 spawnPosition = defaultSpawnPoint.position;

        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        CapsuleCollider capsule = playerTransform.GetComponent<CapsuleCollider>();

        if (capsule != null)
        {
            float halfHeight = capsule.height * 0.5f;
            spawnPosition.y += halfHeight + 0.1f;
        }
        else
        {
            spawnPosition.y += 1f;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            bool prevKinematic = rb.isKinematic;
            rb.isKinematic = true;
            rb.position = spawnPosition;
            rb.isKinematic = prevKinematic;
        }
        else
        {
            playerTransform.position = spawnPosition;
        }

        Log($"기본 시작 위치 적용: {spawnPosition}");
    }

    private void SetResource(ResourceInventory inv, ResourceDefinition res, int targetAmount)
    {
        if (inv == null || res == null)
            return;

        int currentAmount = inv.GetAmount(res);

        if (targetAmount > currentAmount)
        {
            inv.Add(res, targetAmount - currentAmount);
        }
        else if (targetAmount < currentAmount)
        {
            inv.Spend(res, currentAmount - targetAmount);
        }
    }

    #endregion

    #region Reflection Set/Get

    private bool SetIntValue(MonoBehaviour target, string memberName, int value)
    {
        if (target == null || string.IsNullOrWhiteSpace(memberName))
            return false;

        System.Type type = target.GetType();

        FieldInfo field = type.GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            if (field.FieldType == typeof(int))
            {
                field.SetValue(target, value);
                return true;
            }

            if (field.FieldType == typeof(float))
            {
                field.SetValue(target, (float)value);
                return true;
            }
        }

        PropertyInfo property = type.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property != null && property.CanWrite)
        {
            if (property.PropertyType == typeof(int))
            {
                property.SetValue(target, value);
                return true;
            }

            if (property.PropertyType == typeof(float))
            {
                property.SetValue(target, (float)value);
                return true;
            }
        }

        return false;
    }

    private bool SetFloatValue(MonoBehaviour target, string memberName, float value)
    {
        if (target == null || string.IsNullOrWhiteSpace(memberName))
            return false;

        System.Type type = target.GetType();

        FieldInfo field = type.GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            if (field.FieldType == typeof(float))
            {
                field.SetValue(target, value);
                return true;
            }

            if (field.FieldType == typeof(int))
            {
                field.SetValue(target, Mathf.RoundToInt(value));
                return true;
            }
        }

        PropertyInfo property = type.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property != null && property.CanWrite)
        {
            if (property.PropertyType == typeof(float))
            {
                property.SetValue(target, value);
                return true;
            }

            if (property.PropertyType == typeof(int))
            {
                property.SetValue(target, Mathf.RoundToInt(value));
                return true;
            }
        }

        return false;
    }

    private int GetIntValue(MonoBehaviour target, string memberName, int defaultValue)
    {
        if (target == null || string.IsNullOrWhiteSpace(memberName))
            return defaultValue;

        System.Type type = target.GetType();

        FieldInfo field = type.GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            object value = field.GetValue(target);

            if (value is int intValue)
                return intValue;

            if (value is float floatValue)
                return Mathf.RoundToInt(floatValue);
        }

        PropertyInfo property = type.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property != null && property.CanRead)
        {
            object value = property.GetValue(target);

            if (value is int intValue)
                return intValue;

            if (value is float floatValue)
                return Mathf.RoundToInt(floatValue);
        }

        return defaultValue;
    }

    private float GetFloatValue(MonoBehaviour target, string memberName, float defaultValue)
    {
        if (target == null || string.IsNullOrWhiteSpace(memberName))
            return defaultValue;

        System.Type type = target.GetType();

        FieldInfo field = type.GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            object value = field.GetValue(target);

            if (value is float floatValue)
                return floatValue;

            if (value is int intValue)
                return intValue;
        }

        PropertyInfo property = type.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property != null && property.CanRead)
        {
            object value = property.GetValue(target);

            if (value is float floatValue)
                return floatValue;

            if (value is int intValue)
                return intValue;
        }

        return defaultValue;
    }

    #endregion

    #region 보조 함수

    private void TryInvokeMethod(MonoBehaviour target, string methodName)
    {
        if (target == null || string.IsNullOrWhiteSpace(methodName))
            return;

        System.Type type = target.GetType();

        MethodInfo method = type.GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null)
        {
            Log($"메서드 호출 생략: [{methodName}] 없음");
            return;
        }

        method.Invoke(target, null);
        Log($"메서드 호출 성공: {methodName}");
    }

    private void Log(string message)
    {
        if (showDebugLog)
            Debug.Log($"[SaveLoadController] {message}");
    }

    #endregion
}