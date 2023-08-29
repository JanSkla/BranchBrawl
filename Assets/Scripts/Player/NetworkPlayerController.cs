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
    private TextMeshPro _tag;
    [SerializeField]
    private float _speed = 10;
    [SerializeField]
    private float _turnSpeed = 150;
    [SerializeField]
    private GameObject head;

    private int _tick = 0;
    private float _tickRate = 1.0f / 60.0f;
    private float _tickDeltaTime = 0.0f;

    private const int BUFFER_SIZE = 1024;
    private InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
    private TransformState _previousTransformState;

    public override void OnNetworkSpawn()
    {
        ServerTransformState.OnValueChanged += OnServerStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        ServerTransformState.OnValueChanged -= OnServerStateChanged;
    }

    private void OnServerStateChanged(TransformState _previousState, TransformState serverState)
    {
        if (!IsLocalPlayer || IsServer) return;

        if (_previousTransformState.IsUnityNull())
        {
            _previousTransformState = serverState;
        }

        TransformState calculatedState = _transformStates.First(localState => localState.Tick == serverState.Tick);
        if (calculatedState.Position != serverState.Position)
        {
            Debug.Log("TP");
            TeleportPlayer(serverState); //here



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
                    Facing = head.transform.rotation,
                    HasStartedMoving = true
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
    }

    private void TeleportPlayer(TransformState state)
    {
        transform.position = state.Position;
        transform.rotation = state.Rotation;
        head.transform.rotation = state.Facing;

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
        _tag.text = ServerTransformState.Value.Tick + " " + ServerTransformState.Value.Position + " " + ServerTransformState.Value.Rotation;
        if (IsLocalPlayer)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveInput = new Vector3(horizontalInput, 0, verticalInput);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotationInput = new Vector3(mouseY, mouseX, 0);

            ProcessLocalPlayerMovement(moveInput, rotationInput);
        }
        else
        {
            ProcessSimulatedPlayerMovement();
        }
    }
    private void ProcessLocalPlayerMovement(Vector3 moveInput, Vector3 rotationInput)
    {
        _tickDeltaTime += Time.deltaTime;
        if (_tickDeltaTime > _tickRate)
        {
            int bufferIndex = _tick % BUFFER_SIZE;

            if (IsServer)
            {
                HandleMovement(moveInput, rotationInput);

                TransformState state = new TransformState()
                {
                    Tick = _tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                _previousTransformState = ServerTransformState.Value;
                ServerTransformState.Value = state;
            }
            else
            {
                HandleMovement(moveInput, rotationInput);
                MovePlayerRequestServerRpc(moveInput, rotationInput);

                //transform.position = ServerTransformState.Value.Position;
                //transform.rotation = ServerTransformState.Value.Rotation;
            }

            InputState inputState = new InputState() {
                Tick = _tick,
                movementInput = moveInput,
                rotationInput = rotationInput,
            };

            TransformState transformState = new TransformState()
            {
                Tick = _tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Facing = head.transform.rotation,
                HasStartedMoving = true
            };

            _inputStates[bufferIndex] = inputState;
            _transformStates[bufferIndex] = transformState; 

            _tickDeltaTime -= _tickRate;
            _tick++;
        }
    }

    private void ProcessSimulatedPlayerMovement()
    {
        _tickDeltaTime += Time.deltaTime;

        if (!IsServer && ServerTransformState.Value.HasStartedMoving)
        {
            transform.position = Vector3.Lerp(transform.position, ServerTransformState.Value.Position, Time.deltaTime * _speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, ServerTransformState.Value.Rotation, Time.deltaTime * _speed);
            head.transform.rotation = Quaternion.Lerp(head.transform.rotation, ServerTransformState.Value.Facing, Time.deltaTime * _speed);
        }

        if (_tickDeltaTime > _tickRate)
        {
            _tickDeltaTime -= _tickRate;
            _tick++;
        }

    }

    private void HandleMovement(Vector3 moveInput, Vector3 rotationInput)
    {
        transform.Translate(moveInput * _speed * _tickRate);
        transform.Rotate(new Vector3(0, rotationInput.y, 0) * _turnSpeed * _tickRate); ;
        head.transform.Rotate(new Vector3(-rotationInput.x, 0, 0) * _turnSpeed * _tickRate);

        //handhelditem rotation
        GetNetworkObject(GetComponent<PlayerInventory>().EquippedItem.Value.NetworkObjectId).gameObject.transform.Rotate(new Vector3(-rotationInput.x, 0, 0) * _turnSpeed * _tickRate);
    }

    [ServerRpc]
    private void MovePlayerRequestServerRpc(Vector3 moveInput, Vector3 rotationInput)
    {
        HandleMovement(moveInput, rotationInput);

        TransformState state = new TransformState()
        {
            Tick = _tick,
            Position = transform.position,
            Rotation = transform.rotation,
            Facing = head.transform.rotation,
            HasStartedMoving = true
        };

        _previousTransformState = ServerTransformState.Value;
        ServerTransformState.Value = state;
    }
}
