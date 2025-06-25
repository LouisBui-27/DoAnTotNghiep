using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public static PlayerSkillManager Instance;
    public List<SkillRuntimeData> equippedSkills = new List<SkillRuntimeData>();
    public int extraBullets = 0;
    public float explosionDamage = 5f;
    public float explosionScale = 4f;
    public float critChance = 0f;

    public int currentAttackSkillCount = 0;
    public int currentSupportSkillCount = 0;

    // Các giới hạn slot
    private const int MAX_ATTACK_SKILLS = 5;
    private const int MAX_SUPPORT_SKILLS = 5;

    [Header("Settings Prefabs")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private GameObject LightningPrefab;
    [SerializeField] private GameObject PetPrefab;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private GameObject orbitingObjectPrefab;
    [SerializeField] private GameObject whipPrefab;
    [SerializeField] private GameObject tornadoPrefab;
    [SerializeField] private GameObject auraPrefab;
    private GameObject auraInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(gameObject);
        }    
    }
    public void EquipSkill(SkillData skillData)
    {
        SkillRuntimeData existing = equippedSkills.Find(s => s.baseSkill.skillType == skillData.skillType);
        if (existing != null)
        {
            if (!existing.IsMaxLevel())
            {
                existing.currentLevel++;
                ApplySkillEffect(existing.baseSkill.skillType);
            }
            else
            {
                Debug.Log($"{existing.baseSkill.skillName} đã đạt cấp tối đa!");
            }
        }
        else
        {
            // Kiểm tra slot trước khi thêm skill mới
            if (skillData.skillCategory == SkillCategory.Attack && currentAttackSkillCount >= MAX_ATTACK_SKILLS)
            {
                Debug.Log($"Đã đạt giới hạn skill tấn công ({MAX_ATTACK_SKILLS})");
                return; // Không thêm skill mới nếu đã đủ slot
            }
            if (skillData.skillCategory == SkillCategory.Support && currentSupportSkillCount >= MAX_SUPPORT_SKILLS)
            {
                Debug.Log($"Đã đạt giới hạn skill hỗ trợ ({MAX_SUPPORT_SKILLS})");
                return; // Không thêm skill mới nếu đã đủ slot
            }

            SkillRuntimeData newRuntime = new SkillRuntimeData(skillData);
            equippedSkills.Add(newRuntime);

            // Tăng số lượng skill đã trang bị
            if (skillData.skillCategory == SkillCategory.Attack)
            {
                currentAttackSkillCount++;
            }
            else // Support
            {
                currentSupportSkillCount++;
            }

            ApplySkillEffect(skillData.skillType);
        }
    }

    // Thêm hàm kiểm tra xem có đủ slot trống cho skill mới hay không
    public bool HasAvailableSlot(SkillCategory category)
    {
        if (category == SkillCategory.Attack)
        {
            return currentAttackSkillCount < MAX_ATTACK_SKILLS;
        }
        else // Support
        {
            return currentSupportSkillCount < MAX_SUPPORT_SKILLS;
        }
    }

    // Thêm hàm kiểm tra tổng số slot đã đầy
    public bool AllSlotsFull()
    {
        return currentAttackSkillCount >= MAX_ATTACK_SKILLS && currentSupportSkillCount >= MAX_SUPPORT_SKILLS;
    }
    public void ApplySkillEffect(SkillType type)
    {
        int level = GetSkillLevel(type);
        switch (type)
        {
            case SkillType.IncreaseBulletCount:
                extraBullets += 1;
                break;
            case SkillType.ArrowShoot:
                ActivateArrowShooter();
                break;
            case SkillType.IncreaseHealth:
                IncreaseHealth();
                break;
            case SkillType.RocketShoot:
                ActivateRocketShooter();
                break;
            case SkillType.Boomerang:
                ActivateBoomerangShooter();
                break;
            case SkillType.Heal:
                ActiveHealSkill();
                break;
            case SkillType.Lightning:
                ActiveLighningSkill();
                break;
            case SkillType.Aura:
                ActivateAura();
                break;
            case SkillType.shootBullet:
                ActivateBulletShooter();
                break; 
            case SkillType.OrbitingWeapon:
                ActivateOrbitingWeapon();
                break;
            case SkillType.SpeedBoost:
                ActivateSpeedBoost(level);
                break;
            case SkillType.Whip:
                ActivateWhipSkill();
                break;
            case SkillType.IncreasePickupRange:
                ActivatePickupRadiusSkill();
                break;
            case SkillType.IncreaseCritChance:
                ApplyCritChanceBoost(level);
                break;
            case SkillType.Tornado:
                ActivateTornadoSkill();
                break;
        }
    }
    void ActivateArrowShooter()
    {
        ArrowShooter arrowShooter = GetComponent<ArrowShooter>();
        if (arrowShooter == null)
        {
            arrowShooter = gameObject.AddComponent<ArrowShooter>();
            arrowShooter.arrowPrefab = arrowPrefab;
            arrowShooter.shootInterval = 1.5f;
        }
        int level = GetSkillLevel(SkillType.ArrowShoot);
        Debug.Log(level);
        int newDamage = 10 + (level - 1) * 5;  // Ví dụ, mỗi cấp tăng thêm 5 damage
        float newSpeed = 10f + (level-1) * 2;  // Ví dụ, mỗi cấp tăng thêm 2 speed

        arrowShooter.UpdateArrowStats(newDamage, newSpeed);
    }
    void ActivateBulletShooter()
    {
        BulletShooter bulletShooter = GetComponent<BulletShooter>();
        if (bulletShooter == null)
        {
            bulletShooter = gameObject.AddComponent<BulletShooter>();
            bulletShooter.bulletPrefab = bulletPrefabs;
            bulletShooter.attackRate = 1f;
        }
        int level = GetSkillLevel(SkillType.shootBullet);
        Debug.Log(level);
        int newDamage = 10 + (level - 1) * 5; // Ví dụ, mỗi cấp tăng thêm 5 damage
        float newSpeed = 7f + (level - 1) * 2;  // Ví dụ, mỗi cấp tăng thêm 2 speed

        bulletShooter.UpdateArrowStats(newDamage, newSpeed);
    }
    void ActivateRocketShooter()
    {
        RocketShooter rocketShooter = GetComponent<RocketShooter>();
        if(rocketShooter == null)
        {
            rocketShooter = gameObject.AddComponent<RocketShooter>();
            rocketShooter.rocketPrefab = rocketPrefab;
            rocketShooter.shootInterval = 3f;
        }
        int level = GetSkillLevel(SkillType.RocketShoot);
        int damage = 30 +(level-1) * 10;
        int explosionDamage = 5 + (level-1) * 3;
        float explosionScale = 4f + (level - 1) * 0.3f;

        PlayerSkillManager.Instance.explosionDamage = explosionDamage;
        PlayerSkillManager.Instance.explosionScale = explosionScale;
        rocketShooter.UpdateRocketStats(damage);
    }
    void ActivateBoomerangShooter()
    {
        BoomerangShooter shooter = GetComponent<BoomerangShooter>();
        if (shooter == null)
        {
            shooter = gameObject.AddComponent<BoomerangShooter>();
            shooter.boomerangPrefab = boomerangPrefab;
            shooter.shootInterval = 3f;
        }

        int level = GetSkillLevel(SkillType.Boomerang);
        shooter.damage = 20 + (level - 1) * 10;
        shooter.moveSpeed = 7f + (level - 1) * 1.25f;
        shooter.returnSpeed = 10f + (level - 1) * 1.25f;
        shooter.maxDistance = 5f + (level - 1) * 1.25f;
    }
    void ActiveLighningSkill()
    {
        LightningShooter shooter = GetComponent<LightningShooter>();
        if (shooter == null)
        {
            shooter = gameObject.AddComponent<LightningShooter>();
            shooter.lightningPrefab = LightningPrefab; // drag prefab từ editor hoặc gán ở đây
            shooter.shootInterval = 5f;
        }
        int level = GetSkillLevel(SkillType.Lightning);
        shooter.damage = 2f + (level - 1) * 1f;
        float scaleFactor = 4f + (level - 1) * 0.3f;
        shooter.lightningPrefab.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        //Debug.Log("Lightning damage: " + shooter.damage);
    }
    void ActivateAura()
    {
        if (auraInstance == null)
        {
            auraInstance = Instantiate(auraPrefab, transform.position, Quaternion.identity, transform);
        }

        int level = GetSkillLevel(SkillType.Aura);

        float damage = 10f + (level - 1) * 5f;
        float lifesteal = (level >= 4) ? (0.2f + (level - 4) * 0.05f) : 0f;
        float radius = 7.5f + (level - 1) * 1f;

        Aura auraComponent = auraInstance.GetComponent<Aura>();
        if (auraComponent != null)
        {
            auraComponent.SetStats(damage, lifesteal, radius);
        }
    }
    void ActivateOrbitingWeapon()
    {
        OrbitingWeapon orbitingWeapon = GetComponent<OrbitingWeapon>();
        if (orbitingWeapon == null)
        {
            orbitingWeapon = gameObject.AddComponent<OrbitingWeapon>();
            orbitingWeapon.orbitingPrefab = orbitingObjectPrefab;
        }

        int level = GetSkillLevel(SkillType.OrbitingWeapon);

        int objectCount = 1 + (level - 1); // Mỗi cấp +1 vật thể
        float damage = 10f + (level - 1) * 5f;
        float appearTime =   3f + (level - 1) * 1f;
        float disappearTime = Mathf.Max(1.5f, 7f - (level - 1) * 0.5f);

        orbitingWeapon.UpdateSkill(objectCount, damage, appearTime, disappearTime);
    }
    void ActivateWhipSkill()
    {
        WhipShooter shooter = GetComponent<WhipShooter>();
        if (shooter == null)
        {
            shooter = gameObject.AddComponent<WhipShooter>();
            shooter.whipPrefab = whipPrefab;
            shooter.baseAttackRate = 1.5f;
            shooter.baseDamage = 10f;
            shooter.damageIncreasePerLevel = 2f;
            shooter.attackRateReductionPerLevel = 0.1f;
            shooter.maxLevel = 5;
        }

        int level = GetSkillLevel(SkillType.Whip);
        shooter.UpdateWhipLevel(level);
    }
    void ActivateTornadoSkill()
    {
        TornadoSpawner spawner = GetComponent<TornadoSpawner>();
        if (spawner == null)
        {
            spawner = gameObject.AddComponent<TornadoSpawner>();
            spawner.tornadoPrefab = tornadoPrefab;
            spawner.spawnInterval = 3f;
        }

        int level = GetSkillLevel(SkillType.Tornado);
        float damage = 10f + (level - 1) * 5f;
        float speed = 1f + (level - 1) * 1f;
        float spawn = 8f - (level - 1) * 1f;
        int count = 1 + (level - 1); // Triệu hồi nhiều lốc hơn mỗi cấp

        spawner.UpdateTornadoStats(damage, speed, count,spawn);
    }

    void SummenPet()
    {
        Vector3 spawnPos = transform.position + new Vector3(1f, 0f, 0f); // bên cạnh player
        GameObject pet = Instantiate(PetPrefab, spawnPos, Quaternion.identity);
        PetController petController = pet.GetComponent<PetController>();
        if (petController != null)
        {
            petController.SetOwner(transform); // gán player làm chủ
        }
    }

    void IncreaseHealth()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            int level = GetSkillLevel(SkillType.IncreaseHealth);
            Debug.Log(level);
            float healthExtra = 100 * level;
            playerHealth.setMaxHealth(playerHealth.baseHealth + healthExtra);
        }
    }
    void ActiveHealSkill()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            float amount = playerHealth.maxHP * 0.25f;
            playerHealth.Heal(amount);
        }
    }
    public void ActivateSpeedBoost(int level)
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Mỗi cấp tăng tốc độ lên 20% (có thể điều chỉnh theo yêu cầu)
            float speedMultiplier = 1f + (level) * 0.2f;
            playerController.ActivateSpeedBoost(speedMultiplier);
        }
    }
    void ActivatePickupRadiusSkill()
    {
        ItemCollector collector = GetComponent<ItemCollector>();
        if (collector == null) 
        {
            collector = gameObject.AddComponent<ItemCollector>();
            collector.basePickupRadius = 0.5f;
        }
        if (collector != null)
        {
            int level = GetSkillLevel(SkillType.IncreasePickupRange);
            float newRadius = 0.5f + (level - 1) * 0.2f; // mỗi cấp +1 bán kính
            collector.UpdatePickupRadius(newRadius);
        }
    }
    void ApplyCritChanceBoost(int level)
    {
        // Mỗi cấp +5% tỷ lệ chí mạng
        critChance = level * 0.05f; // 5% mỗi cấp
        Debug.Log($"Tỷ lệ chí mạng hiện tại: {critChance * 100}%");
    }
    public bool IsSkillMaxLevel(SkillData skillData)
    {
        SkillRuntimeData runtimeSkill = equippedSkills
            .Find(s => s.baseSkill == skillData);

        return runtimeSkill != null && runtimeSkill.IsMaxLevel();
    }
    public void UpgradeCritChance(float newCrit)
    {
        critChance = newCrit;
    }
    int GetSkillLevel(SkillType skillType)
    {
        SkillRuntimeData runtime = equippedSkills.Find(s => s.baseSkill.skillType == skillType);
        return runtime != null ? runtime.currentLevel : 1;
    }
    public int CountSkillsByCategory(SkillCategory category)
    {
        return equippedSkills.FindAll(s => s.baseSkill.skillCategory == category).Count;
    }
    public void PreEquipStartingSkill(SkillData skillData, int initialLevel)
    {
        // Đảm bảo skill này chưa có trong equippedSkills (tránh trùng lặp khi khởi tạo)
        if (equippedSkills.Find(s => s.baseSkill.skillType == skillData.skillType) != null)
        {
            Debug.LogWarning($"Kỹ năng khởi đầu {skillData.skillName} đã tồn tại trong equippedSkills. Bỏ qua việc thêm.");
            return;
        }

        SkillRuntimeData newRuntime = new SkillRuntimeData(skillData);
        newRuntime.currentLevel = initialLevel; // Set level khởi tạo
        equippedSkills.Add(newRuntime);

        // Tăng số lượng skill đã trang bị theo category
        if (skillData.skillCategory == SkillCategory.Attack)
        {
            currentAttackSkillCount++;
        }
        else // Support
        {
            currentSupportSkillCount++;
        }
        Debug.Log($"Đã thêm kỹ năng khởi đầu: {skillData.skillName} (Level: {initialLevel}, Loại: {skillData.skillCategory}). Attack: {currentAttackSkillCount}, Support: {currentSupportSkillCount}");

        // Áp dụng hiệu ứng skill
        ApplySkillEffect(skillData.skillType);
    }
}
