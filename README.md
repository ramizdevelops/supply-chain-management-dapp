# Supply Chain Integrity DApp (SCID)

![Node.js](https://img.shields.io/badge/Node.js-18+-green?logo=node.js)
![.NET](https://img.shields.io/badge/.NET-6.0-blueviolet?logo=dotnet)
![Solidity](https://img.shields.io/badge/Solidity-Smart%20Contracts-black?logo=ethereum)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow)

A Minimum Viable Product (MVP) demonstrating anti-counterfeiting measures using a **C# Web API**, a **Solidity Smart Contract**, and a **modern JavaScript/HTML frontend**.

---

## Project Overview (TrueSource Model)
This project creates an **immutable, verifiable record** of a product's manufacturing data (its **Digital Fingerprint / hash**) on a local blockchain.  
Any change to the product's data after registration results in a failed verification, immediately flagging the item as **tampered or counterfeit**.

---

## Architecture
The application runs as a **three-tier Decentralized Application (dApp):**

- **Frontend (UI):** Single-page HTML/JS for user interaction and manufacturer login.  
- **Backend (API):** C# (.NET) service that calculates the secure hash and acts as the secure signing bridge to the blockchain.  
- **Blockchain:** Solidity Smart Contract deployed on a local Ganache network for immutable data storage.  

---

## Prerequisites
Before starting, ensure you have the following installed:

- [Node.js & npm](https://nodejs.org/)  
- [.NET SDK (v6.0 or higher)](https://dotnet.microsoft.com/en-us/download)  
- [Ganache CLI](https://www.npmjs.com/package/ganache) → `npm install -g ganache`  

---

## Setup and Run Guide

### A. Start the Local Blockchain
Open a dedicated terminal for **Ganache** (with persistence enabled):

```bash
ganache -p 7545 --db ./ganache_db
```

### B. Deploy the Smart Contract
Open a second terminal in the project root (Supply Chain Integrity DApp).

1.Install Dependencies

```bash
npm install
```

2.Deploy Contract

```bash
npx hardhat run scripts/deploy.js --network local
```

Copy the Contract Address from the output.

### C. Configure the C# Backend
Update configuration with contract details and administrator key.

Required Data:

New Contract Address: From Step B.2.

Contract ABI: JSON under **"abi"** in
**artifacts/contracts/ProductRegistry.sol/ProductRegistry.json.**

Admin Private Key: From Ganache (Account Index 0).

Update Files:

**backend/Services/BlockchainService.cs → Replace _contractAddress and _abi.**

**backend/appsettings.json → Replace PrivateKey with Ganache Admin Private Key.**

### D. Run the C# Backend

Launch the backend API:

```bash
cd backend
dotnet run
```

It will listen at http://localhost:5000
 (keep running).

### E. Run the Frontend UI

Open project in VS Code.

**Install Live Server extension.**

Right-click index.html → Open with Live Server.
(Usually runs at http://127.0.0.1:5500
).

---

## Manufacturer Authentication (Wallet Simulation)

To access the registration page:

Go to Manufacturer tab.

Register new account (e.g., user@test.com / password).

Login → unlocks registration form.

---

## Testing

Registration:
Manufacturer form → click Register (sends signed transaction to Ganache).

Verification (Success):
Consumer tab → enter identical data → click Verify → Status = ✅ AUTHENTIC

Verification (Failure / Tampering):
Consumer tab → modify Serial Number → click Verify → Status = ❌ TAMPERED

---

## LICENSE
MIT License. See [LICENSE](./LICENSE) for details