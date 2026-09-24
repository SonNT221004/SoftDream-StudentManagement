using StudentWeb.Models;

namespace StudentWeb.Services.Interface
{
    public interface IClassService
    {
        public Task<List<ClassViewModel>> GetAllClassesAsync();
    }
}
