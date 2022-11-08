using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDataStore _dataStore;
        private readonly string _dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

        public int DataStoreType { get; set; }

        /// <summary>
        /// Determines and intialises data store obj based on the data store type 
        /// </summary>
        /// <param name="dataStore">Injects data store object</param>
        public PaymentService(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// parameter less constructor to maintain backward compatibility with unit tests
        /// </summary>
        public PaymentService()
        {
            _dataStore = DataStoreFactory.GetDataStore(_dataStoreType);
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            Account account = GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();
            result.Success = ValidateAccount(request, account);

            if (result.Success)
            {
                account.Balance -= request.Amount;
                UpdateAccount(account);

                //Added new property to account class to validate the return info for unit test cases
                //This also provides the user updated account info on successful payment
                result.UpdatedAccount = GetAccount(request.DebtorAccountNumber);
            }

            return result;
        }

        #region "Account releated operations"
        
        private Account GetAccount(string accountNumber)
        {
            return _dataStore.GetAccount(accountNumber);
        }

        private void UpdateAccount(Account account)
        {
            _dataStore.UpdateAccount(account);
        }

        #endregion


        #region "Validations: Business logic is not altered"

        private static bool ValidateAccount(MakePaymentRequest request, Account account)
        {
            //gaurd clause
            if (account == null)
               return false;

            //Common validation 
            if (!ValidateAccountByAllowedPaymentSchemes(request, account))
                return false;

            //payment scheme specific validation
            if (!ValidateAccountStateByPaymentScheme(request, account))
                return false;

            return true;


        }

        private static bool ValidateAccountByAllowedPaymentSchemes(MakePaymentRequest request, Account account)
        {
            if (!account.AllowedPaymentSchemes.HasFlag((AllowedPaymentSchemes)Enum.Parse(typeof(AllowedPaymentSchemes), request.PaymentScheme.ToString())))
            {
                return false;
            }
           
            return true;
        }

        private static bool ValidateAccountStateByPaymentScheme(MakePaymentRequest request, Account account)
        {
            var isValidAccount = true;

            //Following are the condition checks for an invalid account
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    break;

                case PaymentScheme.FasterPayments:
                    if (account.Balance < request.Amount)
                    {
                        isValidAccount = false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (account.Status != AccountStatus.Live)
                    {
                        isValidAccount = false;
                    }
                    break;
            }

            return isValidAccount;
        }

        #endregion

    }
}
