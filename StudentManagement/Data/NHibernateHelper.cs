using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using Npgsql;
using StudentManagement.Mapping;
using System;
using System.Linq;

namespace StudentManagement.Data
{
    public static class NHibernateHelper
    {
        /// <summary>
        /// Create an ISessionFactory configured for PostgreSQL (NeonDB). Provide a valid connection string.
        /// </summary>
        public static ISessionFactory CreateSessionFactory(string connectionString, bool createSchema = false)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required", nameof(connectionString));

            var cfg = new Configuration();

            cfg.DataBaseIntegration(db =>
            {
                db.ConnectionString = connectionString;
                db.Driver<NHibernate.Driver.NpgsqlDriver>();
                db.Dialect<NHibernate.Dialect.PostgreSQLDialect>();
                db.LogSqlInConsole = false;
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            });

            // add mappings from the Mapping assembly
            var mapper = new ModelMapper();
            mapper.AddMappings(typeof(TeacherMap).Assembly.GetTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddMapping(mapping);

            if (createSchema)
            {
                // Create database objects. This is intended for development or CI usage.
                var export = new SchemaExport(cfg);
                // show DDL to console and execute against DB
                export.Create(false, true);
            }

            return cfg.BuildSessionFactory();
        }

        /// <summary>
        /// Helper that runs schema export/update using the same mapping. For development only.
        /// </summary>
        public static void CreateSchema(string connectionString)
        {
            // reuse CreateSessionFactory logic but call SchemaExport explicitly
            var cfg = new Configuration();

            cfg.DataBaseIntegration(db =>
            {
                db.ConnectionString = connectionString;
                db.Driver<NHibernate.Driver.NpgsqlDriver>();
                db.Dialect<NHibernate.Dialect.PostgreSQLDialect>();
                db.LogSqlInConsole = false;
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            });

            var mapper = new ModelMapper();
            mapper.AddMappings(typeof(TeacherMap).Assembly.GetTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddMapping(mapping);

            var export = new SchemaExport(cfg);
            export.Create(false, true);

            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("""
        ALTER TABLE classes
          ALTER COLUMN id SET DEFAULT nextval('classes_id_seq');

        ALTER SEQUENCE classes_id_seq
          OWNED BY classes.id;

        ALTER TABLE students
            ALTER COLUMN id SET DEFAULT nextval('students_id_seq');

        ALTER SEQUENCE students_id_seq
            OWNED BY students.id;

        ALTER TABLE teachers
            ALTER COLUMN id SET DEFAULT nextval('teachers_id_seq');
        ALTER SEQUENCE teachers_id_seq
            OWNED BY teachers.id;
        """, connection);

            command.ExecuteNonQuery();

        }
    }
}
