async function main() {
  const [deployer] = await ethers.getSigners();
  console.log("Deploying with", deployer.address);
  
  const Registry = await ethers.getContractFactory("ProductRegistry");
  const reg = await Registry.deploy(); 
  
  // Use 'target' property to get the deployed address (standard for modern Ethers/Hardhat)
  console.log("ProductRegistry deployed to:", reg.target);
}

main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});