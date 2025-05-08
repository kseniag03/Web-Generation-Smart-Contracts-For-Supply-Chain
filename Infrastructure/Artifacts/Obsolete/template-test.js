const { expect } = require("chai");
const { ethers } = require("hardhat");

describe("{CONTRACT_NAME} (complete)", function () {
    let contract;
    let owner, labelCreator, ioTSender, ioTReceiver, nonAuthorized, anotherSender;

    beforeEach(async () => {
        [owner, labelCreator, ioTSender, ioTReceiver, nonAuthorized, anotherSender] = await ethers.getSigners();

        // Deploy updated contract
        const IOTContract = await ethers.getContractFactory("{CONTRACT_NAME}");
        contract = await IOTContract.deploy(owner.address);

        // By default, we will authorize labelCreator as a label creator
        await contract.connect(owner).setLabelCreatorAuthorization(labelCreator.address, true);

        // By default, we will authorize ioTSender as a sender only
        await contract.connect(owner).setIoTAuthorization(ioTSender.address, true, false, true);
        // By default, we will authorize ioTReceiver as a receiver only
        await contract.connect(owner).setIoTAuthorization(ioTReceiver.address, false, true, true);
    });

    //
    // 1. should allow only authorized LabelCreator to create a label
    //
    it("1. should allow only authorized LabelCreator to create a label", async () => {
        const labelId = 1;{EVENTS_OPTIONAL}

        await expect(
            contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address)
        )
            .to.emit(contract, "LabelCreated")
            .withArgs(1, ioTSender.address, ioTReceiver.address);{EVENTS_OPTIONAL}

        // Non-authorized user tries to create a label
        await expect(
            contract.connect(nonAuthorized).createLabel(labelId, ioTSender.address, ioTReceiver.address)
        ).to.be.revertedWith("Not an authorized Label Creator");
    });

    //
    // 2. should allow only owner to authorize roles
    //
    it("2. should allow only owner to authorize roles", async () => {
        // Non-owner tries to set label creator
        await expect(
            contract.connect(nonAuthorized).setLabelCreatorAuthorization(anotherSender.address, true)
        ).to.be.revertedWithCustomError(contract, "OwnableUnauthorizedAccount");

        // Owner can authorize new label creator
        await contract.connect(owner).setLabelCreatorAuthorization(anotherSender.address, true);
        expect(await contract.authorizedLabelCreators(anotherSender.address)).to.equal(true);

        // Non-owner tries to set IoT authorization
        await expect(
            contract.connect(nonAuthorized).setIoTAuthorization(anotherSender.address, true, true, true)
        ).to.be.revertedWithCustomError(contract, "OwnableUnauthorizedAccount");

        // Owner can set IoT authorization
        await contract.connect(owner).setIoTAuthorization(anotherSender.address, true, true, true);
        expect(await contract.authorizedIoTSenders(anotherSender.address)).to.equal(true);
        expect(await contract.authorizedIoTReceivers(anotherSender.address)).to.equal(true);
    });

    //
    // 3. should allow only authorized IoTSender to mark as Sent
    //
    it("3. should allow only authorized IoTSender to mark as Sent", async () => {
        const labelId = 1;

        // First, create a label (by an authorized labelCreator)
        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);{EVENTS_OPTIONAL}

        // The authorized sender (ioTSender) can mark as sent
        await expect(contract.connect(ioTSender).markAsSent(labelId))
            .to.emit(contract, "LabelSent")
            .withArgs(labelId, ioTSender.address);{EVENTS_OPTIONAL}

        // A non-authorized user tries to mark as sent => revert
        await expect(contract.connect(nonAuthorized).markAsSent(labelId))
            .to.be.revertedWith("Not an authorized IoT Sender");
    });

    //
    // 4. should allow only authorized IoTReceiver to mark as Received
    //
    it("4. should allow only authorized IoTReceiver to mark as Received", async () => {
        const labelId = 1;

        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);

        // Mark it as sent first
        await contract.connect(ioTSender).markAsSent(labelId);{EVENTS_OPTIONAL}

        // The authorized receiver can mark as received
        await expect(contract.connect(ioTReceiver).markAsReceived(labelId))
            .to.emit(contract, "LabelReceived")
            .withArgs(labelId, ioTReceiver.address);{EVENTS_OPTIONAL}

        // A non-authorized user tries to mark as received => revert
        await expect(contract.connect(nonAuthorized).markAsReceived(labelId))
            .to.be.revertedWith("Not an authorized IoT Receiver");
    });

    //
    // 5. should revert when trying to create label with invalid receiver
    //
    it("5. should revert when trying to create label with invalid receiver", async () => {
        const labelId = 1;
        const ZERO_ADDRESS = ethers.ZeroAddress;

        await expect(
            contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ZERO_ADDRESS)
        ).to.be.revertedWith("Invalid receiver address");
    });

    //
    // 6. should revert if non-authorized user tries to execute restricted functions
    //
    it("6. should revert if non-authorized user tries to execute restricted functions", async () => {
        const labelId = 1;

        // Check markAsSent
        await expect(
            contract.connect(nonAuthorized).markAsSent(labelId)
        ).to.be.revertedWith("Not an authorized IoT Sender");

        // Check markAsReceived
        await expect(
            contract.connect(nonAuthorized).markAsReceived(labelId)
        ).to.be.revertedWith("Not an authorized IoT Receiver");

        // Check setLabelCreatorAuthorization
        await expect(
            contract.connect(nonAuthorized).setLabelCreatorAuthorization(labelCreator.address, true)
        ).to.be.revertedWithCustomError(contract, "OwnableUnauthorizedAccount");
    });

    //
    // 7. should store labels correctly in the mapping
    //
    it("7. should store labels correctly in the mapping", async () => {
        const labelId = 1;

        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);

        const label = await contract.labels(labelId);

        expect(label.id).to.eq(labelId);
        expect(label.sender).to.eq(ioTSender.address);
        expect(label.receiver).to.eq(ioTReceiver.address);
        expect(label.sent).to.eq(false);
        expect(label.received).to.eq(false);{VOID_OPTIONAL}
        expect(label.voided).to.eq(false);{VOID_OPTIONAL}
    });

    //
    // 8. should initialize the contract with the correct owner
    //
    it("8. should initialize the contract with the correct owner", async () => {
        expect(await contract.owner()).to.equal(owner.address);
    });

    //
    // 9. Should prevent replay attack on markAsReceived
    //
    it("9. Should prevent replay attack on markAsReceived", async () => {
        const labelId = 1;

        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);

        // Must be sent before it can be received
        await contract.connect(ioTSender).markAsSent(labelId);

        // Mark as received (first time)
        await contract.connect(ioTReceiver).markAsReceived(labelId);

        // Mark as received (second time) => revert
        await expect(contract.connect(ioTReceiver).markAsReceived(labelId))
            .to.be.revertedWith("Label already marked as received");
    });

    //
    // 10. Should not allow unauthorized sender to mark label as sent
    //
    it("10. Should not allow unauthorized sender to mark label as sent", async () => {
        const labelId = 1;

        // We will authorize labelCreator, but not 'anotherSender'
        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);

        // If 'ioTReceiver' is also authorized as a sender, we need to demonstrate that
        // even if they are authorized, they're not the *correct* sender for that label
        // So let's authorize ioTReceiver as a sender, but the label sender is 'ioTSender'
        await contract.connect(owner).setIoTAuthorization(ioTReceiver.address, true, true, true);

        // Now try: ioTReceiver calls markAsSent => revert because they're not the label's `sender`
        await expect(
            contract.connect(ioTReceiver).markAsSent(labelId)
        ).to.be.revertedWith("Not the sender of this label");
    });

    //
    // 11. Should not allow unauthorized sender to create label
    //
    it("11. Should not allow unauthorized sender to create label", async () => {
        const labelId = 1;

        // 'labelCreator' is authorized to create a label, but we pass 'nonAuthorized' as the label's sender
        // If 'nonAuthorized' is not an authorized IoTSender, it fails
        await expect(
            contract.connect(labelCreator).createLabel(labelId, nonAuthorized.address, ioTReceiver.address)
        ).to.be.revertedWith("Sender not authorized");
    });{VOID_OPTIONAL}

    //
    // 12. Should prevent double voiding of label
    //
    it("12. Should prevent double voiding of label", async () => {
        const labelId = 1;

        // We'll create a label
        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);

        // We (ioTSender) are authorized as a sender device => can also void
        await contract.connect(ioTSender).voidLabel(labelId);

        // Attempt voiding again
        await expect(
            contract.connect(ioTSender).voidLabel(labelId)
        ).to.be.revertedWith("Label already voided");
    });{VOID_OPTIONAL}

    //
    // ---------------- Extended Features ----------------
    //

    //
    // 13. Should (de)authorize LabelCreators via setLabelCreatorAuthorization
    //
    it("13. should (de)authorize LabelCreators via setLabelCreatorAuthorization", async () => {
        // labelCreator is authorized from beforeEach
        expect(await contract.authorizedLabelCreators(labelCreator.address)).to.equal(true);

        // De-authorize
        await contract.connect(owner).setLabelCreatorAuthorization(labelCreator.address, false);
        expect(await contract.authorizedLabelCreators(labelCreator.address)).to.equal(false);

        const labelId = 1;

        // Now labelCreator can no longer create labels
        await expect(
            contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address)
        ).to.be.revertedWith("Not an authorized Label Creator");
    });

    //
    // 14. Should (de)authorize IoT roles with setIoTAuthorization
    //
    it("14. should (de)authorize IoT roles with setIoTAuthorization", async () => {
        // Initially, ioTSender is authorized as a sender, ioTReceiver is authorized as a receiver
        expect(await contract.authorizedIoTSenders(ioTSender.address)).to.equal(true);
        expect(await contract.authorizedIoTReceivers(ioTReceiver.address)).to.equal(true);

        const labelId = 1;

        await contract.connect(labelCreator).createLabel(labelId, ioTSender.address, ioTReceiver.address);

        // De-authorize the sender
        await contract.connect(owner).setIoTAuthorization(ioTSender.address, true, false, false);
        expect(await contract.authorizedIoTSenders(ioTSender.address)).to.equal(false);

        // De-authorize the receiver
        await contract.connect(owner).setIoTAuthorization(ioTReceiver.address, false, true, false);
        expect(await contract.authorizedIoTReceivers(ioTReceiver.address)).to.equal(false);

        // Attempt to markAsSent with ioTSender => revert now that they're not authorized
        await expect(
            contract.connect(ioTSender).markAsSent(1)
        ).to.be.revertedWith("Not an authorized IoT Sender");
    });

    //
    // 15. Only owner should be able to set device info
    //
    it("15. Only owner should be able to set device info", async () => {
        const location = "Moscow";

        // Owner sets device info
        await contract.connect(owner).setDeviceInfo(ioTReceiver.address, location);

        // Verify the stored device info
        const deviceInfo = await contract.deviceInfo(ioTReceiver.address);
        expect(deviceInfo.location).to.equal(location);

        // Non-owner attempt should fail
        await expect(contract.connect(ioTSender).setDeviceInfo(ioTReceiver.address, location))
            .to.be.revertedWithCustomError(contract, "OwnableUnauthorizedAccount");
    });
});