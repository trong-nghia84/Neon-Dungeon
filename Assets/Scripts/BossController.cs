using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class BossController : EnemyBase
{
    [Header("Boss Stats")]
    public float maxBossHealth = 500f;
    private float currentBossHealth;
    private int currentPhase = 1;

    [Header("Boss General Skills Settings")]
    public float skillCooldown = 4f; 
    private float skillTimer = 0f;

    [Header("Skill 1: Vạn Tiễn Xuyên Tâm")]
    public GameObject bossBulletPrefab;
    public int bulletCount = 8;

    [Header("Skill 2: Thiết Giáp Xung Kích")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    private bool isBossDashing = false;

    [Header("Skill 3: Triệu Hồi Thuộc Hạ (Cooldown 20s)")]
    public GameObject minionPrefab;
    public Transform[] spawnPoints;
    public float minionSkillCooldown = 20f; 
    private float minionCooldownTimer = 0f;  

    [Header("Cấu hình Phát hiện Player")]
    public float aggroRange = 8f;    
    public float deaggroRange = 14f;  
    public bool isAggroed = false;    

    private SpriteRenderer bossSprite;
    private bool isFacingRightBoss = false; 
    private BossHealthBarUI bossUI;         

    [Header("Cấu hình Video End Game tại chỗ")]
    public GameObject endGameVideoPanel;
    public float videoDuration = 10f;
    protected override void Awake()
    {
        base.Awake();
        currentBossHealth = maxBossHealth;
        bossSprite = GetComponent<SpriteRenderer>();

        minionCooldownTimer = 0f;

        bossUI = GameObject.FindObjectOfType<BossHealthBarUI>(true);
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || (switchManager != null && switchManager.isAllDead))
        {
            StopMovement();
            if (bossUI != null) bossUI.gameObject.SetActive(false); // Ẩn thanh máu khi kết thúc game
            return;
        }

        if (player != null) 
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!isAggroed && distanceToPlayer <= aggroRange)
            {
                isAggroed = true;

                if (bossUI != null)
                {
                    bossUI.SetupBossHealthBar(this, "CYBER SENTINEL");
                }
                Debug.Log("Player xâm nhập! Kích hoạt thực thể Boss.");
            }
            else if (isAggroed && distanceToPlayer >= deaggroRange)
            {
                isAggroed = false;
                StopMovement(); 

                if (bossUI != null)
                {
                    bossUI.gameObject.SetActive(false);
                }
                Debug.Log("Player chạy quá xa! Reset trạng thái ẩn của Boss.");
            }
        }
        else
        {
            if (isAggroed)
            {
                isAggroed = false;
                StopMovement();
                if (bossUI != null) bossUI.gameObject.SetActive(false);
            }
        }

        if (!isAggroed)
        {
            return;
        }

     

        if (minionCooldownTimer > 0f)
        {
            minionCooldownTimer -= Time.deltaTime;
        }

        skillTimer += Time.deltaTime;
        if (skillTimer >= skillCooldown && !isBossDashing)
        {
            ChooseRandomSkill();
            skillTimer = 0f;
        }

        if (!isBossDashing)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (direction.x > 0 && !isFacingRightBoss)
        {
            FlipBoss();
        }
        else if (direction.x < 0 && isFacingRightBoss)
        {
            FlipBoss();
        }
    }

    private void FlipBoss()
    {
        isFacingRightBoss = !isFacingRightBoss;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void ChooseRandomSkill()
    {
        if (currentPhase == 2) skillCooldown = 2.5f;

        int maxAttempts = 10;
        int chosenSkill = 1;

        for (int i = 0; i < maxAttempts; i++)
        {
            int randomSkill = Random.Range(1, 4);

            if (randomSkill == 3 && minionCooldownTimer > 0f)
            {
                continue;
            }

            chosenSkill = randomSkill;
            break;
        }

        switch (chosenSkill)
        {
            case 1:
                StartCoroutine(SkillNovaShot());
                break;
            case 2:
                StartCoroutine(SkillDashAttack());
                break;
            case 3:
                SkillSpawnMinions();
                break;
        }
    }

    private IEnumerator SkillNovaShot()
    {
        Debug.Log("Boss: Vạn Tiễn Xuyên Tâm!");
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulletDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulletDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector2 bulletMoveVector = new Vector2(bulletDirX, bulletDirY);
            Vector2 bulletDirection = (bulletMoveVector - (Vector2)transform.position).normalized;

            GameObject bullet = Instantiate(bossBulletPrefab, transform.position, Quaternion.identity);
            BulletManager bm = bullet.GetComponent<BulletManager>();
            if (bm != null)
            {
                bm.owner = BulletManager.OwnerType.Enemy;
                bm.Launch(bulletDirection);
            }

            angle += angleStep;
        }
    }

    private IEnumerator SkillDashAttack()
    {
        Debug.Log("Boss: Thiết Giáp Xung Kích!");
        isBossDashing = true;

        Vector2 dashDirection = (player.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        isBossDashing = false;
    }

    private void SkillSpawnMinions()
    {
        Debug.Log("Boss: Triệu Hồi Thuộc Hạ!");
        if (minionPrefab != null && spawnPoints.Length > 0)
        {
            foreach (Transform sp in spawnPoints)
            {
                Instantiate(minionPrefab, sp.position, Quaternion.identity);
            }
            minionCooldownTimer = minionSkillCooldown;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (isDead) return;

        if (!isAggroed)
        {
            isAggroed = true;
            if (bossUI != null) bossUI.SetupBossHealthBar(this, "CYBER SENTINEL");
        }

        currentBossHealth -= damage;
        Debug.Log($"Boss HP: {currentBossHealth}/{maxBossHealth}");

        if (currentPhase == 1 && currentBossHealth <= maxBossHealth / 2f)
        {
            EnterPhase2();
        }

        if (currentBossHealth <= 0)
        {
            BossDie();
        }
    }

    private void EnterPhase2()
    {
        currentPhase = 2;
        moveSpeed *= 1.5f;
        dashSpeed *= 1.3f;

        if (bossSprite != null) bossSprite.color = Color.red;
        Debug.Log("--- BOSS BƯỚC VÀO PHA 2: TRẠNG THÁI CUỒNG NỘ! ---");
    }

    private void BossDie()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        if (bossUI != null) bossUI.gameObject.SetActive(false); 

        Debug.Log("Boss đã bị tiêu diệt!");
        

        if (endGameVideoPanel != null)
        {
            StartCoroutine(PlayEndGameVideoRoutine());
        }
    }

    private void StopMovement()
    {
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public float GetHealthNormalized()
    {
        return Mathf.Clamp01(currentBossHealth / maxBossHealth);
    }

    IEnumerator PlayEndGameVideoRoutine()
    {
        endGameVideoPanel.SetActive(true);

        VideoPlayer vp = endGameVideoPanel.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.Play();
        }

        yield return new WaitForSeconds(videoDuration);

        if (vp != null)
        {
            vp.Stop(); 
        }
        endGameVideoPanel.SetActive(false); 

        Destroy(gameObject);


    }
}