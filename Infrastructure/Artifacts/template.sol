// SPDX-License-Identifier: MIT
pragma solidity ^0.8.22;

import "@openzeppelin/contracts/access/Ownable.sol";

contract IOTContractMonitoring is Ownable {
    struct Label {
        uint id;
        address sender;
        address receiver;
        bool sent;
        bool received;
        bool voided;
    }

    struct IoTDeviceInfo {
        address device;
        string location;
    }

    uint public labelCounter;

    mapping(uint => Label) public labels;

    mapping(address => bool) public authorizedLabelCreators;
    mapping(address => bool) public authorizedIoTSenders;
    mapping(address => bool) public authorizedIoTReceivers;

    // to store extra data of device
    mapping(address => IoTDeviceInfo) public deviceInfo;

    event LabelCreated(uint labelId, address sender, address receiver);
    event LabelSent(uint labelId, address sender);
    event LabelReceived(uint labelId, address receiver);
    event LabelVoided(uint labelId, address by);

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
    // Combined functions to set or unset authorization with a single call.
    // Set `_status` to `true` to authorize, `false` to de-authorize.
    // ------------------------------------------------------------------------

    function setLabelCreatorAuthorization(
        address _account,
        bool _status
    ) external onlyOwner {
        authorizedLabelCreators[_account] = _status;
    }

    /**
     * @dev This single function toggles (de)authorization
     *     for IoT Sender and/or IoT Receiver in one call.
     * @param _asSender   Set true if you want to (de)authorize as Sender
     * @param _asReceiver Set true if you want to (de)authorize as Receiver
     * @param _status     True = authorize, False = de-authorize
     */
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

    /**
     * @notice Optionally store extra device info (like location).
     *         This can also be used to ensure that the device’s location
     *         matches a Label’s destination, if your use-case requires it.
     */
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
        address _sender,
        address _receiver
    ) external onlyLabelCreator {
        require(_receiver != address(0), "Invalid receiver address");
        require(authorizedIoTSenders[_sender], "Sender not authorized");

        labelCounter++;
        labels[labelCounter] = Label({
            id: labelCounter,
            sender: _sender,
            receiver: _receiver,
            sent: false,
            received: false,
            voided: false
        });

        emit LabelCreated(labelCounter, _sender, _receiver);
    }

    function markAsSent(uint _labelId) external onlyIoTSender {
        Label storage label = labels[_labelId];

        require(label.id != 0, "Label does not exist");
        require(!label.sent, "Label already marked as sent");
        require(label.sender == msg.sender, "Not the sender of this label");

        label.sent = true;
        emit LabelSent(_labelId, msg.sender);
    }

    function markAsReceived(uint _labelId) external onlyIoTReceiver {
        Label storage label = labels[_labelId];

        require(label.id != 0, "Label does not exist");
        require(label.sent, "Label not marked as sent");
        require(!label.received, "Label already marked as received");
        require(label.receiver == msg.sender, "Not the receiver of this label");

        label.received = true;
        emit LabelReceived(_labelId, msg.sender);
    }

    // “Void” a label. For example if the package is lost or returned.
    function voidLabel(uint _labelId) external onlyIoTDevice {
        Label storage label = labels[_labelId];

        require(label.id != 0, "Label does not exist");
        require(!label.voided, "Label already voided");

        label.voided = true;
        emit LabelVoided(_labelId, msg.sender);
    }
}