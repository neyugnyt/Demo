using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using Xunit;
using Common.Constants;
using Common.Http;
using Common.Pagination;

// No other changes are needed as the 'Fact' attribute is part of the Xunit namespace.
using Domain.DTOs.Products;
using Domain.Entities;
using Service.Products;
using Infrastructure.EntityFramework;

public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _productRepoMock;
    private readonly Mock<IRepository<Category>> _categoryRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IRepository<Product>>();
        _categoryRepoMock = new Mock<IRepository<Category>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new ProductService(
            _categoryRepoMock.Object,
            _productRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public void Create_ReturnsInvalidString_WhenNameOrDescriptionIsNull()
    {
        var dto = new CreateProductDTO { Name = null, Description = null };
        _mapperMock.Setup(m => m.Map<CreateProductDTO, Product>(dto)).Returns(new Product());
        _mapperMock.Setup(m => m.Map<Product, ProductDTO>(It.IsAny<Product>())).Returns(new ProductDTO());

        var result = _service.Create(dto);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.InvalidString, result.Message);
    }

    [Fact]
    public void Create_ReturnsError_WhenCategoryNotFound()
    {
        var dto = new CreateProductDTO { Name = "Test", Description = "Desc", CategoryId = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<CreateProductDTO, Product>(dto)).Returns(new Product());
        _categoryRepoMock.Setup(r => r.Find(dto.CategoryId)).Returns((Category)null);

        var result = _service.Create(dto);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.Error, result.Message);
    }

    [Fact]
    public void Create_ReturnsSuccess_WhenValid()
    {
        var dto = new CreateProductDTO { Name = "Test", Description = "Desc", CategoryId = Guid.NewGuid() };
        var category = new Category { Id = dto.CategoryId };
        var product = new Product { CategoryId = dto.CategoryId, Category = category };
        var productDto = new ProductDTO();

        _mapperMock.Setup(m => m.Map<CreateProductDTO, Product>(dto)).Returns(product);
        _categoryRepoMock.Setup(r => r.Find(dto.CategoryId)).Returns(category);
        _mapperMock.Setup(m => m.Map<Product, ProductDTO>(product)).Returns(productDto);

        var result = _service.Create(dto);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.CreateSuccess, result.Message);
        Assert.Equal(productDto, result.Data);
    }

    [Fact]
    public void Delete_ReturnsError_WhenProductNotFound()
    {
        var dto = new DeleteProductDTO { Id = Guid.NewGuid() };
        _productRepoMock.Setup(r => r.Find(dto.Id)).Returns((Product)null);

        var result = _service.Delete(dto);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.Error, result.Message);
    }

    [Fact]
    public void Delete_ReturnsSuccess_WhenProductFound()
    {
        var dto = new DeleteProductDTO { Id = Guid.NewGuid() };
        var product = new Product();
        var productDto = new ProductDTO();

        _productRepoMock.Setup(r => r.Find(dto.Id)).Returns(product);
        _mapperMock.Setup(m => m.Map<Product, ProductDTO>(product)).Returns(productDto);

        var result = _service.Delete(dto);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.DeleteSuccess, result.Message);
        Assert.Equal(productDto, result.Data);
    }

    [Fact]
    public void GetByCategory_ReturnsProducts()
    {
        var categoryId = Guid.NewGuid();
        var products = new List<Product> { new Product { CategoryId = categoryId } }.AsQueryable();
        var productDtos = new List<ProductDTO> { new ProductDTO() };

        _productRepoMock.Setup(r => r.Queryable()).Returns(products);
        _mapperMock.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>())).Returns(productDtos);

        var result = _service.GetByCategory(categoryId);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.ListSuccess, result.Message);
        Assert.Equal(productDtos, result.Data);
    }

    [Fact]
    public void SearchPagination_ReturnsError_WhenSearchIsNull()
    {
        var result = _service.SearchPagination(null);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.Error, result.Message);
    }

    [Fact]
    public void SearchPagination_ReturnsPaginatedList()
    {
        var searchDto = new SearchPaginationDTO<ProductDTO>
        {
            Search = new ProductDTO { Name = "Test" },
            PageIndex = 0,
            PageSize = 10
        };
        var products = new List<Product> { new Product { Name = "Test" } }.AsQueryable();
        var paginated = new PaginatedList<Product>(products, 0, 10);
        var paginatedDto = new PaginatedList<ProductDTO>();

        _productRepoMock.Setup(r => r.Queryable()).Returns(products);
        _mapperMock.Setup(m => m.Map<PaginatedList<Product>, PaginatedList<ProductDTO>>(It.IsAny<PaginatedList<Product>>()))
            .Returns(paginatedDto);

        var result = _service.SearchPagination(searchDto);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.ListSuccess, result.Message);
        Assert.Equal(paginatedDto, result.Data);
    }

    [Fact]
    public void Update_ReturnsInvalidString_WhenNameOrDescriptionIsNull()
    {
        var dto = new UpdateProductDTO { Name = null, Description = null };
        _mapperMock.Setup(m => m.Map<UpdateProductDTO, Product>(dto)).Returns(new Product());
        _mapperMock.Setup(m => m.Map<Product, ProductDTO>(It.IsAny<Product>())).Returns(new ProductDTO());

        var result = _service.Update(dto);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.InvalidString, result.Message);
    }

    [Fact]
    public void Update_ReturnsError_WhenCategoryNotFound()
    {
        var dto = new UpdateProductDTO { Name = "Test", Description = "Desc", CategoryId = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<UpdateProductDTO, Product>(dto)).Returns(new Product());
        _categoryRepoMock.Setup(r => r.Find(dto.CategoryId)).Returns((Category)null);

        var result = _service.Update(dto);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.Error, result.Message);
    }

    [Fact]
    public void Update_ReturnsSuccess_WhenValid()
    {
        var dto = new UpdateProductDTO { Name = "Test", Description = "Desc", CategoryId = Guid.NewGuid() };
        var category = new Category { Id = dto.CategoryId };
        var product = new Product { CategoryId = dto.CategoryId, Category = category };
        var productDto = new ProductDTO();

        _mapperMock.Setup(m => m.Map<UpdateProductDTO, Product>(dto)).Returns(product);
        _categoryRepoMock.Setup(r => r.Find(dto.CategoryId)).Returns(category);
        _mapperMock.Setup(m => m.Map<Product, ProductDTO>(product)).Returns(productDto);

        var result = _service.Update(dto);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.UpdateSuccess, result.Message);
        Assert.Equal(productDto, result.Data);
    }

    [Fact]
    public void GetById_ReturnsError_WhenProductNotFound()
    {
        var id = Guid.NewGuid();
        _productRepoMock.Setup(r => r.Queryable()).Returns(new List<Product>().AsQueryable());

        var result = _service.GetById(id);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.Error, result.Message);
    }

    [Fact]
    public void GetById_ReturnsError_WhenCategoryNotFound()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, CategoryId = Guid.NewGuid(), IsDeleted = false };
        _productRepoMock.Setup(r => r.Queryable()).Returns(new List<Product> { product }.AsQueryable());
        _categoryRepoMock.Setup(r => r.Find(product.CategoryId)).Returns((Category)null);

        var result = _service.GetById(id);

        Assert.True(result.HasError);
        Assert.Equal(MessageConstants.Error, result.Message);
    }

    [Fact]
    public void GetById_ReturnsSuccess_WhenValid()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, CategoryId = Guid.NewGuid(), IsDeleted = false };
        var category = new Category { Id = product.CategoryId };
        var productDto = new ProductDTO();

        _productRepoMock.Setup(r => r.Queryable()).Returns(new List<Product> { product }.AsQueryable());
        _categoryRepoMock.Setup(r => r.Find(product.CategoryId)).Returns(category);
        _mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDto);

        var result = _service.GetById(id);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.ListSuccess, result.Message);
        Assert.Equal(productDto, result.Data);
    }

    [Fact]
    public void UpdateCount_ReturnsSuccess()
    {
        var dto = new UpdateProductDTO { SaleCount = 1 };
        var product = new Product();

        _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);

        var result = _service.UpdateCount(dto, 2);

        Assert.False(result.HasError);
        Assert.Equal(MessageConstants.UpdateSuccess, result.Message);
        Assert.Equal(3, dto.SaleCount);
    }
}