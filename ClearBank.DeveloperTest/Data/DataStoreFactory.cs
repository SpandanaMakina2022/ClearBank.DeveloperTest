namespace ClearBank.DeveloperTest.Data
{
    public class DataStoreFactory
    {       
        public static IDataStore GetDataStore(string dataStoreType)
        {
            IDataStore dataStore = null;

            if (dataStoreType == "Backup")
                dataStore = new BackupAccountDataStore();
            else
                dataStore = new AccountDataStore();

            return dataStore;
        }
    }
}