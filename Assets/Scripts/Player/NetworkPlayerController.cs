using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField]
    private float _speed = 10;
    [SerializeField]
    private float _turnSpeed = 150;
    [SerializeField]
    private float _jumpPower = 1;


    [SerializeField]
    private Animator _animator;


    private float _terrainColliderHeight = 0f;

    private Player player;

    private float _rotationX = 0;
    private bool _isGrounded = false;

    private int _tick = 0;
    private float _tickRate = 1.0f / 90.0f;
    private float _tickDeltaTime = 0.0f;

    private const int BUFFER_SIZE = 1024;
    private InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> ServerTransformState = new();

    private void Start()
    {
        _terrainColliderHeight = GetComponent<CapsuleCollider>().height;
        if (!NetworkManager.IsServer && !IsLocalPlayer)
        {
            GetComponent<Rigidbody>().useGravity = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        ServerTransformState.OnValueChanged += OnServerStateChanged;

        player = GetComponent<Player>();
    }

    public override void OnNetworkDespawn()
    {
        ServerTransformState.OnValueChanged -= OnServerStateChanged;
    }

    private void OnServerStateChanged(TransformState _previousState, TransformState serverState)
    {
        if (NetworkManager.IsServer || !IsLocalPlayer) return;


        TransformState calculatedState = _transformStates.First(localState => localState.Tick == serverState.Tick);
        if (Vector3.Distance(calculatedState.Position, serverState.Position) < 0.3 && Mathf.Abs(calculatedState.Position.y - serverState.Position.y) < 1) return;

        TeleportPlayer(serverState);



        IEnumerable<InputState> inputs = _inputStates.Where(input => input.Tick > serverState.Tick);

        inputs = from input in inputs orderby input.Tick select input;

        foreach (InputState inputState in inputs)
        {
            HandleMovement(inputState.movementInput, inputState.rotationInput);

            TransformState newTransformState = new TransformState()
            {
                Tick = inputState.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Facing = player.Head.transform.rotation,
            };

            for (int i = 0; i < _transformStates.Length; i++)
            {
                if (_transformStates[i].Tick == serverState.Tick)
                {
                    _transformStates[i] = newTransformState;
                    break;
                }
            }
        }
    }

    private void TeleportPlayer(TransformState state)
    {
        transform.position = state.Position;
        transform.rotation = state.Rotation;
        player.Head.transform.rotation = state.Facing;
        player.Hand.transform.rotation = state.Facing;

        for (int i = 0; i < _transformStates.Length; i++)
        {
            if (_transformStates[i].Tick == state.Tick)
            {
                _transformStates[i] = state;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _tickDeltaTime += Time.deltaTime;

        if (!_isGrounded && GroundCheck())
        {
            _isGrounded = true;
            _animator.SetBool("IsGrounded", true);
        }

        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleJump();
                if (!NetworkManager.IsServer)
                {
                    JumpServerRPC();
                }
            }

            if (_tickDeltaTime > _tickRate)
            {
                if (!CanMove()) return;


                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                Vector3 moveInput = new(horizontalInput, 0, verticalInput);

                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                Vector2 rotationInput = new(mouseY, mouseX);
                ProcessLocalPlayerMovement(moveInput, rotationInput);
            }
        }
        else
        {
            ProcessSimulatedPlayerMovement();
        }
    }
    private void ProcessLocalPlayerMovement(Vector3 moveInput, Vector3 rotationInput)
    {
        int bufferIndex = _tick % BUFFER_SIZE;

        if (NetworkManager.IsServer)
        {
            HandleMovement(moveInput, rotationInput);

            TransformState state = new TransformState()
            {
                Tick = _tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Facing = player.Head.transform.rotation,
            };

            ServerTransformState.Value = state;
        }
        else
        {
            HandleMovement(moveInput, rotationInput);
            MovePlayerRequestServerRpc(_tick, moveInput, rotationInput);

            //transform.position = ServerTransformState.Value.Position;
            //transform.rotation = ServerTransformState.Value.Rotation;
        }

        InputState inputState = new()
        {
            Tick = _tick,
            movementInput = moveInput,
            rotationInput = rotationInput,
        };

        TransformState transformState = new()
        {
            Tick = _tick,
            Position = transform.position,
            Rotation = transform.rotation,
            Facing = player.Head.transform.rotation,
        };

        _inputStates[bufferIndex] = inputState;
        _transformStates[bufferIndex] = transformState;

        _tickDeltaTime -= _tickRate;
        _tick++;
    }

    private void ProcessSimulatedPlayerMovement()
    {
        if (!NetworkManager.IsServer)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, ServerTransformState.Value.Position, _tickDeltaTime * _speed);

            Debug.Log(Vector3.Distance(transform.position, newPosition));
            _animator.SetFloat("Speed", Vector3.Distance(transform.position, newPosition));

            transform.position = newPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, ServerTransformState.Value.Rotation, _tickDeltaTime * _speed);
            Quaternion facing = Quaternion.Lerp(player.Head.transform.rotation, ServerTransformState.Value.Facing, _tickDeltaTime * _speed);
            player.Head.transform.rotation = facing;
            player.Hand.transform.rotation = facing;
        }

        UpdateTick();
    }

    private void HandleMovement(Vector3 moveInput, Vector3 rotationInput)
    {
        _animator.SetFloat("Speed", Vector3.Distance(Vector3.zero, moveInput));

        transform.Translate(_speed * _tickRate * moveInput);
        transform.Rotate(_tickRate * _turnSpeed * new Vector3(0, rotationInput.y, 0));

        _rotationX -= rotationInput.x * _turnSpeed * _tickRate;
        _rotationX = Mathf.Clamp(_rotationX, -90, 90);
        player.Head.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        player.Hand.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        //transform.Rotate(new Vector3(0, rotationInput.y, 0) * _turnSpeed * _tickRate);
        //player.Head.transform.Rotate(new Vector3(-rotationInput.x, 0, 0) * _turnSpeed * _tickRate);
        //player.Hand.transform.Rotate(new Vector3(-rotationInput.x, 0, 0) * _turnSpeed * _tickRate);
    }
    private void HandleJump()
    {
        if (GroundCheck())
        {
            _isGrounded = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _animator.SetBool("IsGrounded", false);
        }
    }

    private bool CanMove() => player.IsAlive;

    private bool GroundCheck()
    {
        return (Physics.Raycast(transform.position, Vector3.down, _terrainColliderHeight / 2 + 0.1f));
    }

    private void UpdateTick()
    {
        if (_tickDeltaTime > _tickRate)
        {
            _tickDeltaTime -= _tickRate;
            _tick++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MovePlayerRequestServerRpc(int tick, Vector3 moveInput, Vector3 rotationInput)
    {
        HandleMovement(moveInput, rotationInput);

        TransformState state = new TransformState()
        {
            Tick = tick,
            Position = transform.position,
            Rotation = transform.rotation,
            Facing = player.Head.transform.rotation,
        };

        ServerTransformState.Value = state;
    }

    [ServerRpc(RequireOwnership = false)]
    private void JumpServerRPC()
    {
        HandleJump();
    }

    public new bool IsLocalPlayer
    {
        get { return player.IsLocalPlayer; }
    }
}
