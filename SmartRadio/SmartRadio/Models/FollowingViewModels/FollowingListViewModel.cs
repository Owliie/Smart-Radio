using SmartRadio.Data;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models.FollowingViewModels
{
    public class FollowingListViewModel : IMapFrom<User>
    {
        public string RadioStation { get; set; }

        public string UserName { get; set; }

        public string Id { get; set; }
    }
}
