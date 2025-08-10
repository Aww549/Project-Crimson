using UnityEngine;

/// <summary>
/// Manages the player's in-run stats and actions, like shooting and collecting items.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [Header("In-Run Stats")]
    public int survivorCount = 1;
    public float bulletDamage = 10f;
    public int scrapCount = 0; // Scrap collected *during this run*

    [Header("Fire Rate")]
    public float baseFireRate = 2f;
    public float fireRateBonusPerSurvivor = 0.2f;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public UIManager uiManager;

    private float fireCooldown = 0f;

    void Start()
    {
        // --- APPLY PERMANENT UPGRADES ---
        // At the start of the run, get the saved upgrade levels and apply them.
        if (GameDataManager.Instance != null)
        {
            // Set starting survivors based on the "Rally the Troops" upgrade.
            survivorCount = GameDataManager.Instance.StartingSurvivorsUpgradeLevel;

            // Set base bullet damage based on the "Fortify Ammunition" upgrade.
            // We can define the bonus formula here. Let's say +5 damage per level.
            float damageBonus = (GameDataManager.Instance.DamageUpgradeLevel - 1) * 5f;
            bulletDamage += damageBonus;
        }

        // Update the UI with the initial stats
        if (uiManager != null)
        {
            uiManager.UpdateSurvivorCount(survivorCount);
            uiManager.UpdateScrapCount(scrapCount);
        }
    }

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

    void OnDestroy()
    {
        // When the player is destroyed (Game Over), add the collected scrap to the total.
        if (GameDataManager.Instance != null && scrapCount > 0)
        {
            GameDataManager.Instance.AddScrap(scrapCount);
        }
    }

    void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        newBullet.GetComponent<BulletLogic>().damage = bulletDamage;
    }

    public void AddSurvivors(int amount)
    {
        survivorCount += amount;
        if (uiManager != null)
        {
            uiManager.UpdateSurvivorCount(survivorCount);
        }
    }

    public void AddDamage(float amount)
    {
        bulletDamage += amount;
        Debug.Log("Damage increased! New damage: " + bulletDamage);
    }

    public void AddScrap(int amount)
    {
        scrapCount += amount;
        if (uiManager != null)
        {
            uiManager.UpdateScrapCount(scrapCount);
        }
    }
}
