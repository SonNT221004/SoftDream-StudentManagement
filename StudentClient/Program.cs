using Grpc.Net.Client;
using StudentManagement.Grpc;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

public static class Program
{
    private static readonly Regex ClassIdsPattern = new(@"^\s*\d+(\s*,\s*\d+)*\s*$", RegexOptions.Compiled);
    public static async Task Main(string[] args)
    {
        Console.WriteLine("gRPC client starting...");

        // Update address/port to match your gRPC server (use https in dev if server uses TLS)
        var serverAddress = "https://localhost:5000";

        using var channel = GrpcChannel.ForAddress(serverAddress);
        var studentClient = new StudentService.StudentServiceClient(channel);
        var classClient = new ClassService.ClassServiceClient(channel);
        Console.WriteLine("Student Management System");
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("==========================");
            Console.WriteLine("Menu:");
            Console.WriteLine("1. List Students");
            Console.WriteLine("2. Add Student");
            Console.WriteLine("3. Update Student");
            Console.WriteLine("4. Delete Student");
            Console.WriteLine("5. Arrange Students by Name");
            Console.WriteLine("6. Find Student by ID");
            Console.WriteLine("7. Exit");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    {
                        var students = await studentClient.GetAllStudentsAsync(new GetAllStudentsRequest());
                        foreach (var student in students.Students)
                        {
                            Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
                        }
                        break;
                    }
                case "2":
                    {
                        string name;
                        DateTime dob = DateTime.MinValue;
                        string address;
                        List<int> classIds = new List<int>();
                        bool isValidDate = false;
                        bool isValidClassIds = false;
                        //name
                        Console.Write("Enter Name: ");
                        name = Console.ReadLine() ?? String.Empty;
                        //dob
                        Console.Write("Enter Date of Birth (yyyy-MM-dd): ");
                        while (!isValidDate)
                        {
                            try
                            {
                                string input = Console.ReadLine() ?? String.Empty;

                                dob = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                                isValidDate = true;
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd:");
                            }
                        }
                        //address
                        Console.Write("Enter Address: ");
                        address = Console.ReadLine() ?? String.Empty;
                        //classes
                        Console.WriteLine("Available Classes:");
                        var classes = await classClient.GetAllClassesAsync(new GetAllClassesRequest());
                        foreach (var clazz in classes.Classes)
                        {
                            Console.WriteLine($"Class ID: {clazz.Id}, Name: {clazz.Name}, Teacher: {clazz.Teacher?.Name ?? "N/A"}");
                        }
                        while (!isValidClassIds)
                        {
                            Console.Write("Enter Class IDs (comma-separated): ");
                            var classIdsInput = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(classIdsInput))
                            {
                                if (classIdsInput.Length > 0 && ClassIdsPattern.IsMatch(classIdsInput))
                                {
                                    {
                                        classIds = classIdsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(idStr => int.TryParse(idStr.Trim(), out var id) ? id : (int?)null)
                                            .Where(id => id.HasValue)
                                            .Select(id => id.Value)
                                            .ToList();
                                        var validIds = classes.Classes.Select(c => c.Id).ToHashSet();
                                        var invalid = classIds.Where(i => !validIds.Contains(i)).ToList();
                                        if (invalid.Count > 0)
                                        {
                                            Console.WriteLine($"Warning: these class IDs were not found and will be ignored: {string.Join(", ", invalid)}");
                                            continue;
                                        }
                                        classIds = classIds.Where(i => validIds.Contains(i)).ToList();
                                        isValidClassIds = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please enter a comma-separated list of numeric class IDs.");
                                }
                            }
                            else
                            {
                                isValidClassIds = true; // Allow empty input for no classes
                            }
                        }

                        var newStudent = new AddStudentDto
                        {
                            Name = name,
                            DateOfBirth = dob.ToString("yyyy-MM-dd"),
                            Address = address
                        };
                        foreach(int id in classIds)
                        {
                            newStudent.ClassIds.Add(id);
                        }
                        await studentClient.AddStudentAsync(new AddStudentRequest { Student = newStudent });
                        Console.WriteLine("Student added successfully.");

                        break;
                    }
                case "3":
                    {
                        StudentDto? student = new StudentDto();
                        string newName;
                        DateTime newDateOfBirth = DateTime.MinValue;
                        string newAddress;
                        bool isValidDate = false;
                        bool isValidClassIds = false;
                        List<int> newClassIds = new List<int>();
                        Console.Write("Enter Student ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            var rawStudent = await studentClient.GetStudentByIdAsync(new GetStudentByIdRequest { Id = id });
                            student = rawStudent.Student;
                            if (student == null)
                            {
                                Console.WriteLine("Student not found.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"Current Student Info: ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
                            }
                            if (student?.Classes != null && student.Classes.Count > 0)
                            {
                                Console.WriteLine("Classes:");
                                foreach (var cls in student.Classes)
                                {
                                    Console.WriteLine($" - ID: {cls.Id}: {cls.Name} - Teacher: {cls.Teacher?.Name ?? "N/A"}");
                                }
                            }
                        }
                        //name
                        Console.Write("Enter new Name (leave blank to keep current): ");
                        newName = Console.ReadLine() ?? String.Empty;
                        //dob
                        Console.WriteLine("Enter new Date of Birth  (yyyy-MM-dd) (leave blank to keep current):");
                        while (!isValidDate)
                        {
                            try
                            {
                                string input = Console.ReadLine() ?? String.Empty;
                                if (input == String.Empty)
                                {
                                    newDateOfBirth = DateTime.ParseExact(student.DateOfBirth, "yyyy-MM-dd", null);
                                    isValidDate = true;
                                    break;
                                }
                                newDateOfBirth = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                                isValidDate = true;
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd:");

                            }
                        }


                        Console.WriteLine("Enter new Address (leave blank to keep current):");
                        newAddress = Console.ReadLine() ?? String.Empty;

                        //classes
                        Console.WriteLine("Available Classes:");
                        var classes = await classClient.GetAllClassesAsync(new GetAllClassesRequest());
                        foreach (var clazz in classes.Classes)
                        {
                            Console.WriteLine($"Class ID: {clazz.Id}, Name: {clazz.Name}, Teacher: {clazz.Teacher?.Name ?? "N/A"}");
                        }
                        while (!isValidClassIds)
                        {
                            Console.WriteLine("Enter new Class IDs (comma-separated) (leave blank to keep current):");
                            var classIdsInput = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(classIdsInput))
                            {
                                if (classIdsInput.Length > 0 && ClassIdsPattern.IsMatch(classIdsInput))
                                {
                                    newClassIds = classIdsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(idStr => int.TryParse(idStr.Trim(), out var id) ? id : (int?)null)
                                    .Where(id => id.HasValue)
                                    .Select(id => id.Value)
                                    .ToList();
                                    var validIds = classes.Classes.Select(c => c.Id).ToHashSet();
                                    var invalid = newClassIds.Where(i => !validIds.Contains(i)).ToList();
                                    if (invalid.Count > 0)
                                    {
                                        Console.WriteLine($"Warning: these class IDs were not found and will be ignored: {string.Join(", ", invalid)}");
                                        continue;
                                    }
                                    newClassIds = newClassIds.Where(i => validIds.Contains(i)).ToList();
                                    isValidClassIds = true;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please enter a comma-separated list of numeric class IDs.");
                                }
                            }
                            else
                            {
                                isValidClassIds = true; // Allow empty input for no classes
                            }
                        }

                        UpdateStudentDto updateStudentDto = new UpdateStudentDto();
                        updateStudentDto.Id = student!.Id;
                        updateStudentDto.Name = newName == String.Empty ? student.Name : newName;
                        updateStudentDto.DateOfBirth = newDateOfBirth.ToString() == DateTime.MinValue.ToString() ? student.DateOfBirth.ToString() : newDateOfBirth.ToString();
                        updateStudentDto.Address = newAddress == String.Empty ? student.Address : newAddress;
                        foreach (int cid in newClassIds)
                        {
                            updateStudentDto.ClassIds.Add(cid);
                        }

                        var updatingStudent = await studentClient.UpdateStudentAsync(new UpdateStudentRequest { Student = updateStudentDto });
                        var updatedStudent = updatingStudent?.Student;
                        Console.WriteLine("Updated Student Successfully: " + $"ID: {updatedStudent?.Id}, Name: {updatedStudent?.Name}, Date of Birth: {updatedStudent?.DateOfBirth}, Address: {updatedStudent?.Address}");
                        if (updatedStudent?.Classes != null && updatedStudent.Classes.Count > 0)
                        {
                            Console.WriteLine("Classes:");
                            foreach (var cls in updatedStudent.Classes)
                            {
                                Console.WriteLine($" - {cls.Id}: {cls.Name}");
                            }
                        }

                        break;
                    }
                case "4":
                    {
                        Console.Write("Enter Student ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            var success = await studentClient.DeleteStudentAsync(new DeleteStudentRequest { Id = id });
                            if (success.Success)
                            {
                                Console.WriteLine("Student deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Student not found or could not be deleted.");
                            }
                        }
                        break;
                    }
                case "5":
                    {
                        var students = await studentClient.ArrangeStudentsByNameAsync(new GetAllStudentsRequest());
                        foreach (var student in students.Students)
                        {
                            Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
                        }
                        break;

                    }
                case "6":
                    {
                        Console.Write("Enter Student ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            var rawStudent = await studentClient.GetStudentByIdAsync(new GetStudentByIdRequest { Id = id });
                            if (rawStudent != null)
                            {
                                var student = rawStudent.Student;
                                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Date of Birth: {student.DateOfBirth}, Address: {student.Address}");
                                if (student.Classes != null && student.Classes.Count > 0)
                                {
                                    Console.WriteLine("Classes:");
                                    foreach (var cls in student.Classes)
                                    {
                                        Console.WriteLine($" - ID: {cls.Id}: {cls.Name} - Teacher: {cls.Teacher?.Name ?? "N/A"}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Student not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }

                        break;
                    }
                case "7":
                    {
                        Console.WriteLine("Exiting...");
                        return;
                    }
                default:
                    {
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                    }
            }
        }
    }
}

