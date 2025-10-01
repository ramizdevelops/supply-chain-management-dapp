// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

contract ProductRegistry {
    struct Product {
        string productId;
        bytes32 productHash;
        address manufacturer;
        uint256 timestamp;
        bool exists;
        bool isFake;
    }

    mapping(string => Product) private products;
    address public admin;

    event ProductRegistered(string indexed productId, address indexed manufacturer, bytes32 productHash, uint256 timestamp);
    event ProductMarkedFake(string indexed productId, address indexed admin, uint256 timestamp);

    modifier onlyAdmin() {
        require(msg.sender == admin, "only admin");
        _;
    }

    constructor() {
        admin = msg.sender;
    }

    function registerProduct(string memory productId, bytes32 productHash) public {
        require(!products[productId].exists, "already registered");
        products[productId] = Product(productId, productHash, msg.sender, block.timestamp, true, false);
        emit ProductRegistered(productId, msg.sender, productHash, block.timestamp);
    }

    function markProductFake(string memory productId) public onlyAdmin {
        require(products[productId].exists, "not exists");
        products[productId].isFake = true;
        emit ProductMarkedFake(productId, msg.sender, block.timestamp);
    }

    function getProduct(string memory productId) public view returns (bool exists, bytes32 productHash, address manufacturer, uint256 timestamp, bool isFake) {
        Product memory p = products[productId];
        if (!p.exists) return (false, 0x0, address(0), 0, false);
        return (true, p.productHash, p.manufacturer, p.timestamp, p.isFake);
    }
}