using Microsoft.AspNetCore.Mvc;
using ASPPRODUCT.Models;
using Microsoft.AspNetCore.Authorization;
using ASPPRODUCT.Container;
using ASPPRODUCT.Entity;
using Serilog;

namespace ASPPRODUCT.Controllers;

[Authorize(Roles = "admin,user")]
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductContainer _productContainer;

    public ProductsController(IProductContainer productContainer)
    {
        _productContainer = productContainer;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> Get()
    {
        Log.Information("Ah, there you are!");
        var products = await this._productContainer.GetAll();
        return Ok(products);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("GetByCode/{code}")]
    public async Task<IActionResult> GetByCode(int code)
    {
        var product = await this._productContainer.GetByCode(code);
        return Ok(product);
    }

    [HttpDelete("Remove/{code}")]
    public async Task<IActionResult> Remove(int code)
    {
        var isRemoved = await this._productContainer.Remove(code);
        return Ok(isRemoved);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ProductEntity _product)
    {
        await this._productContainer.Save(_product);
        return Ok(true);
    }
}
