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

    public Transform LeftClamp;
    public Transform BotClamp;

    public float GameTime { get; private set; }

    private Action<Game> _onStop; 

    public void Initialize(int id, Action<Game> onStop)
    {
        ID = id;
        _onStop = onStop;

        _devourer.GetComponent<DevourerController>().SetOnEatAction(StopGame);
        MovePermission(false);
    }

    public void StartGame(Brain playerBrain, Vector3 playerPosition, Vector3 devourerPosition)
    {
        IsPlaying = true;
        GameTime = Time.time;
        Player.InitializePlayer(playerBrain, (DevourerController)_devourer);
        MovePermission(true);
        ResetAll(playerPosition, devourerPosition);
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

    private void ResetAll(Vector3? playerPos = null, Vector3? devourerPos = null)
    {
        _player.ResetVelocity();
        _devourer.ResetVelocity();

        Vector3 newPlayerPos = _firstPlace.position;
        if (playerPos != null)
        {
            newPlayerPos = playerPos.Value;
            newPlayerPos.x += LeftClamp.position.x;
            newPlayerPos.z += BotClamp.position.z;
         
        }
        _player.transform.position = newPlayerPos;

        Vector3 newDevourerPos = _secondPlace.position;
        if (devourerPos != null)
        {
            newDevourerPos = devourerPos.Value;
            newDevourerPos.x += LeftClamp.position.x;
            newDevourerPos.z += BotClamp.position.z;
        }
        _devourer.transform.position = newDevourerPos;
    }

    private void MovePermission(bool p)
    {
        PhysicsBehaviour player = _player;
        player.Stop = !p;
        _devourer.Stop = !p;
    }
}
