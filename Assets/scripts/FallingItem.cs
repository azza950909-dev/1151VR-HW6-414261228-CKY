using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("掉落設定")]
    public float fallSpeed = 3.5f;
    public float destroyY = -2f;

    private GameDirector director;

    public void Initialize(float speed, GameDirector gameDirector)
    {
        fallSpeed = speed;
        director = gameDirector;
    }

    private void Update()
    {
        if (director != null && director.IsGameOver)
            return;

        // 不靠重力，直接沿世界 Y 軸負方向移動
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (transform.position.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }
}