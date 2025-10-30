// using System.Net;
// using System.Net.Http.Json;
// using ExpenseTracker.DTOS.Auth;
// using ExpenseTracker.DTOS.Common;
// using ExpenseTracker.DTOS.User;
// using FluentAssertions;
// using Xunit;

// namespace ExpenseTracker.Tests;

// public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
// {
//     private readonly HttpClient _client;

//     public AuthEndpointsTests(CustomWebApplicationFactory factory)
//     {
//         _client = factory.CreateClient();
//     }

//     [Fact]
//     public async Task RegisterAndLogin_ShouldSucceed()
//     {
//         // Arrange: Create a new user registration request
//         var uniqueEmail = $"testuser_{Guid.NewGuid()}@example.com";
//         var registerRequest = new CreateUserRequest
//         {
//             FullName = "Test User",
//             Email = uniqueEmail,
//             Password = "Password123!",
//             ConfirmPassword = "Password123!"
//         };

//         // Act: Register the new user
//         var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerRequest);

//         // Assert: Registration was successful
//         registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
//         var registerResult = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//         registerResult.Should().NotBeNull();
//         registerResult!.Success.Should().BeTrue();
//         registerResult.Data!.Email.Should().Be(uniqueEmail);

//         // Act: Log in with the new user's credentials
//         var loginRequest = new LoginRequest { Email = uniqueEmail, Password = "Password123!" };
//         var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

//         // Assert: Login was successful and a token was returned
//         loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
//         var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
//         loginResult!.Success.Should().BeTrue();
//         loginResult.Data!.Token.Should().NotBeNullOrEmpty();
//     }
// }
