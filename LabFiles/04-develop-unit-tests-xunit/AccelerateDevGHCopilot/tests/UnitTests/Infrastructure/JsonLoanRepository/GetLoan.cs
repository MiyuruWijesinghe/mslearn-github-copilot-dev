using NSubstitute;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Library.UnitTests.Infrastructure.JsonLoanRepositoryTests;

public class GetLoanTest
{
    private readonly ILoanRepository _mockLoanRepository;
    private readonly JsonLoanRepository _jsonLoanRepository;
    private readonly IConfiguration _configuration;
    private readonly JsonData _jsonData;

    public GetLoanTest()
    {
        _mockLoanRepository = Substitute.For<ILoanRepository>();
        var jsonPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "Library.Console", "Json"));
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JsonPaths:Authors"] = Path.Combine(jsonPath, "Authors.json"),
                ["JsonPaths:Books"] = Path.Combine(jsonPath, "Books.json"),
                ["JsonPaths:BookItems"] = Path.Combine(jsonPath, "BookItems.json"),
                ["JsonPaths:Patrons"] = Path.Combine(jsonPath, "Patrons.json"),
                ["JsonPaths:Loans"] = Path.Combine(jsonPath, "Loans.json")
            })
            .Build();
        _jsonData = new JsonData(_configuration);
        _jsonLoanRepository = new JsonLoanRepository(_jsonData);
    }

    [Fact]
    public async Task GetLoan_ReturnsLoanWhenLoanIdIsFound()
    {
        const int loanId = 1;
        var expectedLoan = new Loan { Id = loanId };
        _mockLoanRepository.GetLoan(loanId).Returns(expectedLoan);

        var arrangedLoan = await _mockLoanRepository.GetLoan(loanId);
        var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

        Assert.NotNull(actualLoan);
        Assert.Equal(arrangedLoan!.Id, actualLoan.Id);
    }

    
     [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null when ID is not found")]
     public async Task GetLoan_ReturnsNullWhenIdIsNotFound()
     {
         // Arrange
         var loanId = 999; // Loan ID that does not exist in Loans.json

         _mockLoanRepository.GetLoan(loanId).Returns((Loan?)null);

         // Act
         var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

         // Assert
         Assert.Null(actualLoan);
     }

}
