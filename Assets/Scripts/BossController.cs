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
    public float skillCooldown = 4f; // Giãn cách giữa các lần ra chiêu nói chung
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
    public float minionSkillCooldown = 20f; // Thời gian hồi chiêu riêng của Skill gọi đệ
    private float minionCooldownTimer = 0f;  // Bộ đếm thời gian hồi chiêu gọi đệ

    [Header("Cấu hình Phát hiện Player")]
    public float aggroRange = 8f;    // Khoảng cách Player đi vào thì Boss mới đánh và hiện thanh máu
    public float deaggroRange = 14f;  // Khoảng cách Player chạy quá xa thì Boss ngừng đuổi và ẩn thanh máu
    public bool isAggroed = false;    // Cờ trạng thái kiểm tra Boss đang chiến đấu hay đứng yên

    private SpriteRenderer bossSprite;
    private bool isFacingRightBoss = false; // Mặc định lúc làm Prefab boss đang nhìn sang phải
    private BossHealthBarUI bossUI;         // Lưu tham chiếu cache đến hệ thống UI thanh máu

    [Header("Cấu hình Video End Game tại chỗ")]
    // Kéo đối tượng EndGameVideoPanel trong Hierarchy vào ô này
    public GameObject endGameVideoPanel;
    public float videoDuration = 10f;
    protected override void Awake()
    {
        base.Awake();
        currentBossHealth = maxBossHealth;
        bossSprite = GetComponent<SpriteRenderer>();

        // Mới vào game cho phép triệu hồi được ngay
        minionCooldownTimer = 0f;

        // Lưu sẵn bộ tìm kiếm UI (Sử dụng true để quét được cả Panel khi nó đang ẩn)
        bossUI = GameObject.FindObjectOfType<BossHealthBarUI>(true);
    }

    protected override void Update()
    {
        // Gọi hàm cha để xử lý cập nhật PlayerSwitchManager và logic tàng hình
        base.Update();

        // 1. NGẮT LẬP TỨC: Nếu Boss chết, hoặc toàn đội Player thua cuộc
        if (isDead || (switchManager != null && switchManager.isAllDead))
        {
            StopMovement();
            if (bossUI != null) bossUI.gameObject.SetActive(false); // Ẩn thanh máu khi kết thúc game
            return;
        }

        // 2. HỆ THỐNG QUÉT KHOẢNG CÁCH CHIẾN ĐẤU (AGGRO LOGIC)
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
                StopMovement(); // Ép Boss đứng yên tại chỗ

                // Ẩn thanh máu khẩn cấp trên màn hình Canvas
                if (bossUI != null)
                {
                    bossUI.gameObject.SetActive(false);
                }
                Debug.Log("Player chạy quá xa! Reset trạng thái ẩn của Boss.");
            }
        }
        else
        {
            // Nếu không tìm thấy player (hoặc Player đang bật skill Tàng hình)
            if (isAggroed)
            {
                isAggroed = false;
                StopMovement();
                if (bossUI != null) bossUI.gameObject.SetActive(false);
            }
        }

        // 3. CHẶN HÀNH VI: Nếu chưa vào trạng thái khiêu khích (isAggroed = false) thì ngắt không cho xả chiêu
        if (!isAggroed)
        {
            return;
        }

        // =========================================================
        // LOGIC CHIẾN ĐẤU (Chỉ thực thi khi isAggroed == true)
        // =========================================================

        // ĐẾM NGƯỢC THỜI GIAN HỒI CHIÊU RIÊNG CỦA SKILL TRIỆU HỒI
        if (minionCooldownTimer > 0f)
        {
            minionCooldownTimer -= Time.deltaTime;
        }

        // Đếm ngược giãn cách ra chiêu chung của Boss
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

        // Cơ chế bổ trợ: Nếu quái chưa kích hoạt Aggro nhưng người chơi bắn trộm từ xa, ép Boss vào thế chiến đấu luôn
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

        if (bossUI != null) bossUI.gameObject.SetActive(false); // Xóa thanh máu khi thắng trận

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
        // 1. Bật đối tượng Video Panel lên màn hình
        endGameVideoPanel.SetActive(true);

        // 2. Lấy thành phần Video Player và ra lệnh phát video
        VideoPlayer vp = endGameVideoPanel.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.Play();
        }

        // 3. ĐÓNG BĂNG HỆ THỐNG TRONG ĐÚNG 10 GIÂY
        // Sử dụng WaitForSeconds để bộ đếm chạy song hành với tiến trình game
        yield return new WaitForSeconds(videoDuration);

        // 4. Hết 10 giây, tiến hành tắt video và trả lại màn hình game
        if (vp != null)
        {
            vp.Stop(); // Dừng phát video hoàn toàn
        }
        endGameVideoPanel.SetActive(false); // Ẩn hoàn toàn đối tượng UI video

        Destroy(gameObject);


    }
}