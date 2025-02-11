using UnityEngine;

public class MiniPoolDemo : MonoBehaviour
{
    [Tooltip("Prefab cần pool (đảm bảo prefab có Component)")]
    public GameObject prefab;
    [Tooltip("Số lượng khởi tạo ban đầu")]
    public int initialPoolSize = 10;

    private MiniPool<Transform> pool;

    void Start()
    {
        pool = new MiniPool<Transform>();
        // Dùng Transform của prefab vì Transform luôn có trên GameObject
        pool.Initialize(prefab.transform, initialPoolSize, transform);
    }

    void Update()
    {
        // Nhấn Space để spawn đối tượng tại vị trí ngẫu nhiên
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            pool.Spawn(pos, Quaternion.identity);
        }

        // Nhấn C để despawn tất cả đối tượng đang active
        if (Input.GetKeyDown(KeyCode.C))
        {
            pool.DespawnAll();
        }
    }

    // Khi không sử dụng, hủy pool để giải phóng bộ nhớ
    void OnDisable()
    {
        pool.ReleasePool();
    }
}