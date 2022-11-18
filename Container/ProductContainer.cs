using ASPPRODUCT.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPPRODUCT.Container;

public class ProductsContainer : IProductContainer
{
    private readonly LearnContext _learnContext;
    public ProductsContainer(LearnContext context)
    {
        this._learnContext = context;
    }
    public async Task<List<TblProduct>> GetAll()
    {
        return await this._learnContext.TblProducts.ToListAsync();
    }

    public async Task<TblProduct> GetByCode(int code)
    {
        var product = await this._learnContext.TblProducts.FindAsync(code);
        if (product != null)
        {
            return product;
        }
        else
        {
            return new TblProduct();
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

    public async Task<bool> Save(TblProduct _product)
    {
        var product = this._learnContext.TblProducts.FirstOrDefault(o => o.Code == _product.Code);
        if (product != null)
        {
            product.Name = _product.Name;
            product.Amount = _product.Amount;
            await this._learnContext.SaveChangesAsync();
        }
        else
        {
            this._learnContext.TblProducts.Add(_product);
            await this._learnContext.SaveChangesAsync();
        }
        return true;
    }
}