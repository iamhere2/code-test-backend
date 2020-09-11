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
            var companyData = MapToCompanyDataRequest(application.CompanyData);

            return application.Product switch
               {
                   SelectiveInvoiceDiscount sid
                       => SubmitRequest(companyData, sid),

                   ConfidentialInvoiceDiscount cid
                       => SubmitRequest(companyData, cid),

                   BusinessLoans loans
                       => SubmitRequest(companyData, loans),

                   _ => throw new InvalidOperationException(
                       $"Unknown/unsupported product: {application.Product.GetType().Name}")
               };
        }

        private IApplicationResult SubmitRequest(CompanyDataRequest companyData, BusinessLoans loans)
        {
            var loansRequest = new LoansRequest
            {
                InterestRatePerAnnum = loans.InterestRatePerAnnum,
                LoanAmount = loans.LoanAmount
            };

            var result = _businessLoansService.SubmitApplicationFor(
                companyData, loansRequest);

            return result;
        }

        private IApplicationResult SubmitRequest(CompanyDataRequest companyData, ConfidentialInvoiceDiscount cid)
        {
            var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                companyData, cid.TotalLedgerNetworth, cid.AdvancePercentage, cid.VatRate);

            return result;
        }

        private IApplicationResult SubmitRequest(CompanyDataRequest companyData, SelectiveInvoiceDiscount sid)
        {
            var companyNumberStr = companyData.CompanyNumber.ToString(CultureInfo.InvariantCulture);

            int code = _selectInvoiceService.SubmitApplicationFor(
                companyNumberStr, sid.InvoiceAmount, sid.AdvancePercentage);

            return new PlainCodeApplicationResult(code);
        }

        private static CompanyDataRequest MapToCompanyDataRequest(SellerCompanyData companyData)
            => new CompanyDataRequest
            {
                // TODO: Consider using AutoMapper
                CompanyFounded = companyData.Founded,
                CompanyNumber = companyData.Number,
                CompanyName = companyData.Name,
                DirectorName = companyData.DirectorName
            };
    }
}
