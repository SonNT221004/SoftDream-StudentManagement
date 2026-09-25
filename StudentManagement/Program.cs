using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Automapper;
using StudentManagement.Dal.Implementation;
using StudentManagement.Dal.Interface;
using StudentManagement.Data;
using StudentManagement.DTO.Student;
using StudentManagement.Extensions;
using StudentManagement.Repository.Implementation;
using StudentManagement.Repository.Interface;
using StudentManagement.Service.Implementation;
using StudentManagement.Service.Interface;
using System.Text.RegularExpressions;
public static class Program
{
    private static readonly Regex ClassIdsPattern = new(@"^\s*\d+(\s*,\s*\d+)*\s*$", RegexOptions.Compiled);
    private static bool _nHibernateRegistered = false;

    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

        var port = configuration.GetValue<int>("Grpc:Port", 5001);
        // Load .env (if present) so Environment.GetEnvironmentVariable can find NEON_CONNECTION for local dev
        try
        {
            Env.TraversePath().Load();
        }
        catch
        {
            // ignore if .env not present or fails to load
        }

        var storagePath = @"D:\VisualStudio\source\repos\StudentManagement\StudentManagement\Storage";

        // Create service provider used by console UI
        var serviceProvider = CreateServiceProvider(storagePath);
        // Start gRPC server in background (uses same registrations)
        (var grpcApp, var grpcTask) = StartGrpcHost(args, storagePath, port);
        var scope = serviceProvider.CreateScope();
        var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
        var classService = scope.ServiceProvider.GetRequiredService<IClassService>();
        var anlyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();
        //menu
        //Console.WriteLine("Student Management System");
        //while (true)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("==========================");
        //    Console.WriteLine("Menu:");
        //    Console.WriteLine("1. List Students");
        //    Console.WriteLine("2. Add Student");
        //    Console.WriteLine("3. Update Student");
        //    Console.WriteLine("4. Delete Student");
        //    Console.WriteLine("5. Arrange Students by Name");
        //    Console.WriteLine("6. Find Student by ID");
        //    Console.WriteLine("7. Change Storage");
        //    Console.WriteLine("8. Exit");
        //    Console.Write("Choose an option: ");

        //    var choice = Console.ReadLine();
        //    switch (choice)
        //    {
        //        case "1":
        //            {
        //                var response = await studentService.GetAllStudentsAsync(0, 0);
        //                foreach (var student in response.Students)
        //                {
        //                    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
        //                }
        //                //var studentAddress = await anlyticsService.GetStudentCountByAddressAsync();
        //                //foreach (var sa in studentAddress)
        //                //{
        //                //    Console.WriteLine($"Location: {sa.Location}, Number: {sa.NumberOfStudent}");
        //                //}

        //                //var classTeacher = await anlyticsService.GetClassCountPerTeacherAsync();
        //                //foreach (var ct in classTeacher)
        //                //{
        //                //    Console.WriteLine($"Name: {ct.TeacherName}, Number: {ct.NumberOfClass}");
        //                //}

        //                break;
        //            }
        //        case "2":
        //            {
        //                string name;
        //                DateTime dob = DateTime.MinValue;
        //                string address;
        //                List<int> classIds = new List<int>();
        //                bool isValidDate = false;
        //                bool isValidClassIds = false;
        //                //name
        //                Console.Write("Enter Name: ");
        //                name = Console.ReadLine() ?? String.Empty;
        //                //dob
        //                Console.Write("Enter Date of Birth (yyyy-MM-dd): ");
        //                while (!isValidDate)
        //                {
        //                    try
        //                    {
        //                        string input = Console.ReadLine() ?? String.Empty;

        //                        dob = DateTime.ParseExact(input, "yyyy-MM-dd", null);
        //                        isValidDate = true;
        //                    }
        //                    catch (FormatException)
        //                    {
        //                        Console.WriteLine("Invalid date format. Please use yyyy-MM-dd:");
        //                    }
        //                }
        //                //address
        //                Console.Write("Enter Address: ");
        //                address = Console.ReadLine() ?? String.Empty;
        //                //classes
        //                Console.WriteLine("Available Classes:");
        //                var classes = await classService.GetAllClassesAsync();
        //                foreach (var clazz in classes)
        //                {
        //                    Console.WriteLine($"Class ID: {clazz.Id}, Name: {clazz.Name}, Teacher: {clazz.Teacher?.Name ?? "N/A"}");
        //                }
        //                while (!isValidClassIds)
        //                {
        //                    Console.Write("Enter Class IDs (comma-separated): ");
        //                    var classIdsInput = Console.ReadLine();
        //                    if (!string.IsNullOrWhiteSpace(classIdsInput))
        //                    {
        //                        if (classIdsInput.Length > 0 && ClassIdsPattern.IsMatch(classIdsInput))
        //                        {
        //                            {
        //                                classIds = classIdsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                                    .Select(idStr => int.TryParse(idStr.Trim(), out var id) ? id : (int?)null)
        //                                    .Where(id => id.HasValue)
        //                                    .Select(id => id.Value)
        //                                    .ToList();
        //                                var validIds = classes.Select(c => c.Id).ToHashSet();
        //                                var invalid = classIds.Where(i => !validIds.Contains(i)).ToList();
        //                                if (invalid.Count > 0)
        //                                {
        //                                    Console.WriteLine($"Warning: these class IDs were not found and will be ignored: {string.Join(", ", invalid)}");
        //                                    continue;
        //                                }
        //                                classIds = classIds.Where(i => validIds.Contains(i)).ToList();
        //                                isValidClassIds = true;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Invalid input. Please enter a comma-separated list of numeric class IDs.");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        isValidClassIds = true; // Allow empty input for no classes
        //                    }
        //                }

        //                var newStudent = new AddStudentDTO
        //                {
        //                    Name = name,
        //                    DateOfBirth = dob.ToString(),
        //                    Address = address,
        //                    ClassIds = classIds
        //                };
        //                await studentService.CreateStudentAsync(newStudent);
        //                Console.WriteLine("Student added successfully.");

        //                break;
        //            }
        //        case "3":
        //            {
        //                ViewStudentDTO? student = new ViewStudentDTO();
        //                string newName;
        //                DateTime newDateOfBirth = DateTime.MinValue;
        //                string newAddress;
        //                bool isValidDate = false;
        //                bool isValidClassIds = false;
        //                List<int> newClassIds = new List<int>();
        //                Console.Write("Enter Student ID to update: ");
        //                if (int.TryParse(Console.ReadLine(), out int id))
        //                {
        //                    student = await studentService.GetStudentByIdAsync(id);
        //                    if (student == null)
        //                    {
        //                        Console.WriteLine("Student not found.");
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Current Student Info: ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
        //                    }
        //                    if (student?.Classes != null && student.Classes.Count > 0)
        //                    {
        //                        Console.WriteLine("Classes:");
        //                        foreach (var cls in student.Classes)
        //                        {
        //                            Console.WriteLine($" - ID: {cls.Id}: {cls.Name} - Teacher: {cls.Teacher?.Name ?? "N/A"}");
        //                        }
        //                    }
        //                }
        //                //name
        //                Console.Write("Enter new Name (leave blank to keep current): ");
        //                newName = Console.ReadLine() ?? String.Empty;
        //                //dob
        //                Console.WriteLine("Enter new Date of Birth  (yyyy-MM-dd) (leave blank to keep current):");
        //                while (!isValidDate)
        //                {
        //                    try
        //                    {
        //                        string input = Console.ReadLine() ?? String.Empty;
        //                        if (input == String.Empty)
        //                        {
        //                            newDateOfBirth = DateTime.ParseExact(student.DateOfBirth, "yyyy-MM-dd", null);
        //                            isValidDate = true;
        //                            break;
        //                        }
        //                        newDateOfBirth = DateTime.ParseExact(input, "yyyy-MM-dd", null);
        //                        isValidDate = true;
        //                    }
        //                    catch (FormatException)
        //                    {
        //                        Console.WriteLine("Invalid date format. Please use yyyy-MM-dd:");

        //                    }
        //                }


        //                Console.WriteLine("Enter new Address (leave blank to keep current):");
        //                newAddress = Console.ReadLine() ?? String.Empty;

        //                //classes
        //                Console.WriteLine("Available Classes:");
        //                var classes = await classService.GetAllClassesAsync();
        //                foreach (var clazz in classes)
        //                {
        //                    Console.WriteLine($"Class ID: {clazz.Id}, Name: {clazz.Name}, Teacher: {clazz.Teacher?.Name ?? "N/A"}");
        //                }
        //                while (!isValidClassIds)
        //                {
        //                    Console.WriteLine("Enter new Class IDs (comma-separated) (leave blank to keep current):");
        //                    var classIdsInput = Console.ReadLine();
        //                    if (!string.IsNullOrWhiteSpace(classIdsInput))
        //                    {
        //                        if (classIdsInput.Length > 0 && ClassIdsPattern.IsMatch(classIdsInput))
        //                        {
        //                            newClassIds = classIdsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                            .Select(idStr => int.TryParse(idStr.Trim(), out var id) ? id : (int?)null)
        //                            .Where(id => id.HasValue)
        //                            .Select(id => id.Value)
        //                            .ToList();
        //                            var validIds = classes.Select(c => c.Id).ToHashSet();
        //                            var invalid = newClassIds.Where(i => !validIds.Contains(i)).ToList();
        //                            if (invalid.Count > 0)
        //                            {
        //                                Console.WriteLine($"Warning: these class IDs were not found and will be ignored: {string.Join(", ", invalid)}");
        //                                continue;
        //                            }
        //                            newClassIds = newClassIds.Where(i => validIds.Contains(i)).ToList();
        //                            isValidClassIds = true;
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Invalid input. Please enter a comma-separated list of numeric class IDs.");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        isValidClassIds = true; // Allow empty input for no classes
        //                    }
        //                }

        //                //update student entity with new values (if provided) and call service to update
        //                UpdateStudentDTO updateStudentDTO = new UpdateStudentDTO();
        //                updateStudentDTO.Id = student!.Id;
        //                updateStudentDTO.Name = newName == String.Empty ? student.Name : newName;
        //                updateStudentDTO.DateOfBirth = newDateOfBirth.ToString() == DateTime.MinValue.ToString() ? student.DateOfBirth.ToString() : newDateOfBirth.ToString();
        //                updateStudentDTO.Address = newAddress == String.Empty ? student.Address : newAddress;
        //                updateStudentDTO.ClassIds = newClassIds == null || newClassIds.Count == 0 ? student.Classes.Select(c => c.Id).ToList() : newClassIds;

        //                var updatedStudent = await studentService.UpdateStudentAsync(updateStudentDTO);
        //                Console.WriteLine("Updated Student Successfully: " + $"ID: {updatedStudent?.Id}, Name: {updatedStudent?.Name}, Date of Birth: {updatedStudent?.DateOfBirth}, Address: {updatedStudent?.Address}");
        //                if (updatedStudent?.Classes != null && updatedStudent.Classes.Count > 0)
        //                {
        //                    Console.WriteLine("Classes:");
        //                    foreach (var cls in updatedStudent.Classes)
        //                    {
        //                        Console.WriteLine($" - {cls.Id}: {cls.Name}");
        //                    }
        //                }

        //                break;
        //            }
        //        case "4":
        //            {
        //                Console.Write("Enter Student ID to delete: ");
        //                if (int.TryParse(Console.ReadLine(), out int id))
        //                {
        //                    var success = await studentService.DeleteStudentAsync(id);
        //                    if (success)
        //                    {
        //                        Console.WriteLine("Student deleted successfully.");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("Student not found or could not be deleted.");
        //                    }
        //                }
        //                break;
        //            }
        //        case "5":
        //            {
        //                var students = await studentService.ArrangeStudentsByNameAsync();
        //                foreach (var student in students)
        //                {
        //                    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
        //                }
        //                break;

        //            }
        //        case "6":
        //            {
        //                Console.Write("Enter Student ID: ");
        //                if (int.TryParse(Console.ReadLine(), out int id))
        //                {
        //                    var student = await studentService.GetStudentByIdAsync(id);
        //                    if (student != null)
        //                    {
        //                        Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
        //                        if (student.Classes != null && student.Classes.Count > 0)
        //                        {
        //                            Console.WriteLine("Classes:");
        //                            foreach (var cls in student.Classes)
        //                            {
        //                                Console.WriteLine($" - ID: {cls.Id}: {cls.Name} - Teacher: {cls.Teacher?.Name ?? "N/A"}");
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("Student not found.");
        //                    }
        //                }
        //                else
        //                {
        //                    Console.WriteLine("Invalid ID format.");
        //                }

        //                break;
        //            }
        //        case "7":
        //            {
        //                // dispose the current scope and service provider before creating a new one
        //                scope.Dispose();
        //                serviceProvider.Dispose();

        //                // stop previous gRPC host and restart it with new storage config
        //                if (grpcApp != null)
        //                {
        //                    try
        //                    {
        //                        await grpcApp.StopAsync();
        //                    }
        //                    catch { }
        //                    grpcApp = null;
        //                }

        //                // ConfigureStorage ask about storage mode and register the appropriate DAOs
        //                serviceProvider = CreateServiceProvider(storagePath);
        //                scope = serviceProvider.CreateScope();

        //                // Get the new services from the new scope
        //                studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
        //                classService = scope.ServiceProvider.GetRequiredService<IClassService>();

        //                // restart gRPC host
        //                (grpcApp, grpcTask) = StartGrpcHost(args, storagePath, port);

        //                Console.WriteLine("Storage mode changed successfully.");
        //                break;

        //            }
        //        case "8":
        //            {
        //                Console.WriteLine("Exiting...");
        //                // Stop gRPC host gracefully
        //                if (grpcApp != null)
        //                {
        //                    try
        //                    {
        //                        await grpcApp.StopAsync();
        //                    }
        //                    catch { }
        //                }

        //                return;
        //            }
        //        default:
        //            {
        //                Console.WriteLine("Invalid option. Please try again.");
        //                break;
        //            }
        //    }
        //}
        while (true)
        {

        }
    }

    private static void ConfigureStorage(IServiceCollection services, string storagePath)
    {
        Console.WriteLine("Select storage mode:");
        Console.WriteLine("1) NeonDB (PostgreSQL)");
        Console.WriteLine("2) Local file storage");
        Console.Write("Enter choice (1 or 2). Press Enter for default (auto-detect): ");
        var storageChoice = Console.ReadLine()?.Trim();

        if (storageChoice == "1")
        {
            _nHibernateRegistered = true;
        }
        else if (storageChoice == "2")
        {
            _nHibernateRegistered = false;
        }
        else
        {
            // default: auto-detect based on NEON_CONNECTION env var
            var envConnDetect = Environment.GetEnvironmentVariable("NEON_CONNECTION");
            _nHibernateRegistered = !string.IsNullOrWhiteSpace(envConnDetect);
        }

        if (_nHibernateRegistered)
        {
            // Prompt for connection string (allow override of environment variable)
            var envConnRaw = Environment.GetEnvironmentVariable("NEON_CONNECTION");
            var neoConn = ConnectionStringHelper.NormalizePostgresConnectionString(envConnRaw);

            if (!string.IsNullOrWhiteSpace(neoConn))
            {
                // Register NHibernate and NHibernate-backed DAOs
                services.AddNHibernate(neoConn);
                services.AddScoped<IStudentDAO, StudentDaoNHibernate>();
                services.AddScoped<IClassDAO, ClassDaoNHibernate>();
                services.AddScoped<ITeacherDAO, TeacherDaoNHibernate>();
                Console.WriteLine("Using NeonDB (NHibernate) storage.");
            }
            else
            {
                Console.WriteLine("No valid connection string provided. Falling back to file-based storage.");
                services.AddSingleton<IStudentDAO>(_ => new StudentDaoLocalStorage(storagePath));
                services.AddSingleton<IClassDAO>(_ => new ClassDaoLocalStorage(storagePath));
                services.AddSingleton<ITeacherDAO>(_ => new TeacherDaoLocalStorage(storagePath));
            }
        }
        else
        {
            // Use local file storage
            services.AddSingleton<IStudentDAO>(_ => new StudentDaoLocalStorage(storagePath));
            services.AddSingleton<IClassDAO>(_ => new ClassDaoLocalStorage(storagePath));
            services.AddSingleton<ITeacherDAO>(_ => new TeacherDaoLocalStorage(storagePath));
            Console.WriteLine("Using local file storage.");

            // Original (commented) registrations for reference (do not delete):
            // .AddSingleton<IStudentDAO>(_ => new StudentDAOusingConsole(storagePath))
            // .AddScoped<IClassDAO>(_ => new ClassDAOusingConsole(storagePath))
        }
    }

    private static void RegisterCommonServices(IServiceCollection services)
    {

        services
            .AddScoped<IStudentService, StudentService>()
            .AddScoped<IStudentRepository, StudentRepository>()
            .AddScoped<IClassService, ClassService>()
            .AddScoped<IClassRepository, ClassRepository>()
            .AddScoped<ITeacherService, TeacherService>()
            .AddScoped<ITeacherRepository, TeacherRepository>()
            .AddScoped<IAnalyticsService, AnalyticsService>()
            .AddScoped<IAnalyticsRepository, AnalyticsRepository>();

        //automapper
        services.AddLogging();
        services.AddAutoMapper(
    cfg => { },
    typeof(StudentMappingProfile));
    }

    private static ServiceProvider CreateServiceProvider(string storagePath)
    {
        var services = new ServiceCollection();
        RegisterCommonServices(services);
        ConfigureStorage(services, storagePath);
        return services.BuildServiceProvider();
    }

    private static (WebApplication app, Task serverTask) StartGrpcHost(
        string[] args,
        string storagePath,
        int port)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(port, listen =>
            {
                listen.UseHttps();
                listen.Protocols = HttpProtocols.Http2;
            });
        });

        RegisterCommonServices(builder.Services);

        var envConnRawServer =
            Environment.GetEnvironmentVariable("NEON_CONNECTION");

        var neoConnServer =
            ConnectionStringHelper.NormalizePostgresConnectionString(
                envConnRawServer);

        if (!string.IsNullOrWhiteSpace(neoConnServer) &&
            _nHibernateRegistered)
        {
            builder.Services.AddNHibernate(neoConnServer);

            builder.Services.AddScoped<IStudentDAO, StudentDaoNHibernate>();
            builder.Services.AddScoped<IClassDAO, ClassDaoNHibernate>();
            builder.Services.AddScoped<ITeacherDAO, TeacherDaoNHibernate>();
        }
        else
        {
            builder.Services.AddSingleton<IStudentDAO>(
                _ => new StudentDaoLocalStorage(storagePath));

            builder.Services.AddSingleton<IClassDAO>(
                _ => new ClassDaoLocalStorage(storagePath));

            builder.Services.AddSingleton<ITeacherDAO>(
                _ => new TeacherDaoLocalStorage(storagePath));
        }

        builder.Services.AddGrpc();

        var app = builder.Build();

        app.MapGrpcService<StudentManagement.Grpc.StudentGrpcService>();
        app.MapGrpcService<StudentManagement.Grpc.ClassGrpcService>();
        app.MapGrpcService<StudentManagement.Grpc.AnalyticsGrpcService>();

        app.MapGet("/health", () => Results.Ok(new
        {
            status = "Healthy",
            processId = Environment.ProcessId
        }));

        app.MapGet("/", () =>
            $"StudentManagement gRPC server running on port {port}.");

        var serverTask = app.RunAsync();

        return (app, serverTask);
    }
}