using Foutain.Scene;
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
        public AudioClip bgm;
        public AudioTrack track;
        private void Start()
        {
            StartCoroutine(StartPrepare());
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<ControlFountainReadyEvent>(GameReady);
            GameEventBus.Subscribe<ControlFountainStartEvent>(GameStart);
            GameEventBus.Subscribe<LoadSceneEvent>(StopBGM);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<ControlFountainReadyEvent>(GameReady);
            GameEventBus.Unsubscribe<ControlFountainStartEvent>(GameStart);
            GameEventBus.Unsubscribe<LoadSceneEvent>(StopBGM);
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
            GameEventBus.Publish<PlaySoundEvent>(new PlaySoundEvent()
            { Clip = this.bgm, Track = this.track, IsLoop = true });
        }
        private void GameStart(ControlFountainStartEvent e)
        {
            gameTimer.StartCountDown();
            gameTimer.CountdownEnd += gameFlowController.EndGame;
        }
        private void StopBGM(LoadSceneEvent e)
        {
            if (e.SceneToUnload!=this.gameObject.scene.name)
            {
                return; 
            }
            GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent() 
            { Track = this.track });
        }
    }
}
