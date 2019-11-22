using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int ID { get; private set; }

    [SerializeField] private Transform _firstPlace;
    [SerializeField] private Transform _secondPlace;

    public PlayerController Player => _player;
    [SerializeField] private PlayerController _player;
    [SerializeField] private PhysicsBehaviour _devourer;

    public float GameTime { get; private set; }

    private Action<Game> _onStop; 

    public void Initialize(int id, Action<Game> onStop)
    {
        ID = id;
        _onStop = onStop;

        _devourer.GetComponent<DevourerController>().SetOnEatAction(StopGame);
        MovePermission(false);
    }

    public void StartGame(Brain playerBrain)
    {
        GameTime = Time.time;
        Player.InitializePlayer(playerBrain, (DevourerController)_devourer);
        MovePermission(true);
        ResetAll();
    }

    private void ResetAll()
    {
        _player.ResetVelocity();
        _devourer.ResetVelocity();

        _player.transform.position = _firstPlace.position;
        _devourer.transform.position = _secondPlace.position;
    }

    public void StopGame()
    {
        MovePermission(false);
        GameTime = Time.time - GameTime;

        _player.Initialized = false;

        ResetAll();
        _onStop?.Invoke(this);
    }

    private void MovePermission(bool p)
    {
        PhysicsBehaviour player = _player;
        player.Stop = !p;
        _devourer.Stop = !p;
    }
}
