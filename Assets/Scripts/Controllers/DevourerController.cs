using System;
using UnityEngine;

public class DevourerController : PhysicsBehaviour
{
    [SerializeField] private PlayerController _player;

    [SerializeField] private float _forceFade;
    [SerializeField] private float _eatDistance;

    public void SetOnEatAction(Action a) => _onEat += a;
    private Action _onEat;

    private void Update()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < _eatDistance)
        {
            Eat();
            return;
        }

        FollowEat();
        ClampVelocity();
    }

    private void FollowEat()
    {
        Vector3 playerPosition = _player.transform.position;
        forceVector = playerPosition - transform.position;
        forceVector = forceVector.normalized * force;

        RigidBody.AddForce(forceVector);
        RigidBody.velocity *= _forceFade;

        transform.LookAt(_player.transform);
    }

    private void Eat()
    {
        // Restarting game
        _onEat();
    }
}
