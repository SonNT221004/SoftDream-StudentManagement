using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StudentWeb.Models;
using StudentWeb.Resilience;
using StudentWeb.Services.Interface;

namespace StudentWeb.Components.Pages
{
    public partial class Home : ComponentBase, IDisposable
    {
        [Inject] public IStudentService StudentService { get; set; } = default!;
        [Inject] public IClassService ClassService { get; set; } = default!;
        [Inject] public GrpcConnectionState Connection { get; set; } = default!;

        // Paging
        private int currentPage = 1;
        private int totalCount;
        private int totalPages =>
            PageConfig.PageSize <= 0
                ? 1
                : Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageConfig.PageSize));

        // move between pages
        private async Task GoToPageAsync(int page)
        {
            if (page < 1 || page > totalPages || page == currentPage)
            {
                return;
            }

            currentPage = page;
            isLoading = true;
            try
            {
                await ReloadStudentsAsync();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        private Task PreviousPageAsync() => GoToPageAsync(currentPage - 1);
        private Task NextPageAsync() => GoToPageAsync(currentPage + 1);

        private string? errorMessage;
        private string? succesMessage;
        // For view and arrange function
        private List<StudentForListViewModel> students = [];
        private bool isArranged = false;
        private string arrangeButtonName = "Arrange By Name";
        // For view detail, update or delete function
        private StudentViewModel? selectedStudent;
        private UpdateStudentViewModel? updatingStudent;

        // For Add Function
        private bool isAddFormVisible = false;
        private AddStudentViewModel newStudent = new();
        private List<ClassViewModel> availableClasses = [];

        // Load list at initial step
        private bool isLoading;

        protected override void OnInitialized()
        {
            Connection.Changed += OnConnectionChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            await FirstLoadAllAsync();
        }

        private void OnConnectionChanged() => _ = InvokeAsync(StateHasChanged);

        private async Task FirstLoadAllAsync()
        {
            isLoading = true;
            try
            {
                errorMessage = null;
                await ReloadStudentsAsync();
                availableClasses = await ClassService.GetAllClassesAsync();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        public void Dispose()
        {
            Connection.Changed -= OnConnectionChanged;
        }

        private async Task ReloadStudentsAsync()
        {
            try
            {
                errorMessage = null;
                isLoading = true;
                var (Students, TotalCount) = await StudentService.GetAllStudentsAsync(currentPage);
                students = Students;
                totalCount = TotalCount;
                if (isArranged)
                {
                    students = [.. students.OrderBy(student => student.Name)];
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task AddStudent()
        {
            try
            {
                errorMessage = null;
                succesMessage = null;
                isLoading = true;
                await StudentService.CreateStudentAsync(newStudent);
                if (students.Count == PageConfig.PageSize)
                {
                    currentPage++;
                }
                await ReloadStudentsAsync();
                succesMessage = "Added Successfully";
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }
            finally
            {
                isLoading = false;
            }
            CloseAddForm();
        }

        private async Task ShowStudentDetailAsync(StudentForListViewModel student)
        {
            try
            {
                errorMessage = null;
                isLoading = true;
                selectedStudent = await StudentService.GetStudentByIdAsync(student.Id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task EditStudent()
        {
            try
            {
                if (updatingStudent != null)
                {
                    errorMessage = null;
                    succesMessage = null;
                    isLoading = true;
                    var updatedStudent = await StudentService.UpdateStudentAsync(updatingStudent);
                    await ReloadStudentsAsync();
                    succesMessage = "Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
            CloseUpdateForm();
        }

        private async Task DeleteStudent()
        {
            try
            {
                errorMessage = null;
                succesMessage = null;
                isLoading = true;
                if (selectedStudent != null)
                {
                    var success = await StudentService.DeleteStudentAsync(selectedStudent.Id);
                    if (success)
                    {
                        CloseStudentDetailPopup();
                        if (students.Count == 1 && currentPage > 1)
                        {
                            currentPage--;
                        }
                        await ReloadStudentsAsync();
                        succesMessage = "Deleted successfully";
                    }
                    else
                    {
                        errorMessage = "Deleted failed";
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ArrangeByName()
        {
            if (!isArranged)
            {
                students = [.. students.OrderBy(student => student.Name)];
                arrangeButtonName = "Cancel Arrangement";
                isArranged = true;
            }
            else
            {
                students = [.. students.OrderBy(student => student.Id)];
                arrangeButtonName = "Arrange By Name";
                isArranged = false;
            }
        }

        // Open popup form
        private void OpenUpdateForm()
        {
            if (selectedStudent is null)
            {
                return;
            }

            updatingStudent = new UpdateStudentViewModel
            {
                Id = selectedStudent.Id,
                Name = selectedStudent.Name,
                DateOfBirth = selectedStudent.DateOfBirth,
                Address = selectedStudent.Address,
                ClassIds = [.. selectedStudent.Classes.Select(c => c.Id)]
            };

            CloseStudentDetailPopup();
        }
        private void OpenAddForm() => isAddFormVisible = true;

        // Close popup
        private void CloseErrorPopup() => errorMessage = null;
        private void CloseSuccessPopup() => succesMessage = null;

        private void CloseStudentDetailPopup() => selectedStudent = null;
        private void CloseAddForm()
        {
            isAddFormVisible = false;
            newStudent = new();
        }

        private void CloseUpdateForm() => updatingStudent = null;
    }
}