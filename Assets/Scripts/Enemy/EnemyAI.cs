using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;
    [SerializeField] private MonoBehaviour enemyType;

    // New serialized fields
    [SerializeField] private bool PlayerFollow = false;
    [SerializeField] private float PlayerDistance = 5f;

    private bool canAttack = true;

    private enum State
    {
        Roaming,
        Attacking
    }

    private Vector2 roamPosition;
    private float timeRoaming = 0f;

    private State state;
    private EnemyPathfinding enemyPathfinding;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        switch (state)
        {
            default:
            // default
            case State.Roaming:
                Roaming();
                break;

            case State.Attacking:
                Attacking();
                break;
        }
    }

    private void Roaming()
    {
        timeRoaming += Time.deltaTime;

        // Check if PlayerController.Instance is not null
        if (PlayerController.Instance != null)
        {
            // Check if the enemy should follow the player
            if (PlayerFollow && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < PlayerDistance)
            {
                enemyPathfinding.FollowPlayer(PlayerController.Instance.transform.position);
            }
            else
            {
                enemyPathfinding.MoveTo(roamPosition);

                if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
                {
                    state = State.Attacking;
                }

                if (timeRoaming > roamChangeDirFloat)
                {
                    roamPosition = GetRoamingPosition();
                }
            }
        }
        else
        {
            // Fallback logic if the player is not present
            enemyPathfinding.MoveTo(roamPosition);
        }
    }

    private void Attacking()
    {
        if (PlayerController.Instance != null)
        {
            if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
            {
                state = State.Roaming;
            }

            if (attackRange != 0 && canAttack)
            {
                canAttack = false;
                (enemyType as IEnemy).Attack();

                if (stopMovingWhileAttacking)
                {
                    enemyPathfinding.StopMoving();
                }
                else
                {
                    enemyPathfinding.MoveTo(roamPosition);
                }

                StartCoroutine(AttackCooldownRoutine());
            }
        }
        else
        {
            state = State.Roaming;
        }
    }


    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
