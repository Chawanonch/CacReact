using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dto;
using Api.Entities;
using AutoMapper;

namespace Api.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateCategoryDto, Category>(); 
            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<CreateSourceDto, Source>();
            CreateMap<UpdateSourceDto, Source>();

            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();

            CreateMap<CreateAddProductDto, AddProduct>();
            CreateMap<UpdateAddProductDto, AddProduct>();

            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();
            
            CreateMap<CreateCouponDto, Coupon>();
            CreateMap<UpdateCouponDto, Coupon>();

            CreateMap<CreateAddImageDto, AddImageProduct>();
            CreateMap<UpdateAddImageDto, AddImageProduct>();

        }
    }
}