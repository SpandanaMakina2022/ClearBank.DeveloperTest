using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Data
{
    public interface IDataStore
    {   
        public Account GetAccount(string accountNumber);

        public void UpdateAccount(Account account);
    }
}
