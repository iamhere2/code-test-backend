using System;
using System.Globalization;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public class ProductApplicationService
    {
        private readonly ISelectInvoiceService _selectInvoiceService;
        private readonly IConfidentialInvoiceService _confidentialInvoiceWebService;
        private readonly IBusinessLoansService _businessLoansService;

        public ProductApplicationService(ISelectInvoiceService selectInvoiceService, IConfidentialInvoiceService confidentialInvoiceWebService, IBusinessLoansService businessLoansService)
        {
            _selectInvoiceService = selectInvoiceService 
                ?? throw new ArgumentNullException(nameof(selectInvoiceService));

            _confidentialInvoiceWebService = confidentialInvoiceWebService 
                ?? throw new ArgumentNullException(nameof(confidentialInvoiceWebService));

            _businessLoansService = businessLoansService 
                ?? throw new ArgumentNullException(nameof(businessLoansService));
        }

        public IApplicationResult SubmitApplicationFor(SellerApplication application)
        {
            if (application.Product is SelectiveInvoiceDiscount sid)
            {
                int code = _selectInvoiceService.SubmitApplicationFor(
                    application.CompanyData.Number.ToString(CultureInfo.InvariantCulture), sid.InvoiceAmount, sid.AdvancePercentage);
                return new PlainCodeApplicationResult(code);
            }

            if (application.Product is ConfidentialInvoiceDiscount cid)
            {
                var companyData = ToCompanyDataRequest(application.CompanyData);

                var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                    companyData, cid.TotalLedgerNetworth, cid.AdvancePercentage, cid.VatRate);

                return result;
            }

            if (application.Product is BusinessLoans loans)
            {
                var companyData = ToCompanyDataRequest(application.CompanyData);
                var loansRequest = new LoansRequest
                {
                    InterestRatePerAnnum = loans.InterestRatePerAnnum,
                    LoanAmount = loans.LoanAmount
                };

                var result = _businessLoansService.SubmitApplicationFor(
                    companyData, loansRequest);

                return result;
            }

            throw new InvalidOperationException($"Unknown/unsupported product: {application.Product}");
        }

        private static CompanyDataRequest ToCompanyDataRequest(SellerCompanyData companyData)
            => new CompanyDataRequest
            {
                CompanyFounded = companyData.Founded,
                CompanyNumber = companyData.Number,
                CompanyName = companyData.Name,
                DirectorName = companyData.DirectorName
            };
    }
}
