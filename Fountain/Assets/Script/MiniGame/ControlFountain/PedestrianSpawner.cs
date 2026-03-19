using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    public class PedestrianSpawner : MonoBehaviour
    {
        [Header("生成设置")]
        [Tooltip("路人预制件")]
        [SerializeField]
        private GameObject pedestrianPrefab;
        [Tooltip("生成间隔")]
        [SerializeField]
        private float spawnInterval = 1.5f;
        [Tooltip("生成y轴偏移最小值")]
        [SerializeField]
        private float spawnRangeYMin;
        [Tooltip("生成y轴偏移最大值")]
        [SerializeField]
        private float spawnRangeYMax;
        [Tooltip("生成间隔")]
        [SerializeField]
        private float spawnRangeX = 8f;
        [Tooltip("基础速度数值")]
        [SerializeField]
        private float baseSpeed;
        [Tooltip("生成路人的最大速度比值,随时间达到最大")]
        [SerializeField]
        private float maxSpeedRate;
        [Tooltip("游戏进行中的计时器")]
        [SerializeField]
        private CountdownTimer gameTimer;

        private float nextSpawnTime;
        private void Start()
        {
            this.enabled = false;
            GameEventBus.Subscribe<ControlFountainStartEvent>((e) =>
            {
                this.enabled = true;
            });
            GameEventBus.Subscribe<ControlFountainEndEvent>((e) =>
            {
                this.enabled = false;
            });
                
        }

        private void Update()
        {
            //if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
    
            if (Time.time >= nextSpawnTime)
            {
                Spawn();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    
        private void Spawn()
        {
            //float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            float randomX = Random.Range(-spawnRangeX, 0);
            float randomY = Random.Range(spawnRangeYMin,spawnRangeYMax);

            Vector3 spawnPos = new Vector3
                (this.transform.position.x + randomX,
                this.transform.position.y + randomY, 0f);
            GameObject go =Instantiate(pedestrianPrefab, spawnPos, Quaternion.identity);
            Pedestrian pedestrian= go.GetComponent<Pedestrian>();
            float rate = ( gameTimer.GetTotalTime() - gameTimer.GetRemainingTime() ) /
                gameTimer.GetTotalTime();
            pedestrian.speed = this.baseSpeed +
                this.baseSpeed * maxSpeedRate * rate;
        }
    }
    
}
