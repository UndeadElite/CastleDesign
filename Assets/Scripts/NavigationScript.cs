using UnityEngine;
using UnityEngine.AI;
public class NavigationScript : MonoBehaviour
{

    public Transform Player;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = Player.position;
    }
}
