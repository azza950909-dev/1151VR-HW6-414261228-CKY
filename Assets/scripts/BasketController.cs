using UnityEngine;

public class BasketController : MonoBehaviour
{
    [Header("九宮格設定")]
    [Tooltip("九宮格每格的中心座標（例如 -1, 0, 1）")]
    public float[] gridCoords = new float[] { -1f, 0f, 1f };
    [Tooltip("藤籃固定的 Y 高度")]
    public float basketY = 0.5f;
    [Tooltip("射線偵測要打中的九宮格地板 Layer")]
    public LayerMask groundLayer;

    [Header("移動手感")]
    public float moveSpeed = 15f;

    [Header("流程關聯")]
    public GameDirector director;

    private Vector3 targetPosition;

    private void Start()
    {
        // 初始放在九宮格正中央 (gridCoords[1], basketY, gridCoords[1])
        float center = gridCoords.Length >= 2 ? gridCoords[1] : 0f;
        targetPosition = new Vector3(center, basketY, center);
        transform.position = targetPosition;
    }

    private void Update()
    {
        if (director != null && director.IsGameOver)
            return;

        HandleMouseClick();
        MoveToTarget();
    }

    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 若有指定 groundLayer 就用它打射線，沒指定就打所有碰撞體
        bool isHit = groundLayer.value != 0
            ? Physics.Raycast(ray, out hit, 100f, groundLayer)
            : Physics.Raycast(ray, out hit, 100f);

        if (isHit)
        {
            float snappedX = SnapToGrid(hit.point.x);
            float snappedZ = SnapToGrid(hit.point.z);
            targetPosition = new Vector3(snappedX, basketY, snappedZ);
        }
    }

    private float SnapToGrid(float rawValue)
    {
        float bestCoord = gridCoords[0];
        float minDiff = Mathf.Abs(rawValue - bestCoord);

        for (int i = 1; i < gridCoords.Length; i++)
        {
            float diff = Mathf.Abs(rawValue - gridCoords[i]);
            if (diff < minDiff)
            {
                minDiff = diff;
                bestCoord = gridCoords[i];
            }
        }

        // 自動限制在九宮格邊界內，不會超出範圍
        return bestCoord;
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (director != null && director.IsGameOver)
            return;

        // 因為蘋果或炸彈可能有多個子物件，往上找根物件或直接比對 Tag
        GameObject rootObj = other.transform.root != null ? other.transform.root.gameObject : other.gameObject;
        Debug.Log($"碰到 {rootObj.name}，Tag 是: {rootObj.tag}，director 是否存在: {director != null}");

        if (rootObj.CompareTag("Apple") || other.CompareTag("Apple"))
        {
            if (director != null)
                director.OnCatchApple();
                Debug.Log($"呼叫加分後，目前分數: {director.Score}");

            Destroy(rootObj);
        }
        else if (rootObj.CompareTag("Bomb") || other.CompareTag("Bomb"))
        {
            if (director != null)
                director.OnCatchBomb();

            Destroy(rootObj);
        }
    }
}