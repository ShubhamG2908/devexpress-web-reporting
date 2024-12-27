using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Web;
using DevExpress.Xpo.DB;

namespace DevExpressReportMVCDemo.Services
{
    public class MyDBSchemaProvider : IDBSchemaProviderEx
    {
        DBSchemaProviderEx provider;
        public MyDBSchemaProvider()
        {
            this.provider = new DBSchemaProviderEx();
        }
        public DBStoredProcedure[] GetProcedures(SqlDataConnection connection, params string[] procedureList)
        {
            return [];
        }

        public DBTable[] GetTables(SqlDataConnection connection, params string[] tableList)
        {
            return [];
        }

        public DBTable[] GetViews(SqlDataConnection connection, params string[] viewList)
        {
            return provider.GetViews(connection, viewList).ToArray();
        }

        public void LoadColumns(SqlDataConnection connection, params DBTable[] tables)
        {
            provider.LoadColumns(connection, tables);
        }
    }

    public class MyDataSourceWizardDBSchemaProviderFactory : IDataSourceWizardDBSchemaProviderExFactory
    {
        public IDBSchemaProviderEx Create()
        {
            return new MyDBSchemaProvider();
        }
    }
}
