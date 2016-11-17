using NHibernate.Tool.hbm2ddl;
using System;
using System.Data.SQLite;
using System.IO;

namespace MovieReviews.Back
{
    public static class MovieReviewDatabaseStructure
    {
        public static void UpdateSchema()
        {
            var schemaUpdate = new SchemaUpdate(NHibernateHelper.Configuration);
            schemaUpdate.Execute(true, true);
        }

        public static void CreateDatabase(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                SQLiteConnection.CreateFile(dbPath);
            }
        }
    }
}
