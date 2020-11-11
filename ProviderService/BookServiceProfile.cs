using AutoMapper;
using BusinessLogic.DTOModels;
using BusinessLogic.Models;
using System.Linq;

namespace ProviderService
{
    public class BookServiceProfile : Profile
    {
        public BookServiceProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Book[], BookDto[]>().ReverseMap();
            CreateMap<UserBook, UserBookDto>().ReverseMap();
            CreateMap<User, UserAddDto>().ReverseMap();
            CreateMap<User, UserDto>()
                .ForMember(dto => dto.Books, c => c.MapFrom(c => c.UserBooks.Select(c => c.Book))).ReverseMap();

        

        }
    }
}
