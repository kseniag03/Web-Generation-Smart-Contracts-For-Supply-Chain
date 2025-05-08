// SPDX-License-Identifier: MIT
pragma solidity ^0.8.22;

import "@openzeppelin/contracts/access/Ownable.sol";

contract {CONTRACT_NAME} is Ownable {
    struct Label {
        {UINT_TYPE} id;
        address sender;
        address receiver;
        bool sent;
        bool received;{VOID_OPTIONAL}
        bool voided;{VOID_OPTIONAL}
    }

    struct IoTDeviceInfo {
        address device;
        string location;
    }

    {UINT_TYPE} public labelCounter;

    mapping({UINT_TYPE} => Label) public labels;

    mapping(address => bool) public authorizedLabelCreators;
    mapping(address => bool) public authorizedIoTSenders;
    mapping(address => bool) public authorizedIoTReceivers;

    // to store extra data of device
    mapping(address => IoTDeviceInfo) public deviceInfo;{EVENTS_OPTIONAL}
    
    event LabelCreated({UINT_TYPE} labelId, address sender, address receiver);
    event LabelSent({UINT_TYPE} labelId, address sender);
    event LabelReceived({UINT_TYPE} labelId, address receiver);
    event LabelVoided({UINT_TYPE} labelId, address by);{EVENTS_OPTIONAL}

    modifier onlyLabelCreator() {
        require(
            authorizedLabelCreators[msg.sender],
            "Not an authorized Label Creator"
        );
        _;
    }

    modifier onlyIoTSender() {
        require(
            authorizedIoTSenders[msg.sender],
            "Not an authorized IoT Sender"
        );
        _;
    }

    modifier onlyIoTReceiver() {
        require(
            authorizedIoTReceivers[msg.sender],
            "Not an authorized IoT Receiver"
        );
        _;
    }

    modifier onlyIoTDevice() {
        require(
            authorizedIoTSenders[msg.sender] ||
                authorizedIoTReceivers[msg.sender],
            "Not an authorized IoT Device"
        );
        _;
    }

    constructor(address initialOwner) Ownable(initialOwner) {}

    // ------------------------------------------------------------------------
    // Authorization functions
    // ------------------------------------------------------------------------

    function setLabelCreatorAuthorization(
        address _account,
        bool _status
    ) external onlyOwner {
        authorizedLabelCreators[_account] = _status;
    }

    function setIoTAuthorization(
        address _account,
        bool _asSender,
        bool _asReceiver,
        bool _status
    ) external onlyOwner {
        if (_asSender) {
            authorizedIoTSenders[_account] = _status;
        }
        if (_asReceiver) {
            authorizedIoTReceivers[_account] = _status;
        }
    }

    function setDeviceInfo(
        address _device,
        string calldata _location
    ) external onlyOwner {
        deviceInfo[_device] = IoTDeviceInfo({
            device: _device,
            location: _location
        });
    }

    // ------------------------------------------------------------------------
    // Core label logic
    // ------------------------------------------------------------------------

    function createLabel(
        {UINT_TYPE} _labelId,
        address _sender,
        address _receiver
    ) external onlyLabelCreator {
        require(_receiver != address(0), "Invalid receiver address");
        require(authorizedIoTSenders[_sender], "Sender not authorized");

        labelCounter++;
        labels[_labelId] = Label({
            id: _labelId,
            sender: _sender,
            receiver: _receiver,
            sent: false,
            received: false{VOID_OPTIONAL},
            voided: false{VOID_OPTIONAL}
        });{EVENTS_OPTIONAL}

        emit LabelCreated(labelCounter, _sender, _receiver);{EVENTS_OPTIONAL}
    }

    function markAsSent({UINT_TYPE} _labelId) external onlyIoTSender {
        Label storage label = labels[_labelId];

        require(label.id != 0, "Label does not exist");
        require(!label.sent, "Label already marked as sent");
        require(label.sender == msg.sender, "Not the sender of this label");

        label.sent = true;{EVENTS_OPTIONAL}

        emit LabelSent(_labelId, msg.sender);{EVENTS_OPTIONAL}
    }

    function markAsReceived({UINT_TYPE} _labelId) external onlyIoTReceiver {
        Label storage label = labels[_labelId];

        require(label.id != 0, "Label does not exist");
        require(label.sent, "Label not marked as sent");
        require(!label.received, "Label already marked as received");
        require(label.receiver == msg.sender, "Not the receiver of this label");

        label.received = true;{EVENTS_OPTIONAL}

        emit LabelReceived(_labelId, msg.sender);{EVENTS_OPTIONAL}
    }{VOID_OPTIONAL}

    function voidLabel({UINT_TYPE} _labelId) external onlyIoTDevice {
        Label storage label = labels[_labelId];

        require(label.id != 0, "Label does not exist");
        require(!label.voided, "Label already voided");

        label.voided = true;{EVENTS_OPTIONAL}

        emit LabelVoided(_labelId, msg.sender);{EVENTS_OPTIONAL}
    }{VOID_OPTIONAL}
}
