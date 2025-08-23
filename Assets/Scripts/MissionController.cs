using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public static MissionController Instance { get; private set; }

    public List<MissionData> allPossibleMissions;

    [Header("Debug")]
    [SerializeField] private bool debugForceShortMissions = true;
    [SerializeField] private int debugMissionSeconds = 30;

    [Header("Runtime")]
    [SerializeField] private float missionPollIntervalSeconds = 0.5f;
    private Coroutine missionPollRoutine;

    // NEW: Prevent double-processing the same missionId from rapid/double clicks
    private readonly HashSet<string> processingClaims = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllMissions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (missionPollRoutine == null)
        {
            missionPollRoutine = StartCoroutine(MissionReadyPoller());
        }
    }

    void OnDisable()
    {
        if (missionPollRoutine != null)
        {
            StopCoroutine(missionPollRoutine);
            missionPollRoutine = null;
        }
    }

    void Start()
    {
        CheckForCompletedMissions();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            CheckForCompletedMissions();
        }
    }

    private void LoadAllMissions()
    {
        allPossibleMissions = Resources.LoadAll<MissionData>("Missions").ToList();
        Debug.Log($"Loaded {allPossibleMissions.Count} missions from Resources.");
    }

    public void StartMission(MissionData missionData, List<Survivor> assignedSurvivors)
    {
        if (missionData == null)
        {
            Debug.LogError("StartMission called with null missionData.");
            return;
        }
        if (assignedSurvivors == null || assignedSurvivors.Count == 0)
        {
            Debug.LogWarning("StartMission called with no survivors. Aborting.");
            return;
        }
        if (GameDataManager.Instance == null || GameDataManager.Instance.gameData == null)
        {
            Debug.LogError("StartMission: GameDataManager or gameData is null.");
            return;
        }

        float finalSuccessChance = missionData.baseSuccessChance;
        if (assignedSurvivors.Count > 1)
        {
            finalSuccessChance += (assignedSurvivors.Count - 1) * missionData.bonusSuccessChancePerSurvivor;
        }
        foreach (var survivor in assignedSurvivors)
        {
            if (survivor == null || survivor.traits == null) continue;
            foreach (var trait in survivor.traits.Where(t => t != null))
            {
                finalSuccessChance += trait.successChanceModifier;
            }
        }
        finalSuccessChance = Mathf.Clamp01(finalSuccessChance);

        ActiveMission newMission = new ActiveMission
        {
            missionId = Guid.NewGuid().ToString(),
            missionDataName = missionData.name,
            assignedSurvivorIds = assignedSurvivors.Where(s => s != null).Select(s => s.survivorId).ToList(),
            finalSuccessChance = finalSuccessChance,
            isReadyToClaim = false
        };

        double durationInSeconds = debugForceShortMissions ? debugMissionSeconds : missionData.durationHours * 3600.0;
        DateTime endTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
        newMission.missionEndTimeTicks = endTime.Ticks;

        foreach (var survivor in assignedSurvivors.Where(s => s != null))
        {
            survivor.status = SurvivorStatus.OnMission;
            survivor.assignedMissionId = newMission.missionId;
        }

        GameDataManager.Instance.gameData.activeMissions.Add(newMission);
        GameDataManager.Instance.SaveGame();

        Debug.Log($"Started mission '{missionData.missionName}' with {assignedSurvivors.Count} survivors. Success chance: {finalSuccessChance * 100}%. Ends at: {endTime}");
    }

    private IEnumerator MissionReadyPoller()
    {
        var wait = new WaitForSecondsRealtime(missionPollIntervalSeconds);
        while (true)
        {
            CheckForCompletedMissions();
            yield return wait;
        }
    }

    private void CheckForCompletedMissions()
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.gameData == null) return;

        bool anyChanged = false;
        long now = DateTime.UtcNow.Ticks;

        foreach (var mission in GameDataManager.Instance.gameData.activeMissions)
        {
            if (!mission.isReadyToClaim && now >= mission.missionEndTimeTicks)
            {
                mission.isReadyToClaim = true;
                anyChanged = true;
            }
        }

        if (anyChanged)
        {
            GameDataManager.Instance.SaveGame();
        }
    }

    public void ClaimMission(string missionId)
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.gameData == null)
        {
            Debug.LogError("ClaimMission: GameData not available.");
            return;
        }

        var mission = GameDataManager.Instance.gameData.activeMissions.FirstOrDefault(m => m.missionId == missionId);
        if (mission == null)
        {
            Debug.LogWarning($"ClaimMission: missionId {missionId} not found.");
            return;
        }

        // Ensure we don't double-process the same missionId concurrently
        if (!processingClaims.Add(missionId))
        {
            Debug.Log($"ClaimMission: Already processing missionId {missionId}, ignoring duplicate click.");
            return;
        }

        try
        {
            // If player claims slightly after timer but before poller tick, force ready
            if (!mission.isReadyToClaim && DateTime.UtcNow.Ticks >= mission.missionEndTimeTicks)
            {
                mission.isReadyToClaim = true;
            }

            if (!mission.isReadyToClaim)
            {
                Debug.Log("ClaimMission: Mission not ready yet.");
                return;
            }

            ProcessMissionResult(mission);
        }
        finally
        {
            processingClaims.Remove(missionId);
        }
    }

    private void ProcessMissionResult(ActiveMission mission)
    {
        MissionData missionData = allPossibleMissions.FirstOrDefault(m => m.name == mission.missionDataName);
        if (missionData == null)
        {
            Debug.LogError($"Could not find MissionData for completed mission: {mission.missionDataName}");
            return;
        }

        bool wasSuccessful = UnityEngine.Random.value < mission.finalSuccessChance;

        if (wasSuccessful)
        {
            int rewardAmount = missionData.baseRewardAmount;
            if (missionData.rewardType == MissionRewardType.Materials)
            {
                GameDataManager.Instance.gameData.materials += rewardAmount;
                Debug.Log($"Mission '{missionData.missionName}' SUCCEEDED! +{rewardAmount} Materials. Total Materials: {GameDataManager.Instance.gameData.materials}");
            }
            else
            {
                GameDataManager.Instance.AddScrap(rewardAmount);
                Debug.Log($"Mission '{missionData.missionName}' SUCCEEDED! +{rewardAmount} Scrap. Total Scrap: {GameDataManager.Instance.gameData.totalScrap}");
            }
        }
        else
        {
            Debug.Log($"Mission '{missionData.missionName}' FAILED...");
            // Failure consequences can be added here (Task 1.2: Survivor death mechanic)
        }

        // Cleanup assigned survivors
        foreach (var survivorId in mission.assignedSurvivorIds)
        {
            var survivor = GameDataManager.Instance.gameData.sanctuarySurvivors.FirstOrDefault(s => s.survivorId == survivorId);
            if (survivor != null)
            {
                survivor.status = SurvivorStatus.Idle;
                survivor.assignedMissionId = string.Empty;
            }
        }

        // Remove mission and save
        GameDataManager.Instance.gameData.activeMissions.Remove(mission);
        GameDataManager.Instance.SaveGame();
    }
}