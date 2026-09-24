using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using StudentManagement.Data;

namespace StudentManagement.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register NHibernate ISessionFactory and scoped ISession.
        /// Provide a PostgreSQL (NeonDB) connection string.
        /// </summary>
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var sessionFactory = NHibernateHelper.CreateSessionFactory(connectionString);
            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => factory.GetRequiredService<ISessionFactory>().OpenSession());
            return services;
        }
    }
}
