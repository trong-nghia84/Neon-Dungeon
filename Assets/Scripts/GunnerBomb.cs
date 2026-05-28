using UnityEngine;

public class GunnerBomb : MonoBehaviour
{
    [Header("Cấu hình Bom")]
    public float delay = 2f;            // Thời gian đếm ngược kích nổ (2 giây)
    public float explosionRadius = 3f;   // Bán kính vụ nổ
    public float damage = 40f;           // Sát thương gây ra
    public float destroyDelay = 0.5f;    // Thời gian chờ xóa bom sau khi nổ (để diễn hết Anim nổ)

    private float countdown;
    private bool hasExploded = false;

    // Các thành phần nội bộ để xử lý ẩn/hiện
    private SpriteRenderer bombSprite;
    private Rigidbody2D rb;
    private Transform bombEffectTransform;

    void Awake()
    {
        // Lấy Sprite Renderer của chính quả bom để ẩn đi khi nổ
        bombSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Tự động tìm đối tượng con tên là "BombEffect"
        bombEffectTransform = transform.Find("BombEffect");

        if (bombEffectTransform != null)
        {
            // Đảm bảo lúc mới ném ra, hiệu ứng nổ phải được TẮT
            bombEffectTransform.gameObject.SetActive(false);
        }
        else
        {
        }
    }

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        if (hasExploded) return;

        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        // 1. Ẩn hình ảnh quả bom và dừng vật lý để bom không lăn tiếp khi đang nổ
        if (bombSprite != null) bombSprite.enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // 2. BẬT hiệu ứng nổ (đối tượng con BombEffect)
        if (bombEffectTransform != null)
        {
            bombEffectTransform.gameObject.SetActive(true);
        }

        // 3. Quét và gây sát thương diện rộng lên Kẻ địch (Enemy)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {
                // Tìm script EnemyBase ở chính đối tượng đó hoặc ở cha của nó
                EnemyBase enemy = nearbyObject.GetComponent<EnemyBase>();
                if (enemy == null) enemy = nearbyObject.GetComponentInParent<EnemyBase>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        // 4. Xóa toàn bộ quả bom (bao gồm cả con của nó) sau khi hiệu ứng diễn xong
        Destroy(gameObject, destroyDelay);
    }

    // Vẽ vòng tròn bán kính nổ trong cửa sổ Scene để Nghĩa dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}