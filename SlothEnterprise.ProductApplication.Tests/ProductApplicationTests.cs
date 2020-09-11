using System;
using System.Linq;
using Moq;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests
{
    public class ProductApplicationTests
    {
        [Fact]
        public void Calls_SelectInvoiceService_With_Proper_Args_And_Success_Result()
        {
            // Arrange
            var sis = new Mock<ISelectInvoiceService>(MockBehavior.Strict);
            var cis = new Mock<IConfidentialInvoiceService>(MockBehavior.Strict);
            var bls = new Mock<IBusinessLoansService>(MockBehavior.Strict);

            var pas = new ProductApplicationService(sis.Object, cis.Object, bls.Object);

            (string num, decimal amount, decimal advPct)? calledArgs = null;
            sis.Setup(s => s.SubmitApplicationFor(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns<string, decimal, decimal>(
                    (num, amount, advPct) =>
                    {
                        calledArgs = (num, amount, advPct);
                        return 0;
                    });

            var app = new SellerApplication
            {
                CompanyData = new SellerCompanyData
                {
                    DirectorName = "Smith",
                    Founded = new DateTime(2000, 01, 01),
                    Name = "ACME",
                    Number = 123
                },
                Product = new SelectiveInvoiceDiscount
                {
                    InvoiceAmount = 123000.456M,
                    AdvancePercentage = 5.67M
                }
            };

            // Act
            var result = pas.SubmitApplicationFor(app);

            // Assert
            Assert.True(result.Success);
            Assert.True(!result.Errors.Any());
            Assert.Null(result.ApplicationId);

            Assert.NotNull(calledArgs);
            Assert.Equal("123", calledArgs.Value.num);
            Assert.Equal(123000.456M, calledArgs.Value.amount);
            Assert.Equal(5.67M, calledArgs.Value.advPct);
        }

        // TODO: Tests for calling other two services
    }
}
