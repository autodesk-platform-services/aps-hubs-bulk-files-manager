using AutoMapper;
using Bulk_Uploader_Electron.Models;

namespace Bulk_Uploader_Electron.Utils
{
    public static class ClassMappings
    {
        public static readonly IMapper Mapper;


        /// <summary>
        /// Configures a static instance of a mapper
        /// </summary>
        static ClassMappings()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<MigrationProfiles>());
            Mapper = config.CreateMapper();
        }
    }

    /// <summary>
    /// Defines set of Automapper profiles
    /// </summary>
    public class MigrationProfiles : Profile
    {
        public MigrationProfiles()
        {
           // CreateMap<Account, AccountXls>().ReverseMap();

            // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
        }
    }
}