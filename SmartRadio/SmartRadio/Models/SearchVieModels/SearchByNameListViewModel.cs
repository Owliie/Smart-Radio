using SmartRadio.Data;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models.SearchVieModels
{
    public class SearchByNameListViewModel : IMapFrom<User>
    {
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
