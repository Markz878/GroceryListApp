namespace GroceryListHelper.Core.DataAccess.HelperMethods;
internal interface ITable : ITableEntity
{
    static abstract string GetTableName();
}
