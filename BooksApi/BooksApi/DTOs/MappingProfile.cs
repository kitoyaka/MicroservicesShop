using AutoMapper;
using BooksApi.Models;
using BooksApi.DTOs;

namespace BooksApi.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>()
        .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name));

        CreateMap<CreateBookDto, Book>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
    }


}