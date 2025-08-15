using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("In-Run Stats")]
    public int survivorCount = 1;
    public float bulletDamage = 10f;
    public int scrapCount = 0;
    public int zombiesKilled = 0;

    [Header("Fire Rate")]
    public float baseFireRate = 2f;
    public float fireRateBonusPerSurvivor = 0.2f;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public UIManager uiManager;
    public PlayerVisuals playerVisuals;
    public GameObject shieldVisualPrefab;

    private float fireCooldown = 0f;
    private GameObject activeShieldVisual;
    public int shieldCount { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    void Start()
    {
        if (GameDataManager.Instance != null)
        {
            survivorCount = GameDataManager.Instance.gameData.startingSurvivorsUpgradeLevel;
            float damageBonus = (GameDataManager.Instance.gameData.damageUpgradeLevel - 1) * 5f;
            bulletDamage += damageBonus;
            shieldCount = GameDataManager.Instance.gameData.firstAidKitLevel;

            // Reset the in-run survivor data at the start of a new run.
            GameDataManager.Instance.gameData.survivorsRescuedThisRun = 0;
            GameDataManager.Instance.gameData.rescuedSurvivorArchetype = null;
        }

        // ... (rest of Start method is unchanged)
        #region Unchanged Start Logic
        UpdateShieldVisual();

        if (playerVisuals != null)
        {
            for (int i = 0; i < survivorCount - 1; i++)
            {
                playerVisuals.AddVisualSurvivor();
            }
        }

        if (uiManager != null)
        {
            uiManager.UpdateSurvivorCount(survivorCount);
            uiManager.UpdateScrapCount(scrapCount);
        }
        #endregion
    }

    void OnDestroy()
    {
        if (GameDataManager.Instance != null)
        {
            // --- Final Survivor Rescue & Pity Timer Logic ---
            if (GameDataManager.Instance.gameData.rescuedSurvivorArchetype != null)
            {
                Debug.Log($"Run ended. Processing rescued survivor: {GameDataManager.Instance.gameData.rescuedSurvivorArchetype.archetypeName}");
                SanctuaryManager.Instance.CreateAndAddSurvivor(GameDataManager.Instance.gameData.rescuedSurvivorArchetype);
            }
            else
            {
                Debug.Log("Run ended. No survivor was rescued. Increasing pity timer.");
                GameDataManager.Instance.gameData.survivorPityChance += 0.05f; // Increase by 5%
            }

            // --- Data Cleanup ---
            GameDataManager.Instance.gameData.survivorsRescuedThisRun = 0;
            GameDataManager.Instance.gameData.rescuedSurvivorArchetype = null;

            // --- Scrap Calculation ---
            int baseReward = 20;
            int bonusTiers = zombiesKilled / 10;
            float killBonusMultiplier = 1.0f + (bonusTiers * 0.1f);
            int killScrap = Mathf.RoundToInt(zombiesKilled * killBonusMultiplier);
            int finalScrapTotal = baseReward + scrapCount + killScrap;

            if (finalScrapTotal > 0)
            {
                GameDataManager.Instance.AddScrap(finalScrapTotal);
            }

            // Save all changes at the very end of the run.
            GameDataManager.Instance.SaveGame();
        }
    }

    // ... (rest of script is unchanged)
    #region Unchanged Methods
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            int additionalSurvivors = survivorCount - 1;
            float currentFireRate = baseFireRate + (additionalSurvivors * fireRateBonusPerSurvivor);
            fireCooldown = 1f / currentFireRate;
        }
    }

    public void UseShield()
    {
        if (shieldCount > 0)
        {
            shieldCount--;
            UpdateShieldVisual();
        }
    }

    private void UpdateShieldVisual()
    {
        if (shieldCount > 0 && activeShieldVisual == null)
        {
            if (shieldVisualPrefab != null)
            {
                activeShieldVisual = Instantiate(shieldVisualPrefab, transform);
                activeShieldVisual.transform.localPosition = Vector3.zero;
            }
        }
        else if (shieldCount <= 0 && activeShieldVisual != null)
        {
            Destroy(activeShieldVisual);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<BulletLogic>().damage = bulletDamage;
        if (playerVisuals != null)
        {
            playerVisuals.TriggerMuzzleFlash();
        }
    }

    public void AddSurvivors(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            survivorCount++;
            if (playerVisuals != null)
            {
                playerVisuals.AddVisualSurvivor();
            }
        }
        if (uiManager != null)
        {
            uiManager.UpdateSurvivorCount(survivorCount);
        }
    }

    public void AddDamage(float amount)
    {
        bulletDamage += amount;
    }

    public void AddScrap(int amount)
    {
        scrapCount += amount;
        if (uiManager != null)
        {
            uiManager.UpdateScrapCount(scrapCount);
        }
    }
    #endregion
}
