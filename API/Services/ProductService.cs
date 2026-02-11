using API.DAL;
using API.Entities;

namespace API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Product> GetProductsForDisplay()
        {
            // Logic: Perhaps sort by name before sending to UI
            return _repository.GetAll().OrderBy(p => p.Name);
        }

        public Product GetSingleProduct(int id) => _repository.GetById(id);

        public void CreateProduct(Product product) => _repository.Add(product);

        public void UpdateProduct(Product product) => _repository.Update(product);

        public void RemoveProduct(int id) => _repository.Delete(id);
    }
}
