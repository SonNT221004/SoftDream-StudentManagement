using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StudentWeb.Models;
using StudentWeb.Services.Interface;
using AntDesign.Charts;
using StudentWeb.Resilience.ConnectionState;

namespace StudentWeb.Components.Pages
{
    public partial class Dashboard : ComponentBase, IDisposable
    {
        [Inject] public IAnalyticsService AnalyticsService { get; set; } = default!;
        [Inject] public AnalyticsGrpcConnectionState Connection { get; set; } = default!;

        private string? errorMessage;
        private bool isLoading;
        private object[] studentAddressInfo = [];
        private object[] teacherClassInfo = [];
        private int totalStudents;
        private int totalTeachers;
        private int totalClasses;
        private double avgStudentsPerClass => totalClasses > 0 ? (double)totalStudents / totalClasses : 0;

        protected override void OnInitialized()
        {
            Connection.Changed += OnConnectionChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            await FirstLoadChartAsync();
        }

        private void OnConnectionChanged() => _ = InvokeAsync(StateHasChanged);

        private async Task FirstLoadChartAsync()
        {
            isLoading = true;
            try
            {
                errorMessage = null;
                var studentAddress = await AnalyticsService.GetStudentCountByAddressAsync();

                studentAddressInfo = [.. studentAddress
                    .Select(sa => (object)new
                    { 
                        sa.LocationName,
                        sa.NumberOfStudent
                    })];

                var teacherClass = await AnalyticsService.GetClassCountPerTeacherAsync();
                teacherClassInfo = [.. teacherClass
                    .Select(sa => (object)new
                    {
                        sa.TeacherName,
                        sa.NumberOfClass
                    })];

                totalStudents = await AnalyticsService.GetTotalNumberOfStudents();
                totalClasses = await AnalyticsService.GetTotalNumberOfClasses();
                totalTeachers = await AnalyticsService.GetTotalNumberOfTeachers();
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

        public void Dispose() => Connection.Changed -= OnConnectionChanged;

        private void CloseErrorPopup() => errorMessage = null;

        readonly BarConfig configForBar = new()
        {
            AutoFit = true,
            XField = "numberOfClass",
            YField = "teacherName",
            Label = new BarViewConfigLabel
            {
                Visible = true,
                Position = "middle"
            }
        };

        readonly PieConfig configForPie = new()
        {
            AutoFit = true,
            Radius = 0.8,
            Padding = "auto",
            AngleField = "numberOfStudent",
            ColorField = "locationName"
        };
    }
}