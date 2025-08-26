using System;
using System.Collections.Generic;
using System.Linq;
using LightItUp.Data;
using LightItUp.Game;
using LightItUp.Singletons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LightItUp
{
    public class MissilesController : SingletonCreate<MissilesController>
    {
        private HashSet<BlockController> targetedBlocks = new HashSet<BlockController>();
        
        public void LaunchMissiles()
        {
            PlayerController playerController= FindObjectOfType<PlayerController>();
            
            int missilesToLaunch = GameSettings.PowerUps.missilesPerLaunch;
            
            List<BlockController> unlitBlocks =  GameManager.Instance.currentLevel.blocks.FindAll(x => !x.IsLit);
            List<BlockController> unlitSortedBlocks = unlitBlocks.OrderBy<BlockController, object>((x) => ((Vector2)(x.transform.position - playerController.transform.position)).magnitude).ToList();
            List<BlockController> prioritizedBlocks = unlitSortedBlocks
                .OrderByDescending(b => b.gameObject.name.StartsWith("SimpleBlock"))
                .ToList();

            int assigned = 0;

            foreach (var block in prioritizedBlocks)
            {
                if (assigned >= missilesToLaunch)
                {
                    break;
                }

                if (targetedBlocks.Contains(block))
                {
                    continue;
                }

                Missile missile = ObjectPool.GetMissile();
                missile.transform.position = playerController.transform.position;
                missile.SetTarget(block);

                targetedBlocks.Add(block);
                assigned++;
            }
        }
    }
}