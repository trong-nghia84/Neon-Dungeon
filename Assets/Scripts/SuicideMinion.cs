using System.Collections;
using UnityEngine;

public class SuicideMinion : EnemyBase
{
    [Header("Cấu hình Nổ")]
    public float explosionDamage = 30f;
    public float explosionRadius = 2f;
    public float timeToExplode = 1f; // Thời gian chờ nổ sau khi chạm

    [Header("Hiệu ứng")]
    public GameObject explosionVFXPrefab; // Prefab hiệu ứng nổ (VFX)
    public GameObject warningAreaVFX;       // Đối tượng con hiển thị vùng nổ màu đỏ

    private bool isPreparingToExplode = false;
    private SpriteRenderer sprite;

    protected override void Awake()
    {
        base.Awake();
        sprite = GetComponent<SpriteRenderer>();
        if (warningAreaVFX != null) warningAreaVFX.SetActive(false); // Đảm bảo tắt lúc đầu
    }

    protected override void Update()
    {
        // 1. Gọi Update cha để xử lý tàng hình và lấy vị trí Player
        base.Update();

        // 2. Chặn di chuyển nếu Minion chết, không thấy player, hoặc đang gồng nổ
        if (isDead || player == null || isPreparingToExplode)
        {
            StopMovement();
            return;
        }

        // 3. Logic di chuyển đuổi theo Player (Tận dụng moveSpeed của EnemyBase)
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Xử lý quay mặt
        if (direction.x > 0 && transform.localScale.x < 0) Flip();
        else if (direction.x < 0 && transform.localScale.x > 0) Flip();
    }

    private void Flip()
    {
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // XỬ LÝ VA CHẠM: Khi chạm vào Player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || isPreparingToExplode) return;

        if (collision.CompareTag("Player"))
        {
            // Bắt đầu quá trình nổ sau 1 giây
            StartCoroutine(ExplosionRoutine());
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        isPreparingToExplode = true;
        StopMovement(); // Đứng yên gồng nổ

        Debug.Log(gameObject.name + " chuẩn bị kích nổ!");

        // 1. Bật hiệu ứng cảnh báo (Vòng tròn đỏ)
        if (warningAreaVFX != null) warningAreaVFX.SetActive(true);

        // 2. Hiệu ứng nhấp nháy màu Sprite (Tùy chọn) để tăng độ nguy hiểm
        if (sprite != null)
        {
            for (int i = 0; i < 3; i++)
            {
                sprite.color = Color.red; // Chuyển sang đỏ
                yield return new WaitForSeconds(timeToExplode / 6f);
                sprite.color = Color.white; // Về bình thường
                yield return new WaitForSeconds(timeToExplode / 6f);
            }
        }
        else
        {
            yield return new WaitForSeconds(timeToExplode);
        }

        // 3. THỰC HIỆN VỤ NỔ
        ExecuteFinalExplosion();
    }

    private void ExecuteFinalExplosion()
    {
        isDead = true; // Đánh dấu quái đã chết

        // 1. SINH RA HIỆU ỨNG NỔ RIÊNG TỪ PREFAB
        if (explosionVFXPrefab != null)
        {
            // Sinh ra hiệu ứng nổ mới tại đúng vị trí con quái đứng
            GameObject vfx = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);

            // Tự động xóa cái hiệu ứng này đi sau 0.5 giây (hoặc bằng thời gian chạy hết Anim)
            Destroy(vfx, 0.5f);
        }

        // 2. Ẩn Sprite và ngắt vật lý của chính con quái đi
        SpriteRenderer minionSprite = GetComponent<SpriteRenderer>();
        if (minionSprite != null) minionSprite.enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // 3. Quét và gây sát thương lên Player trong phạm vi
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in colliders)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerBase p = obj.GetComponent<PlayerBase>();
                if (p == null) p = obj.GetComponentInParent<PlayerBase>();

                if (p != null)
                {
                    p.TakeDamage(explosionDamage);
                    Debug.Log("Minion nổ trúng Player!");
                }
            }
        }

        // 4. Xóa con quái ngay lập tức vì hiệu ứng đã được tự quản lý ở bước 1
        Destroy(gameObject);
    }

    // Vẽ vùng nổ trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}