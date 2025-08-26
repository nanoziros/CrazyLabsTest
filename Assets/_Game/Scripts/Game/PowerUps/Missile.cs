using System.Collections.Generic;
using LightItUp.Data;
using LightItUp.Game;
using UnityEngine;

namespace LightItUp
{
    public class Missile:PooledObject
    {
        public float speed = 15f;
        public float rotateSpeed = 360f;
        public float avoidanceForce = 8f;
        public float avoidanceRadius = 2f;
        
        private Rigidbody2D rb;
        private BlockController targetedBlock;
        private List<BlockController> allBlocks => GameManager.Instance.currentLevel.blocks;
        
        public void SetTarget(BlockController blockTarget)
        {
            targetedBlock = blockTarget;
        }

        private void Update()
        {
            if (targetedBlock == null)
            {
                return;
            }

            if (targetedBlock.IsLit)
            {
                ObjectPool.ReturnMissile(this);
                targetedBlock = null;
                return;
            }

            Vector2 missilePos = transform.position;
            Vector2 targetPos = targetedBlock.transform.position;

            Vector2 desiredDir = (targetPos - missilePos).normalized;
            Vector2 steering = desiredDir;

            float closestDist = float.MaxValue;
            Vector2 strongestAvoidance = Vector2.zero;

            foreach (BlockController block in allBlocks)
            {
                if (block == null || block == targetedBlock || !block.gameObject.activeInHierarchy)
                {
                    continue;
                }

                Collider2D blockCol = block.GetComponent<Collider2D>();
                if (blockCol == null)
                {
                    continue;
                }

                Vector2 closestPoint = blockCol.ClosestPoint(missilePos);
                float dist = Vector2.Distance(missilePos, closestPoint);

                if (dist < avoidanceRadius && dist < closestDist)
                {
                    Vector2 away = (missilePos - closestPoint).normalized;
                    strongestAvoidance = away * (avoidanceForce * (avoidanceRadius - dist) / avoidanceRadius);
                    closestDist = dist;
                }
            }

            if (closestDist < float.MaxValue)
            {
                // Extreme avoidance if very close
                if (closestDist < avoidanceRadius * 0.5f)
                    steering = strongestAvoidance.normalized;
                else
                    steering = (desiredDir + strongestAvoidance).normalized;
            }

            float angle = Mathf.Atan2(steering.y, steering.x) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.LerpAngle(
                transform.eulerAngles.z,
                angle,
                rotateSpeed * Time.deltaTime / 360f
            );
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);

            transform.position += (Vector3)(steering * speed * Time.deltaTime);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            BlockController block = other.GetComponent<BlockController>();
            if (block != null && block == targetedBlock)
            {
                block.Collide();
                ObjectPool.ReturnMissile(this);
            }
        }
    }
}