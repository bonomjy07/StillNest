using UnityEngine;
using UnityEngine.AI;

public enum AIMode
{
    None,
    Chasing,
    Moving,
}

public class AICharacter : MonoBehaviour
{
    private NavMeshAgent _agent;
    
    private AIMode _mode = AIMode.None;
    private Transform _target;
    private Vector3 _destination;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (_mode)
        {
            case AIMode.Chasing:
            {
                if (_target)
                {
                    _agent.SetDestination(_target.position);
                    CheckArrival();
                }
            }
                break;

            case AIMode.Moving:
            {
                _agent.SetDestination(_destination);
                CheckArrival();
            }
                break;
        }
    }
    
    private void CheckArrival()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                _mode = AIMode.None;
                _destination = Vector3.zero;
                _target = null;
            }
        }
    }

    public void MoveTo(Vector3 destination)
    {
        _mode = AIMode.Moving;
        _destination = destination;
        _target = null;
    }

    public void SetTarget(Transform target)
    {
        _mode = AIMode.Chasing;
        _destination = Vector3.zero;
        _target = target;
    }
}
