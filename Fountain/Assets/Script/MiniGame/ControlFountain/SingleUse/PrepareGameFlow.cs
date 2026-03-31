using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    public class PrepareGameFlow : MonoBehaviour
    {
        [Header("开始流程的脚本,暂时就手动拖上去")]
        [Tooltip("游戏开始前的时间")]
        [SerializeField]
        private float timeBeforeStart;
        public CountdownTimer startTimer;
        public CountdownTimer gameTimer;
        public GameFlowController gameFlowController;
        private void Start()
        {
            StartCoroutine(StartPrepare());
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<ControlFountainReadyEvent>(GameReady);
            GameEventBus.Subscribe<ControlFountainStartEvent>(GameStart);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<ControlFountainReadyEvent>(GameReady);
            GameEventBus.Unsubscribe<ControlFountainStartEvent>(GameStart);
        }
        private IEnumerator StartPrepare()
        {
            yield return null;//等待一帧让别的脚本初始化完成
            gameFlowController.PrepareGame();            
        }
        private void GameReady(ControlFountainReadyEvent e)
        {
            startTimer.StartCountDown(timeBeforeStart);
            startTimer.CountdownEnd += gameFlowController.StartGame;

        }
        private void GameStart(ControlFountainStartEvent e)
        {
            gameTimer.StartCountDown();
            gameTimer.CountdownEnd += gameFlowController.EndGame;

        }
    }
}
