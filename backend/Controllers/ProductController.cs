using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using System.Text.Json;
using System.Security.Cryptography;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase {
    private readonly BlockchainService _bc;
    public ProductController(BlockchainService bc) { _bc = bc; }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ProductDto dto) {
        var json = JsonSerializer.Serialize(dto);
        var hashBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
        // Calculate hashHex for the API response only
        var hashHex = "0x" + BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); 
        
        // *** FIX: Passing raw hashBytes (byte[]) instead of the hex string ***
        var tx = await _bc.RegisterProductAsync(dto.ProductId, hashBytes);
        
        return Ok(new { productId = dto.ProductId, hash = hashHex, tx });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] ProductDto dto) {
        var json = JsonSerializer.Serialize(dto);
        var hashBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
        var hashHex = "0x" + BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        var onchain = await _bc.GetProductAsync(dto.ProductId);
        if (!onchain.exists) return NotFound(new { message = "Not registered" });
        var match = (onchain.productHash.ToLower() == hashHex.ToLower());
        return Ok(new { registered = onchain.exists, onchainHash = onchain.productHash, match, isFake = onchain.isFake });
    }
}