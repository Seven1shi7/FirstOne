using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 状态类型枚举
/// </summary>
public enum StateType
{
    Idle,    // 待机状态
    Move,    // 移动状态
    Attack   // 攻击状态
}

/// <summary>
/// 状态接口
/// </summary>
public interface IState
{
    StateType stateType { get; }
    void Enter(StateMachine stateMachine);
    void Update(StateMachine stateMachine);
    void Exit(StateMachine stateMachine);
}

/// <summary>
/// 待机状态
/// </summary>
public class IdleState : IState
{
    public StateType stateType { get; private set; }

    public IdleState()
    {
        stateType = StateType.Idle;
    }

    public void Enter(StateMachine stateMachine)
    {
        Debug.Log($"{stateMachine.gameObject.name} 进入待机状态");
    }

    public void Update(StateMachine stateMachine)
    {
        // 如果有目标且在攻击范围内，切换到攻击状态
        if (stateMachine.target != null && stateMachine.CanAttack())
        {
            stateMachine.ChangeState(StateType.Attack);
        }
        // 如果有目标但不在攻击范围内，切换到移动状态
        else if (stateMachine.target != null && Vector3.Distance(stateMachine.transform.position, stateMachine.target.position) > stateMachine.attackRange)
        {
            stateMachine.ChangeState(StateType.Move);
        }
    }

    public void Exit(StateMachine stateMachine)
    {
        Debug.Log($"{stateMachine.gameObject.name} 退出待机状态");
    }
}

/// <summary>
/// 移动状态
/// </summary>
public class MoveState : IState
{
    public StateType stateType { get; private set; }

    public MoveState()
    {
        stateType = StateType.Move;
    }

    public void Enter(StateMachine stateMachine)
    {
        Debug.Log($"{stateMachine.gameObject.name} 进入移动状态");
    }

    public void Update(StateMachine stateMachine)
    {
        // 执行移动
        if (stateMachine.target != null)
        {
            Vector3 direction = (stateMachine.target.position - stateMachine.transform.position).normalized;
            stateMachine.transform.Translate(direction * stateMachine.moveSpeed * Time.deltaTime);
            Debug.Log($"{stateMachine.gameObject.name} 向目标移动中...");
        }

        // 如果到达攻击范围，切换到攻击状态
        if (stateMachine.target != null && stateMachine.CanAttack())
        {
            stateMachine.ChangeState(StateType.Attack);
        }
        // 如果失去目标，切换到待机状态
        else if (stateMachine.target == null)
        {
            stateMachine.ChangeState(StateType.Idle);
        }
    }

    public void Exit(StateMachine stateMachine)
    {
        Debug.Log($"{stateMachine.gameObject.name} 退出移动状态");
    }
}

/// <summary>
/// 攻击状态
/// </summary>
public class AttackState : IState
{
    public StateType stateType { get; private set; }

    public AttackState()
    {
        stateType = StateType.Attack;
    }

    public void Enter(StateMachine stateMachine)
    {
        Debug.Log($"{stateMachine.gameObject.name} 进入攻击状态");
    }

    public void Update(StateMachine stateMachine)
    {
        // 执行攻击
        if (stateMachine.CanAttack())
        {
            Debug.Log($"{stateMachine.gameObject.name} 发动攻击！");
            stateMachine.attackTimer = stateMachine.attackCooldown;

            // 这里可以添加攻击伤害逻辑
            // 例如: stateMachine.target.GetComponent<Enemy>().TakeDamage(10f);
        }

        // 如果目标不在攻击范围内，切换到移动状态
        if (stateMachine.target != null && Vector3.Distance(stateMachine.transform.position, stateMachine.target.position) > stateMachine.attackRange)
        {
            stateMachine.ChangeState(StateType.Move);
        }
        // 如果失去目标，切换到待机状态
        else if (stateMachine.target == null)
        {
            stateMachine.ChangeState(StateType.Idle);
        }
    }

    public void Exit(StateMachine stateMachine)
    {
        Debug.Log($"{stateMachine.gameObject.name} 退出攻击状态");
    }
}

/// <summary>
/// 有限状态机管理器
/// </summary>
public class StateMachine : MonoBehaviour
{
    // 状态字典
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    // 当前状态
    private IState currentState;

    // 状态机配置
    [Header("状态配置")]
    public float moveSpeed = 5f; // 移动速度
    public float attackRange = 2f; // 攻击范围
    public float attackCooldown = 1f; // 攻击冷却时间
    public Transform target; // 攻击目标

    // 状态计时器
    public float attackTimer = 0f;

    void Start()
    {
        // 初始化状态字典
        states.Add(StateType.Idle, new IdleState());
        states.Add(StateType.Move, new MoveState());
        states.Add(StateType.Attack, new AttackState());

        // 默认进入待机状态
        ChangeState(StateType.Idle);
    }

    void Update()
    {
        // 更新当前状态
        if (currentState != null)
        {
            currentState.Update(this);
        }

        // 攻击冷却计时
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void ChangeState(StateType newStateType)
    {
        // 检查状态是否存在
        if (!states.TryGetValue(newStateType, out IState newState))
        {
            Debug.LogError($"状态 {newStateType} 不存在！");
            return;
        }

        // 退出当前状态
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        // 进入新状态
        currentState = newState;
        currentState.Enter(this);
    }

    /// <summary>
    /// 检查是否可以攻击
    /// </summary>
    public bool CanAttack()
    {
        return attackTimer <= 0 && target != null && Vector3.Distance(transform.position, target.position) <= attackRange;
    }
}