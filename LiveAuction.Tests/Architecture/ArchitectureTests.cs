using System;
using FluentAssertions;
using LiveAuction.API.Controllers.V1;
using LiveAuction.Application.Services;
using LiveAuction.Core.Entites;
using LiveAuction.Infrastructure.Data;
using NetArchTest.Rules;
using Xunit;

namespace LiveAuction.UnitTests.Architecture
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "LiveAuction.Core";
        private const string ApplicationNamespace = "LiveAuction.Application";
        private const string InfrastructureNamespace = "LiveAuction.Infrastructure";
        private const string APINamespace = "LiveAuction.API";

        [Fact]
        public void Domain_Should_Not_DependOn_Other_Layers()
        {
            var result = Types.InAssembly(typeof(ApplicationUser).Assembly)
                .ShouldNot()
                .HaveDependencyOn(ApplicationNamespace)
                .And()
                .HaveDependencyOn(InfrastructureNamespace)
                .And()
                .HaveDependencyOn(APINamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue("Domain layer should not depend on any other layer.");
        }

        [Fact]
        public void Application_Should_Not_DependOn_Infrastructure_Or_API()
        {

            var result = Types.InAssembly(typeof(AuthService).Assembly)
                .ShouldNot()
                .HaveDependencyOn(InfrastructureNamespace)
                .And()
                .HaveDependencyOn(APINamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue("Application layer should not depend on Infrastructure or API.");
        }

        [Fact]
        public void Infrastructure_Should_Not_DependOn_API()
        {
            var result = Types.InAssembly(typeof(AppDbContext).Assembly)
                .ShouldNot()
                .HaveDependencyOn(APINamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue("Infrastructure layer should not depend on API.");
        }

        [Fact]
        public void Controllers_Should_Not_Depend_Directly_On_Repositories()
        {

            var result = Types.InAssembly(typeof(AuthController).Assembly)
                .That()
                .HaveNameEndingWith("Controller")
                .ShouldNot()
                .HaveDependencyOn("LiveAuction.Infrastructure.Repositories")
                .GetResult();

            result.IsSuccessful.Should().BeTrue("Controllers should communicate via Services/Handlers, not Repositories directly.");
        }

        [Fact]
        public void Services_Should_Have_Name_Ending_With_Service()
        {

            var result = Types.InAssembly(typeof(AuthService).Assembly)
                .That()
                .ImplementInterface(typeof(LiveAuction.Application.Interfaces.IAuthService)) // مثال
                .Should()
                .HaveNameEndingWith("Service")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}