namespace GroceryListHelper.Core.HelperMethods;
internal interface ITable : ITableEntity
{
    static abstract string GetTableName();
}
