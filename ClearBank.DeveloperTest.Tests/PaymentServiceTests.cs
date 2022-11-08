using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NSubstitute;
using AutoFixture;
using FluentAssertions;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceTests
    {
        private readonly PaymentService _sutAccount;
        private readonly PaymentService _sutBackup;
        private readonly IDataStore _accountDataStore = Substitute.For<IAccountDataStore>();
        private readonly IDataStore _backupDataStore = Substitute.For<IBackupAccountDataStore>();

        private readonly IFixture _fixture = new Fixture();


        public PaymentServiceTests()
        {
            _sutAccount = new PaymentService(_accountDataStore);
            _sutBackup = new PaymentService(_backupDataStore);
        }

        #region "Bacs test cases"

        #region "Positive test cases"

        [Theory]
        [InlineData("12345678", 200.00, 800.00, AccountStatus.Live)]
        [InlineData("BACS1234", 200.00, 800.00, AccountStatus.Disabled)]
        [InlineData("BACS1234", 200.00, 800.00, AccountStatus.InboundPaymentsOnly)]
        public void PaymentService_ShouldMakePayment_WhenValidBacsAccountDataStore(string debtorAccountNumber,
                                                                                    decimal paymentAmount,
                                                                                    decimal accountBalance,
                                                                                    AccountStatus accountStatus)
        {
            // Arrange
            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, accountStatus, AllowedPaymentSchemes.Bacs);
            _accountDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);
                    
           var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _accountDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Bacs);

            // Act
            var result = _sutAccount.MakePayment(paymentRequest);


            //Assert   
            Assert.True(result.Success);
            result.Equals(accountToUpdate);

        }

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenValidBacsBackupDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.Bacs);
            _backupDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _backupDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Bacs);

            // Act
            var result = _sutBackup.MakePayment(paymentRequest);


            //Assert   
            Assert.True(result.Success);
            result.Equals(accountToUpdate);

        }

        #endregion

        #region "Negative test cases"


        [Fact]
        public void PaymentService_ShouldMakePayment_WhenInValidBacsAccountDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _accountDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Bacs);

            // Act
            var result = _sutAccount.MakePayment(paymentRequest);

            //Assert   
            Assert.False(result.Success);

        }

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenInValidBacsBackupDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _backupDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

           
            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Bacs);

            // Act
            var result = _sutBackup.MakePayment(paymentRequest);


            //Assert   
            Assert.False(result.Success);

        }

        #endregion

        #endregion

        #region "FasterPayments tests"

        #region "Positive test cases"
        [Fact]
        public void PaymentService_ShouldMakePayment_WhenValidFasterPaymentAccountDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _accountDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _accountDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.FasterPayments);

            // Act
            var result = _sutAccount.MakePayment(paymentRequest);


            //Assert   
            Assert.True(result.Success);
            result.Equals(accountToUpdate);

        }

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenValidFasterPaymentBackupDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _backupDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _backupDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.FasterPayments);

            // Act
            var result = _sutBackup.MakePayment(paymentRequest);


            //Assert   
            Assert.True(result.Success);
            result.Equals(accountToUpdate);

        }

        #endregion

        #region "Negative test cases"

        [Theory]
        [InlineData("12345678", 900.00, 800.00, AccountStatus.Live)]
        [InlineData("BACS1234", 800.50, 800.00, AccountStatus.Disabled)]
        [InlineData("BACS1234", 10.00, -100.00, AccountStatus.InboundPaymentsOnly)]
        public void PaymentService_ShouldMakePayment_WhenInValidFasterPaymentAccountDataStore(string debtorAccountNumber,
                                                                                    decimal paymentAmount,
                                                                                    decimal accountBalance,
                                                                                    AccountStatus accountStatus)
        {
            // Arrange
            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, accountStatus, AllowedPaymentSchemes.FasterPayments);
            _accountDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _accountDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, accountStatus, PaymentScheme.FasterPayments);

            // Act
            var result = _sutAccount.MakePayment(paymentRequest);


            //Assert   
            Assert.False(result.Success);

        }

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenInValidFasterPaymentBackupDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 900.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _backupDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _backupDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.FasterPayments);

            // Act
            var result = _sutBackup.MakePayment(paymentRequest);


            //Assert   
            Assert.False(result.Success);

        }

        #endregion

        #endregion

        #region "Chaps test cases"

        #region "Positive test cases"

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenValidChapsAccountDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.Chaps);
            _accountDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _accountDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Chaps);

            // Act
            var result = _sutAccount.MakePayment(paymentRequest);

            //Assert   
            Assert.True(result.Success);
            result.Equals(accountToUpdate);

        }

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenValidChapsBackupDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.Live, AllowedPaymentSchemes.Chaps);
            _backupDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var accountToUpdate = PrepareAccountToDataStore(debtorAccountNumber, accountBalance, paymentAmount);
            _backupDataStore.UpdateAccount(accountToUpdate);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Chaps);

            // Act
            var result = _sutBackup.MakePayment(paymentRequest);


            //Assert   
            Assert.True(result.Success);
            result.Equals(accountToUpdate);

        }

        #endregion

        #region "Negative test cases"

        [Theory]
        [InlineData("12345678", 200.00, 800.00, AccountStatus.Disabled)]
        [InlineData("BACS1234", 200.00, 800.00, AccountStatus.InboundPaymentsOnly)]
        public void PaymentService_ShouldMakePayment_WhenInValidChapsAccountDataStore(string debtorAccountNumber,
                                                                                    decimal paymentAmount,
                                                                                    decimal accountBalance,
                                                                                    AccountStatus accountStatus)
        {
            // Arrange

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, accountStatus, AllowedPaymentSchemes.Chaps);
            _accountDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, accountStatus, PaymentScheme.Chaps);

            // Act
            var result = _sutAccount.MakePayment(paymentRequest);

            //Assert   
            Assert.False(result.Success);

        }

        [Fact]
        public void PaymentService_ShouldMakePayment_WhenInValidChapsBackupDataStore()
        {
            // Arrange
            const string debtorAccountNumber = "12345678";
            const decimal paymentAmount = 200.00m;
            const decimal accountBalance = 800.50m;

            var accountFromDataStore = PrepareAccountFromDataStore(debtorAccountNumber, accountBalance, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Chaps);
            _backupDataStore.GetAccount(debtorAccountNumber).Returns(accountFromDataStore);

            var paymentRequest = PreparePaymentRequest(debtorAccountNumber, paymentAmount, AccountStatus.Live, PaymentScheme.Chaps);

            // Act
            var result = _sutBackup.MakePayment(paymentRequest);


            //Assert   
            Assert.False(result.Success);

        }

        #endregion

        #endregion
                
        #region "Build mock objects"

        private Account PrepareAccountFromDataStore(string debtorAccountNumber,
            decimal accountBalance,
            AccountStatus accountStatus,
            AllowedPaymentSchemes allowedPaymentScheme)
        {
            var accountFromDataStore = _fixture.Build<Account>()
               .With(a => a.AccountNumber, debtorAccountNumber)
               .With(a => a.Balance, accountBalance)
               .With(a => a.Status, accountStatus)
               .With(a => a.AllowedPaymentSchemes, allowedPaymentScheme)
               .Create();

            return accountFromDataStore;
        }

        private Account PrepareAccountToDataStore(string debtorAccountNumber,
           decimal accountBalance,
           decimal paymentAmount)
        {
            var accountToUpdate = _fixture.Build<Account>()
                .With(a => a.AccountNumber, debtorAccountNumber)
                .With(a => a.Balance, (accountBalance - paymentAmount))
                .Create();

            return accountToUpdate;
        }

        private MakePaymentRequest PreparePaymentRequest(string debtorAccountNumber,
            decimal paymentAmount,
            AccountStatus accountStatus,
            PaymentScheme paymentSchemes)
        {
            var paymentRequest = _fixture.Build<MakePaymentRequest>()
                .With(pr => pr.DebtorAccountNumber, debtorAccountNumber)
                .With(pr => pr.PaymentScheme, paymentSchemes)
                .With(pr => pr.Amount, paymentAmount)
                .Create();

            return paymentRequest;
        }

        #endregion
    }
}
