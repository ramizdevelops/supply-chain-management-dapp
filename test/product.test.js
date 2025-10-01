const { expect } = require("chai");

describe("ProductRegistry", function() {
  it("registers and returns product", async function() {
    const Registry = await ethers.getContractFactory("ProductRegistry");
    const reg = await Registry.deploy();
    await reg.deployed();

    const productId = "PRD-TEST-0001";
    const hash = ethers.utils.keccak256(ethers.utils.toUtf8Bytes(JSON.stringify({productId:"PRD-TEST-0001"})));

    await reg.registerProduct(productId, hash);
    const res = await reg.getProduct(productId);
    expect(res.exists).to.be.true;
    expect(res.productHash).to.equal(hash);
  });
});