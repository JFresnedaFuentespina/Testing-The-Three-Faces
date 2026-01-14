using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoveNavmesh : MonoBehaviour
{
    public float velocity = 1f;
    private GameObject mainCharacter;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.5f;

        agent.updateRotation = true;
        agent.updateUpAxis = true;

        BuscarJugador();
        if (mainCharacter != null)
        {
            agent.SetDestination(mainCharacter.transform.position);
        }
    }

    void Update()
    {
        if (mainCharacter == null) return;
        // Solo actualizamos destino si el jugador se ha movido lo suficiente
        if (Vector3.Distance(agent.destination, mainCharacter.transform.position) > 0.5f)
        {
            agent.SetDestination(mainCharacter.transform.position);
        }
    }

    private void BuscarJugador()
    {
        mainCharacter = GameObject.Find("Character(Clone)") ?? GameObject.FindGameObjectWithTag("Player");
    }
}
