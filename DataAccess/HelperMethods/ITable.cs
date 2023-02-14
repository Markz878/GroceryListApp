using Azure.Data.Tables;

namespace GroceryListHelper.DataAccess.HelperMethods;
internal interface ITable : ITableEntity
{
    static abstract string GetTableName();
}
