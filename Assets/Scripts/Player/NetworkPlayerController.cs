using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
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

    private float _roomForError = 0.6f;


    [SerializeField]
    private Rigidbody _rb;


    private float _terrainColliderHeight = 0f;

    private Player player;

    private float _rotationX = 0;
    private bool _isGrounded = false;

    private int _tick = 0;
    private float _tickRate = 1.0f / 90.0f;
    private float _tickDeltaTime = 0.0f;
    private int _inputTicks = 0;

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
        //NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(_messageName, RecieveMovePlayerRequestOnServer);
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

        TransformState ?calculatedState = null;
        for (int i = 0; i < _transformStates.Length; i++)
        {
            if (_transformStates[i].Tick == serverState.Tick)
            {
                calculatedState = _transformStates[i];
                break;
            }
        }

        if(calculatedState == null)
        {
            Debug.LogWarning("NoInputStatesWerePresentAt"+serverState.Tick);
            return;
        }

        if (Vector3.Distance(calculatedState.Value.Position, serverState.Position) < _roomForError && Mathf.Abs(calculatedState.Value.Position.y - serverState.Position.y) < 1) return;

        TeleportPlayer(serverState);



        IEnumerable<InputState> inputs = _inputStates.Where(input => input.Tick > serverState.Tick);

        inputs = from input in inputs orderby input.Tick select input;

        foreach (InputState inputState in inputs)
        {
            HandleMovement(inputState.movementInput, inputState.rotationInput, inputState.DeltaTime);
        }

        TransformState newTransformState = new TransformState()
        {
            Tick = serverState.Tick,
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

    private void TeleportPlayer(TransformState state)
    {
        Debug.Log("tp");
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

        bool groundCheck = GroundCheck();
        if (_isGrounded != groundCheck)
        {
            _isGrounded = groundCheck;
            player.RigAnimator.SetBool("IsGrounded", groundCheck);
        }

        if (IsLocalPlayer)
        {
            if (!IsServer && _tickDeltaTime > _tickRate)
            {

                //InputState input = new();

                //for (int i = 1; i < length; i++)
                //{

                //}


                //MovePlayerRequestServerRpc(_tick, inputs.ToArray());
                List<InputState> inputs = new();

                foreach (var inputState in _inputStates)
                {
                    if(inputState.Tick == _tick)
                        inputs.Add(inputState);
                }
                MovePlayerRequestServerRpc(_tick, inputs.ToArray());

                int transformbufferIndex = _tick % BUFFER_SIZE;

                TransformState transformState = new()
                {
                    Tick = _tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    Facing = player.Head.transform.rotation,
                };

                _transformStates[transformbufferIndex] = transformState;


                _tick++;
                _tickDeltaTime -= _tickRate;
            }

            if (player.AreControlsDisabled) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleJump();
                if (!NetworkManager.IsServer)
                {
                    JumpServerRPC();
                }
            }

            //if (_tickDeltaTime > _tickRate)
            //{
                if (!CanMove()) return;


                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                Vector3 moveInput = new(horizontalInput, 0, verticalInput);

                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                Vector2 rotationInput = new(mouseY, mouseX);
                ProcessLocalPlayerMovement(moveInput, rotationInput);
            //}
        }
        else
        {
            ProcessSimulatedPlayerMovement();
        }
    }
    private void ProcessLocalPlayerMovement(Vector3 moveInput, Vector3 rotationInput)
    {
        int inputbufferIndex = _inputTicks % BUFFER_SIZE;
        _inputTicks++;

        if (NetworkManager.IsServer)
        {
            HandleMovement(moveInput, rotationInput, Time.deltaTime);

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
            HandleMovement(moveInput, rotationInput, Time.deltaTime);
            //MovePlayerRequestServerRpc(_tick, moveInput, rotationInput, Time.deltaTime, bufferIndex);

            //transform.position = ServerTransformState.Value.Position;
            //transform.rotation = ServerTransformState.Value.Rotation;
        }

        InputState inputState = new()
        {
            Tick = _tick,
            movementInput = moveInput,
            rotationInput = rotationInput,
            DeltaTime = Time.deltaTime,
        };

        _inputStates[inputbufferIndex] = inputState;

        ///UpdateTick();
        //_tickDeltaTime -= _tickRate;
        //_tick++;
    }

    private void ProcessSimulatedPlayerMovement()
    {
        if (!NetworkManager.IsServer)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, ServerTransformState.Value.Position, _tickDeltaTime * _speed);

            player.RigAnimator.SetFloat("SpeedX", transform.InverseTransformPoint(newPosition).x);
            player.RigAnimator.SetFloat("SpeedY", transform.InverseTransformPoint(newPosition).y);
            player.RigAnimator.SetFloat("Speed", Vector3.Distance(transform.position, newPosition));

            transform.position = newPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, ServerTransformState.Value.Rotation, _tickDeltaTime * _speed);
            Quaternion facing = Quaternion.Lerp(player.Head.transform.rotation, ServerTransformState.Value.Facing, _tickDeltaTime * _speed);
            player.Head.transform.rotation = facing;
            player.Hand.transform.rotation = facing;
        }

        UpdateTick();
    }

    private void HandleMovement(Vector3 moveInput, Vector3 rotationInput, float tickRate)
    {
        player.RigAnimator.SetFloat("SpeedX", moveInput.x);
        player.RigAnimator.SetFloat("SpeedY", moveInput.z);
        player.RigAnimator.SetFloat("Speed", Vector3.Distance(Vector3.zero, moveInput));
        var newPos = transform.TransformDirection(_speed * tickRate * moveInput);
        _rb.MovePosition(transform.position + newPos);
        var newRot = _tickRate * _turnSpeed * new Vector3(0, rotationInput.y, 0);
        //transform.Translate(_speed * _tickRate * moveInput);
        transform.Rotate(newRot);

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
        if (_isGrounded)
        {
            _isGrounded = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            player.RigAnimator.SetBool("IsGrounded", false);
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
    //private void MovePlayerRequestServerRpc(int tick, InputState inputState)
    //{
    //    HandleMovement(inputState.movementInput, inputState.rotationInput, _tickRate);

    //    TransformState state = new TransformState()
    //    {
    //        Tick = tick,
    //        Position = transform.position,
    //        Rotation = transform.rotation,
    //        Facing = player.Head.transform.rotation
    //    };

    //    ServerTransformState.Value = state;
    //}

    [ServerRpc(RequireOwnership = false)]
    private void MovePlayerRequestServerRpc(int tick, InputState[] inputStates)
    {
        foreach (var inputState in inputStates)
        {
            HandleMovement(inputState.movementInput, inputState.rotationInput, inputState.DeltaTime);
        }

        TransformState state = new TransformState()
        {
            Tick = tick,
            Position = transform.position,
            Rotation = transform.rotation,
            Facing = player.Head.transform.rotation
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
    //private string _messageName = "SendMovePlayerRequestToServer";

    //private void RecieveMovePlayerRequestOnServer(ulong senderId, FastBufferReader messagePayload)
    //{
    //    var recieveMessageContent = messagePayload.Serialize();
    //    Debug.Log(recieveMessageContent);
    //    Debug.Log(recieveMessageContent.json);
    //}
    //public void SendMovePlayerRequestToServer(int tick, InputState[] message)
    //{
    //    //Write
    //    var writeSize = FastBufferWriter.GetWriteSize(message) + FastBufferWriter.GetWriteSize(tick); //Get size of bytes to allocate network buffer
    //    using FastBufferWriter writer = new FastBufferWriter(writeSize, Allocator.Temp);

    //    writer.WriteValueSafe(message); //Write to 
    //    writer.WriteValueSafe(tick); //Write to buffer
    //    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(_messageName, NetworkManager.ServerClientId, writer,
    //        NetworkDelivery.ReliableFragmentedSequenced);
    //}
}
