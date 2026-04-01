using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public abstract class BasicEnemyInterface : MonoBehaviour
{

    protected virtual void InitComponents() { }

    protected virtual void Process() { }

    public bool CanSeePlayer() { return true; }

    public bool CanAttackPlayer() { return true; }

    protected virtual void Death() { }

    protected virtual void Idle() { }

    protected virtual void Pursue() { }

    protected virtual void Attack() { }

    protected virtual void Pushed() { }

    public void GetPushed(Vector3 direction, float force, float duration) { }

    public void SetStunned(float duration) { }

    public void StopAgent() { }

    public void ResetAgent() { }
}
