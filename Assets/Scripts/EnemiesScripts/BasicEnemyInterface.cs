using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public abstract class BasicEnemyInterface : MonoBehaviour
{

    protected virtual void InitComponents() { }

    protected virtual void Process() { }

    protected virtual bool CanSeePlayer() { return true; }

    protected virtual bool CanAttackPlayer() { return true; }

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
