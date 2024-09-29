using Bullet_Master_3D.Scripts.Singleton;
using GameAnalyticsSDK;
using UnityEngine;

namespace Bullet_Master_3D.Scripts
{
    public class AnalyticsManager : MonoBehaviour
    {
        private void Start()
        {
            GameAnalytics.Initialize();
            
            Boostrap.Instance.GameEvents.OnLevelStart += OnLevelStart;
            Boostrap.Instance.GameEvents.OnLevelLose += OnLevelLose;
            Boostrap.Instance.GameEvents.OnLevelComplete += OnLevelComplete;
            Boostrap.Instance.GameEvents.OnLevelRestart += OnLevelRestart;
            Boostrap.Instance.GameEvents.OnOpenLevelFromList += OnOpenLevelFromList;
        }

        private void OnLevelStart()
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $" Level {Boostrap.Instance.ScenesService.LevelId} started.");
        }

        private void OnLevelLose()
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $" Level {Boostrap.Instance.ScenesService.LevelId} lose.");
        }

        private void OnLevelComplete()
        {
            var player = Boostrap.Instance.GameManager.PlayerSpawnerService.Player;
            var usedCartridges = player.MaximumCartridgesCount - player.CartridgesCount;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $" Level {Boostrap.Instance.ScenesService.LevelId} completed.", 
                $" Stars - {Boostrap.Instance.GameManager.StarsCount}.", $" Cartridges spent - {usedCartridges}.");
        }

        private void OnLevelRestart()
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $" Level {Boostrap.Instance.ScenesService.LevelId} restarted.");
        }

        private void OnOpenLevelFromList(int levelId)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $" Level {levelId} opened from levels panel.");
        }
    }
}
