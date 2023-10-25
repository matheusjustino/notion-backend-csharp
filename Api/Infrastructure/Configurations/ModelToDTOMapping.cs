namespace NotionBackend.Api.Infrastructure.Configurations;

using NotionBackend.Api.Domain.DTOs.User;
using NotionBackend.Api.Domain.Entities;
using NotionBackend.Api.Domain.DTOs.Document;

using AutoMapper;

public class ModelToDTOMapping : Profile
{
    public ModelToDTOMapping()
    {
        this.CreateMap<User, UserDTO>();
        this.CreateMap<Document, DocumentDTO>();
        this.CreateMap<UpdateDocumentDTO, Document>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}