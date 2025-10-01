require("@nomicfoundation/hardhat-ethers");

module.exports = {
  solidity: "0.8.17",
  networks: {
    local: {
      url: "http://127.0.0.1:7545",
      // Keep this explicit type definition, which was needed previously
      type: 'http' 
    }
  }
};