using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int ID { get; private set; }
    public bool IsPlaying { get; private set; }

    public Transform CameraPlace => _cameraPlace;
    [SerializeField] private Transform _cameraPlace;
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
        IsPlaying = true;
        GameTime = Time.time;
        Player.InitializePlayer(playerBrain, (DevourerController)_devourer);
        MovePermission(true);
        ResetAll();
    }

    public void StopGame()
    {
        IsPlaying = false;
        MovePermission(false);
        GameTime = Time.time - GameTime;

        _player.Initialized = false;

        ResetAll();
        _onStop?.Invoke(this);
    }

    private void ResetAll()
    {
        _player.ResetVelocity();
        _devourer.ResetVelocity();

        _player.transform.position = _firstPlace.position;
        _devourer.transform.position = _secondPlace.position;
    }

    private void MovePermission(bool p)
    {
        PhysicsBehaviour player = _player;
        player.Stop = !p;
        _devourer.Stop = !p;
    }
}
