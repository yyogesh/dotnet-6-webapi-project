using ASPPRODUCT.Entity;
using ASPPRODUCT.Models;

namespace ASPPRODUCT.Container;

public interface IProductContainer
{
    Task<List<ProductEntity>> GetAll();

    Task<ProductEntity> GetByCode(int code);

    Task<bool> Remove(int code);
    Task<bool> Save(ProductEntity _product);
}