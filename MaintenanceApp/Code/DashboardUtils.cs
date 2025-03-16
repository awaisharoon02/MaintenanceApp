using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;

namespace DXApplication1 {
    public static class DashboardUtils
    {
        public static DashboardConfigurator CreateDashboardConfigurator(IConfiguration configuration, IFileProvider fileProvider)
        {
            DashboardConfigurator configurator = new DashboardConfigurator();
            configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath);
            configurator.SetDashboardStorage(dashboardFileStorage);

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            // Registers an SQL data source.
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("Categories").SelectAllColumnsFromTable()
                .Join("Products", "CategoryID").SelectAllColumnsFromTable()
                .Build("Products_Categories");
            sqlDataSource.Queries.Add(query);
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

            // Registers an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            objDataSource.DataId = "Object Data Source Data Id";
            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

            configurator.SetDataSourceStorage(dataSourceStorage);

            return configurator;
        }
    }
}