using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoveNavmesh : MonoBehaviour
{
    public float velocity = 3f; // velocidad del NavMeshAgent
    private GameObject mainCharacter;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.5f;

        BuscarJugador();
    }

    void Update()
    {
        if (mainCharacter != null)
        {
            agent.SetDestination(mainCharacter.transform.position);
        }
        else
        {
            BuscarJugador();
        }
    }

    private void BuscarJugador()
    {
        mainCharacter = GameObject.Find("Character(Clone)");
        if (mainCharacter == null)
        {
            mainCharacter = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
