using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication.Applications
{
    public class SellerApplication
    {
        public IProduct Product { get; set; }
        public SellerCompanyData CompanyData { get; set; }
    }
}