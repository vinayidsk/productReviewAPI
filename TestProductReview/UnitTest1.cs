using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;
using ProductReviewAPI.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ProductReviewAPI.JwtHelpers;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using ProductReviewAPI.Dtos;
using Newtonsoft.Json.Linq;

namespace TestProductReview
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task GetToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<User>>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var mockJwtSettings = new JwtSettings
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = "64A63153-11C1-4919-9133-EFAF99A9B456",
                ValidateIssuer = true,
                ValidIssuer = "your-issuer",
                ValidateAudience = true,
                ValidAudience = "your-audience",
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            var controller = new AccountController(mockRepo.Object, mockJwtSettings, mockLogger.Object);

            var users = TestData.GetTestUsers();
            var validUsername = "testuser";
            var validPassword = "testpassword";

            mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
               .ReturnsAsync(users);

            var userLogins = new UserLogins { Username = validUsername, Password = validPassword };

            // Act
            var result = await controller.GetToken(userLogins);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var token = Assert.IsType<UserTokens>(okResult.Value);
            Assert.NotNull(token);
            Assert.Equal(validUsername, token.Username);
        }

        [Fact]
        public async Task GetList_ValidUserWithRole_ReturnsListOfUsers()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<User>>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var mockJwtSettings = new JwtSettings
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = "64A63153-11C1-4919-9133-EFAF99A9B456",
                ValidateIssuer = true,
                ValidIssuer = "your-issuer",
                ValidateAudience = true,
                ValidAudience = "your-audience",
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            var users = TestData.GetTestUsers();
            var validUsername = "adminuser";
            var validPassword = "adminpassword";

            mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(users);

            var controller = new AccountController(mockRepo.Object, mockJwtSettings, mockLogger.Object);

            var userLogins = new UserLogins { Username = validUsername, Password = validPassword };
            var token = JwtHelpers.GenTokenkey(new UserTokens
            {
                GuidId = Guid.NewGuid(),
                Role = Role.Admin,
                Username = validUsername,
                UserId = users.First(u => u.UserName == validUsername).UserId,
            }, mockJwtSettings);

            // Simulate authenticated user with role
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, validUsername),
                new Claim(ClaimTypes.Role, Role.Admin.ToString())
            }, "mock"));

            // Act
            var result = await controller.GetList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(users.Count(), returnedUsers.Count());
        }

        [Fact]
        public void GetRoles_ReturnsRolesList()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<User>>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var mockJwtSettings = new Mock<JwtSettings>();

            var controller = new AccountController(mockRepo.Object, mockJwtSettings.Object, mockLogger.Object);

            // Act
            var result = controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<Dictionary<int, string>>(okResult.Value);
            Assert.Equal(Enum.GetValues(typeof(Role)).Length, roles.Count());
        }

        [Fact]
        public async Task SignUp_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<User>>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var mockJwtSettings = new JwtSettings
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = "64A63153-11C1-4919-9133-EFAF99A9B456",
                ValidateIssuer = true,
                ValidIssuer = "your-issuer",
                ValidateAudience = true,
                ValidAudience = "your-audience",
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            var controller = new AccountController(mockRepo.Object, mockJwtSettings, mockLogger.Object);

            var newUser = new UserSignUp
            {
                Username = "newuser",
                Password = "newpassword",
                FirstName = "New",
                LastName = "User"
            };

            mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(new List<User>());
            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await controller.SignUp(newUser);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task SignUp_NullFields_ReturnsBadRequest()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<User>>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var mockJwtSettings = new JwtSettings
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = "64A63153-11C1-4919-9133-EFAF99A9B456",
                ValidateIssuer = true,
                ValidIssuer = "your-issuer",
                ValidateAudience = true,
                ValidAudience = "your-audience",
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            var controller = new AccountController(mockRepo.Object, mockJwtSettings, mockLogger.Object);

            // Act
            var result = await controller.SignUp(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var response = badRequestResult.Value;
            Assert.NotNull(response);
            Assert.Equal("Username and Password fields are mandetory", response);
            
        }
        
        // TODO: Needs to add Other test methods for AccountController
    }
    public class CategoriesControllerTests
    {
        [Fact]
        public async Task GetAllCategories_ReturnsCategories()
        {
            // Arrange
            var mockCategoryRepository = new Mock<IDataRepository<Category>>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();

            var controller = new CategoriesController(mockCategoryRepository.Object, mockLogger.Object);

            var categories = TestData.GetTestCategories();

            mockCategoryRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categories);

            // Act
            var result = await controller.GetAllCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
            Assert.Equal(categories.Count(), returnedCategories.Count());
        }
        
        // TODO: Needs to add Other test methods for CategoriesController
    }

    public class ReviewsControllerTests
    {

        [Fact]
        public async Task GetAllReviews_ReturnsAllReviews()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<Review>>();
            var mockLogger = new Mock<ILogger<ReviewsController>>();

            var allReviews = TestData.GetTestReviews();

            mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(allReviews);

            var controller = new ReviewsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllReviews();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReviews = Assert.IsType<List<Review>>(okResult.Value);
            Assert.Equal(allReviews.Count, returnedReviews.Count);
        }

        [Fact]
        public async Task GetReviewsForProduct_ReturnsReviewsForProduct()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<Review>>();
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var productId = 123;

            var reviewsForProduct = TestData.GetTestReviews()
                .Where(r => r.ProductId == productId)
                .ToList();

            mockRepo.Setup(repo => repo.GetAllAsync(r => r.ProductId == productId))
                .ReturnsAsync(reviewsForProduct);

            var controller = new ReviewsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetReviewsForProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReviews = Assert.IsType<List<Review>>(okResult.Value);
            Assert.Equal(reviewsForProduct.Count, returnedReviews.Count);
        }

        [Fact]
        public async Task GetAverageRatingForProduct_ReturnsAverageRating()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<Review>>();
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var productId = 123;

            var reviewsForProduct = TestData.GetTestReviews()
                .Where(r => r.ProductId == productId)
                .ToList();

            mockRepo.Setup(repo => repo.GetAllAsync(r => r.ProductId == productId))
                .ReturnsAsync(reviewsForProduct);

            var controller = new ReviewsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAverageRatingForProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var averageRating = Assert.IsType<double>(okResult.Value);
            Assert.Equal(reviewsForProduct.Average(r => r.Rating), averageRating);
        }

        // TODO: Needs to add Other test methods for ReviewsController
    }

    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetAllProducts_ReturnsOkResultWithProducts()
        {
            // Arrange
            var mockRepo = new Mock<IDataRepository<Product>>();
            var mockLogger = new Mock<ILogger<ProductsController>>();

            var products = TestData.GetTestProductsList(); ;

            mockRepo.Setup(repo => repo.GetAllAsync(null, It.IsAny<Expression<Func<Product, object>>>()))
                    .ReturnsAsync(products);

            var controller = new ProductsController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var productDTOs = Assert.IsAssignableFrom<IEnumerable<ProductDtoWithAvgRating>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            // TODO: Needs to improve Test Data so that we can check more accuratly
            // Assert the number of products returned
            //Assert.Equal(products.Count(), productDTOs.Count());
        }

        // TODO: Needs to add Other test methods for ProductsController
    }

    public class RepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsListOfProducts()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IAsyncEnumerable<Product>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<Product>(new List<Product>().GetEnumerator()));

            var mockDbContext = new Mock<ReviewDBContext>();
            mockDbContext.Setup(c => c.Set<Product>())
                .Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockDbContext.Object);

            // Act
            var products = await productRepository.GetAllAsync();

            // Assert
            Assert.NotNull(products);
            Assert.IsAssignableFrom<IEnumerable<Product>>(products);

            // TODO: Needs to improve Test Data so that we can check more accuratly
        }

        // TODO: Needs to add Other test methods for Repositorys
    }


    // Helper methods for test data
    public static class TestData
    {
        public static List<User> GetTestUsers()
        {
            return new List<User>
            {
                new User { UserId = 1, UserName = "testuser", Password = "testpassword", Role = Role.User },
                new User { UserId = 2, UserName = "adminuser", Password = "adminpassword", Role = Role.Admin }
            };
        }
        public static IQueryable<Category> GetTestCategories()
        {
            return new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category 1" },
                new Category { CategoryId = 2, Name = "Category 2" }
            }.AsQueryable();
        }

        public static IQueryable<Product> GetTestProductsList()
        {
            return new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", CategoryId = 1 },
                new Product { ProductId = 2, Name = "Product 2", CategoryId = 2 }
            }.AsQueryable();
        }

        public static IQueryable<Product> GetTestProducts()
        {
            return new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", CategoryId = 1 },
                new Product { ProductId = 2, Name = "Product 2", CategoryId = 2 }
            }.AsQueryable();
        }

        public static List<Review> GetTestReviews()
        {
            return new List<Review>
            {
                new Review { ReviewId = 1, Rating = 4, ProductId = 123 },
                new Review { ReviewId = 2, Rating = 5, ProductId = 123 },
                new Review { ReviewId = 3, Rating = 3, ProductId = 456 }
                // Add more test reviews as needed
            };
        }
    }

    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }

    // Helper method for setting up DbSet
    public static class DbSetExtensions
    {
        public static DbSet<T> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
        }
    }
}