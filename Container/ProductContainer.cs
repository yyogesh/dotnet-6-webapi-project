using ASPPRODUCT.Models;
using Microsoft.EntityFrameworkCore;
using ASPPRODUCT.Entity;
using AutoMapper;

namespace ASPPRODUCT.Container;

public class ProductsContainer : IProductContainer
{
    private readonly LearnContext _learnContext;

    private readonly IMapper _mapper;
    public ProductsContainer(LearnContext context, IMapper mapper1)
    {
        this._learnContext = context;
        this._mapper = mapper1;
    }
    public async Task<List<ProductEntity>> GetAll()
    {
        List<ProductEntity> resp = new List<ProductEntity>();
        var product = await this._learnContext.TblProducts.ToListAsync();
        if (product != null)
        {
            resp = _mapper.Map<List<TblProduct>, List<ProductEntity>>(product);
        }
        return resp;
    }

    public async Task<ProductEntity> GetByCode(int code)
    {
        var product = await this._learnContext.TblProducts.FindAsync(code);
        if (product != null)
        {
            ProductEntity resp = _mapper.Map<TblProduct, ProductEntity>(product);
            return resp;
        }
        else
        {
            return new ProductEntity();
        }
    }

    public async Task<bool> Remove(int code)
    {
        var product = await this._learnContext.TblProducts.FindAsync(code);
        if (product != null)
        {
            this._learnContext.Remove(product);
            await this._learnContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> Save(ProductEntity _product)
    {
        var product = this._learnContext.TblProducts.FirstOrDefault(o => o.Code == _product.Code);
        if (product != null)
        {
            product.Name = _product.ProductName;
            product.Amount = _product.Price;
            await this._learnContext.SaveChangesAsync();
        }
        else
        {
            TblProduct _prod = _mapper.Map<ProductEntity, TblProduct>(_product);
            this._learnContext.TblProducts.Add(_prod);
            await this._learnContext.SaveChangesAsync();
        }
        return true;
    }
}