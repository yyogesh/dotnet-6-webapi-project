using ASPPRODUCT.Models;

namespace ASPPRODUCT.Container;

public interface IProductContainer
{
    Task<List<TblProduct>> GetAll();

    Task<TblProduct> GetByCode(int code);

    Task<bool> Remove(int code);
    Task<bool> Save(TblProduct _product);
}